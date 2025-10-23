import 'dart:io';
import 'package:ecomonitor/core/network/api_client.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_upload_request.dart';
import 'package:ecomonitor/models/bin_type/bin_type_response.dart';
import 'package:ecomonitor/services/bin_photo_service.dart';
import 'package:ecomonitor/services/bin_type_service.dart';
import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';


class AddPhotoScreen extends StatefulWidget {
  @override
  State<AddPhotoScreen> createState() => _AddPhotoScreenState();
}

final BinPhotoService _binPhotoService = BinPhotoService(
  ApiClient("http://localhost:5198/", () async => 'token'));

final BinTypeService _binTypeService = BinTypeService(
  ApiClient("http://localhost:5198/", () async => 'token'));

class _AddPhotoScreenState extends State<AddPhotoScreen> {
  File? _selectedPhoto;
  final _picker = ImagePicker();

  final TextEditingController _fillLevelController = TextEditingController();
  final TextEditingController _commentController = TextEditingController();
  final TextEditingController _totalBins = TextEditingController();
  bool _isOutsideBin = false;
  List<BinTypeResponse> _binTypes = [];
  List<String> _binTypeCode = [];

  @override
  void initState() {
    super.initState();
    _loadBinTypes();
  }

  void _loadBinTypes() async {
    try {
      _binTypes = await _binTypeService.getAllType();
      setState(() {});
    } catch (e) {
      print('Ошибка загрузки типов контейнеров: $e');
    }
  }
  
  Future<void> _pickPhoto() async {
    final pickedFile = await _picker.pickImage(source: ImageSource.gallery);
    if (pickedFile != null) {
      setState(() {
        _selectedPhoto = File(pickedFile.path);
      });
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
      binTypeCode: _binTypeCode,
      fillLevel: double.parse(_fillLevelController.text),
      isOutsideBin: _isOutsideBin,
      comment: _commentController.text,
      totalBins: int.tryParse(_totalBins.text) ?? 1, // или возьмите из поля // подставьте текущий Guid пользователя
    );

    final response = await _binPhotoService.uploadWithMetadata(request);

    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Фото успешно добавлено!')),
    );
    Navigator.pop(context, response); // верните результат на предыдущий экран
  } catch (e) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(content: Text('Ошибка при загрузке: $e')),
    );
  }
}

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Добавить фото')),
      body: Padding(
        padding: EdgeInsets.all(16),
        child: ListView(
          children: [
            ElevatedButton(
              onPressed: _pickPhoto,
              child: Text('Выбрать фото'),
            ),
            SizedBox(height: 10),
            _selectedPhoto != null
                ? Image.file(_selectedPhoto!, height: 200)
                : Text('Фото не выбрано'),
            SizedBox(height: 20),
            TextField(
              controller: _commentController,
              decoration: InputDecoration(labelText: 'Комментарий'),
            ),
            SizedBox(height: 10),
            TextField(
              controller: _fillLevelController,
              keyboardType: TextInputType.number,
              decoration: InputDecoration(labelText: 'Уровень заполнения (число)'),
            ),
            SizedBox(height: 10),
            Row(
              children: [
                Text('Есть мусор вне контейнеров'),
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
            // Здесь добавьте виджеты для ввода binTypeCode (например, мультичекбоксы)
            SizedBox(height: 20),
            Column(
              children: _binTypes.map((binType) {
                return CheckboxListTile(
                  title: Text(binType.name),
                  value: _binTypeCode.contains(binType.id),
                  onChanged: (bool? checked) {
                    setState(() {
                      if (checked == true) {
                        _binTypeCode.add(binType.id);
                      } else {
                        _binTypeCode.remove(binType.id);
                      }
                    });
                  },
                );
              }).toList(),
            ),
            SizedBox(height: 20),
            ElevatedButton(
              onPressed: _submit,
              child: Text('Загрузить фото'),
            ),
          ],
        ),
      ),
    );
  }
}