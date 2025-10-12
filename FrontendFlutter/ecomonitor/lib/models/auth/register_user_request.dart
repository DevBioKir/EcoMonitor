class RegisterUserRequest {
  final String firstname;
  final String surname;
  final String email;
  final String password;

RegisterUserRequest({
  required this.firstname,
  required this.surname,
  required this.email,
  required this.password,
});

Map<String, dynamic> toJson() => {
  'Firstname' : firstname,
  'Surname' : surname,
  'Email' : email,
  'Password' : password,
  };
}