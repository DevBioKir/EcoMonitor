import 'package:ecomonitor/core/network/api_client.dart';

class UserService {
  final ApiClient _apiClient;

  UserService(this._apiClient);

  Future<List<dynamic>> getAllUsers() async {
    final response = await _apiClient.get('api/users');
    return response.data;
  }

  Future<Map<String, dynamic>> getUserById(String userId) async {
    final response = await _apiClient.get('api/user/$userId');
    return response.data;
  }

  Future<Map<String, dynamic>> getUserByEmail(String email) async {
    final response = await _apiClient.get('api/user/ByEmail', 
    queryParameters: {'email': email});
    return response.data;
  }

  Future<void> addUser(Map<String, dynamic> userRequest) async {
    await _apiClient.post('api/user/AddUser', data: userRequest);
  }

  Future<void> deleteUser(String userId) async {
    await _apiClient.delete('api/user/$userId');
  }
}