using System;
using System.IO;

namespace OhiohResearchServer.Net
{
    public class SetOpModePacket : IPacket
    {
        public OpMode OpMode {get; private set;}

        public byte[] ToBuffer()
        {
            using (MemoryStream memStream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(memStream))
            {
                writer.Write((byte)PacketId.SET_OPMODE);
                writer.Write((byte)OpMode);

                return memStream.ToArray();
            }
        }

        public void SetOpMode(OpMode opMode)
        {
            OpMode = opMode;
        }
    }

    public enum OpMode
    {
        SEND,
        RECEIVE
    }
}