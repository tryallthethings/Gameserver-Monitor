using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace PRTG_gsMon
{
    class Program
    {
        static int verbosity = 0;
        static string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        static void Main(string[] args)
        {
            /* Command line options to do
              * discover gameservers on given hostname / IP?
              * Autodetect game type on given port?
              * Give game type name Cod4 instead of gametype (quake3 e.g.)
              * load servers from json or txt?
              * linux compatibility?
            */

            Console.CancelKeyPress += delegate {
                // no cleanup necessary currently
                Environment.Exit(1);
            };

            // these variables will be set when the command line is parsed
            bool shouldShowHelp = false;
            List<string> servers = new List<string>();
            List<string> types = new List<string>();
            List<int> ports = new List<int>();
            // console = default, autorefresh, xml (prtg)
            string outputFormat = "console";
            bool autorefresh = false;
            int waitTime = 10;
            List<string> validGameTypes = GenerateValidGameTypes();

            // these are the available options, not that they set the variables
            var options = new OptionSet {
                { "s|server=", "the server name (FQDN) or IP address.", s => servers.Add (s) },
                { "t|type=",   "the server type (protocol).",           t => types.Add (t.ToLower()) },
                { "p|port=", "the server port.", (int p) => ports.Add (p) },
                { "o|output=", "output formatting.", o => outputFormat = o },
                { "w|wait=", "wait time between checks - only for autorefresh.", (int w) => waitTime = w },

                { "v", "increase debug message verbosity", v => {
                    if (v != null)
                        ++verbosity;
                } },
                { "h|help", "show this message and exit", h => shouldShowHelp = h != null },
            };


            // parse extra command line arguments
            List<string> extra;
            try
            {
                // parse the command line
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                // output some error message
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write("Error: ");
                switch (e.OptionName)
                {
                    case "-s":
                        Console.WriteLine("-s needs to a valid hostname or IP address");
                        break;
                    case "-p":
                        Console.WriteLine("-p needs to be a number");
                        break;
                    case "-t":
                        Console.WriteLine("-t needs to be a valid game type");
                        break;
                    case "-o":
                        Console.WriteLine("-o needs to be a valid game type");
                        break;
                    case "-w":
                        Console.WriteLine("-w needs to be a number");
                        break;
                }
                //Console.WriteLine(e.Message);
                Console.ResetColor();
                Console.WriteLine("Try `--help' for more information.");
                return;
            }

            // Show help if help parameter is set
            if (shouldShowHelp)
            {
                ShowHelp(options);
                return;
            }

            // Validate command line arguments
            // check if amount of given arguments is divisible by 3 without remainder (ensures equal amount of arguments are given per server)
            int equalAmount = servers.Count + types.Count + ports.Count;
            if ((servers.Count >= 1 && types.Count >= 1 && ports.Count >= 1) && equalAmount % 3 == 0)
            {
                // at least 1 required parameter set given
                // check server name / ip against RFC 1123 specification
                Regex ValidIpAddressRegex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
                Regex ValidHostnameRegex = new Regex(@"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$");

                foreach (var server in servers)
                {
                    Match MatchIP = ValidIpAddressRegex.Match(server);
                    Match MatchHostname = ValidHostnameRegex.Match(server);
                    if (MatchIP.Success || MatchHostname.Success)
                    {
                        // valid hostname or IP found
                    }
                    else
                    {
                        // invalid hostname or IP found
                        DisplayParameterError("Error: " + server + " is not a valid hostname or IP!", options);
                    }
                }
                // check if game type is valid
                // fix upper / lowercase issues first
                foreach (var type in types)
                {
                   string result = FindValidGameType(type, validGameTypes);
                   if (result == "")
                   {
                        DisplayParameterError("Error: " + type + " is not a valid game type!", options);
                    }
                }
            }
            else
            {
                // not enough parameters for a valid set given
                // exit and display help message
                DisplayParameterError("Error: Not enough arguments given. Required are a set per server (hostname / IP, port and type)", options);
            }


            // Enable continuous check
            if (outputFormat.Equals("autorefresh"))
            {
                autorefresh = true;
            }

            while (true)
            {
                // Check each server given via command line options and output the result to the console
                int servercounter = 0;
                string output = "";
                AutoUpdate(autorefresh, servers.Count(), waitTime);

                while (servercounter < servers.Count())
                {
                    string errorMsg = "";
                    string errorDetail = "";
                    serverStatus status = new serverStatus();

                    if (CheckReachable(servers.ElementAt(servercounter).ToString()))
                    {
                        status = checkServer(servers.ElementAt(servercounter).ToString(), FindValidGameType(types.ElementAt(servercounter).ToString(), validGameTypes), ports.ElementAt(servercounter));
                    }
                    else
                    {
                        status = new serverStatus();
                        status.serverOnline = false;
                    }

                    if (status.serverOnline)
                    {
                        // Remove color codes for better readability. maybe add colors to console later on
                        //status.serverName = Regex.Replace(status.serverName, @"\^[0-9]{1}", "");
                        switch (outputFormat)
                        {
                            case "console":
                                printColored("Server: " + servers.ElementAt(servercounter).ToString() + " (" + status.serverName + ") is online: " + "Scan time " + status.scanTime + "ms and " + status.playersConnected + "/" + status.maxPlayers + " players are connected. Map: " + status.serverMap, ConsoleColor.DarkGreen);
                                break;
                            case "autorefresh":
                                printColored("Server: " + servers.ElementAt(servercounter).ToString() + " (" + status.serverName + ") is online: " + "Scan time " + status.scanTime + "ms and " + status.playersConnected + "/" + status.maxPlayers + " players are connected. Map: " + status.serverMap, ConsoleColor.DarkGreen);
                                break;
                            case "xml":
                                output += "<prtg>" +
                                            "<result>" +
                                                "<channel>Server scan time</channel>" +
                                                "<value>" + status.scanTime + "</value>" +
                                                "<unit>TimeResponse</unit>" + "</result>" +
                                            "<result>" +
                                                "<channel>Players connected</channel>" +
                                                "<value>" + status.playersConnected + "</value>" +
                                                "<unit>Count</unit>" +
                                                "<LimitMaxWarning>" + (status.maxPlayers - 1) + "</LimitMaxWarning>" +
                                            "</result>" +
                                            "</prtg>";
                                break;
                        }
                        Console.ResetColor();
                    }
                    else
                    {
                        // return error message
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        switch (outputFormat)
                        {
                            case "console":
                                Console.WriteLine("Server: " + servers.ElementAt(servercounter).ToString() + " is offline: " + "Scan time " + status.scanTime);
                                break;
                            case "autorefresh":
                                Console.WriteLine("Server: " + servers.ElementAt(servercounter).ToString() + " is offline: " + "Scan time " + status.scanTime);
                                break;
                            case "xml":
                                output += "<prtg>" +
                                        "<error>" + errorMsg + "</error>" +
                                        "<text>Can't reach server: " + errorDetail + "</text>" +
                                        "</prtg>";
                                break;
                        }
                        Console.ResetColor();
                    }
                    //Console.WriteLine(message, server);
                    servercounter++;
                }

                Console.Write(output);
                if (!autorefresh)
                {
                    break;
                }
                else
                {
                    // Sleep for waitTime seconds before checking servers again
                    Thread.Sleep(waitTime * 1000);
                }
                //Console.ReadLine();
            }
        }

        private static List<string> GenerateValidGameTypes()
        {
            List<string> validGameTypes = new List<string>();
            validGameTypes.Add("Quake3");
            validGameTypes.Add("Ase");
            validGameTypes.Add("HalfLife");
            validGameTypes.Add("GameSpy");
            validGameTypes.Add("GameSpy2");
            validGameTypes.Add("Source");
            validGameTypes.Add("Samp");
            validGameTypes.Add("Doom3");
            return validGameTypes;
        }

        public static string FindValidGameType(string gameTypeArg, List<string>validGameTypes)
        {
            bool found = false;
            string returnValue = "";
            foreach (var gameType in validGameTypes)
            {
                if (gameType.Equals(gameTypeArg, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    returnValue = gameType;
                }
            }
            if (found)
            {
                return returnValue;
            }
            else
            {
                return returnValue = "";
            }
        }

        static void ShowHelp(OptionSet options)
        {
            // show some app description message
            Console.WriteLine("Usage: gsmon.exe [OPTIONS]");
            Console.WriteLine("Example: gsmon.exe -s servername.com -p 12345 -t Quake3");
            Console.WriteLine("Version " + version + " - Visit https://github.com/roots84/Gameserver-Monitor for support");
            Console.WriteLine();

            // output the options
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }

        static void Debug(string format, params object[] args)
        {
            if (verbosity > 0)
            {
                Console.Write("# ");
                Console.WriteLine(format, args);
            }
        }

        static void DisplayParameterError(string message, OptionSet options)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(message);
            Console.ResetColor();
            ShowHelp(options);
            Environment.Exit(1);
        }

        public static void printColored (string message, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            List<int> toColor = new List<int>();
            Regex hasColorcode = new Regex(@"\^[0-9]{1}");

            if (hasColorcode.IsMatch(message))
            {
                foreach (Match match in hasColorcode.Matches(message))
                {
                    toColor.Add(match.Index);
                }

                for (int i=0; i<message.Length; i++)
                {
                    if (toColor.Count == 0)
                    {
                        if (((i+11) < message.Length) && message.Substring(i, 11).Equals(@") is online"))
                        {
                            Console.ForegroundColor = color;
                        }
                        Console.Write(message[i]);
                    }
                    else if (toColor.ElementAt(0) > i)
                    {
                        Console.Write(message[i]);
                    }
                    else
                    {
                        char codeChar = message[i+1];
                        char currentLetter = message[i];
                        char nextLetter = message[i + 1];
                        int codeNumber = (int)Char.GetNumericValue(codeChar);
                        Console.ForegroundColor = getColorbyCode(codeNumber);
                        Console.Write(message[i+2]);
                        if (toColor.Count() > 0)
                        {
                            toColor.RemoveAt(0);
                        }
                        i=i+2;
                    }
                }
            }
            else
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }

        public static ConsoleColor getColorbyCode(int colorCode)
        {
            switch (colorCode)
            {
                case 0:
                    return ConsoleColor.Red;
                case 1:
                    return ConsoleColor.Green;
                case 2:
                    return ConsoleColor.Yellow;
                case 3:
                    return ConsoleColor.Blue;
                case 4:
                    return ConsoleColor.Cyan;
                case 5:
                    return ConsoleColor.DarkMagenta;
                case 6:
                    return ConsoleColor.White;
                case 7:
                    return ConsoleColor.DarkGreen;
                case 8:
                    return ConsoleColor.DarkGray;
                case 9:
                    return ConsoleColor.Black;
            }
            return ConsoleColor.Gray;
        }

        public static serverStatus checkServer(string serveraddress, string gametype, int serverport)
        {
            serverStatus returnValues = new serverStatus();

            // query the selected server
            GameServerInfo.GameType type = (GameServerInfo.GameType)Enum.Parse(typeof(GameServerInfo.GameType), gametype.First().ToString().ToUpper() + gametype.Substring(1));
            GameServerInfo.GameServer server = new GameServerInfo.GameServer(serveraddress, serverport, type);
            // Enable or disable debugmode (writes lastquery.dat to disk)
            server.DebugMode = false;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            server.QueryServer();

            watch.Stop();
            returnValues.scanTime = watch.ElapsedMilliseconds;

            var testD = server.Parameters["sv_maxclients"];

            // Clear old stuff
            if (!server.IsOnline)
            {
                // Server is offline or not responding
                returnValues.serverOnline = false;
                return returnValues;
            }
            else
            {
                returnValues.serverOnline = true;
                returnValues.playersConnected = server.NumPlayers;
                returnValues.maxPlayers = server.MaxPlayers;
                returnValues.serverName= server.Name;
                returnValues.serverMod = server.Mod;
                returnValues.serverMap = server.Map;
                returnValues.serverHasPassword = server.Passworded;
                // returnValues.serverType = server._type;
                return returnValues;
            }
        }

        public static bool CheckReachable(string host)
        {
            Ping ping = new Ping();
            bool resolveOK = false;
            try
            {
                var address = Dns.GetHostEntry(host).AddressList.First();
                resolveOK = true;
            }
            catch (Exception e)
            {
                //Console.Write("error resolving name to address: " + host + "Error: " + e.ToString());
                return false;
            }
            if (resolveOK)
            {
                PingReply reply = ping.Send(host);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public static void AutoUpdate(bool autorefresh, int servers, int waitTime)
        {
            if (autorefresh)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine("Automatic refresh is enabled. Press CTRL+C to cancel");
                Console.WriteLine("Checking " + servers + " server(s) every " + waitTime + " seconds...");
                Console.WriteLine("Last update: " + DateTime.Now);
                Console.WriteLine();
            }
        }
    }

    public class serverStatus
    {
        public bool serverOnline { get; set; }
        public int playersConnected { get; set; }
        public long scanTime { get; set; }
        public int maxPlayers { get; set; }
        public string serverName { get; set; }
        public string serverMod { get; set; }
        public string serverMap { get; set; }
        public bool serverHasPassword { get; set; }
        //public GameServerInfo.GameType serverType { get; set; }
    }

}
