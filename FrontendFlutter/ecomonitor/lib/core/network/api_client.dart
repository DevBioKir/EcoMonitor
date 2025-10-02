import 'package:dio/dio.dart';

class ApiClient{
  final Dio _dio;

  ApiClient(String baseUrl, Future<String?> Function() tokenProvider)
      : _dio = Dio(BaseOptions(
          baseUrl: baseUrl,
          connectTimeout: const Duration(seconds: 10),
          receiveTimeout: const Duration(seconds: 10),
          headers: {'Content-Type': 'application/json'},
        )) {
    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) async {
          final token = await tokenProvider();
          if (token != null && token.isNotEmpty) {
            options.headers['Authorization'] = 'Basic $token';
          }
          handler.next(options);
        },
      ),
    );
  }

  Future<Response<T>> get<T>(String path, {Map<String, dynamic>? queryParameters}) => 
    _dio.get<T>(path, queryParameters: queryParameters);
  
  Future<Response<T>> post<T>(String path, {dynamic data}) => 
    _dio.post<T>(path, data: data);

  Future<Response<T>> put<T>(String path, {dynamic data}) => 
    _dio.put<T>(path, data: data);

  Future<Response<T>> delete<T>(String path, {dynamic data}) => 
    _dio.delete<T>(path, data: data);
}