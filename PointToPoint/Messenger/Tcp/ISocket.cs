﻿using System.Net;
using System.Net.Sockets;

namespace PointToPoint.Messenger.Tcp
{
    /// <summary>
    /// Socket interface to enable mocking for testing purposes.
    /// </summary>
    public interface ISocket
    {
        void Connect(EndPoint remoteEP);

        bool NoDelay { set; }

        int ReceiveTimeout { set; }

        bool Connected { get; }

        void Close();

        void Dispose();

        int Receive(byte[] buffer, int offset, int size, SocketFlags socketFlags);

        int Send(byte[] buffer, int offset, int size, SocketFlags socketFlags);
    }
}