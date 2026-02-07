class PhotoFilter {
    final int page;
    final int pageSize;
    final String sortBy;
    final int? district;
    final bool? onlyOutsideBin;
    final double? minFillLevel;
    final double? maxFillLevel;
    final DateTime? fromDate;
    final DateTime? toDate;
    

    PhotoFilter({
        this.page = 1,
        this.pageSize = 20,
        this.sortBy = 'dateDesc',
        this.district,
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
        'district' : district,
        'onlyOutsideBin' : onlyOutsideBin,
        'minFillLevel' : minFillLevel,
        'maxFillLevel' : maxFillLevel,
        'fromDate' : fromDate?.toUtc().toIso8601String(),
        'toDate' : toDate?.toUtc().toIso8601String(),
    };
}