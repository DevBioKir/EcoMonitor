import 'package:ecomonitor/constants/districts_map.dart';
import 'package:ecomonitor/models/constants/district.dart' hide District;
import 'package:ecomonitor/models/user/user_response.dart';

class BinPhotoResponse {
  final String id;
  final String fileName;
  final String urlFile;
  final double longitude;
  final double latitude;
  final District district;
  final DateTime uploadedAt;
  final List<String> binTypeId;
  final double fillLevel;
  final bool isOutsideBin;
  final int totalBins;
  final String comment;
  final UserResponse uploadedBy;

  BinPhotoResponse({
    required this.id,
    required this.fileName,
    required this.urlFile,
    required this.longitude,
    required this.latitude,
    required this.district,
    required this.uploadedAt,
    required this.binTypeId,
    required this.fillLevel,
    required this.isOutsideBin,
    required this.totalBins,
    required this.comment,
    required this.uploadedBy,
  });

  factory BinPhotoResponse.fromJson(Map<String, dynamic> json) {
    return BinPhotoResponse(
      id: json['id'] as String,
      fileName: json['fileName']?.toString() ?? '',
      urlFile: json['urlFile']?.toString() ?? '',
      longitude: (json['longitude'] as num).toDouble(),
      latitude: (json['latitude'] as num).toDouble(),
      district: districtFromString(json['district']),
      uploadedAt: DateTime.parse(json['uploadedAt'] as String),
      binTypeId: List<String>.from(json['binTypeId']),
      fillLevel: (json['fillLevel'] as num).toDouble(),
      isOutsideBin: json['isOutsideBin'] as bool,
      totalBins: json['totalBins'] as int,
      comment: json['comment']?.toString() ?? '',
      uploadedBy: UserResponse.fromJson(json['uploadedBy'])
      //uploadedById: json['uploadedBy']?['id']?.toString() ?? '',
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'fileName': fileName,
    'urlFile': urlFile,
    'longitude': longitude,
    'latitude': latitude,
    'district' : district,
    'uploadedAt': uploadedAt.toIso8601String(),
    'binTypeId': binTypeId,
    'fillLevel': fillLevel,
    'isOutsideBin': isOutsideBin,
    'totalBins': totalBins,
    'comment': comment,
    'uploadedBy': uploadedBy.toJson(),
  };

  BinPhotoResponse copyWith({
  String? id,
  String? fileName,
  String? urlFile,
  double? longitude,
  double? latitude,
  District? district,
  DateTime? uploadedAt,
  List<String>? binTypeId,
  double? fillLevel,
  bool? isOutsideBin,
  int? totalBins,
  String? comment,
  UserResponse? uploadedBy,
}) {
  return BinPhotoResponse(
    id: id ?? this.id,
    fileName: fileName ?? this.fileName,
    urlFile: urlFile ?? this.urlFile,
    longitude: longitude ?? this.longitude,
    latitude: latitude ?? this.latitude,
    district: district ?? this.district,
    uploadedAt: uploadedAt ?? this.uploadedAt,
    binTypeId: binTypeId ?? this.binTypeId,
    fillLevel: fillLevel ?? this.fillLevel,
    isOutsideBin: isOutsideBin ?? this.isOutsideBin,
    totalBins: totalBins ?? this.totalBins,
    comment: comment ?? this.comment,
    uploadedBy: uploadedBy ?? this.uploadedBy,
  );
}
}

