import React from 'react';
import {
  requireNativeComponent,
  ViewStyle,
  StyleProp,
  ViewProps,
} from 'react-native';

interface Point {
  latitude: number;
  longitude: number;
}

interface CameraPosition {
  zoom: number;
  tilt: number;
  azimut: number;
  point: Point;
  reason: 'GESTURES' | 'APPLICATION';
  finished: boolean;
}

interface MapLoaded {
  renderObjectCount: number;
  curZoomModelsLoaded:number;
  curZoomPlacemarksLoaded: number;
  curZoomLabelsLoaded: number;
  curZoomGeometryLoaded: number;
  tileMemoryUsage: number;
  delayedGeometryLoaded: number;
  fullyAppeared: number;
  fullyLoaded: number;
}

interface InitialRegion extends Point {
  zoom?: number;
  azimuth?: number;
  tilt?: number;
}

type MasstransitVehicles = 'bus' | 'trolleybus' | 'tramway' | 'minibus' | 'suburban' | 'underground' | 'ferry' | 'cable' | 'funicular';

type Vehicles = MasstransitVehicles | 'walk' | 'car';


interface DrivingInfo {
  time: string;
  timeWithTraffic: string;
  distance: number;
}

interface MasstransitInfo {
  time:  string;
  transferCount:  number;
  walkingDistance:  number;
}

interface RouteInfo<T extends(DrivingInfo | MasstransitInfo)> {
  id: string;
  sections: {
    points: Point[];
    sectionInfo: T;
    routeInfo: T;
    routeIndex: number;
    stops: any[];
    type: string;
    transports?: any;
    sectionColor?: string;
  }
}

interface RoutesFoundEvent<T extends(DrivingInfo | MasstransitInfo)> {
  nativeEvent:  {
    status: 'success' | 'error';
    id: string;
    routes: RouteInfo<T>[];
  };
}

type VisibleRegion = {
  bottomLeft: Point;
  bottomRight: Point;
  topLeft: Point;
  topRight: Point;
}

type YamapSuggest = {
  title: string;
  subtitle?: string;
  uri?: string;
}

type YamapCoords = {
  lon: number;
  lat: number;
}

type YamapSuggestWithCoords = {
  lon: number;
  lat: number;
  title: string;
  subtitle?: string;
  uri?: string;
}

type YandexLogoPosition = {
  horizontal: 'left' | 'center' | 'right';
  vertical: 'top' | 'bottom';
}

type YandexLogoPadding = {
  horizontal?: number;
  vertical?: number;
}

interface YandexMapViewProps extends ViewProps {
  latitude: number;
  longitude: number;
  initialRegion?: InitialRegion;
  markers?: Point[];
  onCameraPositionChanged?: (event: { nativeEvent: CameraPosition }) => void;
  onMapLoaded?: (event: { nativeEvent: MapLoaded }) => void;
  onMarkerPress?: (event: { nativeEvent: { id: string } }) => void;
}

const NativeYandexMapView =
  requireNativeComponent<YandexMapViewProps>('YandexMapView');

export default function YandexMapView({
  latitude,
  longitude,
  style,
  initialRegion,
  markers,
  onMarkerPress,
}: YandexMapViewProps) {
  console.log('YandexMapView props:', latitude, longitude);
  return (
    <NativeYandexMapView
      style={style}
      latitude={latitude}
      longitude={longitude}
      initialRegion={initialRegion}
      markers={markers}
      onMarkerPress={onMarkerPress}
    />
  );
}
