/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 */

import { NewAppScreen } from '@react-native/new-app-screen';
import { StatusBar, StyleSheet, useColorScheme, View } from 'react-native';
import YandexMapView from './components/YandexMapView';

export default function App() {
  return (
    <View style={{ flex: 1 }}>
    {/* <View style={styles.container}> */}
      <YandexMapView 
      style={{ width: '100%', height: '100%' }}
      // style={styles.map}
      latitude={56.8389}
      longitude={60.6057} />
    </View>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1 },
  map: { flex: 1 },
});

// export default App;
