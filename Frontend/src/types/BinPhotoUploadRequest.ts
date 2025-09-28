export interface BinPhotoUploadRequest {
  photo: {
    uri: string;
    name: string;
    type: string;
  };
  binTypeId: string[];
  fillLevel: number;
  isOutsideBin: boolean;
  comment: string;
}
