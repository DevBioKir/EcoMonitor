import 'dart:io';

import 'package:dio/dio.dart';

class BinPhotoUploadRequest {
  final File photo;
  final List<String> binTypeCode;
  final double fillLevel;
  final bool isOutsideBin;
  final String comment;
  final int totalBins;
  final String uploadedById;

  BinPhotoUploadRequest({
    required this.photo,
    required this.binTypeCode,
    required this.fillLevel,
    required this.isOutsideBin,
    required this.comment,
    required this.totalBins,
    required this.uploadedById,
  });

  Map<String, dynamic> toFormData() => {
    'Photo': MultipartFile.fromFileSync(photo.path, filename: photo.uri.pathSegments.last),
    'BinTypeCode': binTypeCode,
    'FillLevel': fillLevel,
    'IsOutsideBin': isOutsideBin,
    'Comment': comment,
    'TotalBins': totalBins,
    'UploadedById': uploadedById,
  };
}

