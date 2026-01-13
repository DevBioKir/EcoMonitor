import 'dart:io';
import 'dart:typed_data';
import 'package:dio/dio.dart';
import 'package:ecomonitor/models/bin_type/bin_type_response.dart';
import 'package:image_picker/image_picker.dart';
import 'package:path/path.dart' as path;

class BinPhotoUploadRequest {
  final XFile photo;
  final List<String> binTypeCode;
  final double fillLevel;
  final bool isOutsideBin;
  final String comment;
  final int totalBins;

  BinPhotoUploadRequest({
    required this.photo,
    required this.binTypeCode,
    required this.fillLevel,
    required this.isOutsideBin,
    required this.comment,
    required this.totalBins,
  });

  // Future<Map<String, dynamic>> toFormData() async {
  //   final Uint8List bytes = await photo.readAsBytes();
    
  //   return {
  //     'Photo': MultipartFile.fromBytes(
  //       bytes, 
  //       filename: path.basename(photo.path),
  //     ),
  //     'BinTypeCode': binTypeCode.join(','),
  //     'FillLevel': fillLevel,
  //     'IsOutsideBin': isOutsideBin,
  //     'Comment': comment,
  //     'TotalBins': totalBins,
  //   };
  // }

  Future<FormData> toFormData() async {
    final bytes = await photo.readAsBytes();
    final extension = path.extension(photo.path).toLowerCase();
    
    DioMediaType? contentType;
    switch (extension) {
      case '.heic':
      case '.heif':
        contentType = DioMediaType('image', 'heic');
        break;
      case '.jpg':
      case '.jpeg':
        contentType = DioMediaType('image', 'jpeg');
        break;
      case '.png':
        contentType = DioMediaType('image', 'png');
        break;
      default:
        contentType = DioMediaType('image', 'jpeg');
    }
    
    FormData formData = FormData();
    formData.files.add(MapEntry(
      'Photo',
      await MultipartFile.fromBytes(
        bytes,
        filename: path.basename(photo.path),
        contentType: contentType,  // ✅ DioMediaType!
      )
    ));

    for (int i = 0; i < binTypeCode.length; i++) {
    formData.fields.add(MapEntry('BinTypeCode[$i]', binTypeCode[i]));
  }
  
    formData.fields.addAll([
      MapEntry('FillLevel', fillLevel.toString()),
      MapEntry('IsOutsideBin', isOutsideBin.toString()),
      MapEntry('Comment', comment),
      MapEntry('TotalBins', totalBins.toString()),
    ]);
  
    return formData;
  }
}

