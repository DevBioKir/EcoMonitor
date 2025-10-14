import 'package:flutter/material.dart';
import 'package:yandex_maps_mapkit_lite/image.dart';
import 'package:yandex_maps_mapkit_lite/mapkit.dart' as ymapkit;
import 'package:yandex_maps_mapkit_lite/src/bindings/image/image_provider.dart' as ymapprovider;
import 'package:yandex_maps_mapkit_lite/src/mapkit/geometry/point.dart' as ymapgeometry;
import 'dart:math' as math;
import 'package:yandex_maps_mapkit_lite/mapkit_factory.dart';
import 'package:yandex_maps_mapkit_lite/yandex_map.dart';

class MapScreen extends StatefulWidget {
  const MapScreen({super.key});

  @override
  State<MapScreen> createState() => _MapScreenState();
}

class _MapScreenState extends State<MapScreen> {
  bool _isMapkitActive = false;
  late ymapkit.MapWindow _mapWindow;

  @override
  void initState() {
    super.initState();
    _startMapkit();
  }

  @override
  void dispose() {
    _stopMapkit();
    super.dispose();
  }

  void _startMapkit() {
    if (!_isMapkitActive) {
      _isMapkitActive = true;
      mapkit.onStart();
    }
  }

  void _stopMapkit() {
    if (_isMapkitActive) {
      _isMapkitActive = false;
      mapkit.onStop();
    }
  }

  Future<void> _onMapCreated(ymapkit.MapWindow mapWindow) async {
    _mapWindow = mapWindow;

    final center = const ymapkit.Point(latitude: 56.838926, longitude: 60.605702);
    mapWindow.map.move(
      ymapkit.CameraPosition(center, zoom: 12, azimuth: 0, tilt: 0),
    );

    final placemark = _mapWindow.map.mapObjects.addPlacemark()
    ..geometry = const ymapkit.Point(latitude: 56.838926, longitude: 60.605702)
    ..setText("Special place")
    ..setTextStyle(
      const ymapkit.TextStyle(
        size: 10.0,
        color: Colors.black,
        outlineColor: Colors.white,
        placement: ymapkit.TextStylePlacement.Right,
        offset: 5.0,
      )
    );
    
    placemark.useCompositeIcon()
      ..setIcon(
        ymapprovider.ImageProvider.fromImageProvider(const AssetImage("assets/ic_dollar_pin.png")),
        const ymapkit.IconStyle(
          anchor: math.Point(0.5, 1.0),
          scale: 4.0,
        ),
        name: "pin",
      )
      ..setIcon(
        ymapprovider.ImageProvider.fromImageProvider(const AssetImage("assets/ic_circle.png")),
        const ymapkit.IconStyle(
          anchor: math.Point(0.5, 0.5),
          flat: true,
          scale: 0.2,
        ),
        name: "point",
      );

    // Добавляем слушатель нажатия
    placemark.addTapListener(MapObjectTapListenerImpl(context));
  }
  

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Yandex Map Minimal')),
      body: YandexMap(
        onMapCreated: _onMapCreated,
        platformViewType: PlatformViewType.Hybrid,
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          Navigator.push (
            context,
            MaterialPageRoute(
              builder: (context) => AddPhotoScreen()),
          );
          setState(() {
            
          });
        },
        tooltip: 'Add photo',
        child: const Icon(Icons.add),
        
        ),
      );,
    );
  }
}