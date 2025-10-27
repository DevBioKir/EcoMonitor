import 'package:flutter/material.dart';
import 'package:url_launcher/url_launcher.dart';

class AboutAppScreen extends StatelessWidget {
  const AboutAppScreen({super.key});

  static const String yandexTermsUrl = 'https://yandex.ru/legal/maps_api/';

  void _openYandexTerms() async {
    final Uri url = Uri.parse(yandexTermsUrl);
    if (await canLaunchUrl(url)) {
      await launchUrl(url);
    } else {
      debugPrint('Не удалось открыть $yandexTermsUrl');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('О приложении'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Это приложение использует сервисы Яндекс.Карт. '
              'Использование карт и связанных сервисов осуществляется согласно условиям Яндекс.Карт.',
              style: TextStyle(fontSize: 16),
            ),
            TextButton(
              onPressed: _openYandexTerms,
              child: const Text('Условия использования сервисов Яндекс.Карт'),
            ),
          ],
        ),
      ),
    );
  }
}
