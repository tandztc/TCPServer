#coding:utf-8
import socket
import datetime
import time
HOST = "localhost"  #服务其地址
PORT = 12580       #服务器端口
BUFFERSIZE = 1024
ADDR = (HOST, PORT)
 
TCPClient = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
TCPClient.connect(ADDR) #连接服务器
count = 0
while True:
    time.sleep(0.1)
    curTime = datetime.datetime.now()   #获得当前时间 格式是：datetime.datetime(2012, 3, 13, 1, 29, 51, 872000)
    senddata = curTime.strftime('%Y-%m-%m %H:%M:%S')     #转换格式
    for i in range(0,count):
        senddata = senddata + ' yo!'

    if senddata:
        TCPClient.send('%s' % (senddata))  #发送数据
        count += 1
        print "send:\t\t", senddata
    recvdate = TCPClient.recv(BUFFERSIZE)    #接受数据
    print  "receive:\t", recvdate
    print  count
    if recvdate == '88':
            break  
print "client close"
TCPClient.close()
