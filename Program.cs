using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ProtoBuf;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(100, 1024);
            server.Init();
            server.Start(new IPEndPoint(IPAddress.Any, 12580));
        }
        /*
        static void Main(string[] args)
        {
            //在本机创建一个TcpListener，
            TcpListener listener = new TcpListener(IPAddress.Any, 12580);
            //开始监听，
            listener.Start();
            //主线程循环，等待客户端连接
            while (true)
            {
                //接受客户端的连接，利用client保存连接的客户端
                TcpClient client = listener.AcceptTcpClient();
                System.Console.WriteLine(client.Client.RemoteEndPoint);
                Thread thr = new Thread(RecMsg);
                thr.IsBackground = true;
                thr.Start(client);
            }
        }
        */
        static void RecMsg(object client)
        {
            TcpClient sokClient = client as TcpClient;
            bool isHead = true;
            int len = 0;
            NetworkStream stream = sokClient.GetStream();
            while (true)
            {
                //流不可读，断开连接，结束线程
                if (!stream.CanRead)
                {
                    sokClient.Client.Disconnect(false);
                    return;
                }

                //对方已经断线，结束线程
                if (!sokClient.Client.Connected)
                {
                    return;
                }
                //读取消息体的长度
                if (isHead)
                {
                    if (sokClient.Available < 4)
                    {
                        continue;
                    }
                    byte[] lenByte = new byte[4];
                    stream.Read(lenByte, 0, 4);
                    len = BitConverter.ToInt32(lenByte,0);
                    isHead = false;
                }
                //读取消息体内容
                if (!isHead)
                {
                    if (sokClient.Available < len)
                    {
                        continue;
                    }
                    byte[] msgByte = new byte[len];
                    stream.Read(msgByte, 0, len);
                    MemoryStream kStream = new MemoryStream(msgByte);
                    isHead = true;
                    len = 0;
                    //处理消息
                    onRecMsg(kStream);
                }

                Thread.Sleep(10);   //每次休息10毫秒
            }
        }
        static void onRecMsg(MemoryStream kStream)
        {
            ChatMsg msg = Serializer.Deserialize<ChatMsg>(kStream);
            System.Console.WriteLine(msg.sender);
            System.Console.WriteLine(msg.msg);
            //System.Console.ReadLine();
        }
    }
}
