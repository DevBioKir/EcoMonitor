import 'package:dio/dio.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/auth/login_response.dart';
import 'package:ecomonitor/models/auth/register_user_request.dart';
import 'package:flutter/material.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class AuthService extends ChangeNotifier{
  final ApiClient _apiClient;
  final FlutterSecureStorage _storage = const FlutterSecureStorage();
  bool _isLoggedIn = false;

  bool get isLoggedIn => _isLoggedIn;

  static const _accessToken = 'access_token';
  static const _refreshToken = 'refresh_token';

  AuthService(this._apiClient);
  
  Future<LoginResponse> login(String email, String password) async {
    try{
      final response = await _apiClient.post('/api/public/v1/Authorization/login', data: {
        'email' : email,
        'password' : password,
      });

      final loginResponse = LoginResponse.fromJson(response.data);
      
      // final accessToken = response.data['accessToken'] as String?;
      // final refreshToken = response.data['refreshToken'] as String?;
      // final expires = response.data['expires'] as int?;

      // if (accessToken == null || refreshToken == null){
      //   throw Exception('Authorization token not found in response');
      // }

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

  Future<String?> getRefreshToken() async => await _storage.read(key: _refreshToken);
  Future<String?> getAccessToken() async => await _storage.read(key: _accessToken);

  Future<void> logOut() async{
    await _storage.delete(key: _accessToken);
    await _storage.delete(key: _refreshToken);
  }

  // Future<bool> isLoggedIn() async {
  //   final token = await _storage.read(key: _accessToken);
  //   return token != null && token.isNotEmpty;
  // }

  Future<String> registration(RegisterUserRequest request) async {
      final response = await _apiClient.post(
        'api/authorization/register',
        data: request.toJson()
      );

      final accessToken = response.data['accessToken'] as String?;
      final refreshToken = response.data['refreshToken'] as String?;

      if (accessToken == null || refreshToken == null) {
        throw Exception('Authorization tokens not found in response');
      }

      await _storage.write(key: _accessToken, value: accessToken);
      await _storage.write(key: _refreshToken, value: refreshToken);

      return accessToken;
  }

  Future<String> registerAdmin(RegisterUserRequest request) async {
      final response = await _apiClient.post(
        'api/authorization/register',
        data: request.toJson()
      );

      final accessToken = response.data['accessToken'] as String?;
      final refreshToken = response.data['refreshToken'] as String?;

      if (accessToken == null || refreshToken == null) {
        throw Exception('Authorization tokens not found in response');
      }

      await _storage.write(key: _accessToken, value: accessToken);
      await _storage.write(key: _refreshToken, value: refreshToken);

      return accessToken;
  }

  Future<void> refreshToken() async {
    final refreshToken = getRefreshToken();

    final response = await _apiClient.post(
      'api/authorization/refresh-token', 
      data: {'refreshToken': refreshToken},
      );
      final newAccessToken = response.data['accessToken'] as String?;
    final newRefreshToken = response.data['refreshToken'] as String?;

    if (newAccessToken == null || newRefreshToken == null) {
      throw Exception('Failed to refresh tokens');
    }

    await _storage.write(key: _accessToken, value: newAccessToken);
    await _storage.write(key: _refreshToken, value: newRefreshToken);
  }

 Future<bool> ValidateToken() async {
  final accessToken = getAccessToken();
  if (accessToken == null) return false;    
  try {
    final response = await _apiClient.post(
      'api/authorization/Validate',
      headers: {
        'Authorization' : 'Bearer $accessToken'
      },
    );
    return response.statusCode == 200;
  } catch (e) {
    return false;
  }
}

}
