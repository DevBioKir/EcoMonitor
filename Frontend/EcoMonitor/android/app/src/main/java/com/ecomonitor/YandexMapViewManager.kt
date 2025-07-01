package com.ecomonitor

import com.facebook.react.uimanager.SimpleViewManager
import com.facebook.react.uimanager.ThemedReactContext
import com.facebook.react.uimanager.annotations.ReactProp
import com.yandex.mapkit.mapview.MapView
import com.yandex.mapkit.map.CameraPosition
import com.yandex.mapkit.geometry.Point
import android.util.Log

class YandexMapViewManager : SimpleViewManager<MapView>() {

    override fun getName(): String = "YandexMapView"

    override fun createViewInstance(reactContext: ThemedReactContext): MapView {
        val mapView = MapView(reactContext)
        Log.d("MapKit", "MapView created")

        // Принудительно перемещаем камеру — тест
        // val point = Point(55.751244, 37.618423) // Москва
        // val position = CameraPosition(point, 14.0f, 0.0f, 0.0f)
        // mapView.map.move(position)

        val position = CameraPosition(
        Point(55.751244, 37.618423),
        14.0f,
        0.0f,
        0.0f
        )
        mapView.map.move(position)

        return mapView
    }
}