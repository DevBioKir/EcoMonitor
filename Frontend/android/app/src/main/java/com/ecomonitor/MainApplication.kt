package com.ecomonitor

import android.app.Application
import com.facebook.react.PackageList
import com.facebook.react.ReactApplication
import com.facebook.react.ReactHost
import com.facebook.react.ReactNativeApplicationEntryPoint.loadReactNative
import com.facebook.react.ReactNativeHost
import com.facebook.react.ReactPackage
import com.facebook.react.defaults.DefaultReactHost.getDefaultReactHost
import com.facebook.react.defaults.DefaultReactNativeHost
import com.yandex.mapkit.MapKitFactory;
import com.ecomonitor.BuildConfig;
import com.facebook.react.bridge.ReactApplicationContext
import com.facebook.react.bridge.NativeModule
import com.facebook.react.uimanager.ViewManager
import android.util.Log

class MainApplication : Application(), ReactApplication {
  override val reactNativeHost: ReactNativeHost =
    object : DefaultReactNativeHost(this) {
      override fun getUseDeveloperSupport(): Boolean = BuildConfig.DEBUG

      override fun getPackages(): List<ReactPackage> {
        val packages = PackageList(this).packages.toMutableList()
        packages.add(YandexMapViewPackage())
        Log.d("MainApplication", "Total packages: ${packages.size}")
        return packages
      }

      override fun getJSMainModuleName(): String = "index"
      override val isNewArchEnabled: Boolean = BuildConfig.IS_NEW_ARCHITECTURE_ENABLED
      override val isHermesEnabled: Boolean = BuildConfig.IS_HERMES_ENABLED
    }

  override val reactHost: ReactHost
    get() = getDefaultReactHost(applicationContext, reactNativeHost)

  override fun onCreate() {
    super.onCreate()

    Log.d("MainApplication", "Application onCreate() called")

    // Инициализируем MapKit
    try {
      val apiKey = BuildConfig.MAPKIT_API_KEY
      Log.d("MainApplication", "Initializing MapKit with API key (length: ${apiKey.length})")

      MapKitFactory.setApiKey(apiKey)
      MapKitFactory.initialize(this)

      Log.d("MainApplication", "MapKit initialized successfully")
    } catch (e: Exception) {
      Log.e("MainApplication", "Error initializing MapKit: ${e.message}")
      e.printStackTrace()
    }

    loadReactNative(this)
  }
}
