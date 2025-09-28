import { DEV_API_BASE_URL } from '@env';
import { BinPhotoResponse } from '../types/BinPhotoResponse';

export const getAllPhotos = async (): Promise<BinPhotoResponse[]> => {
  const response = await fetch(
    `${DEV_API_BASE_URL}/api/BinPhoto/GetAllPhotos`,
    {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
      credentials: 'include',
    },
  );

  if (!response.ok) {
    throw new Error('Failed to fetch photos');
  }

  const data: BinPhotoResponse[] = await response.json(); // Преобразуем ответ в массив объектов BinPhotoResponse
  return data; // Возвращаем данные
};
