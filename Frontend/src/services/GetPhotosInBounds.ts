import { DEV_API_BASE_URL } from '@env';
import { MapBounds } from '../types/MapBounds';
import { Marker } from '../types/Markers';

export const getPhotosInBounds = async (
  bounds: MapBounds,
): Promise<Marker[]> => {
  const { north, south, east, west } = bounds;

  const response = await fetch(
    `${DEV_API_BASE_URL}/api/BinPhoto/GetPhotosInBounds?north=${north}&south=${south}&east=${east}&west=${west}`,
    {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    },
  );
  if (!response.ok) {
    throw new Error('Failed to fetch photos');
  }

  return response.json();
};
