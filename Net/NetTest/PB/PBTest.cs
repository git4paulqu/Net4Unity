using System.IO;

namespace NetTest
{
    class PBTest
    {
        public static void Test()
        {
            Data d1 = new Data { id = 1 };
            Data d2 = new Data { id = 2 };

            byte[] b1;
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<Data>(ms, d1);
                b1 = ms.ToArray();
            }

            byte[] b2;
            using (MemoryStream ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize<Data>(ms, d2);
                b2 = ms.ToArray();
            }

            Net.MemoryBufferStream mbs = new Net.MemoryBufferStream(1024);
            mbs.Copy(b1, 0, b1.Length);
            mbs.Copy(b2, 0, b2.Length);

            //byte[] bs = mbs.memoryStream.GetBuffer();
            //mbs.memoryStream.Seek(2, SeekOrigin.Begin);

            //Data dd1 = ProtoBuf.Serializer.Deserialize<Data>(mbs.memoryStream);

            //mbs.Clear(b1.Length);
            //Data dd2 = ProtoBuf.Serializer.Deserialize<Data>(mbs.memoryStream);
        }
    }
}
