import 'package:ecomonitor/abstractions/ibin_photo_service.dart';
import 'package:ecomonitor/screens/photo_details_sheet.dart';
import 'package:flutter/material.dart';
import 'package:yandex_maps_mapkit/mapkit.dart' as ymapkit hide TextStyle;

// class MapObjectTapListenerImpl implements MapObjectTapListener {
//   final BuildContext context;
//   final IBinPhotoService binPhotoService;

//   MapObjectTapListenerImpl({required this.context, required this.binPhotoService});

// //   @override
// //   bool onMapObjectTap(MapObject mapObject, Point point) {
// //     print('Marker tapped!');

//   @override
//   bool onMapObjectTap(MapObject mapObject, Point point) {
//     _showPhotoDetails(point);
//     return true;
//   }

//   Future<void> _showPhotoDetails(Point point) async {
//     try {
//       // Здесь делаем запрос к твоему сервису, чтобы получить фото по координатам
//       final photo = await binPhotoService.getPhotoByCoordinates(
//         latitude: point.latitude,
//         longitude: point.longitude,
//       );

//       if (photo == null) {
//         ScaffoldMessenger.of(context).showSnackBar(
//           const SnackBar(content: Text("Фото не найдено")),
//         );
//         return;
//       }

//       showModalBottomSheet(
//         context: context,
//         isScrollControlled: true,
//         builder: (context) => SingleChildScrollView(
//           padding: const EdgeInsets.all(16),
//           child: Column(
//             crossAxisAlignment: CrossAxisAlignment.start,
//             children: [
//               Center(
//                 child: Container(
//                   width: 40,
//                   height: 4,
//                   decoration: BoxDecoration(
//                     color: Colors.grey[300],
//                     borderRadius: BorderRadius.circular(2),
//                   ),
//                 ),
//               ),
//               const SizedBox(height: 16),
//               ClipRRect(
//                 borderRadius: BorderRadius.circular(12),
//                 child: Image.network(
//                   'http://localhost:5198/${photo.urlFile}',
//                   fit: BoxFit.cover,
//                 ),
//               ),
//               const SizedBox(height: 16),
//               Text(photo.fileName, style: const TextStyle(fontSize: 20, fontWeight: FontWeight.bold)),
//               const SizedBox(height: 8),
//               Text('Район: ${photo.districtName}'),
//               Text('Дата: ${photo.uploadedAt.toLocal()}'),
//               if (photo.comment != null && photo.comment!.isNotEmpty) ...[
//                 const SizedBox(height: 8),
//                 Text('Комментарий: ${photo.comment}'),
//               ],
//             ],
//           ),
//         ),
//       );
//     } catch (e) {
//       ScaffoldMessenger.of(context).showSnackBar(
//         SnackBar(content: Text("Ошибка при получении фото: $e")),
//       );
//     }
//   }
// }


class MapObjectTapListenerImpl implements ymapkit.MapObjectTapListener {
  final void Function(String photoId) onTap;
  // final BuildContext context;
  // final IBinPhotoService binPhotoService;
  //final Map<ymapkit.MapObject, String> placemarkPhotoIds;

  MapObjectTapListenerImpl({
    required this.onTap
    // required this.context,
    // required this.binPhotoService
    //  required this.placemarkPhotoIds,
  });

  @override
  bool onMapObjectTap(
    ymapkit.MapObject mapObject, 
    ymapkit.Point point) {
      print('userData = ${mapObject.userData}');
      print('Marker tapped!');

    final photoId = mapObject.userData as String?;
    if (photoId == null) {
      print('❌ photoId is null');
      return true;
    }

    // WidgetsBinding.instance.addPostFrameCallback((_) async {
    //   try{
    //     print('Запрос фото по id = $photoId');
    //     final photo = await binPhotoService.getBinPhotoById(photoId);
    //     print('Фото получено: ${photo?.fileName}, url = ${photo?.urlFile}');

    //     if (!context.mounted) return;

    //     showModalBottomSheet(
    //       context: context, 
    //       useRootNavigator: true,
    //       isScrollControlled: true,
    //       backgroundColor: Colors.white,
    //       builder: (_) => PhotoDetailsSheet(
    //         photo: photo),
    //       );
    //   } catch (e, st) {
    //     print('Ошибка получения фото: $e\n$st');
    //     if (!context.mounted) return;
    //     ScaffoldMessenger.of(context).showSnackBar(
    //       SnackBar(content: Text('Ошибка загрузки фото: $e'))
    //     );
    //   }
    // });
    // // ScaffoldMessenger.of(context).showSnackBar(
    // //   SnackBar(content: Text('Маркер нажат')),
    // // );

    onTap(photoId);
    
    return true;
  }
}