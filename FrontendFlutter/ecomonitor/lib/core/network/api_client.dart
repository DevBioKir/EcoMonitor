import 'package:dio/dio.dart';
import 'package:ecomonitor/main.dart';
import 'package:ecomonitor/services/auth_service.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class ApiClient{
  final Dio _dio;
  final Dio _refreshDio;

  final Future<String?> Function() _getAccessToken;
  final Future<bool> Function(Dio _refreshDio) _refreshToken;

  bool _isRefreshing = false;
  final List<void Function()> _refreshQueue = [];

  String get baseUrl => _dio.options.baseUrl;
  //final AuthService _authService;

  ApiClient(
    String baseUrl,
    this._getAccessToken,
    this._refreshToken,
  ) : _dio = Dio(
          BaseOptions(
            baseUrl: baseUrl,
            connectTimeout: const Duration(seconds: 10),
            receiveTimeout: const Duration(seconds: 10),
          ),
        ),
        _refreshDio = Dio(
          BaseOptions(
            baseUrl: baseUrl,
            connectTimeout: const Duration(seconds: 10),
            receiveTimeout: const Duration(seconds: 10),
          ),) {
          print('⚡ ApiClient initialized with baseUrl: $baseUrl');
          print('⚡ _dio.options.baseUrl = ${_dio.options.baseUrl}');
          print('⚡ _refreshDio.options.baseUrl = ${_refreshDio.options.baseUrl}');

    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          debugPrint('➡️ REQUEST: ${options.method} ${options.baseUrl}${options.path}');
          debugPrint('Headers before adding token: ${options.headers}');
          debugPrint('QueryParameters: ${options.queryParameters}');
          debugPrint('Data: ${options.data}');
          
          if (options.extra['skipAuth'] == true){
            return handler.next(options);
          }
          // if (!options.path.contains('login') &&
          //     !options.path.contains('refresh-token') &&
          //     !options.path.contains('Markers')) {
            //if (options.extra['skipAuth'] != true) {
            final token = await _getAccessToken();
            if (token != null && token.isNotEmpty) {
              options.headers['Authorization'] = 'Bearer $token';
            }
          handler.next(options);
        },

        onResponse: (response, handler) {
          debugPrint('✅ RESPONSE: ${response.statusCode} ${response.requestOptions.path}');
          debugPrint('Response data: ${response.data}');
          handler.next(response);
        },

        onError: (error, handler) async {
          debugPrint('❌ ERROR: ${error.message}');
          debugPrint('Status code: ${error.response?.statusCode}');
          debugPrint('Error data: ${error.response?.data}');
          debugPrint('Request path: ${error.requestOptions.path}');
          
          if (error.response?.statusCode != 401 ||
              error.requestOptions.extra['retried'] == true) {
                return handler.next(error);
              }
            //debugPrint('🔄 401 — пробуем refresh token');

            error.requestOptions.extra['retried'] = true;

            //final refreshed = await _refreshToken();

            if (_isRefreshing) {
              _refreshQueue.add(() async {
                final token = await _getAccessToken();
                error.requestOptions.headers['Authorization'] =
                    'Bearer $token';
                final response = await _dio.fetch(error.requestOptions);
                handler.resolve(response);
              });
              return;
            }

            _isRefreshing = true;

            final success = await _refreshToken(_refreshDio);

            _isRefreshing = false;

            if (!success) {
              // Вместо await authService?.logOut();
              navigatorKey.currentContext?.read<AuthService>().logOut();
              return handler.next(error);
            }

            for (final retry in _refreshQueue) {
              retry();
            }
            _refreshQueue.clear();

            final token = await _getAccessToken();
            error.requestOptions.headers['Authorization'] =
                'Bearer $token';

            final response = await _dio.fetch(error.requestOptions);
            handler.resolve(response);
        },
      ),
    );
  }

  Future<Response<T>> get<T>(String path, {Options? options,
    Map<String, dynamic>? queryParameters}) => 
    _dio.get<T>(path, queryParameters: queryParameters);
  
  Future<Response<T>> post<T>(
    String path, {
      dynamic data, 
      Map<String, dynamic>? queryParameters,
      Options? options}) => 
    _dio.post<T>(
      path,
      data: data,
      queryParameters: queryParameters,
      //options: Options(headers: headers),
      );

  Future<Response<T>> put<T>(
    String path, 
    {dynamic data, 
    Map<String, dynamic>? queryParameters}) => 
    _dio.put<T>(
      path, 
      data: data,
      queryParameters: queryParameters);

  Future<Response<T>> delete<T>(String path, {dynamic data}) => 
    _dio.delete<T>(path, data: data);

    Dio get refreshDio => _refreshDio;
}