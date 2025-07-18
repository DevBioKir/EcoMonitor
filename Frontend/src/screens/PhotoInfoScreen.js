import { DEV_API_BASE_URL } from '@env';
import React from 'react';
import { View, Text, Image } from 'react-native';

export const PhotoInfoScreen = ({ route }) => {
  const { photo } = route.params;

  return (
    <View style={{ flex: 1, padding: 20 }}>
      {/* Отображение фото */}
      <Image
        source={{ uri: `${DEV_API_BASE_URL}/Photos/${photo.fileName}` }}
        style={{ width: '100%', height: 300 }}
      />

      {/* Отображение метаданных */}
      <Text>Комментарий: {photo.comment}</Text>
      <Text>Тип бака: {photo.binType}</Text>
      <Text>Заполненность: {photo.fillLevel * 100}%</Text>
      <Text>Мусор вне бака: {photo.isOutsideBin ? 'Да' : 'Нет'}</Text>
      <Text>Дата загрузки фото: {new Date(photo.uploadedAt).toLocaleString()}</Text>
      <Text>Координаты бака: {photo.latitude}, {photo.longitude}</Text>
    </View>
  );
};