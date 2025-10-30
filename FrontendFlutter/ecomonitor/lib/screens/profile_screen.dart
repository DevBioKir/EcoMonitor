import 'package:ecomonitor/models/user/user_response.dart';
import 'package:ecomonitor/services/auth_service.dart';
import 'package:ecomonitor/services/user_photos.dart';
import 'package:flutter/material.dart';
import 'package:provider/provider.dart';

class ProfileScreen extends StatelessWidget {
  final UserResponse user;

  const ProfileScreen({
    super.key,
    required this.user
  });

  @override
  Widget build(BuildContext context) {
    final authService = Provider.of<AuthService>(context, listen: false);

    return Scaffold(
      appBar: AppBar(
        title: const Text("Личный кабинет"),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          children: [
            Card(
              child: ListTile(
                leading: const Icon(
                  Icons.account_circle,
                  size: 48,
                ),
                title: Text("${user.firstname} ${user.surname}"),
                subtitle: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text("Email: ${user.email}"),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 20),
            
            // Загруженные фотографии
            Card(
              child: ListTile(
                leading: const Icon(Icons.photo_library),
                title: const Text('Загруженные фотографии'),
                trailing: const Icon(Icons.arrow_forward_ios, size: 16),
                onTap: () {
                  print('Переход к фотографиям');
                  Navigator.push(
                    context,
                    MaterialPageRoute(
                      builder: (_) => UserPhotosScreen(userId: user.id),
                    ),
                  );
                },
              ),
            ),
            const SizedBox(height: 8),
            
            // Отчеты
            Card(
              child: ListTile(
                leading: const Icon(Icons.assessment),
                title: const Text('Отчеты'),
                trailing: const Icon(Icons.arrow_forward_ios, size: 16),
                onTap: () {
                  // Переход к экрану отчетов
                  print('Переход к отчетам');
                  // Navigator.push(context, MaterialPageRoute(builder: (_) => ReportsScreen()));
                },
              ),
            ),
            const SizedBox(height: 8),
            
            // Уведомления
            Card(
              child: ListTile(
                leading: const Icon(Icons.notifications),
                title: const Text('Уведомления'),
                trailing: const Icon(Icons.arrow_forward_ios, size: 16),
                onTap: () {
                  // Переход к экрану уведомлений
                  print('Переход к уведомлениям');
                  // Navigator.push(context, MaterialPageRoute(builder: (_) => NotificationsScreen()));
                },
              ),
            ),
            
            const Spacer(),
            
            ElevatedButton(
              onPressed: () async {
                await authService.logOut();
                Navigator.of(context).popUntil((route) => route.isFirst);
              },
              style: ElevatedButton.styleFrom(
                minimumSize: const Size(double.infinity, 48),
              ),
              child: const Text('Выйти из аккаунта'),
            ),
          ],
        ),
      ),
    );
  }
}