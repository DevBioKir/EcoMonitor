import 'package:ecomonitor/models/user/user_response.dart';
import 'package:flutter/material.dart';

class ProfileScreen extends StatelessWidget {
  final UserResponse user;

  const ProfileScreen({
    super.key,
    required this.user
  });

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text("Личный кабинет")),
        body: Padding(
          padding: EdgeInsets.all(16.0),
          child: Card(
            child: ListTile(
              leading: Icon(
                Icons.account_circle,
                size: 48),
                title: Text("${user.firstname} ${user.surname}"),
                subtitle: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text("Email: ${user.email}"),
                  ],
                ),
            ),
          ),),
    );
  }
}