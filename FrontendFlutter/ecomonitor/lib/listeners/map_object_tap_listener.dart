import 'package:flutter/material.dart';
import 'package:yandex_maps_mapkit/mapkit.dart';


class MapObjectTapListenerImpl extends MapObjectTapListener {
  final BuildContext context;

  MapObjectTapListenerImpl(this.context);

  @override
  bool onMapObjectTap(MapObject mapObject, Point point) {
    print('Marker tapped!');

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Маркер нажат')),
    );
    return true;
  }
}