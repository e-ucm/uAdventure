using System;
using ZetaIpc.Runtime.Server;
using ZetaIpc.Runtime.Helper;

namespace uAdventure.Simva
{
    public static class SimvaUriHandler
    {
        private static IpcServer server;

        public static int Start(Action<string> onUriReceive)
        {
            var port = FreePortHelper.GetFreePort();
            server = new IpcServer();
            server.Start(port); // Passing no port selects a free port automatically.

            Console.WriteLine("Started server on port {0}.", server.Port);

            server.ReceivedRequest += (sender, args) =>
            {
                args.Response = "I've got: " + args.Request;
                args.Handled = true;
                onUriReceive(args.Request);
            };

            return port;
        }

        public static void Stop()
        {
            if(server != null)
            {
                server.Stop();
                server = null;
            }
        }
    }
}
