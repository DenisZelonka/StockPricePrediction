from flask import Flask, request, jsonify, make_response
from keras.models import load_model
import tensorflow as tf
import yfinance as yf
import numpy as np

print(tf.__version__)

app = Flask(__name__)

@app.route("/prediction", methods=['POST'])
def home():

    req = request.get_json()
    ticker = req['name']

    data = yf.Ticker(f"{ticker}")
    data = data.history(period='3d')
    data = data['Close']
    data = data.to_numpy()
    data = np.reshape(data, [3,1])
    data = data.astype(np.float32)
    model = load_model(f'models/{ticker}.h5')
    prediction = np.mean(model.predict(data).flatten())
    res = jsonify({"prediction": str(prediction)})
    print(res)
    return make_response(res, 200)


if __name__ == "__main__":
    app.run(debug=True, host="0.0.0.0")