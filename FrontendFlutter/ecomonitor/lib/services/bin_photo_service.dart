import 'package:dio/dio.dart';
import 'package:ecomonitor/abstractions/ibin_photo_service.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_upload_request.dart';
import 'package:ecomonitor/models/markers/photo_markers_dto.dart';
import 'package:ecomonitor/models/paged_result.dart';
import 'package:ecomonitor/models/photo_filter.dart';
import 'package:ecomonitor/services/auth_service.dart';
import 'package:flutter/widgets.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';
import 'package:path/path.dart' as path;

class BinPhotoService implements IBinPhotoService {
  final ApiClient _apiClient;
  final AuthService _authService;

  BinPhotoService(
    this._apiClient,
    this._authService);

  Future<List<BinPhotoResponse>> getAllBinPhoto() async {
    final response = await _apiClient.get('/api/public/v1/BinPhoto/GetAllPhotos');
    return response.data;
  }

  Future<BinPhotoResponse> getBinPhotoById(String id) async {
    final response = await _apiClient.get('/api/public/v1/BinPhoto/GetBinPhotoById',
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
      
      final response = await _apiClient.get('/api/public/v1/BinPhoto/userUploadedPhotos',
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

  Future<List<PhotoMarkersDTO>> markers() async {
    final response = await _apiClient.post('/api/public/v1/BinPhoto/Markers');
    if (response.data != null){
      return (response.data as List)
      .map((dynamic item) => PhotoMarkersDTO.fromJson(item as Map<String, dynamic>)).toList();
    }
    return [];
  }

  // Future<BinPhotoResponse> addBinPhoto(BinPhotoRequest request) async {
  //   final response = await _apiClient.post('api/binphoto',
  //   data: request.toJson());
  //   return BinPhotoResponse.fromJson(response.data);
  // }

  // Future<BinPhotoResponse> uploadWithMetadata(BinPhotoUploadRequest request) async {
  //   //final formData = FormData.fromMap(await request.toFormData());
  //   final formData = await request.toFormData();
  //   final response = await _apiClient.post('/api/public/v1/BinPhoto/UploadWithMetadata',
  //       data: formData);
  //   return BinPhotoResponse.fromJson(response.data);
  // }

  Future<BinPhotoResponse> uploadWithMetadata(BinPhotoUploadRequest request) async {
  final bytes = await request.photo.readAsBytes();
  
  // ✅ ЧИСТЫЙ http вместо Dio!
  var httpRequest = http.MultipartRequest(
    'POST', 
    Uri.parse('http://localhost:5198/api/public/v1/BinPhoto/UploadWithMetadata')
  );
  
  // ✅ RAW bytes с EXIF
  httpRequest.files.add(http.MultipartFile.fromBytes(
    'Photo',
    bytes,
    filename: path.basename(request.photo.path),
  ));
  
  // ✅ List<string> для сервера
  for (int i = 0; i < request.binTypeCode.length; i++) {
    httpRequest.fields['BinTypeCode[$i]'] = request.binTypeCode[i];
  }
  
  httpRequest.fields.addAll({
    'FillLevel': request.fillLevel.toString(),
    'IsOutsideBin': request.isOutsideBin.toString(),
    'Comment': request.comment,
    'TotalBins': request.totalBins.toString(),
  });
  
  final token = await _authService.getAccessToken();
  httpRequest.headers['Authorization'] = 'Bearer $token';
  
  final response = await httpRequest.send();
  final responseBody = await response.stream.bytesToString();
  
  if (response.statusCode == 200) {
    return BinPhotoResponse.fromJson(json.decode(responseBody));
  } else {
    throw Exception('Upload failed: ${response.statusCode} $responseBody');
  }
}

  Future<String> deleteBinPhoto(String binPhotoId) async {
    final response = await _apiClient.delete(
      'api/binphoto/Delete',
      data: {'binPhotoId': binPhotoId},);
    return response.data as String;
  }
}