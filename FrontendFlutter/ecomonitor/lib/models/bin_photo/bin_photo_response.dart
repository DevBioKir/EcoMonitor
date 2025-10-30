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
    required this.uploadedById,
  });

  factory BinPhotoResponse.fromJson(Map<String, dynamic> json) {
    return BinPhotoResponse(
      id: json['id'] as String,
      fileName: json['fileName'] as String,
      urlFile: json['urlFile'] as String,
      longitude: (json['longitude'] as num).toDouble(),
      latitude: (json['latitude'] as num).toDouble(),
      uploadedAt: DateTime.parse(json['uploadedAt'] as String),
      binTypeId: List<String>.from(json['binTypeId']),
      fillLevel: (json['fillLevel'] as num).toDouble(),
      isOutsideBin: json['isOutsideBin'] as bool,
      totalBins: json['totalBins'] as int,
      comment: json['comment'] as String,
      uploadedById: json['uploadedById'] as String,
    );
  }

  Map<String, dynamic> toJson() => {
    'id': id,
    'fileName': fileName,
    'urlFile': urlFile,
    'longitude': longitude,
    'latitude': latitude,
    'uploadedAt': uploadedAt.toIso8601String(),
    'binTypeId': binTypeId,
    'fillLevel': fillLevel,
    'isOutsideBin': isOutsideBin,
    'totalBins': totalBins,
    'comment': comment,
    'uploadedById': uploadedById,
  };
}

