import 'dart:io';

import 'package:device_info_plus/device_info_plus.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';

class ApiConfig {
  static Future<String> getBaseUrl() async {
    final deviceInfo = DeviceInfoPlugin();

    if (Platform.isAndroid) {
      final androidInfo = await deviceInfo.androidInfo;

      if (androidInfo.isPhysicalDevice == true){
        return dotenv.env['BASE_URL_FOR_FD'] ?? '';
      } else {
        return dotenv.env['BASE_URL_FOR_VD'] ?? '';
      }
    }
    return '';
  }
}