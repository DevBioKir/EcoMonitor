class PagedResult<T> {
  final List<T> items;
  final int totalCount;
  final int page;
  final int pageSize;

  PagedResult({
    required this.items,
    required this.totalCount,
    required this.page,
    required this.pageSize,
  });

  int get totalPages => (totalCount / pageSize).ceil();
  bool get hasNextPage => page < totalPages;
  bool get hasPrevious => page  > 1;

  factory PagedResult.fromJson(Map<String, dynamic> json, T Function(dynamic) fromJson) {
    var list = json['items'] as List;
    List<T> itemsList = list.map((i) => fromJson(i)).toList();
    return PagedResult(
      items: itemsList, 
      totalCount: json['totalCount'], 
      page: json['page'], 
      pageSize: json['pageSize'],
    );
  }
}

