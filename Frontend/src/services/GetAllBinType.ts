import { DEV_API_BASE_URL } from "@env";
import { BinTypeResponse } from "../types/BinTypeResponse";

 export const getAllBinTypes = async (): Promise<BinTypeResponse[]> => {
    const response = await fetch(
        `${DEV_API_BASE_URL}/api/BinType/GetAllBinTypes`,
        {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
          },
          credentials: 'include',
        },
      );
    
      if (!response.ok) {
        throw new Error('Failed to fetch bin types');
      }
    
      const data: BinTypeResponse[] = await response.json(); // Преобразуем ответ в массив объектов BinPhotoResponse
      return data; // Возвращаем данные
 }
