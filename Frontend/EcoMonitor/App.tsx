/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 */

import {YaMap} from 'react-native-yamap';

YaMap.init('b435f7c5-a250-4eb7-a2f8-3fff029ceb53');

const Map = () => {
  return (
    // @ts-ignore
    <YaMap
      userLocationIcon={{ uri: 'https://www.clipartmax.com/png/middle/180-1801760_pin-png.png' }}
      initialRegion={{
        lat: 50,
        lon: 50,
        zoom: 10,
        azimuth: 80,
        tilt: 100
      }}
      style={{ flex: 1 }}
    />
  );
};

export default Map;
