import multiprocessing
from client import pull, push, subscribe

TEST_SIZE = 10 * 1000
KEY_SIZE = 8
SUBSCRIBER_COUNT = 4

def to_infinity():
    index = 0
    while True:
        yield index
        index += 1

def push_key(key: str):
    for i in to_infinity():
        push(key, f"{i}".encode("utf-8"))
        print(key, f"{i}".encode("utf-8"))

def main():
    subscribe(lambda key, val: ...)

    for i in to_infinity():
        p = multiprocessing.Process(target=push_key, args=(str(i),))
        p.start()
        print("did it cap?")
        print("if not, press enter to increase throughput")
        print("if capped, manually scale up the cluster and press enter to see if you can increase the throughput")
        input()

if __name__ == '__main__':
    main()
