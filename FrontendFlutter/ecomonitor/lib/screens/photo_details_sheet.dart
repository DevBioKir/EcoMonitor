import 'package:ecomonitor/constants/districts_map.dart';
import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';
import 'package:ecomonitor/stores/bin_type_store.dart';
import 'package:flutter/material.dart';
import 'package:flutter_dotenv/flutter_dotenv.dart';
import 'package:provider/provider.dart';

class PhotoDetailsSheet extends StatelessWidget {
  final BinPhotoResponse photo;
  final ScrollController scrollController;

  const PhotoDetailsSheet({
    super.key, 
    required this.photo,
    required this.scrollController
  });

  @override
  Widget build(BuildContext context) {
     // Получаем store с названиями типов
    final binTypeStore = context.watch<BinTypeStore>();

    final binTypeNames = photo.binTypeId
        .map((id) => binTypeStore.nameById(id))
        .join(', ');

    // final uploadedByName = '${photo.uploadedById.firstname} ${photo.uploadedBy.surname}';
    
    return SingleChildScrollView(
      controller: scrollController,
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [

          Center(
            child: Container(
              width: 40,
              height: 4,
              margin: const EdgeInsets.only(bottom: 12),
              decoration: BoxDecoration(
                color: Colors.grey[400],
                borderRadius: BorderRadius.circular(2),
              ),
            ),
          ),

          ClipRRect(
            borderRadius: BorderRadius.circular(12),
            child: SizedBox(
              height: 220, // размерность фото
              width: double.infinity,
              child: Image.network(
                photo.urlFile,
                fit: BoxFit.cover,
                errorBuilder: (_, __, ___) => const Center(
                  child: Icon(Icons.broken_image, size: 48),
                ),
              ),
            ),
          ),

          const SizedBox(height: 12),

          _infoRow(
            Icons.location_on_outlined,
            'Координаты',
            '${photo.latitude.toStringAsFixed(5)}, ${photo.longitude.toStringAsFixed(5)}',
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
            photo.fillLevel.toString(),
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

          _infoRow(
            Icons.person_outline,
            'Загрузил',
            '${photo.uploadedBy.firstname} ${photo.uploadedBy.surname}',
          ),

          if (photo.comment.isNotEmpty) ...[
            const SizedBox(height: 8),
            _infoRow(
              Icons.comment_outlined,
              'Комментарий',
              photo.comment,
            ),
          ],
        ],
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
}