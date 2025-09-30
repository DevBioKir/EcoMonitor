import 'package:flutter/material.dart';
import 'package:yandex_maps_mapkit_lite/mapkit.dart';
import 'package:yandex_maps_mapkit_lite/yandex_map.dart';
import 'package:yandex_maps_mapkit_lite/init.dart' as init;
import 'package:yandex_maps_mapkit_lite/image.dart' as image_provider show ImageProvider;

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Scaffold(
        appBar: AppBar(title: const Text('Минимальная Яндекс.Карта')),
        body: YandexMap(
          onMapCreated: (mapWindow) {
            final map = mapWindow.map;

            // Координаты центра Екатеринбурга
            final center = Point(latitude: 56.838926, longitude: 60.605702);

            // Перемещаем камеру
            map.move(CameraPosition(center, zoom: 12, azimuth: 0, tilt: 0));

            // Добавляем простую метку (без кастомного изображения)
            map.mapObjects.addEmptyPlacemark(center);
          },
        ),
      ),
    );
  }
}

// import 'package:flutter/material.dart';
// import 'package:yandex_maps_mapkit_lite/image.dart' as image_provider show ImageProvider;
// import 'package:yandex_maps_mapkit_lite/init.dart' as init;
// import 'package:yandex_maps_mapkit_lite/yandex_map.dart';
// import 'package:yandex_maps_mapkit_lite/mapkit.dart';

// void main() async {
//   WidgetsFlutterBinding.ensureInitialized();
//   init.initMapkit(apiKey: '4e4c2599-8ba3-4d0a-bda1-a60a22dd2b00').then((_) {
//     runApp(const MyApp());
//   });
// }

// class MyApp extends StatefulWidget {
//   const MyApp({super.key});

//   @override
//   State<MyApp> createState() => _MyAppState();
// }

// class _MyAppState extends State<MyApp> {
//   MapWindow? _mapWindow;

//   // Координаты центра Екатеринбурга
//   static const Point _ekaterinburgCenter = Point(latitude: 56.838926, longitude: 60.605702);
//   static const String _placemarkDescription = 'Центр Екатеринбурга';
//   static const String _placemarkIcon = 'assets/ic_pin.png';

//   @override
//   Widget build(BuildContext context) {
//     return MaterialApp(
//       home: Scaffold(
//         appBar: AppBar(title: const Text("Яндекс.Карта")),
//         body: Stack(
//           children: [
//             YandexMap(
//               onMapCreated: (mapWindow) async {
//                 _mapWindow = mapWindow;
//                 final map = mapWindow.map;

//                 // Перемещение камеры на Екатеринбург
//                 map.move(
//                   CameraPosition(
//                     _ekaterinburgCenter,
//                     zoom: 12.0,
//                     azimuth: 0.0,
//                     tilt: 0.0,
//                   ),
//                 );

//                 final image = await image_provider.ImageProvider.fromImageProvider(AssetImage(_placemarkIcon));
//                 // Добавление одной метки
//                 map.mapObjects.addPlacemarkWithImage(
//                   _ekaterinburgCenter,
//                   image
//                 );
//               },
//             ),
//             Positioned(
//               bottom: 20,
//               left: 20,
//               right: 20,
//               child: ElevatedButton(
//                 onPressed: () {
//                   // Перемещение камеры к метке и показ SnackBar
//                   _mapWindow?.map.move(
//                     CameraPosition(
//                       _ekaterinburgCenter,
//                       zoom: 15.0,
//                       azimuth: 0.0,
//                       tilt: 0.0,
//                     ),
//                   );
//                   ScaffoldMessenger.of(context).showSnackBar(
//                     SnackBar(content: Text('Клик по: $_placemarkDescription')),
//                   );
//                 },
//                 child: const Text('Центр Екатеринбурга'),
//               ),
//             ),
//           ],
//         ),
//       ),
//     );
//   }
// }