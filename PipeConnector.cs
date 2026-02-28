using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace ExPipeDll
{
    public static class PipeConnector
    {
        private const string PipeName = "PoE_Pipe";
        private const int ConnectionTimeout = 1000;

        public static void SendMessage(uint message)
        {
            Task.Run(() =>
            {
                try
                {
                    using var client = new NamedPipeClientStream(".", PipeName, PipeDirection.Out);
                    client.Connect(ConnectionTimeout);
                    using var writer = new BinaryWriter(client);
                    {
                        writer.Write(message);
                        writer.Flush();
                    }
                }
                catch
                {
                    // Pipe connection failed - silent fail for reliability
                }
            });
        }
    }
}
