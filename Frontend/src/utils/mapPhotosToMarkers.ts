import { BinPhoto } from "../Models/BinPhoto";
import { Marker } from "../types/Markers";

export const mapPhotosToMarkers = (photos: BinPhoto[]): Marker[] => {
    return photos.map(photo => ({
            latitude: photo.latitude,
            longitude: photo.longitude,
            title: photo.comment,
            id: photo.id,
        }));
};