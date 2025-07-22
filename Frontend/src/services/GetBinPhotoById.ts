import { DEV_API_BASE_URL } from '@env';
import { BinPhotoResponse } from '../types/BinPhotoResponse';

export const getBinPhotoById = async (id: string): Promise<BinPhotoResponse> => {
  const response = await fetch(
    `${DEV_API_BASE_URL}/api/BinPhoto/GetBinPhotoById`,
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

   return response.json(); // Преобразуем ответ в массив объектов BinPhotoResponse
};
