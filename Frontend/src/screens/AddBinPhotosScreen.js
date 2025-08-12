import { useEffect, useState } from 'react';
import {
  Alert,
  View,
  Button,
  Image,
  TextInput,
  Text,
  ScrollView,
  PermissionsAndroid,
  Platform,
} from 'react-native';
import CheckBox from '@react-native-community/checkbox';
import { launchCamera, launchImageLibrary } from 'react-native-image-picker';
import { uploadWithMetadata } from '../services/UploadPhoto';
import { getAllBinTypes } from '../services/GetAllBinType';

export const AddPhotoScreen = () => {
  const [photo, setPhoto] = useState(null);
  const [binTypes, setBinTypes] = useState([]);
  const [selectedBinType, setSelectedBinType] = useState([]);
  const [fillLevel, setFilLevel] = useState('0.5');
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
    Alert.alert(
      'Выбор фото',
      'Фото из галереи может не содержать координаты.\nРекомендуется сделать новое фото.',
      [
        { text: 'Отмена', style: 'cancel' },
        { 
          text: 'Всё равно выбрать', 
          onPress: () => {
            launchImageLibrary(
              {
                mediaType: 'photo',
                includeExtra: false,
                quality: 1,
                includeBase64: false,
              },
              response => {
                if (response.didCancel || response.errorCode) return;
                if (response.assets?.length) {
                  const selected = response.assets[0];
                  console.log('PHOTO из галереи', selected);
                  setPhoto(selected);
                }
              },
            );
          }
        },
      ]
    );
  };

  const requestCameraPermission = async () => {
    if (Platform.OS !== 'android') return true;
    
    try {
      const granted = await PermissionsAndroid.request(
        PermissionsAndroid.PERMISSIONS.CAMERA,
        {
          title: 'Разрешение на камеру',
          message: 'Приложению нужен доступ к камере для съёмки фотографий',
          buttonNeutral: 'Спросить позже',
          buttonNegative: 'Отмена',
          buttonPositive: 'OK',
        },
      );
      return granted === PermissionsAndroid.RESULTS.GRANTED;
    } catch (err) {
      console.warn(err);
      return false;
    }
  };

  const takePhoto = async () => {
    const hasPermission = await requestCameraPermission();
    if (!hasPermission) {
      Alert.alert('Ошибка', 'Нет разрешения на использование камеры');
      return;
    }

    launchCamera(
      {
        mediaType: 'photo',
        includeExtra: false,
        quality: 1,
        includeBase64: false,
        saveToPhotos: true,
      },
      response => {
        if (response.didCancel || response.errorCode) {
          console.log('Camera error:', response.errorMessage);
          return;
        }
        if (response.assets?.length) {
          const selected = response.assets[0];
          console.log('PHOTO с камеры', selected);
          setPhoto(selected);
        }
      },
    );
  };

  const handleUpload = async () => {
    if (!photo) {
      Alert.alert('Ошибка', 'Выберите фото');
      return;
    }

    const fill = parseFloat(fillLevel);

    if (!isFinite(fill) || fill < 0 || fill > 1) {
      Alert.alert('Ошибка', 'Введите корректный уровень заполнения (0-1)');
      return;
    }

    console.log('Отправляем fillLevel:', fill);

    try {
      const photoForUpload = {
        uri: photo.uri,
        name: photo.fileName || photo.name || 'photo.jpg',
        type: photo.type || 'image/jpeg',
      };

      const request = {
        photo: photoForUpload,
        binTypeId: selectedBinType,
        fillLevel: fill,
        isOutsideBin: true,
        comment,
      };

      console.log('Запрос:', request);

      const response = await uploadWithMetadata(request);
      console.log('Успех:', response);
      Alert.alert('Успех', 'Фото загружено!');
    } catch (err) {
      console.error('Error:', err);
      Alert.alert('Ошибка', 'Ошибка при загрузке');
    }
  };

  return (
    <ScrollView 
      style={{ flex: 1, padding: 20 }}
      contentContainerStyle={{ paddingBottom: 50 }}
    >
      <Text style={{ marginBottom: 10, fontSize: 16, fontWeight: 'bold' }}>
        Добавить фото:
      </Text>
      <View style={{ flexDirection: 'row', justifyContent: 'space-between', marginBottom: 10 }}>
        <Button title="Сделать фото" onPress={takePhoto} />
        <Button title="Выбрать из галереи" onPress={selectPhoto} />
      </View>
      <Text style={{ marginBottom: 10, color: '#666', fontSize: 15 }}>
        Для точных координат используйте функцию "Сделать фото"
      </Text>
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
