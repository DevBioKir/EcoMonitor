import 'package:dio/dio.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_upload_request.dart';
import 'package:ecomonitor/models/paged_result.dart';
import 'package:ecomonitor/models/photo_filter.dart';

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

  Future<PagedResult<BinPhotoResponse>> getUserPhotos(PhotoFilter filter) async {
    try {
      final queryParameters = {
        'page' : filter.page.toString(),
        'pageSize' : filter.pageSize.toString(),
        'sortBy' : filter.sortBy,

        if (filter.onlyOutsideBin != null)
        'onlyOutsideBin' : filter.onlyOutsideBin.toString(),

        if (filter.minFillLevel != null) 
        'minFillLevel' : filter.minFillLevel.toString(),

        if (filter.maxFillLevel != null)
        'maxFillLevel' : filter.maxFillLevel.toString(),

        if (filter.fromDate != null)
        'fromData' : filter.fromDate!.toIso8601String(),

        if (filter.toDate != null)
        'toDate' : filter.toDate!.toIso8601String(),
      };
      
      final response = await _apiClient.get('/api/binphoto/userUploadedPhotos',
      queryParameters: queryParameters);

      return PagedResult<BinPhotoResponse>.fromJson(
        response.data, 
        (json) => BinPhotoResponse.fromJson(json),
      );
    } on DioException catch (e) {
      print('Ошибка при загрузке фотографий: ${e.response?.statusCode} - ${e.message}');
      if (e.response != null) {
        print('Response data: ${e.response?.data}');
      }
      rethrow;
    }
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