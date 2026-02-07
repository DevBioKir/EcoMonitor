import 'package:dio/dio.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/auth/login_response.dart';
import 'package:ecomonitor/models/auth/register_user_request.dart';
import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class AuthService extends ChangeNotifier{
  late ApiClient _apiClient;
  //late ApiClient _refreshDio;
  final FlutterSecureStorage _storage = const FlutterSecureStorage();
  //bool _isLoggedIn = false;

  //bool get isLoggedIn => _isLoggedIn;

  static const _accessToken = 'access_token';
  static const _refreshToken = 'refresh_token';

  Future<String?> getAccessToken() async {
  final token = await _storage.read(key: _accessToken);
  print('AccessToken(): $token');
  return token;
}

Future<String?> getRefreshToken() async {
  final token = await _storage.read(key: _refreshToken);
  print('getRefreshToken(): $token');
  return token;
}

Future<void> saveTokens({required String accessToken, required String refreshToken}) async {
  await _storage.write(key: _accessToken, value: accessToken);
  await _storage.write(key: _refreshToken, value: refreshToken);
  }

  //AuthService(this._apiClient);
  //AuthService.isEmpty();
  AuthService();

  void attachApiClient(ApiClient apiClient) {
    _apiClient = apiClient;
  }
  
  Future<LoginResponse> login(String email, String password) async {
    try{
      final response = await _apiClient.post('/api/public/v1/Authorization/login', data: {
        'email' : email,
        'password' : password, },
        options: Options(
          extra: {'skipAuth': true},
        ),
      );

      final loginResponse = LoginResponse.fromJson(response.data);

      print('Получен accessToken: ${loginResponse.accessToken}');
      print('Получен refreshToken: ${loginResponse.refreshToken}');
      print('Время действия токена (секунды): ${loginResponse.expires}');

      await _storage.write(key: _accessToken, value: loginResponse.accessToken);
      await _storage.write(key: _refreshToken, value: loginResponse.refreshToken);

      return loginResponse;
    } on DioException catch (e) {
      if (e.response != null) {
        final errorMessage = e.response?.data['message'] ?? 'Ошибка при входе';
        throw Exception(errorMessage);
      } else {
        throw Exception('Ошибка сети или сервера');
      }
    }
}

Future<void> logOut() async{
  await _storage.delete(key: _accessToken);
  await _storage.delete(key: _refreshToken);
}

  Future<String> register(RegisterUserRequest request) async {
      final response = await _apiClient.post(
        'api/authorization/register',
        data: request.toJson()
      );

      final accessToken = response.data['accessToken'] as String?;
      final refreshToken = response.data['refreshToken'] as String?;

      if (accessToken == null || refreshToken == null) {
        throw Exception('Authorization tokens not found in response');
      }

      await saveTokens(accessToken: accessToken, refreshToken: refreshToken);

      return accessToken;
  }

  Future<bool> refreshToken(Dio dio) async {
    final refreshToken = await getRefreshToken();
    if (refreshToken == null || refreshToken.isEmpty) {
      return false;
    }

    try{
      final response = await dio.post(
        '/api/public/v1/Authorization/refresh-token', 
        data: {'refreshToken': refreshToken},
        options: Options(
          extra: {'skipAuth': true},
        ),
      );

      final newAccessToken = response.data['accessToken'] as String?;
      final newRefreshToken = response.data['refreshToken'] as String?;

      if (newAccessToken == null || newRefreshToken == null) {
        throw Exception('Failed to refresh tokens');
      }

      await saveTokens(accessToken: newAccessToken, refreshToken: newRefreshToken);
      return true;
    }
    catch (_) {
      return false;
    }
  }

 Future<bool> validateToken() async {
    try {
      final response = await _apiClient.post(
        '/api/public/v1/Authorization/Validate',
      );
      return response.statusCode == 200;
    } catch (_) {
      return false;
    }
  }
}
