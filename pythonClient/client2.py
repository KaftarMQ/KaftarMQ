from  clientLib import Api

def f(m):
    print(m)

f(Api.pull())
Api.subscribe(f)