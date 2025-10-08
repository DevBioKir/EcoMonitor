class RegisterUserRequest {
  final String firstname;
  final String surename;
  final String email;
  final String password;

RegisterUserRequest({
  required this.firstname,
  required this.surename,
  required this.email,
  required this.password,
});

Map<String, dynamic> toJson() => {
  'firstname' : firstname,
  'surename' : surename,
  'email' : email,
  'password' : password,
  };
}