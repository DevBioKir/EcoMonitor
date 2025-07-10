/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 */

import { NewAppScreen } from '@react-native/new-app-screen';
import {
  PermissionsAndroid,
  Platform,
  StatusBar,
  StyleSheet,
  useColorScheme,
  View,
} from 'react-native';
import YandexMapView from './components/YandexMapView';
import { useEffect } from 'react';

export default function App() {
  useEffect(() => {
    if (Platform.OS === 'android') {
      PermissionsAndroid.requestMultiple([
        PermissionsAndroid.PERMISSIONS.ACCESS_FINE_LOCATION,
        PermissionsAndroid.PERMISSIONS.ACCESS_COARSE_LOCATION,
      ]).then(statuses => {
        console.log('Permissions:', statuses);
      });
    }
  }, []);
  return (
    <View style={{ flex: 1 }}>
      {/* <View style={styles.container}> */}
      <YandexMapView
        style={{ flex: 1, backgroundColor: 'red' }} // красный фон — проверка отображения
        // style={styles.map}
        latitude={55.1644}
        longitude={61.4368}
        markers={[
          { latitude: 55.75, longitude: 37.61 },
          { latitude: 55.76, longitude: 37.62 },
        ]}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1 },
  map: { flex: 1 },
});

// export default App;
