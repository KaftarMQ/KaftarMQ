from clientLib import Api

def push(key: str, val: bytes):
    Api.push(key, val)

def pull():
    return Api.pull()

def subscribe(action):
    Api.subscribe(action)