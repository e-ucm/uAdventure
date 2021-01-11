using System;
using System.Collections;
using System.IO.Pipes;
using System.Threading;
using UniRx;
using UnityEngine;

namespace uAdventure.Simva
{
    public class SimvaUriHandler
    {

        const string IPC_CHANNEL_NAME_PREFIX = "uAdventureIPC";

        public delegate void OnUriReceived(string value);
        public static OnUriReceived uriReceivedDelegate;
        private static string messageReceived;

        private static CancellationTokenSource listenThread;


        private static void ListenThread()
        {
            using (var server = new NamedPipeServerStream(GetChannelName(), PipeDirection.InOut))
            {
                IAsyncResult connectionWait = null;

                while (!listenThread.IsCancellationRequested)
                {
                    Debug.Log("SERVER: Waiting for connection...");
                    connectionWait = server.BeginWaitForConnection(ProcessConnection, server);
                }

                if(connectionWait != null)
                {
                    Debug.Log("SERVER: Disconnecting...");
                    server.Disconnect();
                }
            }
        }

        private static void ProcessConnection(IAsyncResult ar)
        {
            Debug.Log("SERVER: Connection established. Reading message...");
            var server = (NamedPipeServerStream)ar.AsyncState;
            byte[] buffer = new byte[2048];
            int bytesRead = server.Read(buffer, 0, buffer.Length);
            string msg = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
            char[] chars = "Received!".ToCharArray();
            Array.Reverse(chars);
            var outBytes = System.Text.Encoding.ASCII.GetBytes(chars);
            server.Write(outBytes, 0, outBytes.Length);
        }

        public static bool Listen(OnUriReceived onUriReceive)
        {

            listenThread = new CancellationTokenSource();
            var t = new Thread(SimvaUriHandler.ListenThread);

            Observable.FromCoroutine(() => WaitForMessage()).Subscribe();

            uriReceivedDelegate += onUriReceive;

            return true;

        }

        private static IEnumerator WaitForMessage()
        {
            while (!listenThread.IsCancellationRequested)
            {
                string message = null;
                lock (messageReceived)
                {
                    message = messageReceived;
                }
                uriReceivedDelegate(message);
                yield return null;
            }
        }

        public static bool Unregister()
        {
            if(listenThread == null)
            {
                return false;
            }

            try
            {
                listenThread.Cancel();
            }
            catch
            {
                Debug.Log("Couldn't unregister IPC channel.");
                return false;
            }

            uriReceivedDelegate = null;

            return true;
        }

        private static string GetChannelName()
        {
            return IPC_CHANNEL_NAME_PREFIX + UnityEngine.Application.productName;
        }

        public static bool SendMessage(string message)
        {
            using (var client = new Lachee.IO.NamedPipeClientStream(".", GetChannelName()))
            {
                try
                {
                    Debug.Log("CLIENT: Connecting...");
                    client.Connect();

                    Debug.Log("CLIENT: Sending '"+ message + "'...");
                    byte[] msgbytes = System.Text.Encoding.ASCII.GetBytes(message);
                    client.Write(msgbytes, 0, msgbytes.Length);

                    Debug.Log("CLIENT: Sent. Reading message...");
                    message = "";
                    int bytesRead = 0;
                    byte[] buffer = new byte[2048];
                    while ((bytesRead = client.Read(buffer, 0, buffer.Length)) == 0)
                    {
                        Thread.Sleep(1);
                    }
                    message = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Debug.Log("CLIENT: Server said '"+ message + "'");
                    return true;

                }
                catch (Exception e)
                {
                    Debug.Log("Failed pipe test: " + e.Message);
                    return false;
                }
            }
        }
    }
}
