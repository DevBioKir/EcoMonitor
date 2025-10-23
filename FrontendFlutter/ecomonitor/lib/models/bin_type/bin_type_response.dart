class BinTypeResponse {
  final String id;
  final String code;
  final String name;

  BinTypeResponse({
    required this.id,
    required this.code,
    required this.name,
  });

  factory BinTypeResponse.fromJson(Map<String, dynamic> json) {
    return BinTypeResponse(
      id: json['id'] as String,
      code: json['code'] as String,
      name: json['name'] as String,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id' : id,
      'code' : code,
      'name' : name,
    };
  }
}