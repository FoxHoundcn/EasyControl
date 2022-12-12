using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EasyControl
{
    public class UdpConnection
    {
        private AsyncCallback _socketDataCallback = null;
        private EndPoint _bindEndPoint;
        private Socket _socket = null;
        private EndPoint _client = null;
        public byte[] _dataBuffer = new byte[2048];
        private static Thread _sendThread;
        private List<InterfacePlugin> ipList = new List<InterfacePlugin>();
        public UdpConnection(int port)
        {
            _socketDataCallback = new AsyncCallback(OnDataReceived);
            _bindEndPoint = new IPEndPoint(IPAddress.Any, port);
            _socket = new Socket(AddressFamily.InterNetwork,
                                 SocketType.Dgram,
                                 ProtocolType.Udp);
            _socket.ExclusiveAddressUse = false;
            _socket.Bind(_bindEndPoint);
            _client = new IPEndPoint(IPAddress.Any, 0);
            //-----------------------------------------------------
            _sendThread = new Thread(
                new ThreadStart(WaitForData));
            _sendThread.IsBackground = true;
            _sendThread.Start();
        }
        public void AddInterfacePlugin(InterfacePlugin ip)
        {
            if (!ipList.Contains(ip))
                ipList.Add(ip);
        }

        public void SendData(EndPoint _tarClient, byte[] data)
        {
            try
            {
                if (_tarClient != null)
                {
                    _socket.SendTo(data, _tarClient);
                }
            }
            catch (SocketException se)
            {
                HandleSocketException(se);
            }
            catch
            {
                DebugConstol.AddLog("UDP interface threw unhandled exception sending data. ", LogType.Error);
            }
        }
        private void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                if (_socket != null)
                {
                    int receivedByteCount = _socket.EndReceiveFrom(asyn, ref _client);
                    foreach (InterfacePlugin ip in ipList)
                    {
                        if (ip.Open)
                        {
                            ReceiveUdpMsgHandler handler = new ReceiveUdpMsgHandler(ip.OnReceiveUdpMsg);
                            handler.BeginInvoke(_client, _dataBuffer.Skip(0).Take(receivedByteCount).ToArray(), null, null);
                        }
                    }
                }
            }
            catch (SocketException se)
            {
                HandleSocketException(se);
            }
            catch (Exception e)
            {
                DebugConstol.AddLog("数据解析失败 : " + e.Message, LogType.Error);
            }
            WaitForData();
        }

        private void WaitForData()
        {
            try
            {
                _socket.BeginReceiveFrom(_dataBuffer, 0, _dataBuffer.Length, SocketFlags.None, ref _bindEndPoint, _socketDataCallback, null);
            }
            catch (SocketException se)
            {
                if (HandleSocketException(se))
                {
                    _socket.BeginReceiveFrom(_dataBuffer, 0, _dataBuffer.Length, SocketFlags.None, ref _bindEndPoint, _socketDataCallback, null);
                }
                else
                {
                    DebugConstol.AddLog("接收数据失败 : " + se.Message, LogType.Error);
                }
            }
        }
        private bool HandleSocketException(SocketException se)
        {
            try
            {
                if (se.ErrorCode == 10054)
                {
                    _socket.Close();
                    _socket = null;
                    _socket = new Socket(AddressFamily.InterNetwork,
                                         SocketType.Dgram,
                                         ProtocolType.Udp);
                    _socket.Bind(_bindEndPoint);
                    _client = new IPEndPoint(IPAddress.Any, 0);
                    return true;
                }
                else
                {
                    DebugConstol.AddLog("UDP重连失败 : " + se.ToString(), LogType.Error);
                    return false;
                }
            }
            catch (Exception e)
            {
                DebugConstol.AddLog("UDP重连失败 : " + e.Message.ToString(), LogType.Error);
                return false;
            }
        }
    }
}
