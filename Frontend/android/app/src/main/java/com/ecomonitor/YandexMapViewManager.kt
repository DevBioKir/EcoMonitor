package com.ecomonitor

import android.util.Log
import android.view.View
import com.facebook.react.bridge.*
import com.facebook.react.uimanager.SimpleViewManager
import com.facebook.react.uimanager.ThemedReactContext
import com.facebook.react.uimanager.annotations.ReactProp
import com.facebook.react.uimanager.events.RCTEventEmitter
import com.yandex.mapkit.MapKitFactory
import com.yandex.mapkit.geometry.Point
import com.yandex.mapkit.map.CameraPosition
import com.yandex.mapkit.map.Cluster
import com.yandex.mapkit.map.ClusterListener
import com.yandex.mapkit.map.ClusterizedPlacemarkCollection
import com.yandex.mapkit.mapview.MapView
import com.yandex.runtime.image.ImageProvider
import okhttp3.OkHttpClient
import okhttp3.Request
import org.json.JSONArray

class YandexMapViewManager : SimpleViewManager<MapView>(), LifecycleEventListener {

    private var latitude: Double? = null
    private var longitude: Double? = null
    private var baseUrl: String? = null

    private var mapView: MapView? = null
    private var reactContext: ThemedReactContext? = null

    private var clusterCollection: ClusterizedPlacemarkCollection? = null

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

        setupCameraListener(view)
        ensureClusterCollection(view)

        return view
    }

    @ReactProp(name = "baseUrl")
    fun setBaseUrl(view: MapView, url: String?) {
        baseUrl = url
        Log.d("EcoMonitor", "Set baseUrl: $baseUrl")
    }

    @ReactProp(name = "latitude")
    fun setLatitude(view: MapView, latitude: Double) {
        this.latitude = latitude
        updateCamera()
    }

    @ReactProp(name = "longitude")
    fun setLongitude(view: MapView, longitude: Double) {
        this.longitude = longitude
        updateCamera()
    }

    @ReactProp(name = "markers")
    fun setMarkers(view: MapView, markers: ReadableArray?) {
        if (markers == null) return
        updateClusterMarkersFromReadable(markers)
    }

    private fun ensureClusterCollection(view: MapView) {
        if (clusterCollection != null) return

        val clusterListener = ClusterListener { cluster ->
            try {
                cluster.appearance.setIcon(
                    ImageProvider.fromAsset(view.context, "cluster_icon.png")
                )
            } catch (e: Exception) {
                Log.w("EcoMonitor", "Cluster icon not found or failed: ${e.message}")
            }

            // Новый API — только один параметр
            cluster.addClusterTapListener { tappedCluster ->
                Log.d("EcoMonitor", "Cluster tapped, size=${tappedCluster.size}")
                true
            }
        }

        clusterCollection = view.map.mapObjects.addClusterizedPlacemarkCollection(clusterListener)
    }

    private fun updateCamera() {
        val lat = latitude
        val lon = longitude
        val view = mapView ?: return

        if (lat != null && lon != null) {
            try {
                val position = CameraPosition(Point(lat, lon), 14.0f, 0f, 0f)
                view.map.move(position)
                Log.d("EcoMonitor", "Camera moved to: $lat, $lon")
            } catch (e: Exception) {
                Log.e("EcoMonitor", "Error moving camera: ${e.message}")
            }
        }
    }

    private fun setupCameraListener(view: MapView) {
        view.map.addCameraListener { _, _, _, _ ->
            val visibleRegion = view.map.visibleRegion
            val north = visibleRegion.topLeft.latitude
            val south = visibleRegion.bottomRight.latitude
            val west = visibleRegion.topLeft.longitude
            val east = visibleRegion.bottomRight.longitude

            val boundsEvent = Arguments.createMap().apply {
                putDouble("north", north)
                putDouble("south", south)
                putDouble("west", west)
                putDouble("east", east)
            }
            reactContext?.getJSModule(RCTEventEmitter::class.java)
                ?.receiveEvent(view.id, "onBoundsChange", boundsEvent)

            loadMarkersInBounds(north, south, east, west)
        }
    }

    private fun loadMarkersInBounds(north: Double, south: Double, east: Double, west: Double) {
        val urlBase = baseUrl
        if (urlBase.isNullOrEmpty()) {
            Log.w("EcoMonitor", "Base URL is empty. Pass baseUrl prop from JS.")
            return
        }

        val url =
            "$urlBase/api/BinPhoto/GetPhotosInBounds?north=$north&south=$south&east=$east&west=$west"

        val client = OkHttpClient()
        val request = Request.Builder().url(url).get().build()

        client.newCall(request).enqueue(object : okhttp3.Callback {
            override fun onFailure(call: okhttp3.Call, e: java.io.IOException) {
                Log.e("EcoMonitor", "Ошибка загрузки маркеров: ${e.message}")
            }

            override fun onResponse(call: okhttp3.Call, response: okhttp3.Response) {
                if (!response.isSuccessful) {
                    Log.e("EcoMonitor", "Markers response not successful: ${response.code}")
                    return
                }

                val body = response.body?.string() ?: return
                try {
                    val markersArray = JSONArray(body)
                    val readableArray = Arguments.createArray()
                    for (i in 0 until markersArray.length()) {
                        val obj = markersArray.getJSONObject(i)
                        val map = Arguments.createMap()
                        map.putDouble("latitude", obj.getDouble("latitude"))
                        map.putDouble("longitude", obj.getDouble("longitude"))
                        map.putString("id", obj.getString("id"))
                        readableArray.pushMap(map)
                    }

                    mapView?.post { updateClusterMarkersFromReadable(readableArray) }
                } catch (e: Exception) {
                    Log.e("EcoMonitor", "JSON parse error: ${e.message}")
                }
            }
        })
    }

    private fun updateClusterMarkersFromReadable(markers: ReadableArray) {
        val view = mapView ?: return
        ensureClusterCollection(view)

        val collection = clusterCollection ?: return
        collection.clear()

        for (i in 0 until markers.size()) {
            val markerMap = markers.getMap(i) ?: continue
            val lat = markerMap.getDouble("latitude")
            val lon = markerMap.getDouble("longitude")
            val id = markerMap.getString("id") ?: continue

            val placemark = collection.addPlacemark(Point(lat, lon))
            placemark.userData = id

            placemark.addTapListener { _, _ ->
                reactContext?.let { sendMarkerPressEvent(it, view, id) }
                true
            }
        }

        collection.clusterPlacemarks(60.0, 15)
    }

    private fun sendMarkerPressEvent(reactContext: ReactContext, view: View, id: String) {
        val event: WritableMap = Arguments.createMap()
        event.putString("id", id)
        reactContext.getJSModule(RCTEventEmitter::class.java)
            .receiveEvent(view.id, "onMarkerPress", event)
    }

    override fun onHostResume() {
        try {
            mapView?.onStart()
            MapKitFactory.getInstance().onStart()
        } catch (e: Exception) {
            Log.e("EcoMonitor", "Error in onHostResume: ${e.message}")
        }
    }

    override fun onHostPause() {
        try {
            mapView?.onStop()
            MapKitFactory.getInstance().onStop()
        } catch (e: Exception) {
            Log.e("EcoMonitor", "Error in onHostPause: ${e.message}")
        }
    }

    override fun onHostDestroy() {
        try {
            mapView?.onStop()
            MapKitFactory.getInstance().onStop()
        } catch (e: Exception) {
            Log.e("EcoMonitor", "Error in onHostDestroy: ${e.message}")
        }
    }

    override fun onDropViewInstance(view: MapView) {
        super.onDropViewInstance(view)
        try {
            reactContext?.removeLifecycleEventListener(this)
            view.onStop()
            mapView = null
            reactContext = null
            clusterCollection = null
        } catch (e: Exception) {
            Log.e("EcoMonitor", "Error in onDropViewInstance: ${e.message}")
        }
    }

    override fun getExportedCustomDirectEventTypeConstants(): MutableMap<String, Any> {
        return mutableMapOf(
            "onMarkerPress" to mapOf("registrationName" to "onMarkerPress"),
            "onBoundsChange" to mapOf("registrationName" to "onBoundsChange")
        )
    }
}
