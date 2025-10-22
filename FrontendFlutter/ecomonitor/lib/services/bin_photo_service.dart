import 'package:dio/dio.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_upload_request.dart';

class BinPhotoService {
  final ApiClient _apiClient;

  BinPhotoService(this._apiClient);

  Future<List<BinPhotoResponse>> getAllBinPhoto() async {
    final response = await _apiClient.get('api/binphoto/GetAllPhotos');
    return response.data;
  }

  Future<BinPhotoResponse> getBinPhotoById(String id) async {
    final response = await _apiClient.get('api/binphoto/GetBinPhotoById',
        queryParameters: {'id': id});
    return BinPhotoResponse.fromJson(response.data);
  }

  Future<List<BinPhotoResponse>> getUserPhotos(String userId) async {
    final response = await _apiClient.get('api/binphoto/user/$userId');
    return (response.data as List)
                    .map((item) => BinPhotoResponse.fromJson(item)).toList();
  }

  // Future<BinPhotoResponse> addBinPhoto(BinPhotoRequest request) async {
  //   final response = await _apiClient.post('api/binphoto',
  //   data: request.toJson());
  //   return BinPhotoResponse.fromJson(response.data);
  // }

  Future<BinPhotoResponse> uploadWithMetadata(BinPhotoUploadRequest request) async {
    final formData = FormData.fromMap(request.toFormData());
    final response = await _apiClient.post('api/binphoto/UploadWithMetadata',
        data: formData);
    return BinPhotoResponse.fromJson(response.data);
  }

  Future<String> deleteBinPhoto(String binPhotoId) async {
    final response = await _apiClient.delete(
      'api/binphoto/Delete',
      data: {'binPhotoId': binPhotoId},);
    return response.data as String;
  }
}