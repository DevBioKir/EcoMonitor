import { Platform } from 'react-native';
import { BinPhotoResponse } from '../types/BinPhotoResponse';
import { BinPhotoUploadRequest } from '../types/BinPhotoUploadRequest';
import { DEV_API_BASE_URL } from '@env';

export const UploadWithMetadata = async (
  request: BinPhotoUploadRequest,
): Promise<BinPhotoResponse> => {
  if (!request) throw new Error('Файл не выбран');

  const formData = new FormData();
  formData.append("Photo", {
    uri: request.photo.uri,
    name: request.photo.name,
    type: request.photo.type,
  } as any);
  formData.append('BinType', request.binType);
  formData.append('FillLevel', String(request.fillLevel).replace('.', ','));
  formData.append('IsOutsideBin', String(request.isOutsideBin));
  formData.append('Comment', request.comment);

  try {
    const response = await fetch(
      `${DEV_API_BASE_URL}/api/BinPhoto/UploadWithMetadata`,
      //"http://127.0.0.1:5198/api/BinPhoto/UploadWithMetadata",
      {
        method: "POST",
        body: formData,
      }
    );
    if (!response.ok) {
      // Если сервер вернул ошибку, читаем тело ответа и выводим
      const text = await response.text();
      console.error('Ошибка сервера:', response.status, text);
      throw new Error(`Ошибка сервера: ${response.status} ${text}`);
    }

    // Если всё успешно, парсим JSON
    return response.json();
  } catch (err) {
    console.error('Ошибка при загрузке изображения', err);
    throw err;
  }
};
