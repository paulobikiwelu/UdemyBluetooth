using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyBluetooth.Interfaces;

namespace UdemyBluetooth.Services
{
    public class FileTransfer : IFileTransfer
    {
        public byte[][] Split(byte[] data, int packetSize)
        {
            List<byte[]> output = new List<byte[]>();

            UInt32 offset = 0;
            packetSize -= sizeof(UInt32);

            for (int i = 0; i < data.Length; i += packetSize)
            {
                byte[] packet = new byte[packetSize + sizeof(UInt32)];
                Buffer.BlockCopy(data, i, packet, sizeof(UInt32), packetSize);

                byte[] offsetAsBytes = BitConverter.GetBytes(offset);
                Buffer.BlockCopy(offsetAsBytes, 0, packet, 0, offsetAsBytes.Length);

                offset += (UInt32)packetSize;
                output.Add(packet);
            }

            return output.ToArray();
        }

        public byte[] Combine(byte[][] data)
        {
            Dictionary<UInt32, byte[]> packets = new Dictionary<uint, byte[]>();

            foreach (byte[] packet in data)
            {
                using (MemoryStream ms = new MemoryStream(packet))
                {
                    byte[] offset = new byte[sizeof(UInt32)];
                    _ = ms.Read(offset, 0, offset.Length);

                    byte[] actualData = new byte[packet.Length - sizeof(UInt32)];
                    _ = ms.Read(actualData, 0, actualData.Length);

                    UInt32 offsetValue = BitConverter.ToUInt32(offset, 0);
                    packets.Add(offsetValue, actualData);
                }
            }

            byte[][] processed = packets.OrderBy(a => a.Key)
                .Select(a => a.Value)
                .ToArray();

            byte[] output = default(byte[]);

            using (MemoryStream ms = new MemoryStream())
            {
                foreach (byte[] actualData in processed)
                {
                    ms.Write(actualData, 0, actualData.Length);
                }

                output = ms.ToArray();
            }

            return output;
        }
    }
}
