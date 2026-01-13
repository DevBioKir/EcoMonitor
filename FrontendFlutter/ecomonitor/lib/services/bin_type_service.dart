import 'package:ecomonitor/abstractions/ibin_type_service.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/bin_type/bin_type_response.dart';
import 'package:ecomonitor/services/bin_photo_service.dart';

class BinTypeService implements IBinTypeService {
  final ApiClient _apiClient;

  BinTypeService(this._apiClient);

  Future<List<BinTypeResponse>> getAllType() async {
    final response = await _apiClient.get('/api/public/v1/BinType/GetAllBinTypes');
    print('Получение типов баков');
    return (response.data as List)
                  .map((item) => BinTypeResponse.fromJson(item)).toList();
  } 
}