import 'package:flutter/material.dart';
import 'package:yandex_maps_mapkit_lite/mapkit.dart';
import 'package:yandex_maps_mapkit_lite/mapkit_factory.dart';
import 'package:yandex_maps_mapkit_lite/yandex_map.dart';
import 'package:yandex_maps_mapkit_lite/init.dart' as init;
import 'package:permission_handler/permission_handler.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  // Запрашиваем разрешение на геолокацию в рантайме
  final status = await Permission.location.request();
  if (status.isGranted) {
    print('Location permission granted');
  } else {
    print('Location permission denied');
    // Можно обработать случай отказа
  }

  const apiKey = 'b435f7c5-a250-4eb7-a2f8-3fff029ceb53';
  await init.initMapkit(apiKey: apiKey);

  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: const MapScreen(),
    );
  }
}

class MapScreen extends StatefulWidget {
  const MapScreen({super.key});

  @override
  State<MapScreen> createState() => _MapScreenState();
}

class _MapScreenState extends State<MapScreen> {
  bool _isMapkitActive = false;

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

  void _onMapCreated(MapWindow mapWindow) {
    final center = Point(latitude: 56.838926, longitude: 60.605702);
    mapWindow.map.move(
      CameraPosition(center, zoom: 12, azimuth: 0, tilt: 0),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Yandex Map Minimal')),
      body: YandexMap(
        onMapCreated: _onMapCreated,
        platformViewType: PlatformViewType.Hybrid,
      ),
    );
  }
}

// class MyApp extends StatelessWidget {
//   const MyApp({super.key});

//   @override
//   Widget build(BuildContext context) {
//     return MaterialApp(
//       home: Scaffold(
//         appBar: AppBar(title: const Text('Минимальная Яндекс.Карта')),
//         body: SizedBox.expand(
//           child: YandexMap(
//             onMapCreated: (mapWindow) async {
//               print('Map created');
//               final map = mapWindow.map;
//               final center = Point(latitude: 56.838926, longitude: 60.605702);
//               try {
//                 map.move(CameraPosition(center, zoom: 12, azimuth: 0, tilt: 0));
//                 map.mapObjects.addEmptyPlacemark(center);
//                 print('Map moved and placemark added');
//               } catch (e) {
//                 print('Error moving map or adding placemark: $e');
//               }
//             },
//           ),
//         ),
//       ),
//     );
//   }
// }
