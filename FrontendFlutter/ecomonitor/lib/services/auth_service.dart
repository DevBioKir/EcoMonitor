import 'package:ecomonitor/core/network/api_client.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class AuthService {
  final ApiClient _apiClient;
  final FlutterSecureStorage _storage = const FlutterSecureStorage();
  static const _tokenKey = 'auth_token';

  AuthService(this._apiClient);

  Future<String> login(String email, String password) async {
    final response = await _apiClient.post('api/authorization/login', data: {
      'email' : email,
      'password' : password,
    });

    final token = response.data['token'] as String?;
    if (token == null){
      throw Exception('Authorization token not found in response');
    }
    await _storage.write(key: _tokenKey, value: token);
    return token;
  }

  Future<String?> getToken() async => await _storage.read(key: _tokenKey);

  Future<void> logOut() async => await _storage.delete(key: _tokenKey);

  // Future<Map<String, dynamic>> fetchUserProfile() async {
  //   final response = await _apiClient.get('api/user/profile');
  //   return response.data;
  // }
}
