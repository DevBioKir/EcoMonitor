/**
 * Sample React Native App
 * https://github.com/facebook/react-native
 *
 * @format
 */

import React from 'react';
// @ts-ignore
import * as YaMapModule from 'react-native-yamap';

// @ts-ignore
YaMap.init('b435f7c5-a250-4eb7-a2f8-3fff029ceb53');

const Map = () => {
  return (
    <>
    {/* @ts-ignore */}
    <YaMap />
    </>
    
  );
};

export default Map;
