import 'dart:io';
import 'package:ecomonitor/abstractions/ibin_photo_service.dart';
import 'package:ecomonitor/abstractions/ibin_type_service.dart';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_upload_request.dart';
import 'package:ecomonitor/models/bin_type/bin_type_response.dart';
import 'package:ecomonitor/services/bin_photo_service.dart';
import 'package:ecomonitor/services/bin_type_service.dart';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';


class AddPhotoScreen extends StatefulWidget {
  final IBinPhotoService binPhotoService;
  final IBinTypeService binTypeService;
  final ApiClient apiClient;

  AddPhotoScreen({
    required this.binPhotoService,
    required this.binTypeService,
    required this.apiClient,
  });

  @override
  State<AddPhotoScreen> createState() => _AddPhotoScreenState();
}

// final BinPhotoService _binPhotoService = BinPhotoService(
//   ApiClient("http://localhost:5198/", () async => 'token'));

// final BinTypeService _binTypeService = BinTypeService(
//   ApiClient("http://localhost:5198/", () async => 'token'));

class _AddPhotoScreenState extends State<AddPhotoScreen> {
  XFile? _selectedPhoto;
  final _picker = ImagePicker();

  final TextEditingController _fillLevelController = TextEditingController();
  final TextEditingController _commentController = TextEditingController();
  final TextEditingController _totalBins = TextEditingController();
  bool _isOutsideBin = false;
  List<BinTypeResponse> _binTypes = [];
  Set<String> _selectedBinTypes = {};

  @override
  void initState() {
    super.initState();
    _loadBinTypes();
  }

  Future<void> _loadBinTypes() async {
    try {
      final binTypeService = BinTypeService(widget.apiClient);  
      _binTypes = await binTypeService.getAllType();
      setState(() {});
    } catch (e) {
      if (mounted) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text('Ошибка загрузки типов: $e')),
        );
      }
    }
  }
  
  Future<void> _pickPhoto() async {
    final pickedFile = await _picker.pickImage(
      source: ImageSource.camera,
      imageQuality: 100,
      requestFullMetadata: true);
    // final pickedFile = await _picker.pickImage(source: ImageSource.gallery);
    if (pickedFile != null) {
      setState(() => _selectedPhoto = pickedFile);
    }
  }

  void _submit() async {
  if (_selectedPhoto == null) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Выберите фото')),
    );
    return;
  }
  if (_fillLevelController.text.isEmpty) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Введите уровень заполнения')),
    );
    return;
  }

  try {
    final request = BinPhotoUploadRequest(
      photo: _selectedPhoto!,
      binTypeCode: _selectedBinTypes.toList(),
      fillLevel: double.parse(_fillLevelController.text),
      isOutsideBin: _isOutsideBin,
      comment: _commentController.text,
      totalBins: int.tryParse(_totalBins.text) ?? 1,
    );

    final response = await widget.binPhotoService.uploadWithMetadata(request);

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Фото успешно добавлено!')),
    );
    Navigator.pop(context, response);
  } catch (e) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Ошибка при загрузке: $e')),
    );
  }
}

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Добавить фото')),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: ListView(
          children: [
            ElevatedButton(
              onPressed: _pickPhoto,
              child: const Text('📸 Сделать фото'),
            ),
            const SizedBox(height: 10),
            _selectedPhoto != null
                ? Image.file(
                  File(_selectedPhoto!.path), 
                  height: 200,)
                : const Text('Фото не сделано', style: TextStyle(color: Colors.grey)),
            const SizedBox(height: 20),
            
            TextField(
              controller: _commentController,
              decoration: const InputDecoration(
                labelText: 'Комментарий',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 10),
            
            TextField(
              controller: _fillLevelController,
              keyboardType: TextInputType.number,
              decoration: const InputDecoration(
                labelText: 'Уровень заполнения (0.0 - 1.0)',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 10),
            
            TextField(
              controller: _totalBins,
              keyboardType: const TextInputType.numberWithOptions(decimal: true),
              decoration: const InputDecoration(
                labelText: 'Количество баков',
                border: OutlineInputBorder(),
              ),
            ),
            const SizedBox(height: 15),
            
            Row(
              children: [
                const Text('🗑️ Мусор вне контейнеров'),
                Checkbox(
                  value: _isOutsideBin,
                  onChanged: (value) {
                    setState(() {
                      _isOutsideBin = value ?? false;
                    });
                  },
                ),
              ],
            ),
            
            const SizedBox(height: 20),
            const Text(
              'Типы контейнеров:',
              style: TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 10),
            Container(
              constraints: const BoxConstraints(maxHeight: 400),
              child: _binTypes.isEmpty
                  ? const Center(child: CircularProgressIndicator())
                  : ListView.builder(
                      shrinkWrap: true,
                      physics: const NeverScrollableScrollPhysics(),
                      itemCount: _binTypes.length,
                      itemBuilder: (context, index) {
                        final binType = _binTypes[index];
                        return CheckboxListTile(
                          title: Text(binType.name ?? 'Без названия'),
                          // subtitle: Text(binType.id), // показывает id
                          value: _selectedBinTypes.contains(binType.code),
                          onChanged: (bool? checked) {
                            setState(() {
                              if (checked == true) {
                                _selectedBinTypes.add(binType.code);
                              } else {
                                _selectedBinTypes.remove(binType.code);
                              }
                            });
                          },
                        );
                      },
                    ),
            ),
            
            const SizedBox(height: 10),
            Text(
              'Выбрано: ${_selectedBinTypes.length} типов',
              style: TextStyle(
                color: _selectedBinTypes.isEmpty ? Colors.red : Colors.green,
                fontWeight: FontWeight.bold,
              ),
            ),
            
            const SizedBox(height: 20),
            SizedBox(
              width: double.infinity,
              child: ElevatedButton(
                onPressed: _submit,
                style: ElevatedButton.styleFrom(padding: const EdgeInsets.symmetric(vertical: 16)),
                child: const Text('Загрузить фото', style: TextStyle(fontSize: 18)),
              ),
            ),
          ],
        ),
      ),
    );
  }
}