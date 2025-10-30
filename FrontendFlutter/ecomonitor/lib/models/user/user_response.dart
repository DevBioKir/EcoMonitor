import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';

class UserResponse {
  final String id;
  final String firstname;
  final String surname;
  final String email;
  final List<BinPhotoResponse>? binPhoto;

UserResponse({
  required this.id,
  required this.firstname,
  required this.surname,
  required this.email,
  this.binPhoto,
});

factory UserResponse.fromJson(Map<String, dynamic> json) {
    return UserResponse(
      id: json['id'] as String,
      firstname: json['firstname'] as String,
      surname: json['surname'] as String,
      email: json['email'] as String,
      binPhoto: json['binPhoto'] != null
          ? (json['binPhoto'] as List)
              .map((e) => BinPhotoResponse.fromJson(e as Map<String, dynamic>))
              .toList()
          : null,
    );
  }

  Map<String, dynamic> toJson() => {
    'id' : id,
    'firstname' : firstname,
    'surname' : surname,
    'email' : email,
    'binPhoto': binPhoto?.map((e) => e.toJson()).toList(),
    };
}