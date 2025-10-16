import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';

class BinPhotoService {
  final ApiClient _apiClient;

  BinPhotoService(this._apiClient);

  Future<List<BinPhotoResponse>> getAllBinPhoto() async {
    final response = await _apiClient.get('api/binphoto/GetAllPhoto');
    return response.data;
  }

  f
}