/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 */

import { NewAppScreen } from '@react-native/new-app-screen';
import {
  Alert,
  Button,
  PermissionsAndroid,
  Platform,
  ScrollView,
  StatusBar,
  StyleSheet,
  Switch,
  Text,
  TextInput,
  useColorScheme,
  View,
} from 'react-native';
import YandexMapView from './components/YandexMapView';
import React, { useEffect, useState } from 'react';
import { UploadWithMetadata } from './services/uploadPhotoService';
import { launchImageLibrary } from 'react-native-image-picker';
import Slider from '@react-native-community/slider';

export default function App() {
  const [binType, setBinType] = useState('Plastic');
  const [fillLevel, setFillLevel] = useState<number>(0.5);
  const [isOutsideBin, setIsOutsideBin] = useState(true);
  const [comment, setComment] = useState('');
  const [selectedPhoto, setSelectedPhoto] = useState<any>(null);

  useEffect(() => {
    if (Platform.OS === 'android') {
      PermissionsAndroid.requestMultiple([
        PermissionsAndroid.PERMISSIONS.ACCESS_FINE_LOCATION,
        PermissionsAndroid.PERMISSIONS.ACCESS_COARSE_LOCATION,
        PermissionsAndroid.PERMISSIONS.READ_EXTERNAL_STORAGE,
      ]).then(statuses => {
        console.log('Permissions:', statuses);
      });
    }
  }, []);

  const handleSelectImage = async () => {
    const result = await launchImageLibrary({
      mediaType: 'photo',
      quality: 0.8,
    });

    if (result.didCancel) {
      console.log('Выбор фото отменён');
      return;
    }

    const asset = result.assets?.[0];
    if (!asset || !asset.uri || !asset.fileName || !asset.type) {
      console.error('Некорректный файл:', asset);
      return;
    }

    const photo = {
      uri: asset.uri,
      name: asset.fileName,
      type: asset.type,
    };

    console.log('📷 Выбранное фото:', photo);
    setSelectedPhoto(photo);
  };

  const handleUpload = async () => {
    if (!selectedPhoto) {
      console.warn('Сначала выберите фото');
      return;
    }

    const request = {
      photo: selectedPhoto,
      binType,
      fillLevel: fillLevel,
      isOutsideBin,
      comment,
    };

    console.log('📤 Отправка запроса:', request);

    try {
      console.log('📤 Данные для загрузки:', request);
      const response = await UploadWithMetadata(request);
      console.log('✅ Успешно загружено:', response);
    } catch (error) {
      console.error('❌ Ошибка при загрузке:', error);
    }
  };

  return (
    <View style={{ flex: 1 }}>
      <YandexMapView
        style={{ flex: 1 }}
        latitude={55.154}
        longitude={61.4291}
        markers={[
          { latitude: 56.8376375, longitude: 60.6022782 },
          { latitude: 56.837097, longitude: 60.605209 },
        ]}
      />
      <View style={styles.formContainer}>
        <Button title="📷 Выбрать фото" onPress={handleSelectImage} />

        <Text style={styles.label}>Тип бака:</Text>
        <TextInput
          style={styles.input}
          value={binType}
          onChangeText={setBinType}
          placeholder="Тип (напр. Plastic)"
        />

        <Text style={styles.label}>
          Уровень заполнения: {fillLevel.toFixed(2)}
        </Text>
        <Slider
          style={{ width: '100%', height: 40 }}
          minimumValue={0}
          maximumValue={1}
          value={fillLevel}
          onValueChange={setFillLevel}
          step={0.01}
        />

        <View style={styles.switchRow}>
          <Text style={styles.label}>Наружный бак:</Text>
          <Switch value={isOutsideBin} onValueChange={setIsOutsideBin} />
        </View>

        <TextInput
          style={styles.input}
          value={comment}
          onChangeText={setComment}
          placeholder="Комментарий"
        />

        <Button title="⬆️ Загрузить" onPress={handleUpload} />
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  formContainer: {
    position: 'absolute',
    bottom: 20,
    left: 10,
    right: 10,
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 12,
    elevation: 5,
  },
  label: {
    fontSize: 14,
    marginTop: 10,
  },
  input: {
    borderWidth: 1,
    borderColor: '#ccc',
    padding: 8,
    marginTop: 4,
    borderRadius: 8,
  },
  switchRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginVertical: 10,
  },
});
