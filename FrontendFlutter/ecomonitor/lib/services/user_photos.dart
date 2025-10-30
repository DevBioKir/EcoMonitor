import 'package:ecomonitor/models/bin_photo/bin_photo_response.dart';
import 'package:ecomonitor/services/bin_photo_service.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

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
  List<BinPhotoResponse>? _photos;
  bool _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadPhotos();
  }

  Future<void> _loadPhotos() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    try {
      print('Начинаем загрузку фотографий для userId: ${widget.userId}');
      
      final photoService = Provider.of<BinPhotoService>(context, listen: false);
      print('PhotoService получен');
      
      final photos = await photoService.getUserPhotos(widget.userId);
      print('Фотографии загружены, количество: ${photos.length}');

      setState(() {
        _photos = photos;
        _isLoading = false;
      });
    } catch (e, stackTrace) {
      print('Ошибка при загрузке фотографий: $e');
      print('Stack trace: $stackTrace');
      
      setState(() {
        _error = "Не удалось загрузить фотографии: $e"; // Покажите конкретную ошибку
        _isLoading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text("Загруженные фотографии"),
        actions: [
          IconButton(
            onPressed: _loadPhotos, 
            icon: const Icon(Icons.refresh),
            ),
        ],
      ),
      body: _buildBody(),
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
      itemBuilder: (context, index) {
        final photo = _photos![index];
        return _buildPhotoCard(photo);
      },
    );
  }

  Widget _buildPhotoCard(BinPhotoResponse photo) {
    return Card(
      margin: const EdgeInsets.symmetric(vertical: 8, horizontal: 4),
      child: InkWell(
        onTap: () => _showPhotoDetails(photo),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Изображение
            AspectRatio(
              aspectRatio: 16 / 9,
              child: Image.network(
                'http://localhost:5198/${photo.urlFile}', // Замените на ваш baseUrl
                fit: BoxFit.cover,
                errorBuilder: (context, error, stackTrace) {
                  return Container(
                    color: Colors.grey[300],
                    child: const Icon(Icons.broken_image, size: 64),
                  );
                },
                loadingBuilder: (context, child, loadingProgress) {
                  if (loadingProgress == null) return child;
                  return Center(
                    child: CircularProgressIndicator(
                      value: loadingProgress.expectedTotalBytes != null
                          ? loadingProgress.cumulativeBytesLoaded /
                              loadingProgress.expectedTotalBytes!
                          : null,
                    ),
                  );
                },
              ),
            ),
            
            // Информация о фото
            Padding(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    photo.fileName,
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 16,
                    ),
                  ),
                  const SizedBox(height: 8),
                  
                  Row(
                    children: [
                      Icon(Icons.calendar_today, size: 16, color: Colors.grey[600]),
                      const SizedBox(width: 4),
                      Text(
                        _formatDate(photo.uploadedAt),
                        style: TextStyle(color: Colors.grey[600], fontSize: 14),
                      ),
                      const SizedBox(width: 16),
                      Icon(Icons.delete, size: 16, color: Colors.grey[600]),
                      const SizedBox(width: 4),
                      Text(
                        '${photo.totalBins} контейнеров',
                        style: TextStyle(color: Colors.grey[600], fontSize: 14),
                      ),
                    ],
                  ),
                  
                  if (photo.comment != null && photo.comment!.isNotEmpty) ...[
                    const SizedBox(height: 8),
                    Text(
                      photo.comment!,
                      maxLines: 2,
                      overflow: TextOverflow.ellipsis,
                      style: TextStyle(color: Colors.grey[700]),
                    ),
                  ],
                  
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      _buildBadge(
                        'Заполнение: ${(photo.fillLevel * 100).toInt()}%',
                        Colors.blue,
                      ),
                      const SizedBox(width: 8),
                      if (photo.isOutsideBin)
                        _buildBadge('Вне контейнера', Colors.orange),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildBadge(String text, Color color) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: color.withOpacity(0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: color.withOpacity(0.3)),
      ),
      child: Text(
        text,
        style: TextStyle(
          color: color,
          fontSize: 12,
          fontWeight: FontWeight.w500,
        ),
      ),
    );
  }

  String _formatDate(DateTime date) {
    return '${date.day}.${date.month}.${date.year}';
  }

  void _showPhotoDetails(BinPhotoResponse photo) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      builder: (context) => DraggableScrollableSheet(
        initialChildSize: 0.9,
        minChildSize: 0.5,
        maxChildSize: 0.95,
        expand: false,
        builder: (context, scrollController) {
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
                    decoration: BoxDecoration(
                      color: Colors.grey[300],
                      borderRadius: BorderRadius.circular(2),
                    ),
                  ),
                ),
                const SizedBox(height: 16),
                
                // Полное изображение
                ClipRRect(
                  borderRadius: BorderRadius.circular(12),
                  child: Image.network(
                    'http://localhost:5198/${photo.urlFile}',
                    fit: BoxFit.cover,
                  ),
                ),
                
                const SizedBox(height: 16),
                Text(
                  photo.fileName,
                  style: const TextStyle(
                    fontSize: 20,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                
                const SizedBox(height: 16),
                _buildDetailRow('Дата загрузки', _formatDate(photo.uploadedAt)),
                _buildDetailRow('Координаты', 
                    '${photo.latitude.toStringAsFixed(6)}, ${photo.longitude.toStringAsFixed(6)}'),
                _buildDetailRow('Заполнение', '${(photo.fillLevel * 100).toInt()}%'),
                _buildDetailRow('Количество контейнеров', '${photo.totalBins}'),
                _buildDetailRow('Вне контейнера', photo.isOutsideBin ? 'Да' : 'Нет'),
                
                if (photo.comment != null && photo.comment!.isNotEmpty) ...[
                  const SizedBox(height: 16),
                  const Text(
                    'Комментарий',
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 16,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(photo.comment!),
                ],
              ],
            ),
          );
        },
      ),
    );
  }

  Widget _buildDetailRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 150,
            child: Text(
              label,
              style: TextStyle(
                color: Colors.grey[600],
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: const TextStyle(fontWeight: FontWeight.w500),
            ),
          ),
        ],
      ),
    );
  }
}