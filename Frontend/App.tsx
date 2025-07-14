/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 */

import { NewAppScreen } from '@react-native/new-app-screen';
import {
  Button,
  PermissionsAndroid,
  Platform,
  StatusBar,
  StyleSheet,
  useColorScheme,
  View,
} from 'react-native';
import YandexMapView from './components/YandexMapView';
import { useEffect } from 'react';
import { UploadWithMetadata } from './services/uploadPhotoService';
import { launchImageLibrary } from 'react-native-image-picker';

export default function App() {
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

  const handleImageUpload = async () => {
    const result = await launchImageLibrary({
      mediaType: 'photo',
      quality: 0.8,
    });

    if (result.didCancel) {
      console.log('Выбор отменён');
      return;
    }

    const asset = result.assets?.[0];
    if (!asset || !asset.uri || !asset.type || !asset.fileName) {
      console.error('Файл не выбран или невалиден');
      return;
    }

    const photo: any = {
      uri: asset.uri,
      name: asset.fileName,
      type: asset.type,
    };

    const request = {
      photo: photo,
      binType: 'plastic',
      fillLevel: 80,
      isOutsideBin: false,
      comment: 'Автозагрузка с карты',
    };

    try {
      const response = await UploadWithMetadata(request);
      console.log('Загружено:', response);
    } catch (error) {
      console.error('Ошибка при загрузке:', error);
    }
  };

  return (
    <View style={{ flex: 1 }}>
      {/* <View style={styles.container}> */}
      <YandexMapView
        style={{ flex: 1, backgroundColor: 'red' }} // красный фон — проверка отображения
        // style={styles.map}
        latitude={55.154}
        longitude={61.4291}
        markers={[
          { latitude: 56.8376375, longitude: 60.6022782 },
          { latitude: 56.837097, longitude: 60.605209 },
        ]} 
      />
    {/* Кнопка поверх карты */}
      <View style={styles.buttonContainer}>
        <Button title="Загрузить фото" onPress={handleImageUpload} />
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  buttonContainer: {
    position: 'absolute',
    bottom: 30,
    alignSelf: 'center',
    backgroundColor: '#fff',
    borderRadius: 10,
    padding: 5,
    elevation: 4,
  },
});

