import requests, io, base64
from PIL import Image

class StableDiffusionImageProvider:
    def __init__(self):
        pass

    def generateImageBase64(self, prompt, steps):
        payload = {
            "prompt": prompt,
            "steps": steps
        }

        response = requests.post(url=f'http://127.0.0.1:7860/sdapi/v1/txt2img', json=payload)
        r = response.json()
        
        return r['images']
    
    def decodeImage(self, encodedImg):
        return Image.open(io.BytesIO(base64.b64decode(encodedImg.split(",",1)[0])))
