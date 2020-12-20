using System;
using System.IO;

namespace OhiohResearchServer
{
    public enum AssignedNumber
    {
        TX_POWER = 0x0A
    }

    public static class BleDecoder
    {
        public static BleAdvertisedDevice DecodePayload(byte[] buffer)
        {
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(buffer)))
            {
                while (binReader.BaseStream.Position < buffer.Length)
                {
                    byte length = binReader.ReadByte();

                    switch ((AssignedNumber)binReader.ReadByte())
                    {
                        case AssignedNumber.TX_POWER:
                            System.Console.WriteLine($"TX POWER: {BitConverter.ToInt32((binReader.ReadBytes(length - 1)))}");
                        break;

                        default:
                            binReader.ReadBytes(length - 1);
                        break;
                    }

                    System.Console.WriteLine((buffer.Length - binReader.BaseStream.Position) + " bytes left");
                }
            }

            return new BleAdvertisedDevice();
        }
    }

    public class BleAdvertisedDevice
    {

    }
}