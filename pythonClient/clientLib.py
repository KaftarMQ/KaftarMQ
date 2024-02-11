import requests, json
import time
from threading import Thread


class Api:

    @staticmethod
    def internal_pull():
        message = requests.get("http://localhost:5274/Message/pull")

        if message is None:
            return None
        parsed = json.loads(message.text)
        return (parsed["key"], parsed["value"].encode("utf-8"))

    @staticmethod
    def pull():
        while True:
            message = Api.internal_pull()
            if message is not None:
                return message
            time.sleep(1)
            
    
    @staticmethod
    def subscribe(f):
        while True:
            message = Api.internal_pull()
            if message is not None:
                f(message)
            time.sleep(1)


    @staticmethod
    def push(key, value):
        decodedValue = value.decode("utf-8")
        print(f"Pushing message with key: \"{key}\", value: \"{decodedValue}\"")

        response = requests.post("http://localhost:5274/Message/push", params={"key": key, "value": decodedValue})
        response.raise_for_status()
