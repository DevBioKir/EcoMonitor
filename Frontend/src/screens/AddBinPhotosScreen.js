import { useEffect, useState } from 'react';
import {
  Alert,
  View,
  Button,
  Image,
  TextInput,
  Text,
  ScrollView
} from 'react-native';
import CheckBox from '@react-native-community/checkbox';
import { launchImageLibrary } from 'react-native-image-picker';
import { uploadWithMetadata } from '../services/UploadPhoto';
import { getAllBinTypes } from '../services/GetAllBinType';

export const AddPhotoScreen = () => {
  const [photo, setPhoto] = useState(null);
  const [binTypes, setBinTypes] = useState([]);
  const [selectedBinType, setSelectedBinType] = useState([]);
  const [fillLevel, setFilLevel] = useState('0,5');
  const [comment, setComment] = useState('Test photo');

  useEffect(() => {
    const fetchTypes = async () => {
      try {
        const response = await getAllBinTypes();
        setBinTypes(response);
      } catch (err) {
        console.error('Ошибка при зугрузке типов баков', err);
      }
    };
    fetchTypes();
  }, []);

  const toggleBinType = id => {
    setSelectedBinType(prev =>
      prev.includes(id) ? prev.filter(x => x !== id) : [...prev, id],
    );
  };

  const selectPhoto = () => {
    launchImageLibrary({ mediaType: 'photo' }, response => {
      if (response.didCancel || response.errorCode) return;
      if (response.assets?.length) {
        setPhoto(response.assets[0]);
      }
    });
  };

  const fill = parseFloat(fillLevel);
  if (!isFinite(fill)) {
    Alert.alert('Введите корректный уровень заполнения');
    return;
  }


  const handleUpload = async () => {
    if (!photo) {
      Alert('Select photo');
      return;
    }
    try {
      const response = await uploadWithMetadata({
        photo,
        binTypeId: selectedBinType,
        fillLevel: fill,
        isOutsideBin: true,
        comment,
      });
      console.log('Успех:', response.data);
      Alert('Фото загружено!');
    } catch (err) {
      console.error('Error:', err);
      Alert('Error upload');
    }
  };

  return (
    <ScrollView style={{ padding: 20 }}>
      <Button title="Select photo" onPress={selectPhoto} />
      {photo && (
        <Image
          source={{ uri: photo.uri }}
          style={{ width: 200, height: 200, marginVertical: 10 }}
        />
      )}

      <Text style={{ marginTop: 10, fontWeight: 'bold' }}>Типы баков:</Text>
      {binTypes.map(type => (
        <View
          key={type.id}
          style={{
            flexDirection: 'row',
            alignItems: 'center',
            marginVertical: 4,
          }}
        >
          <CheckBox
            value={selectedBinType.includes(type.id)}
            onValueChange={() => toggleBinType(type.id)}
          />
          <Text>{type.name}</Text>
        </View>
      ))}

      <TextInput
        placeholder="FillLevel"
        value={fillLevel}
        onChangeText={setFilLevel}
        style={{ borderWidth: 1, marginVertical: 5 }}
      />
      <TextInput
        placeholder="Comment"
        value={comment}
        onChangeText={setComment}
        style={{ borderWidth: 1, marginVertical: 5 }}
      />

      <Button title="Загрузить" onPress={handleUpload} />
    </ScrollView>
  );
};
