package com.example.ecomonitor

import io.flutter.embedding.android.FlutterActivity
import com.yandex.mapkit.MapKitFactory

class MainActivity: FlutterActivity() {
    override fun onCreate(savedInstanceState: android.os.Bundle?) {
        super.onCreate(savedInstanceState)
        MapKitFactory.setApiKey("b435f7c5-a250-4eb7-a2f8-3fff029ceb53")  // Замените на ваш API-ключ
        MapKitFactory.initialize(this)
    }
}