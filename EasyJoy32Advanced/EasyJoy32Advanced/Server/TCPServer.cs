using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyControl
{
    public class TCPServer
    {
        //-------------------------------------------------------------
        public bool Running { get; private set; } = false;

        private Dictionary<string, JoyObject> clientList = new Dictionary<string, JoyObject>();
        private Thread serverThread;
        private Socket tcpServer = null;
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Init
        public static readonly TCPServer Instance = new TCPServer();
        private TCPServer()
        {
        }
        public void RunServer(IPEndPoint IP)
        {
            try
            {
                if (!Running)
                {
                    tcpServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//创建socket对象
                    tcpServer.Bind(IP);//绑定IP和申请端口

                    tcpServer.Listen(JoyConst.MaxJoyObject);//设置客户端最大连接数
                    DebugConstol.AddLog("服务器已启动，等待连接.........", LogType.NormalB);
                    serverThread = new Thread(ServerRunning);//开启线程执行循环接收消息
                    serverThread.Start();
                    Running = true;
                }
            }
            catch (Exception e)
            {
                DebugConstol.AddLog("RunServer Error : " + e.Message + "\n" + e.StackTrace);
                return;
            }
        }
        #endregion
        public void StopServer()
        {
            try
            {
                if (Running)
                {
                    tcpServer.Close();
                    tcpServer = null;
                    DebugConstol.AddLog("服务器已关闭", LogType.NormalB);
                    //释放client
                    foreach (string ipKey in clientList.Keys)
                    {
                        clientList[ipKey].Dispose();
                    }
                    clientList.Clear();
                    Running = false;
                }
            }
            catch (Exception e)
            {
                DebugConstol.AddLog("StopServer Error : " + e.Message + "\n" + e.StackTrace);
                return;
            }
        }
        void ServerRunning()
        {
            while (true)//循环等待新客户端的连接
            {
                try
                {
                    Socket clientSocket = tcpServer.Accept();
                    string ip = (clientSocket.RemoteEndPoint as IPEndPoint).Address.ToString();
                    if (!clientList.ContainsKey(ip))
                    {
                        DebugConstol.AddLog((clientSocket.RemoteEndPoint as IPEndPoint).Address + "已连接");
                        JoyObject client = new JoyObject(clientList.Count, clientSocket);
                        clientList.Add(ip, client);
                        client.Reconnection = true;
                    }
                }
                catch (Exception e)
                {
                    DebugConstol.AddLog("ServerRunning Error : " + e.Message + "\n" + e.StackTrace);
                    return;
                }
            }
        }
        public List<JoyObject> GetJoyClientList()
        {
            List<JoyObject> currentList = new List<JoyObject>();
            foreach (JoyObject obj in clientList.Values)
            {
                currentList.Add(obj);
            }
            return currentList;
        }
    }
}
