import 'package:ecomonitor/abstractions/ibin_photo_service.dart';
import 'package:ecomonitor/abstractions/ibin_type_service.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/listeners/map_object_tap_listener.dart';
import 'package:ecomonitor/main.dart';
import 'package:ecomonitor/screens/add_photo_screen.dart';
import 'package:ecomonitor/screens/login_screen.dart';
import 'package:ecomonitor/screens/profile_screen.dart';
import 'package:ecomonitor/screens/register_screen.dart';
import 'package:ecomonitor/services/auth_service.dart';
import 'package:ecomonitor/services/bin_photo_service.dart';
import 'package:ecomonitor/services/bin_type_service.dart';
import 'package:ecomonitor/services/user_service.dart';
import 'package:flutter/material.dart' hide TextStyle;
import 'package:permission_handler/permission_handler.dart';
import 'package:provider/provider.dart';
import 'package:url_launcher/url_launcher.dart';
import 'package:yandex_maps_mapkit/mapkit.dart' as ymapkit;
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
  final ApiClient apiClient;
  //final AuthService authService;

  MapScreen({Key? key})
      : apiClient = ApiClient("http://localhost:5198/", () async => 'token'),
        super(key: key);
  

  @override
  State<MapScreen> createState() => _MapScreenState();
}

class _MapScreenState extends State<MapScreen> {
  bool _isMapkitActive = false;
  late ymapkit.MapWindow _mapWindow;
  late IBinPhotoService _binPhotoService;
  late IBinTypeService _binTypeService;

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
    _binPhotoService = BinPhotoService(widget.apiClient);
    _binTypeService = BinTypeService(widget.apiClient);
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

//   Future<void> _initLocationLayer() async {
//    final locationPermissionIsGranted =
//        await Permission.location.request().isGranted;


//    if (locationPermissionIsGranted) {
//      await ymapkit.toggleUserLayer(visible: true);
//    } else {
//      WidgetsBinding.instance.addPostFrameCallback((_) {
//        ScaffoldMessenger.of(context).showSnackBar(
//          const SnackBar(
//            content: Text('Нет доступа к местоположению пользователя'),
//          ),
//        );
//      });
//    }
//  }

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

  void _onRegisterPressed() {
  final authService = Provider.of<AuthService>(context, listen: false);
  Navigator.push(
    context,
    MaterialPageRoute(
      builder: (_) => RegisterScreen(authService: authService),
    ),
  );
}

  Future<void> _onProfilePressed() async {
    print('Нажали на профиль');
    final userService = Provider.of<UserService>(context, listen: false);
    final authService = Provider.of<AuthService>(context, listen: false);

    print('Выполняем запрос к текущему пользователю');
    final user = await userService.getCurrentUser();

    if (!mounted) return; // Защита от использования context после await

    if (user != null) {
      print('Переход в Profile Screen');
      Navigator.push(
        context,
        MaterialPageRoute(builder: (_) => ProfileScreen(user: user)),
      );
      } else {
    final loginSuccess = await Navigator.push<bool>(
      context,
      MaterialPageRoute(
        builder: (_) => LoginScreen(
          authService: authService,
          onRegister: _onRegisterPressed,
          onLoginSuccess: () async {
            final loggedInUser = await userService.getCurrentUser();
            if (!mounted) return;
            if (loggedInUser != null) {
              Navigator.push(context, MaterialPageRoute(builder: (_) => ProfileScreen(user: loggedInUser)));
            }
          },
        ),
      ),
    );
  }
}

  Future<void> _onAddPhotoPressed() async {
  final authService = Provider.of<AuthService>(context, listen: false);
  print('перед проверкой на валидацию токена');
  final isTokenValid = await authService.ValidateToken();

  print('Проверка статуса логина: isLoggedIn = $isTokenValid');

  if (!mounted) return;

  if (isTokenValid) {
    Navigator.push(context, MaterialPageRoute(builder: (_) => AddPhotoScreen(
      binPhotoService: _binPhotoService,
      binTypeService: _binTypeService)));
  } else {
    final loginSuccess = await Navigator.push<bool>(
      context,
      MaterialPageRoute(
        builder: (_) => LoginScreen(
          authService: authService,
          onRegister: _onRegisterPressed,
          onLoginSuccess: () {
            Navigator.push(context, MaterialPageRoute(builder: (_) => AddPhotoScreen(
              binPhotoService: _binPhotoService,
              binTypeService: _binTypeService)));
          },
        ),
      ),
    );

    if (loginSuccess != true) {
      // обработка неудачного логина, если нужно
    }
  }
}

@override
Widget build(BuildContext context) {
  return Scaffold(
    body: Builder(
      builder: (scaffoldContext) {
        return ymap.YandexMap(
          onMapCreated: _onMapCreated,
          platformViewType: ymap.PlatformViewType.Hybrid,
        );
      },
    ),
    floatingActionButton: FloatingActionButton(
        onPressed: _onAddPhotoPressed,
        backgroundColor: const Color.fromARGB(255, 122, 162, 230),
        tooltip: 'Add photo',
        child: const Icon(Icons.add),
      ),
    floatingActionButtonLocation: FloatingActionButtonLocation.centerDocked,
      bottomNavigationBar: BottomAppBar(
        shape: const CircularNotchedRectangle(),
        notchMargin: 8.0,
        child: Row(
          mainAxisSize: MainAxisSize.max,
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: <Widget>[
            IconButton(
              icon: const Icon(Icons.account_circle_outlined),
              tooltip: 'Личный кабинет',
              onPressed: _onProfilePressed,
            ),
            IconButton(
              icon: const Icon(Icons.info_outline),
              tooltip: 'Условия использования',
              onPressed: () async {
                await openYandexTerms();
              },
            ),
          ],
        ),
      ),
  );
}
}