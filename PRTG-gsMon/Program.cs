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
             * resolve FQDN to IP?
             * discover gameservers on given hostname / IP?
             *
             * Server name colors
             * The color codes are:
                    ^1 = red
                    ^2 = green
                    ^3 = yellow
                    ^4 = blue
                    ^5 = light blue
                    ^6 = purple
                    ^7 = white
                    ^8 is a color that changes depending what level you are on.
                    American maps = Dark Green
                    Russian maps = Dark red/marroon
                    British maps = Dark Blue
                    ^9 = grey
                    ^0 = black

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
            bool resolveFQDN = false;
            int waitTime = 10;
            List<string> validGameTypes = GenerateValidGameTypes();

            // these are the available options, not that they set the variables
            var options = new OptionSet {
                { "s|server=", "the server name (FQDN) or IP address.", s => servers.Add (s) },
                { "t|type=",   "the server type (protocol).",           t => types.Add (t.ToLower()) },
                { "p|port=", "the server port.", (int p) => ports.Add (p) },
                { "o|output=", "output formatting.", o => outputFormat = o },
                { "r|resolve=", "resolve FQDN to IP-address before query", r => resolveFQDN = r != null },
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
                   string typeCorrected = type.First().ToString().ToUpper() + type.Substring(1);
                   var matchingvalues = validGameTypes.Where(stringToCheck => stringToCheck.Contains(typeCorrected));
                   if (matchingvalues.Count() == 0)
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

            // Show help if help parameter is set
            if (shouldShowHelp)
            {
                ShowHelp(options);
                return;
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
                        status = checkServer(servers.ElementAt(servercounter).ToString(), types.ElementAt(servercounter).ToString(), ports.ElementAt(servercounter));
                    }
                    else
                    {
                        status = new serverStatus();
                        status.serverOnline = false;
                    }

                    if (status.serverOnline)
                    {
                        // Remove color codes for better readability. maybe add colors to console later on
                        status.serverName = Regex.Replace(status.serverName, @"\^[0-9]{1}", "");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        switch (outputFormat)
                        {
                            case "console":
                                Console.WriteLine("Server: " + servers.ElementAt(servercounter).ToString() + " (" + status.serverName + ") is online: " + "Scan time " + status.scanTime + "ms and " + status.playersConnected + "/" + status.maxPlayers + " players are connected. Map: " + status.serverMap);

                                break;
                            case "autorefresh":
                                Console.WriteLine("Server: " + servers.ElementAt(servercounter).ToString() + " (" + status.serverName + ") is online: " + "Scan time " + status.scanTime + "ms and " + status.playersConnected + "/" + status.maxPlayers + " players are connected. Map: " + status.serverMap);
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
