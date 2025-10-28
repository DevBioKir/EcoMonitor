import 'package:ecomonitor/core/network/api_client.dart';
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

  Future<bool> checkLoginStatus() async {
    final accessToken = await _storage.read(key: _accessToken);
    _isLoggedIn = accessToken != null;
    notifyListeners();
    return _isLoggedIn;
  }
  
  Future<String> login(String email, String password) async {
    final response = await _apiClient.post('api/authorization/login', data: {
      'email' : email,
      'password' : password,
    });

    final accessToken = response.data['accessToken'] as String?;
    final refreshToken = response.data['refreshToken'] as String?;
    final expires = response.data['expires'] as int?;

    if (accessToken == null || refreshToken == null){
      throw Exception('Authorization token not found in response');
    }
    await _storage.write(key: _accessToken, value: accessToken);
    await _storage.write(key: _refreshToken, value: refreshToken);
    
    return accessToken;
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
}
