using System;
using System.Threading.Tasks;
using DeroGoldDaemonProxy.Services.Console;
using TheDialgaTeam.DependencyInjection;

namespace DeroGoldDaemonProxy.Services.Bootstrap
{
    public class BootstrapService : IInitializableAsync
    {
        private LoggerService LoggerService { get; }

        private const string AsciiArt = 
            "MMMMMMMMMMMMMMMMMMMMMMMMMmdyo+::::...`.......-:::/+oydmMMMMMMMMMMMMMMMMMMMMMMMMM\n" +
            "MMMMMMMMMMMMMMMMMMMMNdho-:............................::ohdNMMMMMMMMMMMMMMMMMMMM\n" +
            "MMMMMMMMMMMMMMMMMNdo:-....................................-:sdNMMMMMMMMMMMMMMMMM\n" +
            "MMMMMMMMMMMMMMNho-..........................................``/odMMMMMMMMMMMMMMM\n" +
            "MMMMMMMMMMMMmo:.```````````````````.````````.```````````````````./ymMMMMMMMMMMMM\n" +
            "MMMMMMMMMMNh:```````````````.-:+osyo````````oyso+/-.```````````````:yNMMMMMMMMMM\n" +
            "MMMMMMMMN+-`````````````.:oshhhhhhho````````ohhhhhhhyo/.`````````````.sNMMMMMMMM\n" +
            "MMMMMMMy/.```````````.+shhhhhhhhhhho````````ohhhhhhhhhhhy+-```````````./hMMMMMMM\n" +
            "MMMMMN+-```````````:shhhhhhhhhhhhhho````````ohhhhhhhhhhhhhhy/```````````-oNMMMMM\n" +
            "MMMMm/.``````````/yhhhhhhhhhhhhhhhho````````ohhhhhhhhhhhhhhhhh+.`````````.+mMMMM\n" +
            "MMMd:..........:yhhhhhhhhhhhhhhhhhho`......`ohhhhhhhhhhhhhhhhhhh/..........:hMMM\n" +
            "MMd+..........shhhhhhhhhhhhhhhhhhhho........ohhhhhhhhhhhhhhhhhhhhs-.........+dMM\n" +
            "MN+-........:yhhhhhhhhhhhhhhhhhhs+/-........-/+shhhhhhhhhhhhhhhhhhy:........--mM\n" +
            "Nh:--.-----:hhhhhhhhhhhhhhhhhs/------------------/shhhhhhhhhhhhhhhhh:--......-+N\n" +
            "mo--..-----yhhhhhhhhhhhhhhhs:----------------------:shhhhhhhhhhhhhhhh:-.......-o\n" +
            "s/--..--:-shhhhhhhhhhhhhhh/-:::::::::/+ooo/:::::::::-/hhhhhhhhhhhhhhhs--......--\n" +
            "s:--.--:::hhhhhhhhhhhhhhh::::::::::oyhhhhhhhs/:::::::::hhhhhhhhhhhhhhh/--.....--\n" +
            "/:-----::shhhhhhhhhhhhhh+:::::::::shhhhhhhhhhy:::::::::+hhhhhhhhhhhhhho------.--\n" +
            "::----:::yhhhhhhhhhhhhhh::::::::::dhhhhhhhhhhh+:::::::::hhhhhhhhhhhhhhs:..---.--\n" +
            "::----::/hhhhhhhhhhhhhhy::::::::::hhhhhhhhhhhh+:::::::::yhhhhhhhhhhhhhy:.----.--\n" +
            "::-----:/hhhhhhhhhhhhhhy::::::::::ohhhhhhhhhhs::::::::::yhhhhhhhhhhhhhs:.----..-\n" +
            "::--:--:/hhhhhhhhhhhhhhh:::::::::::shhhhhhhhy:::::::::::hhhhhhhhhhhhhhs-.-..-...\n" + 
            "--------:yhhhhhhhhhhhhhho:--------:hhhhhhhhhh/--------:ohhhhhhhhhhhhhho-........\n" +
            "o--.....-+hhhhhhhhhhhhhhho-------:yhhhhhhhhhhh/-------ohhhhhhhhhhhhhhh/........`\n" +
            "+.......--yhhhhhhhhhhhhhhho------/oooooooooooo+------ohhhhhhhhhhhhhhhs-......../\n" +
            "h:........:hhhhhhhhhhhhhhhhy/-....................-/yhhhhhhhhhhhhhhhy:........-h\n" +
            "Ns.........:hhhhhhhhhhhhhhhhhy/-................-/yhhhhhhhhhhhhhhhhh:........`sN\n" +
            "Mm+.........:yhhhhhhhhhhhhhhhhhhs+/-........-/+shhhhhhhhhhhhhhhhhhy:.........+mM\n" +
            "MMh/.````````-shhhhhhhhhhhhhhhhhhhho```````.ohhhhhhhhhhhhhhhhhhhhs.````````./hMM\n" +
            "MMMh:.`````````/hhhhhhhhhhhhhhhhhhho````````ohhhhhhhhhhhhhhhhhhy:`````````.:hMMM\n" +
            "MMMMm+.`````````.+hhhhhhhhhhhhhhhhho````````ohhhhhhhhhhhhhhhhy/``````````.+mMMMM\n" +
            "MMMMMNo:```````````/yhhhhhhhhhhhhhho````````ohhhhhhhhhhhhhhs:```````````:oNMMMMM\n" +
            "MMMMMMMh+-```````````-+yhhhhhhhhhhho````````ohhhhhhhhhhhs/.```````````-+hMMMMMMM\n" +
            "MMMMMMMMNh/.````````````./oyhhhhhhho````````ohhhhhhhso:.````````````./hNMMMMMMMM\n" +
            "MMMMMMMMMMNh+.``````````````.-/+osyo````````oyso+:-```````````````.+hNMMMMMMMMMM\n" +
            "MMMMMMMMMMMMNds:................................................:sdNMMMMMMMMMMMM\n" +
            "MMMMMMMMMMMMMMMNho:------------------------------------------:ohNMMMMMMMMMMMMMMM\n" +
            "MMMMMMMMMMMMMMMMMNmds+/::::::::::::::::::::::::::::::::::/+sdmNMMMMMMMMMMMMMMMMM\n" +
            "MMMMMMMMMMMMMMMMMMMMNmmyss++////////////////////////++ssymmNMMMMMMMMMMMMMMMMMMMM\n" +
            "MMMMMMMMMMMMMMMMMMMMMMMMMNNmdhssss+ooooooooooosssshdmNNMMMMMMMMMMMMMMMMMMMMMMMMM\n";

        public BootstrapService(LoggerService loggerService)
        {
            LoggerService = loggerService;
        }

        public async Task InitializeAsync()
        {
            await LoggerService.LogMessageAsync(AsciiArt, ConsoleColor.Green).ConfigureAwait(false);
        }
    }
}