/********************************************************************************
** auth： Tan Yong
** date： 7/25/2014 2:58:13 PM
** desc： 
** Ver.:  V1.0.0
** Feedback: mailto:tanyong@cyou-inc.com
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ProtoBuf;

namespace TCPServer
{
    class MessageDispatcher
    {
        public static void ProcessMsg(AsyncUserToken token, MemoryStream kStream, int eType)
        {
            if (0 == eType)
            {
                ChatMsg msg = Serializer.Deserialize<ChatMsg>(kStream);
                kStream.Dispose();
                System.Console.WriteLine(msg.sender);
                System.Console.WriteLine(msg.msg);
                string resp = Server.m_totalBytesRead.ToString();

                token.Send(resp);
            }
        }
    }
}
