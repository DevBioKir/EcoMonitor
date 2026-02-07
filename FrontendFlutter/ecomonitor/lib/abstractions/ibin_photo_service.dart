import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_update_request.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_upload_request.dart';
import 'package:ecomonitor/models/markers/photo_markers_dto.dart';
import 'package:ecomonitor/models/paged_result.dart';
import 'package:ecomonitor/models/photo_filter.dart';

abstract class IBinPhotoService {
  Future<List<BinPhotoResponse>> getAllBinPhoto();
  Future<BinPhotoResponse> getBinPhotoById(String id);
  Future<PagedResult<BinPhotoResponse>> getUserPhotos(PhotoFilter filter);
  Future<List<PhotoMarkersDTO>> markers();
  Future<String> uploadWithMetadata(BinPhotoUploadRequest request);
  Future<String> deleteBinPhoto(String binPhotoId);
  Future<void> updatePhoto(String photoId, BinPhotoUpdateRequest request);
  // Future<> getPhotoByCoordinates(latitude: point.latitude,
  //       longitude: point.longitude,);
}