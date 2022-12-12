using System.Collections.Generic;
using System.Net;

namespace EasyControl
{
    public delegate void ReceiveUdpMsgHandler(EndPoint client, byte[] msg);
    public class UdpReceive
    {
        static object lockObj = new object();
        public static readonly UdpReceive Instance = new UdpReceive();
        Dictionary<int, UdpConnection> connectionList = new Dictionary<int, UdpConnection>();
        private UdpReceive()
        {
        }
        public void CreateReceivePort(InterfacePlugin ip, int port)
        {
            //创建UDP
            if (!connectionList.ContainsKey(port))
            {
                connectionList.Add(port, new UdpConnection(port));
                connectionList[port].AddInterfacePlugin(ip);
            }
            else
            {
                connectionList[port].AddInterfacePlugin(ip);
            }
        }
        public void SendUDPMessage(byte[] message, int port, EndPoint tarClient)
        {
            if (connectionList.ContainsKey(port))
                connectionList[port].SendData(tarClient, message);
        }
    }
}
