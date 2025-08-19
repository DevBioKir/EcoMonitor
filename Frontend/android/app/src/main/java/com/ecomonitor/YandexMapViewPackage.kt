package com.ecomonitor

import com.facebook.react.ReactPackage
import com.facebook.react.bridge.ReactApplicationContext
import com.facebook.react.bridge.NativeModule
import com.facebook.react.uimanager.ViewManager

class YandexMapViewPackage : ReactPackage {
    override fun createNativeModules(context: ReactApplicationContext): List<NativeModule> = listOf()

    override fun createViewManagers(context: ReactApplicationContext): List<ViewManager<*, *>> =
        listOf(YandexMapViewManager(context))
}
