using CommonTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text.RegularExpressions;

namespace Server
{
    class Server
    {
        private const string CONFIG_FILE = "../serverlist.txt";
        static void Main(string[] args)
        {
            if (args.Length < 5)
            {
                Console.WriteLine("usage: ./Server.exe <server_id> <url> <max_faults> <max_delay> <min_delay>");
                return;
            }


            int origWidth, width;
            int origHeight, height;

            origWidth = Console.WindowWidth;
            origHeight = Console.WindowHeight;

            width = origWidth - (origWidth * 1/4);
            height = origHeight;

            Console.SetWindowSize(width, height);

            string server_id = args[0];
            int priority = Int32.Parse(Regex.Match(server_id, @"\d+").Value);
            string url = args[1];
            int max_faults = Int32.Parse(args[2]);
            int min_delay = Int32.Parse(args[3]);
            int max_delay = Int32.Parse(args[4]);
            Uri uri = new Uri(url);

            Dictionary<string, int> servers = new Dictionary<string, int>();
            Console.Title = $"{server_id} at {url}; min_delay: {min_delay}, max_delay: {max_delay}, f: {max_faults},";
            Console.WriteLine($"Starting server: {server_id} {url} {max_faults} {max_delay} {min_delay}");
            TcpChannel channel = new TcpChannel(uri.Port);
            ChannelServices.RegisterChannel(channel, false);

            string leader = url;
            try
            {
                using (StreamReader sr = new StreamReader(CONFIG_FILE))
                {
                    string line;
                    int curr = priority;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] server = line.Split('\t');
                        if (!server[1].Equals(url))
                        {
                            int priorit = Int32.Parse(Regex.Match(server[0], @"\d+").Value);
                            if (priorit > curr)
                            {
                                curr = priorit;
                                leader = server[1];
                            }
                            servers.Add(server[1], priorit);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine($"Could not read the configuration file: {e.Message}");
            }

            RemoteServerObject remoteServerObj = new RemoteServerObject(url, max_faults, max_delay, min_delay, priority, leader, servers);
            RemotingServices.Marshal(remoteServerObj, uri.LocalPath.Trim('/'), typeof(RemoteServerObject));

            Console.WriteLine($"Server started...");

            string command = "";
            while (command != "exit")
            {
                Console.Write("> ");
                command = Console.ReadLine();
                switch (command)
                {
                    case "status":
                        remoteServerObj.Status();
                        break;
                    case "crash":
                        remoteServerObj.Crash();
                        break;
                    case "freeze":
                        remoteServerObj.Freeze();
                        break;
                    case "unfreeze":
                        remoteServerObj.Unfreeze();
                        break;
                }
            }
        }
    }
}
