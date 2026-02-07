import 'dart:io';

import 'package:ecomonitor/abstractions/ibin_photo_service.dart';
import 'package:ecomonitor/constants/districts_map.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_update_request.dart';
import 'package:ecomonitor/models/photo_filter.dart';
import 'package:ecomonitor/services/bin_photo_service.dart';
import 'package:ecomonitor/stores/bin_type_store.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:image_picker/image_picker.dart';
import 'package:provider/provider.dart';


enum PhotoSortField { date, fillLevel, totalBins }
enum SortOrder { ascending, descending }


class UserPhotosScreen extends StatefulWidget {
  final String userId;

  const UserPhotosScreen({
    super.key,
    required this.userId,
  });

  @override
  State<UserPhotosScreen> createState() => _UserPhotosScreenState();
}

class _UserPhotosScreenState extends State<UserPhotosScreen> {

  int _currentPage = 1;
  final int _pageSize = 20;
  int _totalPages = 1;

  bool? _onlyOutsideBin;
  double? _minFillLevel;
  double? _maxFillLevel;
  DateTime? _fromDate;
  DateTime? _toDate;

  List<BinPhotoResponse>? _photos;
  bool _isLoading = true;
  String? _error;
  bool _loaded = false;

  late String _baseUrl;
  District? _selectedDistrict;

  PhotoSortField _sortField = PhotoSortField.date;
  SortOrder _sortOrder = SortOrder.descending;
  

  // @override
  // void initState() {
  //   super.initState();
  //   //_loadPhotos();
  // }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    if (!_loaded) {
      _loaded = true;
      _baseUrl = context.read<ApiClient>().baseUrl;
      _loadPhotos();
      
    }
  }

  void _applySorting() {
  if (_photos == null) return;

  _photos!.sort((a, b) {
    int cmp = 0;
    switch (_sortField) {
      case PhotoSortField.date:
        cmp = a.uploadedAt.compareTo(b.uploadedAt);
        break;
      case PhotoSortField.fillLevel:
        cmp = a.fillLevel.compareTo(b.fillLevel);
        break;
      case PhotoSortField.totalBins:
        cmp = a.totalBins.compareTo(b.totalBins);
        break;
    }

    if (_sortOrder == SortOrder.descending) cmp = -cmp;
    return cmp;
  });
}

  Future<void> _loadPhotos() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      print('Начинаем загрузку фотографий для userId: ${widget.userId}');
      
      final photoService = Provider.of<IBinPhotoService>(context, listen: false);
      print('PhotoService получен');

      final filter = PhotoFilter(
        page: _currentPage,
        pageSize: _pageSize,
        onlyOutsideBin: _onlyOutsideBin,
        minFillLevel: _minFillLevel,
        maxFillLevel: _maxFillLevel,
        fromDate: _fromDate,
        toDate: _toDate,
        district: _selectedDistrict?.index,
      );
      
      final pagedResult = await photoService.getUserPhotos(filter);
      print('Фотографии загружены, количество: ${pagedResult.totalCount}');

      setState(() {
        _photos = pagedResult.items;
        _totalPages = pagedResult.totalPages;
        _isLoading = false;
      });
    } catch (e, stackTrace) {
      print('Ошибка при загрузке фотографий: $e');
      print('Stack trace: $stackTrace');
      
      setState(() {
        _error = "Не удалось загрузить фотографии: $e";
        _isLoading = false;
      });
    }
  }

  void _goToPreviousPage() {
    if (_currentPage > 1) {
      setState(() {
        _currentPage--;
      });
      _loadPhotos();
    }
  }

  void _goToNextPage() {
    if (_currentPage < _totalPages) {
      setState(() {
        _currentPage++;
      });
      _loadPhotos();
    }
  }


  @override
  Widget build(BuildContext context) {

    return Scaffold(
      appBar: AppBar(
        title: const Text('Загруженные фотографии'),
        actions: [
          IconButton(
            icon: const Icon(Icons.refresh),
            onPressed: _loadPhotos,
          ),
        ],
      ),
      body: Column(
        children: [
          _buildFilters(),
          Expanded(child: _buildBody()),
          if (!_isLoading && _photos != null && _photos!.isNotEmpty)
            _buildPagination(),
        ],
      ),
    );
  }


  Widget _buildBody() {
    if (_isLoading) {
      return const Center(
        child: CircularProgressIndicator(),
      );
    }

    if (_error != null) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.error_outline, size: 64, color: Colors.red[300]),
            const SizedBox(height: 16),
            Text(_error!, style: const TextStyle(fontSize: 16)),
            const SizedBox(height: 16),
            ElevatedButton(
              onPressed: _loadPhotos,
              child: const Text('Повторить'),
            ),
          ],
        ),
      );
    }

    if (_photos == null || _photos!.isEmpty) {
      return Center(
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Icon(Icons.photo_library_outlined, size: 64, color: Colors.grey[400]),
            const SizedBox(height: 16),
            const Text(
              'Нет загруженных фотографий',
              style: TextStyle(fontSize: 16, color: Colors.grey),
            ),
          ],
        ),
      );
    }

    return ListView.builder(
      padding: const EdgeInsets.all(8),
      itemCount: _photos!.length,
      itemBuilder: (context, index) 
        => _buildPhotoCard(_photos![index]),
    );
  }

  Widget _buildPaginationControls() {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          IconButton(
            icon: const Icon(Icons.arrow_back),
            onPressed: _currentPage > 1 ? _goToPreviousPage : null,
          ),
          Text('Страница $_currentPage из $_totalPages'),
          IconButton(
            icon: const Icon(Icons.arrow_forward),
            onPressed: _currentPage < _totalPages ? _goToNextPage : null,
          ),
        ],
      ),
    );
  }

  Widget _buildPhotoCard(BinPhotoResponse photo) {
    final binTypeStore = context.watch<BinTypeStore>();
    
    final binTypeNames = photo.binTypeId
      .map((id) => binTypeStore.nameById(id))
      .join(', ');

  return Card(
    margin: const EdgeInsets.symmetric(vertical: 8),
    child: Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        AspectRatio(
          aspectRatio: 16 / 9,
          child: Image.network(
            '$_baseUrl/${photo.urlFile}',
            fit: BoxFit.cover,
            errorBuilder: (_, __, ___) =>
                const Center(child: Icon(Icons.broken_image, size: 64)),
          ),
        ),
        //////////////////описание фото//////////////////////
        Padding(
          padding: const EdgeInsets.all(12),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                photo.fileName,
                style: const TextStyle(
                  fontSize: 16,
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 6),

              _infoRow(
                Icons.data_exploration_sharp,
                'Загружено',
                photo.uploadedAt.toString(),
              ),

              _infoRow(
                Icons.location_city_outlined,
                'Район',
                districtNames[photo.district] ?? 'Неизвестно',
              ),

              _infoRow(
                Icons.delete_outline,
                'Всего баков',
                photo.totalBins.toString(),
              ),

              _infoRow(
                Icons.bar_chart_outlined,
                'Заполненность',
                '${(photo.fillLevel)}',
              ),

              _infoRow(
                Icons.category_outlined,
                'Типы баков',
                binTypeNames,
              ),

              _infoRow(
                Icons.warning_amber_outlined,
                'Мусор вне контейнеров ',
                photo.isOutsideBin ? 'Есть' : 'Нет',
              ),

              if (photo.comment.isNotEmpty) ...[
                const SizedBox(height: 8),
                _infoRow(
                  Icons.comment_outlined,
                  'Комментарий',
                  photo.comment,
                ),
              ],

              // Text('Район: ${(photo.district)}'),
              // Text('Количество контейнеров: ${photo.totalBins}'),
              // Text('Заполнение баков: ${(photo.fillLevel)}'),
              // Text('Тип(ы) бака(ов): ${(binTypeNames)}'),
              // Text('Комментарий: ${(photo.comment)}'),
              
              const SizedBox(height: 12),

              Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  TextButton.icon(
                    icon: const Icon(Icons.edit),
                    label: const Text('Редактировать'),
                    onPressed: () => _showEditPhotoDialog(photo),
                  ),
                ],
              ),
            ],
          ),
        ),
      ],
    ),
  );
}

Future<void> _pickNewPhotoAndReopenDialog(BinPhotoResponse photo) async {
  final pickedFile = await ImagePicker().pickImage(
    source: ImageSource.camera,
    preferredCameraDevice: CameraDevice.rear,
  );

  if (pickedFile == null) return;
  if (!mounted) return;

  _showEditPhotoDialog(photo, initialNewPhoto: XFile(pickedFile.path));
}

  void _showEditPhotoDialog(
    BinPhotoResponse photo,
    {XFile? initialNewPhoto}) 
    {
    XFile? newPhotoFile = initialNewPhoto;
    final fillLevelController = TextEditingController(text: (photo.fillLevel).toString());
    final commentController = TextEditingController(text: photo.comment);
    final totalBinsController =
        TextEditingController(text: photo.totalBins.toString());

    double fillLevel = photo.fillLevel;
    int totalBins = photo.totalBins;
    bool isOutsideBin = photo.isOutsideBin;

    District selectedDistrict = photo.district;
    // int selectedDistrict = districtMap.entries
    //   .firstWhere((e) => e.value == photo.district, orElse: () => MapEntry(0, 'Неизвестно'))
    //   .key;

    bool isSaving = false;
    
    final binTypeStore = context.read<BinTypeStore>();
    final selectedBinTypeIds = photo.binTypeId.toSet();

    String? fillLevelError;

    showDialog(
      context: context,
      builder: (context) => StatefulBuilder(
        builder: (context, setLocalState) {
          return AlertDialog(
            title: const Text('Редактировать фото'),
              content: SingleChildScrollView(
              child: Column(
                children: [
                  if (newPhotoFile != null) ...[
                  Image.file(File(newPhotoFile.path), height: 160, fit: BoxFit.cover),
                  const SizedBox(height: 12),
                ],
                  Row(
                    mainAxisAlignment: MainAxisAlignment.spaceAround,
                    children: [
                      ElevatedButton.icon(
                        icon: const Icon(Icons.camera_alt),
                        label: const Text('Сделать фото'),
                        onPressed: () {
                          Navigator.pop(context);
                          _pickNewPhotoAndReopenDialog(photo);
                          //final pickedFile = await ImagePicker().pickImage(
                            //source: ImageSource.camera,
                          // if (pickedFile != null) {
                          //   setLocalState(() => newPhotoFile = XFile(pickedFile.path));
                          // }
                        },
                      ),
                    ],
                  ),  

                  Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: const [
                      Icon(Icons.warning_amber_rounded, color: Colors.red),
                      SizedBox(width: 8),
                      Expanded(
                        child: Text(
                          'Проверьте, включена ли геолокация на мобильном устройстве',
                          style: TextStyle(
                            color: Colors.red,
                            fontSize: 16,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ),
                    ],
                  ),


                  const SizedBox(height: 12),
                  const Align(
                    alignment: Alignment.centerLeft,
                    child: Text(
                      'Типы баков',
                      style: TextStyle(fontWeight: FontWeight.bold),
                    ),
                  ),
                  const SizedBox(height: 8),

                  ...binTypeStore.types.map((type) {
                    final checked =
                        selectedBinTypeIds.contains(type.id);

                    return CheckboxListTile(
                      dense: true,
                      contentPadding: EdgeInsets.zero,
                      title: Text(type.name ?? 'Без названия'),
                      value: checked,
                      onChanged: (value) {
                        setLocalState(() {
                          if (value == true) {
                            selectedBinTypeIds.add(type.id);
                          } else {
                            selectedBinTypeIds.remove(type.id);
                          }
                        });
                      },
                    );
                  }).toList(),

                  DropdownButtonFormField<District>(
                    initialValue: selectedDistrict,
                    decoration: const InputDecoration(
                      labelText: 'Район',
                    ),
                    items: District.values.map((e) {
                      return DropdownMenuItem(
                        value: e,
                        child: Text(districtNames[e]!),
                      );
                    }).toList(),
                    onChanged: (value) {
                      if (value != null) {
                        setLocalState(() => selectedDistrict = value);
                      }
                    },
                  ),
                  TextField(
                    controller: commentController,
                    decoration:
                        const InputDecoration(labelText: 'Комментарий'),
                  ),
                  const SizedBox(height: 8),
                  TextField(
                    controller: fillLevelController,
                    keyboardType: const TextInputType.numberWithOptions(decimal: true),
                    decoration: InputDecoration(
                      labelText: 'Заполненность (0.0 – 1.0)',
                      //hintText: 'Например: 0.75',
                      //helperText: 'Используйте точку (.), не запятую',
                      errorText: fillLevelError,
                    ),
                    onChanged: (v) {
                      setLocalState(() {
                        final normalized = v.replaceAll(',', '.');

                        if (normalized != v) {
                          fillLevelController.value = TextEditingValue(
                            text: normalized,
                            selection: TextSelection.collapsed(
                              offset: normalized.length,
                            ),
                          );
                          return;
                        }

                        final parsed = double.tryParse(normalized);

                        if (parsed == null) {
                          fillLevelError = 'Введите число';
                          return;
                        }

                        if (parsed < 0 || parsed > 1) {
                          fillLevelError = 'Значение должно быть от 0.0 до 1.0';
                          return;
                        }

                        fillLevelError = null;
                        fillLevel = parsed;
                      });
                    },
                  ),
                  // Slider(
                  //   value: fillLevel,
                  //   min: 0,
                  //   max: 1,
                  //   divisions: 100,
                  //   label: '${(fillLevel)}',
                  //   onChanged: (v) => setLocalState(() => fillLevel = v),
                  // ),
                  TextField(
                    controller: totalBinsController,
                    keyboardType: TextInputType.number,
                    decoration:
                        const InputDecoration(labelText: 'Кол-во баков'),
                    onChanged: (v) =>
                        totalBins = int.tryParse(v) ?? totalBins,
                  ),
                  SwitchListTile(
                    title: const Text('Вне контейнера'),
                    value: isOutsideBin,
                    onChanged: (v) =>
                        setLocalState(() => isOutsideBin = v),
                  ),
                ],
              ),
              ),
            actions: [
              TextButton(
                onPressed: () => Navigator.pop(context),
                child: const Text('Отмена'),
              ),
              ElevatedButton(
                onPressed: isSaving
                    ? null
                    : () async {
                        setLocalState(() => isSaving = true);
                        try {
                          final request = BinPhotoUpdateRequest(
                            photo: newPhotoFile,
                            district: selectedDistrict.index,
                            comment: commentController.text,
                            fillLevel: fillLevel.toString(),
                            totalBins: totalBins,
                            isOutsideBin: isOutsideBin,
                            binTypeId: selectedBinTypeIds.toList(),
                          );

                          await context
                              .read<IBinPhotoService>()
                              .updatePhoto(photo.id, request);

                          await _loadPhotos();

                          // setState(() {
                          //   final index = _photos!
                          //       .indexWhere((p) => p.id == photo.id);
                          //   if (index != -1) {
                          //     _photos![index] = photo.copyWith(
                          //       urlFile: updatedPhoto.urlFile,
                          //       district: selectedDistrict,
                          //       comment: request.comment ?? photo.comment,
                          //       fillLevel:
                          //           double.tryParse(request.fillLevel!) ??
                          //               photo.fillLevel,
                          //       totalBins:
                          //           request.totalBins ?? photo.totalBins,
                          //       isOutsideBin: request.isOutsideBin ??
                          //           photo.isOutsideBin,
                          //       binTypeId: selectedBinTypeIds.toList(),
                          //     );
                          //   }
                          // });

                          Navigator.pop(context);
                        } finally {
                          setLocalState(() => isSaving = false);
                        }
                      },
                child: const Text('Сохранить'),
              ),
            ],
          );
        },
      ),
    );
  }

//   void _showEditPhotoDialog(BinPhotoResponse photo) {
//   final commentController = TextEditingController(text: photo.comment);
//   double fillLevel = photo.fillLevel;
//   int totalBins = photo.totalBins;
//   bool isOutsideBin = photo.isOutsideBin;

//   showDialog(
//     context: context,
//     builder: (context) {
//       return AlertDialog(
//         title: const Text('Редактировать фото'),
//         content: SingleChildScrollView(
//           child: Column(
//             mainAxisSize: MainAxisSize.min,
//             children: [
//               TextField(
//                 controller: commentController,
//                 decoration: const InputDecoration(labelText: 'Комментарий'),
//               ),
//               const SizedBox(height: 8),
//               Row(
//                 children: [
//                   const Text('Заполнение:'),
//                   Expanded(
//                     child: Slider(
//                       value: fillLevel,
//                       onChanged: (val) => fillLevel = val,
//                       min: 0,
//                       max: 1,
//                       divisions: 100,
//                       label: '${(fillLevel * 100).round()}%',
//                     ),
//                   ),
//                 ],
//               ),
//               Row(
//                 children: [
//                   const Text('Количество баков:'),
//                   const SizedBox(width: 8),
//                   Expanded(
//                     child: TextField(
//                       keyboardType: TextInputType.number,
//                       controller: TextEditingController(text: totalBins.toString()),
//                       onChanged: (val) => totalBins = int.tryParse(val) ?? totalBins,
//                     ),
//                   ),
//                 ],
//               ),
//               Row(
//                 children: [
//                   const Text('Вне контейнера'),
//                   Switch(
//                     value: isOutsideBin,
//                     onChanged: (val) => isOutsideBin = val,
//                   ),
//                 ],
//               ),
//             ],
//           ),
//         ),
//         actions: [
//           TextButton(
//             onPressed: () => Navigator.of(context).pop(),
//             child: const Text('Отмена'),
//           ),
//           ElevatedButton(
//             onPressed: () async {
//               final request = BinPhotoUpdateRequest(
//                 comment: commentController.text,
//                 fillLevel: fillLevel.toString(),
//                 totalBins: totalBins,
//                 isOutsideBin: isOutsideBin,
//               );

//               try {
//                 final service = Provider.of<IBinPhotoService>(context, listen: false);
//                 await service.updatePhoto(photo.id, request);

//                 // Обновляем локальный список
//                 setState(() {
//                   final index = _photos!.indexWhere((p) => p.id == photo.id);
//                   if (index != -1) {
//                     _photos![index] = photo.copyWith(
//                       comment: request.comment,
//                       fillLevel: request.fillLevel != null
//                         ? double.tryParse(request.fillLevel!) ?? photo.fillLevel
//                         : photo.fillLevel,
//                       totalBins: request.totalBins,
//                       isOutsideBin: request.isOutsideBin,
//                     );
//                   }
//                 });

//                 Navigator.of(context).pop();
//                 ScaffoldMessenger.of(context).showSnackBar(
//                   const SnackBar(content: Text('Фото обновлено')),
//                 );
//               } catch (e) {
//                 print('Ошибка при обновлении фото: $e');
//                 ScaffoldMessenger.of(context).showSnackBar(
//                   const SnackBar(content: Text('Не удалось обновить фото')),
//                 );
//               }
//             },
//             child: const Text('Сохранить'),
//           ),
//         ],
//       );
//     },
//   );
// }

  String _formatDate(DateTime date) => '${date.day}.${date.month}.${date.year}';

  void _showPhotoDetails(BinPhotoResponse photo) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (_) => Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Image.network('$_baseUrl/${photo.urlFile}'),
            const SizedBox(height: 12),
            ElevatedButton.icon(
              icon: const Icon(Icons.edit),
              label: const Text('Редактировать'),
              onPressed: () {
                Navigator.pop(context);
                _showEditPhotoDialog(photo);
              },
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildPagination() {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        IconButton(
          icon: const Icon(Icons.arrow_back),
          onPressed: _currentPage > 1
              ? () {
                  _currentPage--;
                  _loadPhotos();
                }
              : null,
        ),
        Text('$_currentPage / $_totalPages'),
        IconButton(
          icon: const Icon(Icons.arrow_forward),
          onPressed: _currentPage < _totalPages
              ? () {
                  _currentPage++;
                  _loadPhotos();
                }
              : null,
        ),
      ],
    );
  }

  Widget _buildFilters() {
    return Card(
    margin: const EdgeInsets.all(8),
    child: Padding(
      padding: const EdgeInsets.all(12),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [

          /// Только вне контейнера
          CheckboxListTile(
            contentPadding: EdgeInsets.zero,
            title: const Text('Только вне контейнера'),
            value: _onlyOutsideBin ?? false,
            onChanged: (v) {
              setState(() {
                _onlyOutsideBin = v;
                _currentPage = 1;
              });
              _loadPhotos();
            },
          ),

          const SizedBox(height: 8),

          /// Заполненность
          // Row(
          //   children: [
          //     const Text('Заполненность:'),
          //     const SizedBox(width: 12),
          //     SizedBox(
          //       width: 60,
          //       child: TextField(
          //         keyboardType: TextInputType.number,
          //         decoration: const InputDecoration(
          //           labelText: 'От',
          //           isDense: true,
          //         ),
          //         onSubmitted: (v) {
          //           setState(() {
          //             _minFillLevel =
          //                 v.isNotEmpty ? double.tryParse(v) : null;
          //             _currentPage = 1;
          //           });
          //           _loadPhotos();
          //         },
          //       ),
          //     ),
          //     const SizedBox(width: 12),
          //     SizedBox(
          //       width: 60,
          //       child: TextField(
          //         keyboardType: TextInputType.number,
          //         decoration: const InputDecoration(
          //           labelText: 'До',
          //           isDense: true,
          //         ),
          //         onSubmitted: (v) {
          //           setState(() {
          //             _maxFillLevel =
          //                 v.isNotEmpty ? double.tryParse(v) : null;
          //             _currentPage = 1;
          //           });
          //           _loadPhotos();
          //         },
          //       ),
          //     ),
          //   ],
          // ),

          const SizedBox(height: 12),

          /// Район
          DropdownButtonFormField<District>(
            decoration: const InputDecoration(
              labelText: 'Район',
            ),
            initialValue: _selectedDistrict,
            items: District.values.map((d) {
              return DropdownMenuItem(
                value: d,
                child: Text(districtNames[d]!),
              );
            }).toList(),
            onChanged: (value) {
              setState(() {
                _selectedDistrict = value;
                _currentPage = 1;
              });
              _loadPhotos();
            },
          ),

          const SizedBox(height: 12),

          /// Дата
          Row(
            children: [
              Expanded(
                child: OutlinedButton(
                  child: Text(
                    _fromDate == null
                        ? 'Дата от'
                        : 'От: ${_formatDate(_fromDate!)}',
                  ),
                  onPressed: () async {
                    final date = await showDatePicker(
                      context: context,
                      firstDate: DateTime(2020),
                      lastDate: DateTime.now(),
                      initialDate: _fromDate ?? DateTime.now(),
                    );

                    if (date != null) {
                      setState(() {
                        _fromDate = DateTime.utc(
                          date.year,
                          date.month,
                          date.day,
                          0,
                          0,
                          0,
                        );
                        _currentPage = 1;
                      });
                      _loadPhotos();
                    }
                  },
                ),
              ),

              const SizedBox(width: 8),

              Expanded(
                child: OutlinedButton(
                  child: Text(
                    _toDate == null
                        ? 'Дата до'
                        : 'До: ${_formatDate(_toDate!)}',
                  ),
                  onPressed: () async {
                    final date = await showDatePicker(
                      context: context,
                      firstDate: DateTime(2020),
                      lastDate: DateTime.now(),
                      initialDate: _toDate ?? DateTime.now(),
                    );

                    if (date != null) {
                      setState(() {
                        _toDate = DateTime.utc(
                          date.year,
                          date.month,
                          date.day,
                          23,
                          59,
                          59,
                          999,
                        );
                        _currentPage = 1;
                      });
                      _loadPhotos();
                    }
                  },
                ),
              ),
            ],
          ),

          const SizedBox(height: 8),

          Row(
            children: [
              Expanded(
                child: DropdownButton<PhotoSortField>(
                  value: _sortField,
                  isExpanded: true,
                  items: const [
                    DropdownMenuItem(
                      value: PhotoSortField.date,
                      child: Text('Дата загрузки'),
                    ),
                    DropdownMenuItem(
                      value: PhotoSortField.fillLevel,
                      child: Text('Заполненность'),
                    ),
                    DropdownMenuItem(
                      value: PhotoSortField.totalBins,
                      child: Text('Количество баков'),
                    ),
                  ],
                  onChanged: (value) {
                    if (value != null) {
                      setState(() {
                        _sortField = value;
                        _applySorting();
                      });
                    }
                  },
                ),
              ),
              IconButton(
                icon: Icon(
                  _sortOrder == SortOrder.ascending
                      ? Icons.arrow_upward
                      : Icons.arrow_downward,
                ),
                onPressed: () {
                  setState(() {
                    _sortOrder = _sortOrder == SortOrder.ascending
                        ? SortOrder.descending
                        : SortOrder.ascending;
                    _applySorting();
                  });
                },
              ),
            ],
          ),

          const SizedBox(height: 8),

          /// Сброс
          Align(
            alignment: Alignment.centerRight,
            child: TextButton(
              child: const Text('Сбросить фильтры'),
              onPressed: () {
                setState(() {
                  _onlyOutsideBin = null;
                  _minFillLevel = null;
                  _maxFillLevel = null;
                  _fromDate = null;
                  _toDate = null;
                  _selectedDistrict = null;
                  _currentPage = 1;
                });
                _loadPhotos();
              },
            ),
          ),
        ],
      ),
    ),
  );
  }

  Widget _infoRow(IconData icon, String title, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Icon(icon, size: 20, color: Colors.grey[700]),
          const SizedBox(width: 8),
          Text(
            '$title: ',
            style: const TextStyle(fontWeight: FontWeight.w600),
          ),
          Expanded(
            child: Text(value),
          ),
        ],
      ),
    );
  }

//   Widget _buildFilters() {
//   return Padding(
//     padding: const EdgeInsets.all(8.0),
//     child: Column(
//       children: [
//         Row(
//           children: [
//             Checkbox(
//               value: _onlyOutsideBin ?? false,
//               onChanged: (val) {
//                 setState(() {
//                   _onlyOutsideBin = val;
//                   _currentPage = 1;
//                 });
//                 _loadPhotos();
//               },
//             ),
//             const Text('Только вне контейнера'),
//           ],
//         ),
//         Row(
//           children: [
//             Text('Мин. заполнение:'),
//             SizedBox(
//               width: 60,
//               child: TextField(
//                 keyboardType: TextInputType.number,
//                 decoration: const InputDecoration(hintText: '%'),
//                 onSubmitted: (val) {
//                   final v = double.tryParse(val);
//                   setState(() {
//                     _minFillLevel = v != null ? v / 100 : null;
//                     _currentPage = 1;
//                   });
//                   _loadPhotos();
//                 },
//               ),
//             ),
//             const SizedBox(width: 16),
//             Text('Макс. заполнение:'),
//             SizedBox(
//               width: 60,
//               child: TextField(
//                 keyboardType: TextInputType.number,
//                 decoration: const InputDecoration(hintText: '%'),
//                 onSubmitted: (val) {
//                   final v = double.tryParse(val);
//                   setState(() {
//                     _maxFillLevel = v != null ? v / 100 : null;
//                     _currentPage = 1;
//                   });
//                   _loadPhotos();
//                 },
//               ),
//             ),
//           ],
//         ),
//       ],
//     ),
//   );
// }

  // Widget _buildDetailRow(String label, String value) {
  //   return Padding(
  //     padding: const EdgeInsets.symmetric(vertical: 8),
  //     child: Row(
  //       crossAxisAlignment: CrossAxisAlignment.start,
  //       children: [
  //         SizedBox(
  //           width: 150,
  //           child: Text(
  //             label,
  //             style: TextStyle(
  //               color: Colors.grey[600],
  //               fontWeight: FontWeight.w500,
  //             ),
  //           ),
  //         ),
  //         Expanded(
  //           child: Text(
  //             value,
  //             style: const TextStyle(fontWeight: FontWeight.w500),
  //           ),
  //         ),
  //       ],
  //     ),
  //   );
  // }
}