'use client';
import { useEffect, useState, useCallback, useReducer } from 'react';
import { useFocusEffect } from '@react-navigation/native';
import YandexMapView from '../components/YandexMapView';
import { PermissionsAndroid, Platform } from 'react-native';
import { View, StyleSheet, Button } from 'react-native';
import { getAllPhotos } from '../services/GetAllBinPhotos';
import { getBinPhotoById } from '../services/GetBinPhotoById';
import { mapPhotosToMarkers } from '../utils/mapPhotosToMarkers';

const MapScreen = ({ navigation }) => {
  const [markers, setMarkers] = useState([]);
  const [mapKey, setMapKey] = useState(0);
  const [refreshMap, setRefreshMap] = useState(0);
  //const navigation = useNavigation(); // если юзать этот screen где то ещё

  // Простой обработчик нажатий на маркеры
  const handleMarkerPress = ({ nativeEvent }) => {
    const id = nativeEvent.id;
    console.log('Clicked marker id:', id);
    getBinPhotoById(id)
      .then(photo => {
        console.log('Photo loaded:', photo);
        navigation.navigate('PhotoInfo', { photo });
      })
      .catch(error => {
        console.error('Ошибка загрузки фото:', error);
      });
  };

  const loadMarkers = useCallback(async () => {
    try {
      const photos = await getAllPhotos();
      console.log('Loaded photos:', photos.length);
      const markerData = mapPhotosToMarkers(photos);
      console.log('Mapped markers:', markerData.length);
      setMarkers(markerData);
    } catch (error) {
      console.error('Ошибка загрузки маркеров:', error);
    }
  }, []);

  useEffect(() => {
    if (Platform.OS === 'android') {
      PermissionsAndroid.requestMultiple([
        PermissionsAndroid.PERMISSIONS.ACCESS_FINE_LOCATION,
        PermissionsAndroid.PERMISSIONS.ACCESS_COARSE_LOCATION,
        PermissionsAndroid.PERMISSIONS.READ_EXTERNAL_STORAGE,
      ]).then(statuses => {
        console.log('Permissions', statuses);
      });
    }
    loadMarkers();
  }, [loadMarkers]);

  // Убираем useFocusEffect - может конфликтовать с нативным компонентом
  // useFocusEffect(
  //   useCallback(() => {
  //     console.log('Map screen focused');
  //   }, [])
  // );

  return (
    <View style={styles.container}>
      <YandexMapView
        style={styles.map}
        latitude={55.154}
        longitude={61.4291}
        markers={markers}
        onMarkerPress={({ nativeEvent }) => {
          console.log('Yandex marker pressed:', nativeEvent.id);
          getBinPhotoById(nativeEvent.id)
            .then(photo => navigation.navigate('PhotoInfo', { photo }))
            .catch(console.error);
        }}
      />
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  map: {
    flex: 1,
  },
  buttonContainer: {
    position: 'absolute',
    bottom: 30,
    left: 20,
    right: 20,
  },
});

export default MapScreen;
