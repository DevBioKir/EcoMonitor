export interface BinPhotoResponse {
    id?: string;
    fileName: string;
    urlFile: string;
    latitude: number;
    longitude: number;
    uploadedAt: Date;
    binType: string;
    fillLevel: number;
    IsOutsideBin: boolean;
    comment: string;
}