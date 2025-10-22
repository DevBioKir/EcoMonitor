import 'dart:io';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

class AddPhotoScreen extends StatefulWidget {
  @override
  State<AddPhotoScreen> createState() => _AddPhotoScreenState();
}

class _AddPhotoScreenState extends State<AddPhotoScreen> {
  File? _selectedPhoto;
  final _picker = ImagePicker();

  final TextEditingController _fillLevelController = TextEditingController();
  final TextEditingController _commentController = TextEditingController();
  final TextEditingController _totalBins = TextEditingController();
  bool _isOutsideBin = false;
  List<String> _binTypeCode = [];

  Future<void> _pickPhoto() async {
    final pickedFile = await _picker.pickImage(source: ImageSource.gallery);
    if (pickedFile != null) {
      setState(() {
        _selectedPhoto = File(pickedFile.path);
      });
    }
  }

  void _submit() {
    if (_selectedPhoto == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('Выбрать фото')),
      );
      return;
    }
    print('Отправляем фото с метаданными');
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
                Text('Вне контейнера'),
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