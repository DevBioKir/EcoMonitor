import { BinPhotoResponse } from "../types/BinPhotoResponse";

export const getAllCarts = async (): Promise<BinPhotoResponse[]> => {
    const response = await fetch(`${process.env.NEXT_PUBLIC_DEV_API_BASE_URL}/BinPhoto/GetAllBinPhotosAsync`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });

    if (!response.ok) {
        throw new Error("Failed to fetch carts");
    }

    const data: BinPhotoResponse[] = await response.json(); // Преобразуем ответ в массив объектов BinPhotoResponse
    return data; // Возвращаем данные
};