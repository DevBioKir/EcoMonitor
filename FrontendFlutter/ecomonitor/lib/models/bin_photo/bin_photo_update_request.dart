import 'package:dio/dio.dart';
import 'package:ecomonitor/models/user/user_response.dart';
import 'package:image_picker/image_picker.dart';

class BinPhotoUpdateRequest {
  final XFile? photo;
  final int? district;
  final List<String>? binTypeId;
  final String? fillLevel;
  final bool? isOutsideBin;
  final String? comment;
  final int? totalBins;

  BinPhotoUpdateRequest({
    this.photo,
    this.district,
    this.binTypeId,
    this.fillLevel,
    this.isOutsideBin,
    this.comment,
    this.totalBins,
  });
  
Future<FormData> toFormData() async {
    final map = <String, dynamic>{};

    if (photo != null) {
      map['Photo'] = await MultipartFile.fromFile(
        photo!.path,
        filename: photo!.name,
      );
    }

    if (district != null) map['District'] = district;
    if (binTypeId != null) map['BinTypeId'] = binTypeId;
    if (fillLevel != null) map['FillLevel'] = fillLevel;
    if (isOutsideBin != null) map['IsOutsideBin'] = isOutsideBin;
    if (comment != null) map['Comment'] = comment;
    if (totalBins != null) map['TotalBins'] = totalBins;

    return FormData.fromMap(map);
  }

   Map<String, dynamic> toJson() => {
    'district': district,
    'comment': comment,
    'fillLevel': fillLevel,
    'totalBins': totalBins,
    'isOutsideBin': isOutsideBin,
    'binTypeIds': binTypeId,
  };
}