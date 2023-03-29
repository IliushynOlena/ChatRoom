using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerApp
{
    public class ChatServer
    {
        const short port = 4040;
        const string JOIN_CMD = "$<join>";
        HashSet<IPEndPoint> members = new HashSet<IPEndPoint>();
        UdpClient server = new UdpClient(port);
        IPEndPoint clientEndPoint = null;
        private void AddMember(IPEndPoint member)
        {
            members.Add(member);
            Console.WriteLine("Members was added!!!");
        }
        private void SendToAll(byte[]data)
        {
            foreach (var m in members)
            {
                server.SendAsync(data, data.Length, m);
            }
        }
        public void Start()
        {
            while (true)
            {
                byte[] data = server.Receive(ref clientEndPoint);
                string message = Encoding.UTF8.GetString(data);
                Console.WriteLine($"Got : {message} at " +
                    $"{DateTime.Now.ToShortTimeString()} from {clientEndPoint}");
                switch (message)
                {
                    case JOIN_CMD:
                        AddMember(clientEndPoint);
                        break;
                    default:
                        SendToAll(data);
                        break;
                }
            }

        }

    }
    internal class Program
    {
        static void Main(string[] args)
        {
            ChatServer server = new ChatServer();
            server.Start();
           
           
        }
    
    }
}
