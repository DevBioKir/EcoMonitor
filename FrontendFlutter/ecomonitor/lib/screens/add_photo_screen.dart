import 'dart:io';
import 'package:ecomonitor/abstractions/ibin_photo_service.dart';
import 'package:ecomonitor/abstractions/ibin_type_service.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/constants/district.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_upload_request.dart';
import 'package:ecomonitor/models/bin_type/bin_type_response.dart';
import 'package:ecomonitor/services/bin_photo_service.dart';
import 'package:ecomonitor/services/bin_type_service.dart';
import 'package:flutter/material.dart';
import 'package:geolocator/geolocator.dart';
import 'package:image_picker/image_picker.dart';
import 'package:provider/provider.dart';


// class AddPhotoScreen extends StatefulWidget {
//   final IBinPhotoService binPhotoService;
//   final IBinTypeService binTypeService;
//   final ApiClient apiClient;

//   AddPhotoScreen({
//     required this.binPhotoService,
//     required this.binTypeService,
//     required this.apiClient,
//   });

//   @override
//   State<AddPhotoScreen> createState() => _AddPhotoScreenState();
// }

class AddPhotoScreen extends StatefulWidget {
  final VoidCallback? onPhotoUploaded;

  const AddPhotoScreen({super.key, this.onPhotoUploaded});

  @override
  State<AddPhotoScreen> createState() => _AddPhotoScreenState();
}

class _AddPhotoScreenState extends State<AddPhotoScreen> with WidgetsBindingObserver{
  late IBinPhotoService _binPhotoService;
  late IBinTypeService _binTypeService;

  final _picker = ImagePicker();
  XFile? _selectedPhoto;

  District? _selectedDistrict;
  final _fillLevelController = TextEditingController();
  final  _commentController = TextEditingController();
  final  _totalBinsController = TextEditingController();
  bool _isOutsideBin = false;

  bool _loading = false;
  bool _typesLoaded = false;

  List<BinTypeResponse> _binTypes = [];
  final Set<String> _selectedBinTypes = {};

  double? _fillLevel;
  String? _fillLevelError;

  // Геолокация
  bool _locationEnabled = false;
  bool _checkingLocation = true;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    _binPhotoService = context.read<IBinPhotoService>();
    _binTypeService = context.read<IBinTypeService>();

    if (!_typesLoaded) { 
      _typesLoaded = true;
      _loadBinTypes();
    }
  }

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _checkLocation();
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    _fillLevelController.dispose();
    _commentController.dispose();
    _totalBinsController.dispose();
    super.dispose();
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.resumed) {
      // проверяем геолокацию при возврате на экран
      _checkLocation();
    }
  }

  Future<void> _checkLocation() async {
    setState(() => _checkingLocation = true);

    try {
      final serviceEnabled = await Geolocator.isLocationServiceEnabled();
      if (!serviceEnabled) {
        setState(() {
          _locationEnabled = false;
          _checkingLocation = false;
        });
        return;
      }

      LocationPermission permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.denied) {
        permission = await Geolocator.requestPermission();
      }

      if (permission == LocationPermission.denied ||
          permission == LocationPermission.deniedForever) {
        setState(() {
          _locationEnabled = false;
          _checkingLocation = false;
        });
        return;
      }

      setState(() {
        _locationEnabled = true;
        _checkingLocation = false;
      });
    } catch (_) {
      setState(() {
        _locationEnabled = false;
        _checkingLocation = false;
      });
    }
  }

  // Future<void> _checkLocation() async {
  //   try {
  //     final serviceEnabled = await Geolocator.isLocationServiceEnabled();
  //     if (!serviceEnabled) {
  //       setState(() {
  //         _locationEnabled = false;
  //         _checkingLocation = false;
  //       });
  //       return;
  //     }

  //     LocationPermission permission = await Geolocator.checkPermission();
  //     if (permission == LocationPermission.denied) {
  //       permission = await Geolocator.requestPermission();
  //     }

  //     if (permission == LocationPermission.denied ||
  //         permission == LocationPermission.deniedForever) {
  //       setState(() {
  //         _locationEnabled = false;
  //         _checkingLocation = false;
  //       });
  //       return;
  //     }

  //     setState(() {
  //       _locationEnabled = true;
  //       _checkingLocation = false;
  //     });
  //   } catch (_) {
  //     setState(() {
  //       _locationEnabled = false;
  //       _checkingLocation = false;
  //     });
  //   }
  // }

  Future<void> _loadBinTypes() async {
    try {
      final types = await _binTypeService.getAllType();
      if (!mounted) return;
      setState(() => _binTypes = types);
      //setState(() {});
    } catch (e) {
      if (!mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Ошибка загрузки типов: $e')),
        );
      }
    }
  }
  
  // Future<void> _pickPhoto() async {
  //   final pickedFile = await _picker.pickImage(
  //     source: ImageSource.camera,
  //     imageQuality: 100,
  //     requestFullMetadata: true);
  //   // final pickedFile = await _picker.pickImage(source: ImageSource.gallery);
  //   if (pickedFile != null) {
  //     setState(() => _selectedPhoto = pickedFile);
  //   }
  // }

  bool get _canSubmit {
    return !_loading &&
        //_locationEnabled &&
        _selectedPhoto != null &&
        _selectedDistrict != null &&
        //_fillLevel != null &&
        _selectedBinTypes.isNotEmpty;
  }

  void _submit() async {

    if (!_canSubmit) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Заполните все обязательные поля'),
        ),
      );
      return;
    }

    setState(() => _loading = true);

    // if (_fillLevelController.text.isEmpty) {
    //   ScaffoldMessenger.of(context).showSnackBar(
    //     SnackBar(content: Text('Введите уровень заполнения')),
    //   );
    //   return;
    // }
    // if (_selectedDistrict == null) {
    //   ScaffoldMessenger.of(context).showSnackBar(
    //     SnackBar(content: Text('Выберите район')),
    //   );
    //   return;
    // }

    try {
      final request = BinPhotoUploadRequest(
        photo: _selectedPhoto!,
        district: _selectedDistrict!,
        binTypeCode: _selectedBinTypes.toList(),
        fillLevel: double.parse(_fillLevelController.text),
        isOutsideBin: _isOutsideBin,
        comment: _commentController.text,
        totalBins: int.tryParse(_totalBinsController.text) ?? 1,
      );

      //final response = await widget.binPhotoService.uploadWithMetadata(request);

      await _binPhotoService.uploadWithMetadata(request);

      if (!mounted) return;

      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Фото успешно добавлено')),
      );

      widget.onPhotoUploaded?.call();
      
      Navigator.pop(context);
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Ошибка при загрузке: $e')),
      );
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Добавить фото')),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          ElevatedButton(
            onPressed: () async {
              _selectedPhoto =
                  await _picker.pickImage(source: ImageSource.camera);
              setState(() {});
            },
            child: const Text('Сделать фото'),
          ),

          const SizedBox(height: 12),
          Row(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(
                _checkingLocation
                    ? Icons.hourglass_empty
                    : _locationEnabled
                        ? Icons.check_circle
                        : Icons.location_off,
                color: _checkingLocation
                    ? Colors.grey
                    : _locationEnabled
                        ? Colors.green
                        : Colors.red,
              ),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  _checkingLocation
                      ? 'Проверка геолокации...'
                      : _locationEnabled
                          ? 'Геолокация включена'
                          : 'Геолокация отключена или отсутствует разрешение',
                  style: TextStyle(
                    color: _checkingLocation
                        ? Colors.grey
                        : _locationEnabled
                            ? Colors.green
                            : Colors.red,
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                  ),
                ),
              ),
              if (!_checkingLocation && !_locationEnabled)
                TextButton(
                  onPressed: () => Geolocator.openLocationSettings(),
                  child: const Text('Включить'),
                ),
            ],
          ),
          const SizedBox(height: 12),

          if (_selectedPhoto != null)
            Padding(
              padding: const EdgeInsets.only(top: 12),
              child: Image.file(
                File(_selectedPhoto!.path),
                height: 200,
              ),
            ),

          const SizedBox(height: 12),

          
          // if (_selectedPhoto != null)
          //   Image.file(File(_selectedPhoto!.path), height: 200),

          DropdownButtonFormField<District>(
            initialValue: _selectedDistrict,
            decoration: const InputDecoration(labelText: 'Район *'),
            items: District.values
                .map((d) => DropdownMenuItem(
                      value: d,
                      child: Text(d.name),
                    ))
                .toList(),
            onChanged: (v) => setState(() => _selectedDistrict = v),
          ),

          TextField(
            controller: _fillLevelController,
            keyboardType: const TextInputType.numberWithOptions(decimal: true),
            decoration: InputDecoration(
              labelText: 'Заполненность (0.0 – 1.0) *',
              hintText: 'Например: 0.7',
              // helperText: 'Можно вводить с запятой — она будет заменена на точку',
              errorText: _fillLevelError,
            ),
            onChanged: (v) {
              final normalized = v.replaceAll(',', '.');

              // авто-замена запятой на точку
              if (normalized != v) {
                _fillLevelController.value = TextEditingValue(
                  text: normalized,
                  selection: TextSelection.collapsed(offset: normalized.length),
                );
                return;
              }

              final parsed = double.tryParse(normalized);

              setState(() {
                if (parsed == null) {
                  _fillLevelError = 'Введите число';
                  _fillLevel = null;
                  return;
                }

                if (parsed < 0 || parsed > 1) {
                  _fillLevelError = 'Значение должно быть от 0.0 до 1.0';
                  _fillLevel = null;
                  return;
                }

                // всё корректно
                _fillLevelError = null;
                _fillLevel = parsed;
              });
            },
          ),

          TextField(
            controller: _totalBinsController,
            keyboardType: TextInputType.number,
            decoration: const InputDecoration(labelText: 'Количество баков *'),
          ),

          const SizedBox(height: 12),

          TextField(
            controller: _commentController,
            decoration: const InputDecoration(labelText: 'Комментарий *'),
          ),

          CheckboxListTile(
            title: const Text('Мусор вне контейнеров'),
            value: _isOutsideBin,
            onChanged: (v) => setState(() => _isOutsideBin = v ?? false),
          ),

          const SizedBox(height: 12),

          const Text('Типы контейнеров *',
              style: TextStyle(fontWeight: FontWeight.bold)),

          if (_binTypes.isEmpty)
            const Center(child: CircularProgressIndicator())
          else
            ..._binTypes.map(
              (t) => CheckboxListTile(
                title: Text(t.name ?? ''),
                value: _selectedBinTypes.contains(t.code),
                onChanged: (v) => setState(() {
                  v == true
                      ? _selectedBinTypes.add(t.code)
                      : _selectedBinTypes.remove(t.code);
                }),
              ),
            ),

          const SizedBox(height: 20),

          ElevatedButton(
            onPressed: _canSubmit ? _submit : null,
            child: _loading
                ? const CircularProgressIndicator()
                : const Text('Загрузить'),
          ),
        ],
      ),
    );
  }
}