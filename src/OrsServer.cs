using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using OhiohResearchServer.Net;

namespace OhiohResearchServer
{
    public class OrsServer
    {
        public Socket Socket { get; private set; } = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public static Dictionary<IPAddress, Socket> Clients = new Dictionary<IPAddress, Socket>();
        private const int RECEIVETIMEOUT = 10000;


        public OrsServer(IPAddress host, ushort port)
        {
            Socket.ReceiveTimeout = 6000;

            Socket.Bind(new IPEndPoint(host, port));
        }

        public void Listen()
        {
            Socket.Listen(sizeof(byte));
            Console.WriteLine("Listening...");

            Thread acceptT = new Thread(new ThreadStart(Accept));
            acceptT.Start();
        }

        public void Send(byte[] buffer, params IPAddress[] ipAddresses)
        {
            foreach (IPAddress ipAddr in ipAddresses)
                Clients[ipAddr].Send(buffer);
        }

        public void Send(IPacket packet, params IPAddress[] iPAddresses)
        {
            Send(packet.ToBuffer(), iPAddresses);
        }

        private void Accept()
        {
            while (Socket.IsBound)
            {
                Socket client = Socket.Accept();
                Clients.Add(((IPEndPoint)client.RemoteEndPoint).Address, client);

                // client.ReceiveTimeout = RECEIVETIMEOUT;
                client.ReceiveTimeout = -1;

                // Spawn new thread for new connected client
                new Thread(new ParameterizedThreadStart(ListenClient)).Start(client);
            }
        }

        private void ListenClient(object objParam)
        {
            Socket client = (Socket)objParam;

            Console.WriteLine($"> {client.RemoteEndPoint} connected (Thread {Thread.CurrentThread.ManagedThreadId})");

            while (client.Connected)
            {
                byte[] buffer = new byte[1024];

                try
                {
                    int bufferSize = client.Receive(buffer);
                    Console.WriteLine($"{bufferSize} bytes received");
                    PrintBuffer(buffer, bufferSize);
                    //Console.WriteLine(ASCIIEncoding.ASCII.GetString(buffer, 0, bufferSize));

                    switch ((PacketId)buffer[0])
                    {
                        case PacketId.BLE_SCANREPORT:
                            using (BinaryReader binStream = new BinaryReader(new MemoryStream(buffer, 1, bufferSize - 1)))
                            {
                                int devicesFound = binStream.ReadByte();

                                Console.WriteLine($"{client.RemoteEndPoint} found {devicesFound} device(s):\n\t");

                                for (int i = 0; i < devicesFound; i++)
                                {
                                    int payloadLength = binStream.ReadInt32();
                                    byte[] payload = new byte[payloadLength];
                                    Array.Copy(binStream.ReadBytes(payloadLength), payload, payloadLength);
                                    BleAdvertisedDevice bleDevice = BleDecoder.DecodePayload(payload);

                                    PrintBuffer(payload, payloadLength);
                                }

                                //End of buffer?
                                if (binStream.BaseStream.Position != bufferSize - 1)
                                    Console.WriteLine($"WARNING: Did not expect additional {bufferSize - binStream.BaseStream.Position} byte(s)!");
                            }
                            break;
                    }

                    System.Console.WriteLine();
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"< {client.RemoteEndPoint} disconnected: {e.SocketErrorCode}");
                    client.Close();
                }
            }

            // Remove socket from client dictionary
            Clients.Remove(((IPEndPoint)client.RemoteEndPoint).Address);

            Console.Write($"< Closing thread #{Thread.CurrentThread.ManagedThreadId}");
        }
        private static void PrintBuffer(byte[] buffer, int size)
        {
            for (int i = 0; i < size; i++)
                Console.Write(buffer[i].ToString("X2") + " ");

            Console.WriteLine();
        }
    }
}