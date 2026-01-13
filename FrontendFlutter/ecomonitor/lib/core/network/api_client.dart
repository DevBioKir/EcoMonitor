import 'package:dio/dio.dart';
import 'package:ecomonitor/services/auth_service.dart';
import 'package:flutter/material.dart';

class ApiClient{
  final Dio _dio;
  //final AuthService _authService;

  ApiClient(
    String baseUrl, 
    Future<String?> Function() tokenProvider, //this._authService
    )
      : _dio = Dio(BaseOptions(
          baseUrl: baseUrl,
          connectTimeout: const Duration(seconds: 10),
          receiveTimeout: const Duration(seconds: 10),
          headers: {'Content-Type': 'application/json'},
        )) {
    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          if (options.path.contains('login') || options.path.contains('register')) {
            print('Login без токена');
            handler.next(options);
            return;
          }

          final token = await tokenProvider();
          if (token != null && token.isNotEmpty) {
            options.headers['Authorization'] = 'Bearer $token';
            print('Отправка запроса с токеном Authorization: Bearer $token');
          } else {
            print('Отправка запроса без токена Authorization');
          }
          handler.next(options);
        },
        // onError: (error, handler) async {
        //   if (error.response?.statusCode == 401) {
        //     try{
        //       await _authService.refreshToken();

        //       final opts = error.requestOptions;
        //       opts.headers['Authorization'] = 'Bearer ${await tokenProvider()}';

        //       final response = await _dio.request(
        //         opts.path,
        //         options: Options(
        //           method: opts.method,
        //           headers: opts.headers,
        //         ),
        //       );

        //       return handler.resolve(response);
        //     } catch (e){
        //       print('Refresh token failed: $e');
        //     }
        //   }
        // }
      ),
    );
  }

  Future<Response<T>> get<T>(String path, {Map<String, dynamic>? queryParameters}) => 
    _dio.get<T>(path, queryParameters: queryParameters);
  
  Future<Response<T>> post<T>(String path, {dynamic data, Map<String, dynamic>? headers}) => 
    _dio.post<T>(
      path,
      data: data,
      options: Options(headers: headers),
      );

  Future<Response<T>> put<T>(String path, {dynamic data}) => 
    _dio.put<T>(path, data: data);

  Future<Response<T>> delete<T>(String path, {dynamic data}) => 
    _dio.delete<T>(path, data: data);
}