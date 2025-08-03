import { useState } from 'react';
import { Alert, View, Button, Image, TextInput } from 'react-native';
import { launchImageLibrary } from 'react-native-image-picker';
import { uploadWithMetadata } from '../services/UploadPhoto';

export const AddPhotoScreen = () => {
  const [photo, setPhoto] = useState(null);
  const [binTypeId, setBinTypeId] = useState(['type1', 'type2']);
  const [fillLevel, setFilLevel] = useState('0.5');
  const [comment, setComment] = useState('Test photo');

  const selectPhoto = () => {
    launchImageLibrary({ mediaType: 'photo' }, response => {
      if (response.didCancel || response.errorCode) return;
      if (response.assets?.length) {
        setPhoto(response.assets[0]);
      }
    });
  };

  const handleUpload = async () => {
    if (!photo) {
      Alert('Select photo');
      return;
    }

    try {
      const response = await uploadWithMetadata({
        photo,
        binTypeId,
        fillLevel: parseFloat(fillLevel),
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
    <View style={{ padding: 20 }}>
      <Button title="Select photo" onPress={selectPhoto} />
      {photo && (
        <Image
          source={{ uri: photo.uri }}
          style={{ width: 200, height: 200, marginVertical: 10 }}
        />
      )}

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
    </View>
  );
};
