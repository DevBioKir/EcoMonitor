import 'package:ecomonitor/models/user/user_response.dart';
import 'package:ecomonitor/services/auth_service.dart';
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
            ElevatedButton(
              onPressed: () async {
                await authService.logOut(); // Добавь этот метод в AuthService для удаления токена
                Navigator.of(context).popUntil((route) => route.isFirst);
              },
              child: const Text('Выйти из аккаунта'),
            ),
          ],
        ),
      ),
    );
  }
}