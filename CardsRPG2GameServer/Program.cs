using System;
using System.Collections.Generic;
using CardsRPGGameServer.Socket;
using LogicLayer;

namespace CardsRPGGameServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            MsgHandlerCenter.Instance.Init();
            DataCacheSystem.InitDataCache();
            ConfigCenter.Init();
            SocketServer server = new SocketServer();
            server.Init();
            
            Console.ReadKey();
        }
    }
}