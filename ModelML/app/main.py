import io
from tkinter import Image
from fastapi import FastAPI, File, UploadFile
from fastapi.responses import JSONResponse
from .models import predict_bin


app = FastAPI(title="Bin Classifier API")

@app.get("/")
def root():
    return {"messsage": "Hello from Bin Classifier API!!!"}

@app.post("/predict")
async def predict(file: UploadFile = File(...)):
    try:
        contents = await file.read()
        image = Image.open(io.BytesIO(contents))

        # result = {
        #     "bin_type" : "plastic",
        #     "probably" : 0.95
        # }
        result = predict_bin(image)

        return JSONResponse(content = result)
    except Exception as e:
        return JSONResponse(content = {"error": str(e)}, status_code=404)