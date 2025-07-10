package com.ecomonitor

import com.facebook.react.uimanager.SimpleViewManager
import com.facebook.react.uimanager.ThemedReactContext
import com.facebook.react.uimanager.annotations.ReactProp
import com.yandex.mapkit.mapview.MapView
import com.yandex.mapkit.map.CameraPosition
import com.yandex.mapkit.geometry.Point
import android.util.Log
import com.facebook.react.bridge.LifecycleEventListener
import android.view.ViewGroup
import com.yandex.mapkit.MapKitFactory


class YandexMapViewManager : SimpleViewManager<MapView>(), LifecycleEventListener  {

    private var latitude: Double? = null
    private var longitude: Double? = null
    private var mapView: MapView? = null
    private var reactContext: ThemedReactContext? = null

    override fun getName(): String = "YandexMapView"

    override fun createViewInstance(reactContext: ThemedReactContext): MapView {
        this.reactContext = reactContext

        Log.d("YandexMapViewManager", "Creating MapView instance")

        val view = MapView(reactContext)
        mapView = view

        // Добавляем слушатель жизненного цикла
        reactContext.addLifecycleEventListener(this)

        // Запускаем MapView
        try {
            view.onStart()
            MapKitFactory.getInstance().onStart()
            Log.d("YandexMapViewManager", "MapView started successfully")
        } catch (e: Exception) {
            Log.e("YandexMapViewManager", "Error starting MapView: ${e.message}")
        }

        return view
    }

    @ReactProp(name = "latitude")
    fun setLatitude(view: MapView, latitude: Double) {
        Log.d("YandexMapViewManager", "Setting latitude: $latitude")
        this.latitude = latitude
        updateCamera()
    }

    @ReactProp(name = "longitude")
    fun setLongitude(view: MapView, longitude: Double) {
        Log.d("YandexMapViewManager", "Setting longitude: $longitude")
        this.longitude = longitude
        updateCamera()
    }

    private fun updateCamera() {
        val lat = latitude
        val lon = longitude
        val view = mapView

        if (lat != null && lon != null && view != null) {
            try {
                val position = CameraPosition(Point(lat, lon), 14.0f, 0f, 0f)
                view.map.move(position)
                Log.d("YandexMapViewManager", "Camera moved to: $lat, $lon")
            } catch (e: Exception) {
                Log.e("YandexMapViewManager", "Error moving camera: ${e.message}")
            }
        }
    }

    override fun onHostResume() {
        Log.d("YandexMapViewManager", "onHostResume called")
        try {
            mapView?.onStart()
            MapKitFactory.getInstance().onStart()
        } catch (e: Exception) {
            Log.e("YandexMapViewManager", "Error in onHostResume: ${e.message}")
        }
    }

    override fun onHostPause() {
        Log.d("YandexMapViewManager", "onHostPause called")
        try {
            mapView?.onStop()
            MapKitFactory.getInstance().onStop()
        } catch (e: Exception) {
            Log.e("YandexMapViewManager", "Error in onHostPause: ${e.message}")
        }
    }

    override fun onHostDestroy() {
        Log.d("YandexMapViewManager", "onHostDestroy called")
        try {
            mapView?.onStop()
            MapKitFactory.getInstance().onStop()
        } catch (e: Exception) {
            Log.e("YandexMapViewManager", "Error in onHostDestroy: ${e.message}")
        }
    }

    override fun onDropViewInstance(view: MapView) {
        super.onDropViewInstance(view)
        Log.d("YandexMapViewManager", "onDropViewInstance called")
        try {
            reactContext?.removeLifecycleEventListener(this)
            view.onStop()
            mapView = null
            reactContext = null
        } catch (e: Exception) {
            Log.e("YandexMapViewManager", "Error in onDropViewInstance: ${e.message}")
        }
    }
}

