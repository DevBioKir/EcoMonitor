'use client';
import { useEffect, useState, useCallback, useReducer } from 'react';
import { useFocusEffect } from '@react-navigation/native';
import YandexMapView from '../components/YandexMapView';
import { PermissionsAndroid, Platform } from 'react-native';
import { View, StyleSheet, Button } from 'react-native';
import { getAllPhotos } from '../services/GetAllBinPhotos';
import { getBinPhotoById } from '../services/GetBinPhotoById';
import { mapPhotosToMarkers } from '../utils/mapPhotosToMarkers';
import { getPhotosInBounds } from '../services/GetPhotosInBounds';
import { DEV_API_BASE_URL } from '@env';

const MapScreen = ({ navigation }) => {
  const [markers, setMarkers] = useState([]);
  const [isNavigating, setIsNavigating] = useState(false);
  //const navigation = useNavigation(); // если юзать этот screen где то ещё

  // Простой обработчик нажатий на маркеры
  const handleMarkerPress = ({ nativeEvent }) => {
    const id = nativeEvent.id;
    if (isNavigating) return;

    setIsNavigating(true);
    getBinPhotoById(id)
      .then(photo => {
        navigation.navigate('PhotoInfo', { photo });
        setTimeout(() => setIsNavigating(false), 1000);
      })
      .catch(error => {
        console.error(error);
        setIsNavigating(false);
      });
  };

  const loadMarkersInBounds = async bounds => {
    try {
      const photos = await getPhotosInBounds(bounds);
      const markerData = mapPhotosToMarkers(photos);
      setMarkers(markerData);
    } catch (error) {
      console.error('Ошибка загрузки маркеров:', error);
    }
  };

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
    // loadMarkers();
  }, []);

  return (
    <YandexMapView
      style={{ flex: 1 }}
      latitude={55.154}
      longitude={61.4291}
      baseUrl={DEV_API_BASE_URL}
      onBoundsChange={({ nativeEvent }) => {
        console.log('bounds', nativeEvent);
        loadMarkersInBounds(nativeEvent);
      }}
      onMarkerPress={({ nativeEvent }) => {
        navigation.navigate('PhotoInfo', { photoId: nativeEvent.id });
      }}
    />
  );

  // return (
  //   <View style={styles.container}>
  //     <YandexMapView
  //       style={styles.map}
  //       latitude={55.154}
  //       longitude={61.4291}
  //       markers={markers}
  //       onMarkerPress={({ nativeEvent }) => {
  //         if (isNavigating) {
  //           console.log('Navigation in progress, ignoring click');
  //           return;
  //         }

  //         console.log('Yandex marker pressed:', nativeEvent.id);
  //         setIsNavigating(true);

  //         getBinPhotoById(nativeEvent.id)
  //           .then(photo => {
  //             navigation.navigate('PhotoInfo', { photoId: nativeEvent.id });
  //             // Сбрасываем флаг через время
  //             setTimeout(() => setIsNavigating(false), 1000);
  //           })
  //           .catch(error => {
  //             console.error(error);
  //             onBoundsChange={({ nativeEvent }) => {
  //         loadMarkersInBounds(nativeEvent.bounds);
  //       }}
  //           });
  //       }}
  //     />
  //   </View>
  // );
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
