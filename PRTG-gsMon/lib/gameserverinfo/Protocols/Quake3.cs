#region Using directives
using System;
#endregion

namespace GameServerInfo
{
	internal class Quake3 : GameServerInfo.Protocol
	{
		#region Querystrings
		private const string _PLAYERREGEX = "(?<score>.+) (?<ping>.+) \\\"(?<name>.+)\\\"";
		
		private const string _QUERY_STATUS = @"ÿÿÿÿgetstatus";
		#endregion

        public Quake3(string host, int port)
            : base(host, port)
		{
			base._protocol = GameProtocol.Quake3;
			Connect( host, port );
		}

		/// <summary>
		/// Querys the serverinfos
		/// </summary>
		public override void GetServerInfo()
		{
            base.GetServerInfo();
			if ( !IsOnline )
			{
				return;
			}
			Query( _QUERY_STATUS );

			if ( ResponseString.IndexOf( "disconnect" ) != -1 )
			{
				_isOnline = false;
				return;
			}

			string[] lines = ResponseString.Substring( 3 ).Split( '\n' );
			if ( lines.Length < 2 )
			{
				_isOnline = false;
				return;
			}
			AddParams( lines[1].Split( '\\' ) );

			if ( lines.Length > 2 )
			{
				for ( int i = 2; i < lines.Length; i++ )
				{
					if ( lines[i].Length == 0 )
					{
						continue;
					}
					_players.Add( ParsePlayer( lines[i] ) );
				}
			}
		}

		// TODO
		/*
			The player info strings are a little different depending on the game:

			Quake 3: Score, Ping, Name 
			Soldier of Fortune 2: Score, Ping, Deaths, Name 
		*/
		private Player ParsePlayer( string PlayerInfo )
		{
			System.Text.RegularExpressions.Regex RegPattern = new System.Text.RegularExpressions.Regex( _PLAYERREGEX );
			System.Text.RegularExpressions.Match RegMatch = RegPattern.Match( PlayerInfo );
			return new Player( RegMatch.Groups["name"].Value, Int16.Parse( RegMatch.Groups["score"].Value ), Int16.Parse( RegMatch.Groups["ping"].Value ) );
		}

		#region Virtual members
		/// <summary>
		/// Gets the server name
		/// </summary>
		public override string Name
		{
			get
			{
				if ( !_isOnline )
				{
					return null;
				}
				return _params["sv_hostname"];
			}
		}

		/// <summary>
		/// Gets the active modification
		/// </summary>
		public override string Mod
		{
			get
			{
				if ( !_isOnline )
				{
					return null;
				}
				return _params["gamename"];
			}
		}

		/// <summary>
		/// Gets the maximum player number
		/// </summary>
		public override int MaxPlayers
		{
			get {
                try
                {
                    return Int16.Parse(_params["sv_maxclients"]);
                }
                catch {
                    return 0;
                }
			}
		}

		/// <summary>
		/// Gets if the server is password protected
		/// </summary>
		public override bool Passworded
		{
			get
			{
				if ( !_isOnline )
				{
					return false;
				}
				return ( _params.ContainsKey("g_needpass") && (_params["g_needpass"] == "0") );
			}

		}
		#endregion
	}

	/// <summary>
	/// Quake3 gametypes
	/// </summary>
	public enum Quake3GameType
	{
		/// <summary>
		/// Free for All
		/// </summary>
		Ffa = 0,

		/// <summary>
		/// One on One Tournament
		/// </summary>
		Tournament = 1,

		/// <summary>
		/// Single Player FFA
		/// </summary>
		SinglePlayer = 2,

		/// <summary>
		/// Team Deathmatch
		/// </summary>
		Team = 3,
		
		/// <summary>
		/// Capture the Flag
		/// </summary>
		Ctf = 4,

		/// <summary>
		/// One Flag CTF
		/// </summary>
		OneFlagCtf = 5,
		
		/// <summary>
		/// Obelisk
		/// </summary>
		Obelisk = 6,

		/// <summary>
		/// Harvester
		/// </summary>
		Harvester = 7,

		/// <summary>
		/// Team Tournament
		/// </summary>
		TeamTournament = 8
	}
}