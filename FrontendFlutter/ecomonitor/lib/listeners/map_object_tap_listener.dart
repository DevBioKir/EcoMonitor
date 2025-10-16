import 'package:flutter/material.dart';
import 'package:yandex_maps_mapkit/mapkit.dart';


class MapObjectTapListenerImpl extends MapObjectTapListener {
  //final VoidCallback onTap;
  final BuildContext context;

  //MapObjectTapListenerImpl(this.onTap);
  MapObjectTapListenerImpl(this.context);
  @override
  bool onMapObjectTap(MapObject mapObject, Point point) {
    //onTap();
    print('Marker tapped!');
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Маркер нажат')),
    );
    //print("Tapped the placemark: Point(latitude: ${point.latitude}, longitude: ${point.longitude}");
    //showSnackBar("Tapped the placemark: Point(latitude: ${point.latitude}, longitude: ${point.longitude})");
    return true;
  }
}

// class MapObjectTapListenerImpl implements ymapkit.MapObjectTapListener {
//   final BuildContext context;

//   MapObjectTapListenerImpl(this.context);

//   @override
//     bool onMapObjectTap(ymapkit.MapObject mapObject, ymapkit.Point point) {
//       print("Маркер нажат: ${point.latitude}, ${point.longitude}");
//       ScaffoldMessenger.of(context).showSnackBar(
//         SnackBar(
//           content: Text(
//             'Input marker: Latitude ${point.latitude}, Longitude ${point.longitude}'))
//       );
//       return true;
//     }
// }