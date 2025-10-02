package com.example.ecomonitor

import android.app.Application
import com.yandex.mapkit.MapKitFactory
import android.util.Log

class App : Application() {
    override fun onCreate() {
        super.onCreate()
        Log.d("YandexMap", "YANDEX_MAPS_API_KEY = ${BuildConfig.YANDEX_MAPS_API_KEY}")
        Log.e("YandexMap", "App started")
        println("App started") 
        // Берём API-ключ из BuildConfig
        //MapKitFactory.setApiKey(BuildConfig.YANDEX_MAPS_API_KEY)
        //MapKitFactory.initialize(this)
    }
}