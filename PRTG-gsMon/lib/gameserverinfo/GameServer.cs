#region Using directives
using System;
#endregion

namespace GameServerInfo
{
	/// <summary>
	/// A class to query gaming servers
	/// </summary>
	public class GameServer
	{
		private string _host;
		private int _port, _timeOut = 1500;
		private GameType _type;
		private Protocol _serverInfo;
		private bool _debugMode = false;

		/// <param name="host">The IP address or the hostname of the gameserver</param>
		/// <param name="port">The port of the gameserver</param>
		/// <param name="type">The gameserver type</param>
		public GameServer( string host, int port, GameType type )
		{
			_host = host;
			_port = port;
			_type = type;

			CheckServerType();
		}

		/// <param name="host">The IP address or the hostname of the gameserver</param>
		/// <param name="port">The port of the gameserver</param>
		/// <param name="type">The gameserver type</param>
		/// <param name="timeout">The timeout for the query</param>
		public GameServer( string host, int port, GameType type, int timeout )
		{
			_host = host;
			_port = port;
			_type = type;
			_timeOut = timeout;

			CheckServerType();
		}

		private void CheckServerType()
		{
			switch ( (GameProtocol)_type )
			{
                case GameProtocol.Samp:
                    _serverInfo = new Samp(_host, _port);
                    break;
				case GameProtocol.Ase:
					_serverInfo = new Ase( _host, _port );
					break;

				case GameProtocol.Doom3:
					_serverInfo = new Doom3( _host, _port );
					break;

				case GameProtocol.GameSpy:
					_serverInfo = new GameSpy( _host, _port );
					break;

				case GameProtocol.GameSpy2:
					_serverInfo = new GameSpy2( _host, _port );
					break;

				case GameProtocol.HalfLife:
					_serverInfo = new HalfLife( _host, _port );
					break;
					
				case GameProtocol.Quake3:
					_serverInfo = new Quake3( _host, _port );
					break;

				case GameProtocol.Source:
					_serverInfo = new Source( _host, _port );
					break;

				default:
				case GameProtocol.None:
					throw new System.NotImplementedException();
					
			}
			_serverInfo.DebugMode = _debugMode;
		}

		/// <summary>
		/// Querys the serverinfos
		/// </summary>
		public void QueryServer()
		{
			_serverInfo.GetServerInfo();
		}

		/// <summary>
		/// Cleans the color codes from the player names
		/// </summary>
		/// <param name="name">Playername</param>
		/// <returns>Cleaned playername</returns>
		public static string CleanName( string name )
		{
			System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex( @"(\^\d)|(\$\d)" );
			return regex.Replace( name, "" );
		}

		#region Properties
		/// <summary>
		/// Gets or sets the connectiontimeout
		/// </summary>
		public int Timeout
		{
			get
			{
				return _serverInfo.Timeout;
			}
			set
			{
				_serverInfo.Timeout = value;
			}
		}

		/// <summary>
		/// Gets the parsed parameters
		/// </summary>
		public System.Collections.Specialized.StringDictionary Parameters
		{
			get
			{
				return _serverInfo.Parameters;
			}
		}

		/// <summary>
		/// Gets if the server is online
		/// </summary>
		public bool IsOnline
		{
			get
			{
				return _serverInfo.IsOnline;
			}
		}
		
		/// <summary>
		/// Gets the time the last scan
		/// </summary>
		public DateTime ScanTime
		{
			get
			{
				return _serverInfo.ScanTime;
			}
		}

		/// <summary>
		/// Gets the players on the server
		/// </summary>
		public PlayerCollection Players
		{
			get
			{
				return _serverInfo.Players;
			}
		}

		/// <summary>
		/// Get the teamnames if there are any
		/// </summary>
		public System.Collections.Specialized.StringCollection Teams
		{
			get
			{
				return _serverInfo.Teams;
			}
		}

		/// <summary>
		/// Gets the maximal player number
		/// </summary>
		public int MaxPlayers
		{
			get
			{
				return _serverInfo.MaxPlayers;
			}
		}

		/// <summary>
		/// Gets the number of players on the server
		/// </summary>
		public int NumPlayers
		{
			get
			{
				return _serverInfo.NumPlayers;
			}
		}

		/// <summary>
		/// Gets the servername
		/// </summary>
		public string Name
		{
			get
			{
				return _serverInfo.Name;
			}
		}

		/// <summary>
		/// Gets the active modification
		/// </summary>
		public string Mod
		{
			get
			{
				return _serverInfo.Mod;
			}
		}

		/// <summary>
		/// Gets the mapname
		/// </summary>
		public string Map
		{
			get
			{
				return _serverInfo.Map;
			}
		}

		/// <summary>
		/// Gets if the server is password protected
		/// </summary>
		public bool Passworded
		{
			get
			{
				return _serverInfo.Passworded;
			}
		}

		/// <summary>
		/// Gets the server gametype
		/// </summary>
		public GameType GameType
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Gets the used protocol
		/// </summary>
		/// <value></value>
		public GameProtocol Protocol
		{
			get
			{
				return (GameProtocol)_type;
			}
		}
		/// <summary>
		/// Server Address
		/// </summary>
		public string Host {
			get { return _host; }
		}
		/// <summary>
		/// Server Port
		/// </summary>
		public int Port {
			get { return _port; }
		}
		/// <summary>
		/// Enables the debugging mode
		/// </summary>
		public bool DebugMode
		{
			get
			{
				return _debugMode;
			}
			set
			{
				if ( _serverInfo != null )
				{
					_serverInfo.DebugMode = value;
				}
				_debugMode = value;
			}
		}
		#endregion
	}
}
