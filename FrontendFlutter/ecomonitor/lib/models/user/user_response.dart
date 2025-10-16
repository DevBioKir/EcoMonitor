import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';

class UserResponse {
  final String firstname;
  final String surname;
  final String email;
  final BinPhotoResponse binPhoto;

UserResponse({
  required this.firstname,
  required this.surname,
  required this.email,
  required this.binPhoto,
});

factory UserResponse.fromJson(Map<String, dynamic> json) {
  return UserResponse(
    firstname: json['Firstname'], 
    surname: json['Surname'], 
    email: json['Email'], 
    binPhoto: BinPhotoResponse.fromJson(json['BinPhoto']));
}

Map<String, dynamic> toJson() => {
  'Firstname' : firstname,
  'Surname' : surname,
  'Email' : email,
  'BinPhoto' : binPhoto.toJson(),
  };
}