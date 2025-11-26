import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_upload_request.dart';
import 'package:ecomonitor/models/paged_result.dart';
import 'package:ecomonitor/models/photo_filter.dart';

abstract class IBinPhotoService {
  Future<List<BinPhotoResponse>> getAllBinPhoto();
  Future<BinPhotoResponse> getBinPhotoById(String id);
  Future<PagedResult<BinPhotoResponse>> getUserPhotos(PhotoFilter filter);
  Future<BinPhotoResponse> uploadWithMetadata(BinPhotoUploadRequest request);
  Future<String> deleteBinPhoto(String binPhotoId);
}