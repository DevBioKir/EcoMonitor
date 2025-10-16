import 'package:ecomonitor/listeners/map_object_tap_listener.dart';
import 'package:ecomonitor/main.dart';
//import 'package:ecomonitor/main.dart';
import 'package:flutter/material.dart' hide TextStyle;
import 'package:url_launcher/url_launcher.dart';
import 'package:yandex_maps_mapkit/mapkit.dart' as ymapkit;
//import 'package:yandex_maps_mapkit/init.dart' as init;
import 'package:yandex_maps_mapkit/mapkit_factory.dart';
import 'package:yandex_maps_mapkit/src/bindings/image/image_provider.dart' as ymapprovider;
import 'package:yandex_maps_mapkit/yandex_map.dart' as ymap;
import 'dart:math' as math;

Future<void> openYandexTerms() async {
  const url = 'https://yandex.ru/legal/maps_api/';
  final uri = Uri.parse(url);
  if (await canLaunchUrl(uri)) {
    await launchUrl(uri);
  } else {
    print('Не удалось открыть $url');
  }
}

final class MapObjectTapListenerImpl implements ymapkit.MapObjectTapListener {

  @override
  bool onMapObjectTap(ymapkit.MapObject mapObject, ymapkit.Point point) {
    showSnackBar("Tapped the placemark: Point(latitude: ${point.latitude}, longitude: ${point.longitude})");
    return true;
  }
}

class MapScreen extends StatefulWidget {
  const MapScreen({super.key});

  @override
  State<MapScreen> createState() => _MapScreenState();
}

class _MapScreenState extends State<MapScreen> {
  bool _isMapkitActive = false;
  late ymapkit.MapWindow _mapWindow;

  final List<ymapkit.Point> _points = [
    const ymapkit.Point(latitude: 56.838926, longitude: 60.605702),
    const ymapkit.Point(latitude: 56.839000, longitude: 60.606000),
    const ymapkit.Point(latitude: 56.839500, longitude: 60.607000),
  ];

  List<ymapkit.PlacemarkMapObject> _placemarks = [];

  //MapKit stores weak references to the Listener objects passed to it.
  //It is necessary to store references to them in memory.
  late final ymapkit.MapObjectTapListener _tapListener;

  @override
  void initState() {
    super.initState();
    _tapListener = MapObjectTapListenerImpl();
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
      ymapkit.CameraPosition(center, zoom: 15, azimuth: 0, tilt: 0),
    );

    await _setPlacemarks();


    // print("Создаём стандартный маркер...");

    // final placemark = _mapWindow.map.mapObjects.addPlacemark()
    // ..geometry = center
    // //..geometry = const ymapkit.Point(latitude: 56.838926, longitude: 60.605702)
    // ..setText("Special place")
    // ..setTextStyle(
    //   const ymapkit.TextStyle(
    //     size: 10.0,
    //     color: Colors.black,
    //     outlineColor: Colors.white,
    //     placement: ymapkit.TextStylePlacement.Right,
    //     offset: 5.0,
    //   )
    // );

    // print("Маркер создан: координаты ${center.latitude}, ${center.longitude}");
    
    // // placemark.useCompositeIcon()
    // //   ..setIcon(
    // //     ymapprovider.ImageProvider.fromImageProvider(const AssetImage("assets/ic_dollar_pin.png")),
    // //     const ymapkit.IconStyle(
    // //       anchor: math.Point(0.5, 1.0),
    // //       scale: 4.0,
    // //     ),
    // //     name: "pin",
    // //   )
    // //   ..setIcon(
    // //     ymapprovider.ImageProvider.fromImageProvider(const AssetImage("assets/ic_circle.png")),
    // //     const ymapkit.IconStyle(
    // //       anchor: math.Point(0.5, 0.5),
    // //       flat: true,
    // //       scale: 0.2,
    // //     ),
    // //     name: "point",
    // //   );

    // // Добавляем слушатель нажатия
    // placemark.addTapListener(MapObjectTapListenerImpl(context));

    // print("Слушатель нажатия добавлен");
  }

  Future<void> _setPlacemarks() async{
    _mapWindow.map.mapObjects.clear();
    _placemarks.clear();

    //final center = const ymapkit.Point(latitude: 56.838926, longitude: 60.605702);

    final imageProvider = ymapprovider.ImageProvider.fromImageProvider(const AssetImage('assets/ic_pin6.png'));

    final iconStyle = const ymapkit.IconStyle(
      anchor: math.Point(0.5, 1.0),
      scale: 2.0,
    );

    for (final point in _points) {
      final placemark = _mapWindow.map.mapObjects.addPlacemarkWithImageStyle(
        point,
        imageProvider,
        iconStyle,
      );
    
    // final placemark = _mapWindow.map.mapObjects.addPlacemarkWithImageStyle(
    //   center,
    //   imageProvider,
    //   iconStyle);

  placemark.setText("Special place");
  placemark.setTextStyle(
    const ymapkit.TextStyle(
      size: 10.0,
      color: Colors.black,
      outlineColor: Colors.white,
      placement: ymapkit.TextStylePlacement.Right,
      offset: 5.0,
    ),
  );

  // placemark.addTapListener(MapObjectTapListenerImpl (() {
  //   ScaffoldMessenger.of(context).showSnackBar(
  //   SnackBar(content: Text('Маркер нажат!')),
  //   );
  // }));

  placemark.addTapListener(_tapListener);
  _placemarks.add(placemark);
    }
  }

  

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Map'),
      actions: [
          IconButton(
            icon: const Icon(Icons.info_outline),
            tooltip: 'Условия использования Яндекс.Карт',
            onPressed: () async {
              await openYandexTerms();
            },
          ),
        ],
      ),
      body: Builder(
        builder: (scaffoldContext) {
          return ymap.YandexMap(
            onMapCreated: _onMapCreated,
            platformViewType: ymap.PlatformViewType.Hybrid,
            // onMapCreated: (mapWindow) async {
            //   await _onMapCreated(mapWindow, scaffoldContext);
            // },
            // platformViewType: PlatformViewType.Hybrid,
            );
        },
      ),
      floatingActionButton: FloatingActionButton(
        onPressed: () {
          print('Button Add photo input');
          // Navigator.push (
          //   context,
          //   MaterialPageRoute(
          //     builder: (context) => AddPhotoScreen()),
          // );
          // setState(() {
            
          // });
        },
        backgroundColor: const Color.fromARGB(255, 122, 162, 230),
        tooltip: 'Add photo',
        child: Icon(Icons.add),
        ),
      );
  }
  

  // @override
  // Widget build(BuildContext context) {
  //   return Scaffold(
  //     appBar: AppBar(title: const Text('Map')),
  //     body: YandexMap(
  //       onMapCreated: _onMapCreated,
  //       platformViewType: PlatformViewType.Hybrid,
  //     ),
  //     floatingActionButton: FloatingActionButton(
  //       onPressed: () {
  //         print('Button Add photo input');
  //         // Navigator.push (
  //         //   context,
  //         //   MaterialPageRoute(
  //         //     builder: (context) => AddPhotoScreen()),
  //         // );
  //         // setState(() {
            
  //         // });
  //       },
  //       backgroundColor: const Color.fromARGB(255, 122, 162, 230),
  //       tooltip: 'Add photo',
  //       child: const Icon(Icons.add),
  //       ),
  //     );
  // }
}