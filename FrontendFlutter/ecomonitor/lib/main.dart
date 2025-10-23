import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/screens/login_screen.dart';
import 'package:ecomonitor/screens/map_screen.dart';
import 'package:ecomonitor/services/auth_service.dart';
import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:yandex_maps_mapkit/init.dart' as init;


final GlobalKey<NavigatorState> navigatorKey = GlobalKey<NavigatorState>();

void showSnackBar(String message) {
  final context = navigatorKey.currentContext;
  if (context != null) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text(message)),
    );
  } else {
    print('SnackBar fallback: $message');
  }
}

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  print('Loading .env...');
  await dotenv.load(fileName: ".env.development");

  // Запрашиваем разрешение на геолокацию в рантайме
  final status = await Permission.location.request();
  if (status.isGranted) {
    print('Location permission granted');
  } else {
    print('Location permission denied');
  }

  final apiKey = dotenv.env['YANDEX_MAP_API_KEY'];
  print('API KEY: $apiKey');

  if (apiKey == null) {
    throw Exception('YANDEX_MAP_API_KEY not found in .env file!');
  }

  try {
    print('Initializing MapKit...');
    await init.initMapkit(apiKey: apiKey);
    print('MapKit initialized successfully!');
  } catch (e) {
    print('MapKit initialization failed: $e');
  }

  final storage = const FlutterSecureStorage();
  final apiClient = ApiClient(
    "http://localhost:5198/", () async => await storage.read(key: 'auth_token') ?? '');
  // final apiClient = ApiClient(
  //  "http://10.0.2.2:5198/", () async => await storage.read(key: 'auth_token') ?? '');
  final authService = AuthService(apiClient);

  runApp(MyApp(authService: authService));
}

class MyApp extends StatelessWidget {
  final AuthService authService;

  const MyApp({super.key, required this.authService});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'EcoMonitor',
      navigatorKey: navigatorKey,
      home: MapScreen(authService: authService),
      // home: LoginScreen(
      //   authService: authService,
      //   onRegister: () {
      //     print('Go to the registration screen');
      //   },
      //   ),
        routes: {
          '/map': (context) => MapScreen(authService: authService),
        },
    );
  }
}