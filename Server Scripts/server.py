import flask
import flask_jsonpify
import StableDiffImageProvider as sd

app = flask.Flask("Image Generator for VG")

@app.route('/sd', methods=['POST'])
def sdRequest():
    data = flask.request.get_json()
    prompt = data["prompt"]
    steps = data['steps']

    print(f"Generating Image with Stable Diffusion\nPrompt: {prompt}\nSteps: {steps}")

    sdAPI = sd.StableDiffusionImageProvider()
    res = sdAPI.generateImageBase64(prompt, steps)

    return flask_jsonpify.jsonify({"images": res})
    

if __name__ == '__main__':
    app.run(port=9999)