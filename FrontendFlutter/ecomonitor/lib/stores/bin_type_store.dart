import 'package:ecomonitor/abstractions/ibin_type_service.dart';
import 'package:ecomonitor/models/bin_type/bin_type_response.dart';
import 'package:flutter/material.dart';

class BinTypeStore extends ChangeNotifier {
  final IBinTypeService _service;

  List<BinTypeResponse> _types = [];
  bool _loaded = false;

  List<BinTypeResponse> get types => _types;


  BinTypeStore(this._service);
  
  Future<void> load() async {
    if (_loaded) return;

    _types = await _service.getAllType();
    _loaded = true;
    notifyListeners();
  }

  String nameById(String id) {
    final match = _types.firstWhere(
      (t) => t.id == id,
      orElse: () => BinTypeResponse(id: id, code: '', name: 'Неизвестный тип'),
    );
    return match.name ?? 'Неизвестный тип';
  }

    String nameByCode(String code) {
    final matches = _types.where((t) => t.code == code);
    if (matches.isEmpty) return 'Неизвестный тип';
    return matches.first.name ?? 'Неизвестный тип';
  }
}