import clientLib

while True:
    message = clientLib.Api.pull()
    if message is not None:
        print(message)
#         break
#     time.sleep(0.1)