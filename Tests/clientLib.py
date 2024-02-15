import requests, json
import time
from threading import Thread

ip = 'localhost'

class Api:

    @staticmethod
    def internal_pull():
        message = requests.get(f"http://{ip}/router/Message/pull")

        if message is None or message.text == '':
            return None

        parsed = json.loads(message.text)
        return (parsed["key"], parsed["value"].encode("utf-8"))

    @staticmethod
    def pull():
        while True:
            message = Api.internal_pull()
            if message is not None:
                return message
            #time.sleep(0.3)
            
    
    @staticmethod
    def subscribe(f):
        thread = Thread(target = Api.internal_subscribe, args = (f, ))
        thread.start()

    def internal_subscribe(f):
        while True:
            message = Api.internal_pull()
            print('mess', message)
            if message is not None:
                f(message[0], message[1])
            time.sleep(0.1)


    @staticmethod
    def push(key, value):
        decodedValue = value.decode("utf-8")
        #print(f"Pushing message with key: \"{key}\", value: \"{decodedValue}\"")

        response = requests.post(f"http://{ip}/router/Message/push", params={"key": key, "value": decodedValue})
        response.raise_for_status()
