import React from 'react';
import { requireNativeComponent, ViewStyle, StyleProp } from 'react-native';

export interface YandexMapViewProps {
  /** Обычное prop style у View‑подобных компонентов */
  latitude?: number;
  longitude?: number;
  style?: StyleProp<ViewStyle>;
}

const YandexMapView =
// через дженерики TypeScript понимает, что компонент принимает эти пропсы, включая style
  requireNativeComponent<YandexMapViewProps>('YandexMapView');

export default YandexMapView;