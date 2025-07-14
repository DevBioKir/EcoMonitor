export interface BinPhotoUploadRequest {
    photo: File;
    binType: string;
    fillLevel: number;
    isOutsideBin: boolean;
    comment: string;
}