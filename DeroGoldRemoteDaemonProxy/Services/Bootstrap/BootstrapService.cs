using System;
using DeroGoldRemoteDaemonProxy.DependencyInjection;
using DeroGoldRemoteDaemonProxy.Services.Console;

namespace DeroGoldRemoteDaemonProxy.Services.Bootstrap
{
    public class BootstrapService : IInitializable
    {
        private const string AsciiArt = @"                                                             
                     DeroGoldDeroGoldDer
                 oGoldDeroGoldDeroGoldDeroGo
              ldDeroGoldDeroGoldDeroGoldDeroGol
            dDeroGoldDero           GoldDeroGoldD
          eroGoldDer                     oGoldDeroG
         oldDeroGo                         ldDeroGol
        dDeroGol            dDero            GoldDero   
       GoldDer         oGoldDeroGoldDe         roGoldD  
      eroGold        DeroGoldDeroGoldDer        oGoldDe 
     roGoldD        eroGo         ldDeroG       oldDeroG
     oldDero       GoldDer          oGoldDeroGoldDeroGol
     dDeroGo      ldDeroGo           ldDeroGoldDeroGoldD
     eroGold      DeroGold           DeroGoldDeroGoldDer     
     oGoldDe      roGoldDe           roG         oldDero
     GoldDer       oGoldDe          roGo         ldDeroG
     oldDeroG       oldDer        oGoldDe       roGoldDe    
      roGoldD        eroGoldDeroGoldDero        GoldDer
       oGoldDe         roGoldDeroGoldD         eroGold
        DeroGold            DeroG            oldDeroG
         oldDeroGo                         ldDeroGol
          dDeroGoldD                     eroGoldDer
            oGoldDeroGold           DeroGoldDeroG
              oldDeroGoldDeroGoldDeroGoldDeroGo
                 ldDeroGoldDeroGoldDeroGoldD
                     eroGoldDeroGoldDero
                                                             ";

        private LoggerService LoggerService { get; }

        public BootstrapService(LoggerService loggerService)
        {
            LoggerService = loggerService;
        }

        public void Initialize()
        {
            System.Console.Title = "DeroGold Remote Daemon Proxy (.Net Core)";

            LoggerService.LogMessage("==================================================");
            LoggerService.LogMessage("DeroGold Remote Daemon Proxy (.NET Core)");
            LoggerService.LogMessage("==================================================");
            LoggerService.LogMessage(AsciiArt, ConsoleColor.DarkYellow);
        }
    }
}