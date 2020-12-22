/*   ___  _   _ ___ ___  _   _ 
 *  / _ \| | | |_ _/ _ \| | | |
 * | | | | |_| || | | | | |_| |
 * | |_| |  _  || | |_| |  _  |
 *  \___/|_| |_|___\___/|_| |_|
 * R   E   S   E   A   R   C   H
 */

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using OhiohResearchServer.Net;

namespace OhiohResearchServer
{
    class Program
    {
        private static string _titleb64 = "ICAgX19fICBfICAgXyBfX18gX19fICBfICAgXyAKICAvIF8gXHwgfCB8IHxfIF8vIF8gXHwgfCB8IHwKIHwgfCB8IHwgfF98IHx8IHwgfCB8IHwgfF98IHwKIHwgfF98IHwgIF8gIHx8IHwgfF98IHwgIF8gIHwKICBcX19fL3xffCB8X3xfX19cX19fL3xffCB8X3wKIFIgICBFICAgUyAgIEUgICBBICAgUiAgIEMgICBICg==";
        public static OrsServer Server;

        static void Main(string[] args)
        {
            Console.WriteLine(Encoding.ASCII.GetString(Convert.FromBase64String(_titleb64)));
            ReadLine.HistoryEnabled = true;

            ushort port = 5527;

            Console.WriteLine($"Starting server on port {port}...");

            Server = new OrsServer(IPAddress.Any, port);
            Server.Listen();

            while (true)
            {
                string input = ReadLine.Read("> ").ToLower();

                MatchCollection matches = Regex.Matches(input, @"(\S+)");

                /* Inspect collection
                for (int i = 0; i < matches.Count; i++)
                {
                    Match match = matches[i];

                    Console.WriteLine($"Match:\t\t{i}\nLength:\t\t{match.Length}\nCaptures:\t{match.Captures.Count}\nGroups:\t\t{match.Groups.Count}\nC Value:\t\t{match.Captures[0].Value}\nG1 Value:\t{match.Groups[0].Value}\nG2 Value:\t{match.Groups[1].Value}");

                    System.Console.WriteLine();
                }
                */

                int index = 0;
                try
                {
                    switch (matches[index++].Captures[0].Value)
                    {
                        case "set":
                            if (matches.Count >= 2)
                            {
                                switch (matches[index++].Captures[0].Value)
                                {
                                    case "opmode":
                                        if (matches.Count >= 4)
                                        {
                                            OpMode opMode = (OpMode)Convert.ToByte(matches[index++].Captures[0].Value);
                                            Console.WriteLine(Enum.GetName(typeof(OpMode), opMode));

                                            SetOpModePacket packet = new SetOpModePacket();
                                            packet.SetOpMode(opMode);

                                            // Parse following IP addresses
                                            for (int i = index; i < matches.Count; i++)
                                            {
                                                IPAddress ipAddr = IPAddress.Parse(matches[i].Captures[0].Value);
                                                Server.Send(packet, ipAddr);
                                            }
                                        }
                                        break;
                                }
                            }
                            //else
                            //throw new Exception("Too few parameters supplied");
                            break;

                        case "get":
                            if (matches.Count >= 2)
                            {

                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error executing command: {e.Message}");
                }
            }
        }
    }
}
