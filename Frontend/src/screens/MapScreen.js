'use client';
import { useEffect } from 'react';
import YandexMapView from '../components/YandexMapView';
import { PermissionsAndroid, Platform } from 'react-native';
import { View, StyleSheet, Button } from 'react-native';

const MapScreen = ({ navigation }) => {
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
  }, []);

  return (
    <View style={styles.container}>
      <YandexMapView
        style={styles.map}
        lattitude={55.154}
        longtitude={61.4291}
      />

      <View style={styles.buttonContainer}>
        <Button
          title="Показать все фото"
          onPress={() => navigation.navigate('AllPhotos')}
        />
      </View>
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
