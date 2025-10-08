import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/auth/register_user_request.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class AuthService {
  final ApiClient _apiClient;
  final FlutterSecureStorage _storage = const FlutterSecureStorage();

  static const _accessToken = 'access_token';
  static const _refreshToken = 'refresh_token';

  AuthService(this._apiClient);

  Future<void> login(String email, String password) async {
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
  }

  Future<String?> getRefreshToken() async => await _storage.read(key: _refreshToken);
  Future<String?> getAccessToken() async => await _storage.read(key: _accessToken);

  Future<void> logOut() async{
    await _storage.delete(key: _accessToken);
    await _storage.delete(key: _refreshToken);
  }

  Future<void> register(RegisterUserRequest request) async {
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
