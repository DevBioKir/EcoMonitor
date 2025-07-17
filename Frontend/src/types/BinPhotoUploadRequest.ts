export interface BinPhotoUploadRequest {
  photo: {
    uri: string;
    name: string;
    type: string;
  };
  binType: string;
  fillLevel: number;
  isOutsideBin: boolean;
  comment: string;
}
