import React from 'react';
import { requireNativeComponent, ViewStyle, StyleProp, ViewProps } from 'react-native';

type YandexMapViewProps = ViewProps & {
  latitude: number;
  longitude: number;
};

const NativeYandexMapView = requireNativeComponent<YandexMapViewProps>("YandexMapView");

export default function YandexMapView({ latitude, longitude, style }: YandexMapViewProps) {
  console.log('YandexMapView props:', latitude, longitude);
  return (
    <NativeYandexMapView
      style={style}
      latitude={latitude}
      longitude={longitude}
    />
  );
}