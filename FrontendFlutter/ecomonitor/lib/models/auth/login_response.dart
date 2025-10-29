class LoginResponse {
  final String accessToken;
  final String refreshToken;
  final int expires;

  LoginResponse({
    required this.accessToken,
    required this.refreshToken,
    required this.expires
  });

  factory LoginResponse.fromJson(Map<String, dynamic> json) {
    return LoginResponse(
      accessToken: json['accessToken'] as String,
      refreshToken: json['refreshToken'] as String,
      expires: json['expires'] as int,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'accessToken': accessToken,
      'refreshToken': refreshToken,
      'expires': expires,
    };
  }
}