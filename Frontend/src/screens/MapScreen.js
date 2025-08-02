import { useEffect, useState } from 'react';
import YandexMapView from '../components/YandexMapView';
import { PermissionsAndroid, Platform } from 'react-native';
import { View, StyleSheet, Button } from 'react-native';
import { getAllPhotos } from '../services/GetAllBinPhotos';
import { getBinPhotoById } from '../services/GetBinPhotoById';
import { mapPhotosToMarkers } from '../utils/mapPhotosToMarkers';

const MapScreen = ({ navigation }) => {
  const [markers, setMarkers] = useState([]);
  //const navigation = useNavigation(); // если юзать этот screen где то ещё

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

    const loadMarkers = async () => {
      try {
        const photos = await getAllPhotos();
        const markerData = mapPhotosToMarkers(photos);
        setMarkers(markerData);
      } catch (error) {
        console.error('Ошибка загрузки маркеров:', error);
      }
    };

    loadMarkers();
  }, []);

  return (
    <View style={styles.container}>
      <YandexMapView
        style={styles.map}
        latitude={55.154}
        longitude={61.4291}
        markers={markers}
        // onMarkerPress={({ nativeEvent }) => {
        //   console.log('Marker pressed event with id:', nativeEvent.id);
        // }}
        onMarkerPress={({ nativeEvent }) => {
          const id = nativeEvent.id;
          console.log('Clicked marker id:', id);
          getBinPhotoById(id).then(photo => {
            navigation.navigate('PhotoInfo', { photo });
          });
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
