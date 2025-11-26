import 'package:ecomonitor/models/bin_type/bin_type_response.dart';

abstract class IBinTypeService {
  Future<List<BinTypeResponse>> getAllType();
}