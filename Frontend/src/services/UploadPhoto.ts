import { BinPhotoResponse } from '../types/BinPhotoResponse';
import { BinPhotoUploadRequest } from '../types/BinPhotoUploadRequest';
import { DEV_API_BASE_URL } from '@env';

export const uploadWithMetadata = async (
  request: BinPhotoUploadRequest,
): Promise<BinPhotoResponse> => {
  if (!request) throw new Error('Файл не выбран');

  const formData = new FormData();

  formData.append("Photo", {
    uri: request.photo.uri,
    name: request.photo.name || 'photo.jpg',
    type: request.photo.type || 'image/jpeg',
  });

  request.binTypeId.forEach(id => {
    formData.append('BinTypeId', id)
  });

  // Проверяем FillLevel на валидность
  if (!isFinite(request.fillLevel) || isNaN(request.fillLevel)) {
    throw new Error(`Некорректное значение fillLevel: ${request.fillLevel}`);
  }

  formData.append('FillLevel', String(request.fillLevel).replace('.', ','));
  formData.append('IsOutsideBin', String(request.isOutsideBin));
  formData.append('Comment', request.comment);

  console.log('FormData FillLevel (проверенный):', request.fillLevel, '-> отправляется с запятой:', String(request.fillLevel).replace('.', ','));
  console.log('FormData все поля:', {
    FillLevel: String(request.fillLevel).replace('.', ','),
    IsOutsideBin: String(request.isOutsideBin),
    Comment: request.comment,
    BinTypeId: request.binTypeId
  });

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
