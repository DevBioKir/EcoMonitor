class PhotoMarkersDTO {
  final String id;
  final double latitude;
  final double longitude;
  final String photoUrl;

  PhotoMarkersDTO({
    required this.id,
    required this.latitude,
    required this.longitude,
    required this.photoUrl
  });

  factory PhotoMarkersDTO.fromJson(Map<String, dynamic> json) {
    return PhotoMarkersDTO(
      id: json['id'] as String,
      latitude: json['latitude'] as double,
      longitude: json['longitude'] as double,
      photoUrl: json['photoUrl'] as String,
    );  
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'latitude': latitude,
      'longitude': longitude,
      'photoUrl': photoUrl,
    };
  }
}