import requests, io, base64
from PIL import Image

class StableDiffusionImageProvider:
    def __init__(self):
        pass
    
    def generateImageBase64(prompt):
        payload = {
            "prompt": prompt,
            "steps": 25
        }

        response = requests.post(url=f'http://127.0.0.1:7860/sdapi/v1/txt2img', json=payload)
        r = response.json()
        
        return r['images']
    
    def decodeImage(encodedImg):
        return Image.open(io.BytesIO(base64.b64decode(encodedImg.split(",",1)[0])))
