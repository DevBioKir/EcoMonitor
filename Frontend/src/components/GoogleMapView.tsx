import React from 'react';
import MapView, { Marker } from 'react-native-maps';
import { StyleSheet } from 'react-native';

interface Point {
  id: string;
  latitude: number;
  longitude: number;
  title?: string;
}

interface GoogleMapViewProps {
  style?: any;
  latitude: number;
  longitude: number;
  markers?: Point[];
  onMarkerPress?: (event: { nativeEvent: { id: string } }) => void;
}

export default function GoogleMapView({
  latitude,
  longitude,
  style,
  markers = [],
  onMarkerPress,
}: GoogleMapViewProps) {
  console.log('GoogleMapView render:', { 
    latitude, 
    longitude, 
    markersCount: markers?.length,
    hasOnMarkerPress: typeof onMarkerPress === 'function'
  });

  return (
    <MapView
      style={[styles.map, style]}
      initialRegion={{
        latitude,
        longitude,
        latitudeDelta: 0.01,
        longitudeDelta: 0.01,
      }}
      showsUserLocation={true}
      showsMyLocationButton={true}
    >
      {markers.map((marker) => (
        <Marker
          key={marker.id}
          coordinate={{
            latitude: marker.latitude,
            longitude: marker.longitude,
          }}
          title={marker.title || 'Контейнер'}
          description={`ID: ${marker.id}`}
          onPress={() => {
            console.log('Google Maps marker pressed:', marker.id);
            onMarkerPress?.({ nativeEvent: { id: marker.id } });
          }}
        />
      ))}
    </MapView>
  );
}

const styles = StyleSheet.create({
  map: {
    flex: 1,
  },
});
