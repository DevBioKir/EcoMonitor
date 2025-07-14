import { BinPhotoResponse } from '../types/BinPhotoResponse';
import { BinPhotoUploadRequest } from '../types/BinPhotoUploadRequest';

export const UploadWithMetadata = async (
  request: BinPhotoUploadRequest,
): Promise<BinPhotoResponse> => {
  if (!request) throw new Error('Файл не выбран');

  const formData = new FormData();
  formData.append('Photo', request.photo); // имя должно совпадать с сервером
  formData.append('BinType', request.binType);
  formData.append('FillLevel', String(request.fillLevel));
  formData.append('IsOutsideBin', String(request.isOutsideBin));
  formData.append('Comment', request.comment);

  try {
    const response = await fetch(
      'http://192.168.1.154:5198/api/BinPhoto/UploadWithMetadata',
      {
        method: 'POST',
        body: formData,
      },
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
