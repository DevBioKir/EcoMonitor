import 'package:ecomonitor/models/user/user_response.dart';

class BinPhotoResponse {
  final String id;
  final String fileName;
  final String urlFile;
  final double longitude;
  final double latitude;
  final DateTime uploadedAt;
  final List<String> binTypeId;
  final double fillLevel;
  final bool isOutsideBin;
  final int totalBins;
  final String comment;
  final UserResponse uploadedBy;
  final String uploadedById;

  BinPhotoResponse({
    required this.id,
    required this.fileName,
    required this.urlFile,
    required this.longitude,
    required this.latitude,
    required this.uploadedAt,
    required this.binTypeId,
    required this.fillLevel,
    required this.isOutsideBin,
    required this.totalBins,
    required this.comment,
    required this.uploadedBy,
    required this.uploadedById,
  });

  factory BinPhotoResponse.fromJson(Map<String, dynamic> json) {
    return BinPhotoResponse(
      id: json['Id'], 
      fileName: json['FileName'], 
      urlFile: json['UrlFile'], 
      longitude: (json['Longitude'] as num).toDouble(),
      latitude: (json['Latitude'] as num).toDouble(),
      uploadedAt: DateTime.parse(json['UploadedAt']), 
      binTypeId: List<String>.from(json['binTypeId']), 
      fillLevel: (json['FillLevel'] as num).toDouble(),
      isOutsideBin: json['IsOutsideBin'], 
      totalBins: json['TotalBins'], 
      comment: json['Comment'], 
      uploadedBy: UserResponse.fromJson(json['UploadedBy']), 
      uploadedById: json['uploadedById']);
  }

  Map<String, dynamic> toJson() => {
    'Id' : id,
    'FileName' : fileName,
    'UrlFile' : urlFile,
    'Longitude' : longitude,
    'Latitude' : latitude,
    'UploadedAt' : uploadedAt.toIso8601String(),
    'BinTypeId' : binTypeId.map((e) => e).toList(),
    'FillLevel' : fillLevel,
    'IsOutsideBin' : isOutsideBin,
    'TotalBins' : totalBins,
    'Comment' : comment,
    'UploadedBy' : uploadedBy.toJson(),
    'UploadedById' : uploadedById,
  };

}

