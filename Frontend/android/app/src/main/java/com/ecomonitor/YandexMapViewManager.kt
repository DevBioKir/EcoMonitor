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
import com.facebook.react.bridge.ReadableArray
import com.facebook.react.bridge.ReadableMap
import com.yandex.runtime.image.ImageProvider
import com.yandex.mapkit.map.PlacemarkMapObject
import android.view.View
import com.facebook.react.bridge.ReactContext
import com.facebook.react.bridge.Arguments
import com.facebook.react.bridge.WritableMap
import com.facebook.react.uimanager.events.RCTEventEmitter



class YandexMapViewManager : SimpleViewManager<MapView>(), LifecycleEventListener  {

    private var latitude: Double? = null
    private var longitude: Double? = null
    private var mapView: MapView? = null
    private var reactContext: ThemedReactContext? = null
    private val placemarks = mutableListOf<PlacemarkMapObject>()
    private var currentMarkersData: ReadableArray? = null

    override fun getName(): String = "YandexMapView"

    override fun createViewInstance(reactContext: ThemedReactContext): MapView {
        this.reactContext = reactContext

        val view = MapView(reactContext)
        mapView = view

        if (view.id == View.NO_ID) {
            view.id = View.generateViewId()
            Log.d("YandexMapViewManager", "Generated view ID: ${view.id}")
        }

        reactContext.addLifecycleEventListener(this)

        try {
            view.onStart()
            MapKitFactory.getInstance().onStart()
        } catch (e: Exception) {
            Log.e("YandexMapViewManager", "Error starting MapView: ${e.message}")
        }

        return view
    }

    @ReactProp(name = "latitude")
    fun setLatitude(view: MapView, latitude: Double) {
        Log.d("EcoMonitor", "Setting latitude: $latitude")
        this.latitude = latitude
        updateCamera()
    }

    @ReactProp(name = "longitude")
    fun setLongitude(view: MapView, longitude: Double) {
        Log.d("EcoMonitor", "Setting longitude: $longitude")
        this.longitude = longitude
        updateCamera()
    }

    @ReactProp(name = "markers")
    fun setMarkers(view: MapView, markers: ReadableArray?) {
        Log.d("EcoMonitor", "Received markers array of size: ${markers?.size() ?: 0}")
        
        currentMarkersData = markers
        addMarkersToMap(view, markers)
    }
    
    private fun addMarkersToMap(view: MapView, markers: ReadableArray?) {
        // Очистить старые маркеры
        placemarks.forEach { view.map.mapObjects.remove(it) }
        placemarks.clear()

        if (markers == null) return

        for (i in 0 until markers.size()) {
            val markerMap = markers.getMap(i) ?: continue

            val lat = markerMap.getDouble("latitude")
            val lon = markerMap.getDouble("longitude")
            val id = markerMap.getString("id") ?: continue

            val point = Point(lat, lon)
            val placemark = view.map.mapObjects.addPlacemark(point)

            placemark.userData = id
            placemark.opacity = 1.0f
            placemark.isVisible = true
            placemark.isDraggable = false

            placemark.addTapListener { _, _ ->
                Log.d("EcoMonitor", "Marker tapped: $id")
                reactContext?.let {
                    sendMarkerPressEvent(it, view, id)
                }
                true
            }

            placemarks.add(placemark)
            Log.d("EcoMonitor", "Added marker with listener: $lat, $lon, id: $id")
        }
    }

    private fun updateCamera() {
        val lat = latitude
        val lon = longitude
        val view = mapView

        if (lat != null && lon != null && view != null) {
            try {
                val position = CameraPosition(Point(lat, lon), 14.0f, 0f, 0f)
                view.map.move(position)
                Log.d("EcoMonitor", "Camera moved to: $lat, $lon")
            } catch (e: Exception) {
                Log.e("EcoMonitor", "Error moving camera: ${e.message}")
            }
        }
    }

    private fun sendMarkerPressEvent(reactContext: ReactContext, view: View, id: String) {
        val event: WritableMap = Arguments.createMap()
        event.putString("id", id)
        Log.d("EcoMonitor", "Sending marker press event to JS: $id")
        reactContext.getJSModule(RCTEventEmitter::class.java)
            .receiveEvent(view.id, "onMarkerPress", event)
    }

    override fun onHostResume() {
        Log.d("EcoMonitor", "onHostResume called - recreating markers")
        try {
            mapView?.onStart()
            MapKitFactory.getInstance().onStart()
            
            // ИСПРАВЛЕНИЕ: Полностью пересоздать маркеры при resume
            mapView?.let { view ->
                Log.d("EcoMonitor", "Recreating markers from saved data")
                addMarkersToMap(view, currentMarkersData)
            }
        } catch (e: Exception) {
            Log.e("EcoMonitor", "Error in onHostResume: ${e.message}")
        }
    }

    override fun onHostPause() {
        Log.d("EcoMonitor", "onHostPause called")
        try {
            mapView?.onStop()
            MapKitFactory.getInstance().onStop()
        } catch (e: Exception) {
            Log.e("EcoMonitor", "Error in onHostPause: ${e.message}")
        }
    }

    override fun onHostDestroy() {
        Log.d("EcoMonitor", "onHostDestroy called")
        try {
            mapView?.onStop()
            MapKitFactory.getInstance().onStop()
        } catch (e: Exception) {
            Log.e("EcoMonitor", "Error in onHostDestroy: ${e.message}")
        }
    }

    override fun onDropViewInstance(view: MapView) {
        super.onDropViewInstance(view)
        Log.d("EcoMonitor", "onDropViewInstance called")
        try {
            reactContext?.removeLifecycleEventListener(this)
            view.onStop()
            mapView = null
            reactContext = null
        } catch (e: Exception) {
            Log.e("EcoMonitor", "Error in onDropViewInstance: ${e.message}")
        }
    }
    
    override fun getExportedCustomDirectEventTypeConstants(): MutableMap<String, Any> {
    return mutableMapOf(
        "onMarkerPress" to mapOf("registrationName" to "onMarkerPress")
    )
}
}

