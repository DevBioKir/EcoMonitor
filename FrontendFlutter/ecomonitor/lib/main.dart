import 'package:ecomonitor/abstractions/ibin_photo_service.dart';
import 'package:ecomonitor/abstractions/ibin_type_service.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/core/network/api_config.dart';
import 'package:ecomonitor/screens/map_screen.dart';
import 'package:ecomonitor/services/auth_service.dart';
import 'package:ecomonitor/services/bin_photo_service.dart';
import 'package:ecomonitor/services/bin_type_service.dart';
import 'package:ecomonitor/services/user_service.dart';
import 'package:ecomonitor/stores/bin_type_store.dart';
import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:permission_handler/permission_handler.dart';
import 'package:provider/provider.dart';
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

late ApiClient apiClient;
late AuthService authService;

void main() async {
  WidgetsFlutterBinding.ensureInitialized();
  late final String apiBaseUrl;

  print('Loading .env...');
  await dotenv.load(fileName: ".env.development");
  print('⚡ .env BASE_URL_FOR_VD = ${dotenv.env['BASE_URL_FOR_VD']}');

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

  
  //String apiUrl = dotenv.env['BASE_URL_FOR_VD'] ?? '';
  //String apiUrl = dotenv.env['BASE_URL_FOR_FD'] ?? '';
  //String apiUrl = dotenv.env['BASE_URL'] ?? '';

  //const apiUrl = String.fromEnvironment('BASE_URL');

  // apiBaseUrl = dotenv.env['BASE_URL_FOR_VD'] ?? '';

  // if (apiBaseUrl.isEmpty) {
  //   throw Exception('BASE_URL is not defined');
  // }

  //apiBaseUrl = dotenv.env['BASE_URL_FOR_FD'] ?? '';
  apiBaseUrl = await ApiConfig.getBaseUrl();

  if (apiBaseUrl.isEmpty) {
    throw Exception('BASE_URL is not defined');
  }
  
  final storage = const FlutterSecureStorage();

  authService = AuthService();

  apiClient = ApiClient(
    apiBaseUrl,
    authService.getAccessToken,
    (dio) => authService.refreshToken(dio),
  );
  //print('⚡ ApiClient final baseUrl: ${apiClient.baseUrl}');

  authService.attachApiClient(apiClient);

  // apiClient = ApiClient(
  //   //"http://localhost:5198", () async => await storage.read(key: 'access_token') ?? '');
  //   //"https://10.0.2.2:7198", () async => await storage.read(key: 'access_token') ?? '');
  //   apiUrl,
  //   () async => null,
  //   () async => false,
  //   //() async => await storage.read(key: 'access_token') ?? ''
  //   );

  //   authService.attachApiClient(apiClient);
  
  // apiClient = ApiClient(
  //   //"http://localhost:5198", () async => await storage.read(key: 'access_token') ?? '');
  //   //"https://10.0.2.2:7198", () async => await storage.read(key: 'access_token') ?? '');
  //   apiUrl,
  //   authService.getAccessToken,
  //   () async {
  //     try {
  //       await authService.refreshToken();
  //       return true;
  //     } catch (_) {
  //       return false;
  //     }
  //   },
  // );

    //authService.attachApiClient(apiClient);

  // final authService = AuthService(apiClient);
  // final userService = UserService(apiClient);
  // final binPhotoService = BinPhotoService(apiClient, authService);

  runApp(
    MultiProvider(
      providers: [
        Provider<ApiClient>.value(value: apiClient),
        ChangeNotifierProvider<AuthService>.value(value: authService),

        // Provider<AuthService>(
        //   create: (_) => AuthService(apiClient),
        // ),

        Provider<UserService>(
          create: (_) => UserService(apiClient),
        ),

        Provider<IBinTypeService>(
          create: (_) => BinTypeService(apiClient),
        ),

        Provider<IBinPhotoService>(
          create: (ctx) => BinPhotoService(
            apiClient,
            //ctx.read<AuthService>(),
          ),
        ),
        ChangeNotifierProvider<BinTypeStore>(
        create: (ctx) => BinTypeStore(ctx.read<IBinTypeService>())..load(),
      ),
      ],
      child: const MyApp(),
    ),
  );
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'EcoMonitor',
      navigatorKey: navigatorKey,
      home: Consumer<AuthService>(
        builder: (context, authService, _) {
          return MapScreen(
            authService: authService);
        },
      ),
      routes: {
        '/map': (context) => Consumer<AuthService>(
          builder: (context, authService, _) => MapScreen(
            authService: authService),
        ),
      },
    );
  }
}