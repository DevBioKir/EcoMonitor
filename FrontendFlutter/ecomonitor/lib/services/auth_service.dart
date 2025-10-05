import 'package:ecomonitor/core/network/api_client.dart';

class AuthService {
  final ApiClient _apiClient;

  AuthService(this._apiClient);

  Future<String> login(String username, String password) async {
    final response = await _apiClient.post('api/auth/login', data: {
      'username' : username,
      'password' : password,
    });
    return response.data['token'];
  }

  Future<Map<String, dynamic>> fetchUserProfile() async {
    final response = await _apiClient.get('api/user/profile');
    return response.data;
  }
}
