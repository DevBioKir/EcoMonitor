class PhotoFilter {
    final int page;
    final int pageSize;
    final String sortBy;
    final bool? onlyOutsideBin;
    final double? minFillLevel;
    final double? maxFillLevel;
    final DateTime? fromDate;
    final DateTime? toDate;

    PhotoFilter({
        this.page = 1,
        this.pageSize = 20,
        this.sortBy = 'dateDesc',
        this.onlyOutsideBin,
        this.minFillLevel,
        this.maxFillLevel,
        this.fromDate,
        this.toDate,
    });

    Map<String, dynamic> toJson() => {
        'page' : page,
        'pageSize' : pageSize,
        'sortBy' : sortBy,
        'onlyOutsideBin' : onlyOutsideBin,
        'minFillLevel' : minFillLevel,
        'maxFillLevel' : maxFillLevel,
        'fromDate' : fromDate?.toIso8601String(),
        'toDate' : toDate?.toIso8601String(),
    };
}