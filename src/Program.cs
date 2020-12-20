/*   ___  _   _ ___ ___  _   _ 
 *  / _ \| | | |_ _/ _ \| | | |
 * | | | | |_| || | | | | |_| |
 * | |_| |  _  || | |_| |  _  |
 *  \___/|_| |_|___\___/|_| |_|
 * R   E   S   E   A   R   C   H
 */

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace OhiohResearchServer
{
    class Program
    {
        private static string _titleb64 = "ICAgX19fICBfICAgXyBfX18gX19fICBfICAgXyAKICAvIF8gXHwgfCB8IHxfIF8vIF8gXHwgfCB8IHwKIHwgfCB8IHwgfF98IHx8IHwgfCB8IHwgfF98IHwKIHwgfF98IHwgIF8gIHx8IHwgfF98IHwgIF8gIHwKICBcX19fL3xffCB8X3xfX19cX19fL3xffCB8X3wKIFIgICBFICAgUyAgIEUgICBBICAgUiAgIEMgICBICg==";
        public static Dictionary<IPEndPoint, Socket> Clients = new Dictionary<IPEndPoint, Socket>();
        private const int RECEIVETIMEOUT = 10000;
        private static Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private enum MsgId
        {
            BLE_SCANREPORT = 0xA // BLE scan report
        }

        static void Main(string[] args)
        {
            Console.WriteLine(Encoding.ASCII.GetString(Convert.FromBase64String(_titleb64)));
            Console.WriteLine("Starting server...");

            ushort port = 5527;

            server.ReceiveTimeout = 6000;

            server.Bind(new IPEndPoint(IPAddress.Any, port));

            server.Listen(sizeof(byte));
            Console.WriteLine("Listening...");

            Thread inputT = new Thread(() =>
            {
                while (true)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.S)
                    {
                        foreach (Socket client in Clients.Values)
                        {
                            byte[] buffer = new byte[2];

                            buffer[0] = 0x10;
                            buffer[1] = 1;

                            client.Send(buffer);

                            Console.WriteLine($"Message sent to {client.RemoteEndPoint}!");
                        }
                    }

                }
            });
            inputT.Start();


            Thread acceptT = new Thread(new ThreadStart(Accept));
            acceptT.Start();

        }

        private static void PrintBuffer(byte[] buffer, int size)
        {
            for (int i = 0; i < size; i++)
                Console.Write(buffer[i].ToString("X2") + " ");

            Console.WriteLine();
        }

        private static void Accept()
        {
            while (true)
            {
                Socket client = server.Accept();
                Clients.Add((IPEndPoint)client.RemoteEndPoint, client);

                // client.ReceiveTimeout = RECEIVETIMEOUT;
                client.ReceiveTimeout = -1;

                // Spawn new thread for new connected client
                new Thread(new ParameterizedThreadStart(Listen)).Start(client);
            }
        }

        private static void Listen(object objParam)
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

                    switch ((MsgId)buffer[0])
                    {
                        case MsgId.BLE_SCANREPORT:
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
            Clients.Remove((IPEndPoint)client.RemoteEndPoint);

            Console.Write($"< Closing thread #{Thread.CurrentThread.ManagedThreadId}");
        }
    }
}
