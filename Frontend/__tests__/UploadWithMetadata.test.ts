
import { BinPhotoUploadRequest } from "../src/types/BinPhotoUploadRequest";
import { BinPhotoResponse } from "../src/types/BinPhotoResponse";
import { UploadWithMetadata } from "../src/services/uploadPhotoService";

global.fetch = jest.fn();

describe("UploadWithMetadata", () => {
  beforeEach(() => {
    (fetch as jest.Mock).mockClear();
  });

  it("должен отправить formData и вернуть ответ", async () => {
    // Arrange: мокаем fetch
    const mockResponse: BinPhotoResponse = {
      id: "123",
      fileName: "mock.jpg",
      urlFile: "http://example.com/mock.jpg",
      latitude: 0,
      longitude: 0,
      uploadedAt: new Date(),
      binType: "plastic",
      fillLevel: 50,
      IsOutsideBin: true,
      comment: "Тест",
    };

    (fetch as jest.Mock).mockResolvedValue({
      ok: true,
      json: () => Promise.resolve(mockResponse),
    });

    // Arrange: подготавливаем мок-файл
    const mockFile = {
      uri: "file://test.jpg",
      type: "image/jpeg",
      name: "test.jpg",
    } as unknown as File;

    const request: BinPhotoUploadRequest = {
      photo: mockFile,
      binType: "plastic",
      fillLevel: 50,
      isOutsideBin: true,
      comment: "Тест",
    };

    // Act
    const result = await UploadWithMetadata(request);

    // Assert
    expect(fetch).toHaveBeenCalledTimes(1);
    const fetchArgs = (fetch as jest.Mock).mock.calls[0];
    const url = fetchArgs[0];
    const options = fetchArgs[1];

    expect(url).toContain("/BinPhoto/UploadWithMetadata");
    expect(options.method).toBe("POST");
    expect(options.body).toBeInstanceOf(FormData);

    // Проверяем поля в formData
type ReactNativeFormData = {
  _parts: [string, any][];
};

const formData = options.body as ReactNativeFormData;
expect(formData._parts).toEqual(
  expect.arrayContaining([
    ["Photo", mockFile],
    ["BinType", "plastic"],
    ["FillLevel", "50"],
    ["IsOutsideBin", "true"],
    ["Comment", "Тест"],
  ])
);

    expect(result).toEqual(mockResponse);
  });

  it("должен выбросить ошибку, если нет файла", async () => {
    await expect(
      UploadWithMetadata(null as any)
    ).rejects.toThrow("Файл не выбран");
  });
});