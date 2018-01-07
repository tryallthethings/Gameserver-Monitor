#region Using directives
using System;
#endregion

namespace GameServerInfo
{
	internal class GameSpy : GameServerInfo.Protocol
	{
		private const string _QUERY_INFO = @"\info\";
		
		private const string _QUERY_RULES = @"\rules\";
		
		private const string _QUERY_PLAYERS = @"\players\";

		/// <param name="host"></param>
		/// <param name="port"></param>
        public GameSpy(string host, int port)
            : base(host, port)
		{
			base._protocol = GameProtocol.GameSpy;
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
			Query( _QUERY_INFO );
			AddParams( ResponseString.Split( '\\' ) );

			Query( _QUERY_RULES );
			AddParams( ResponseString.Split( '\\' ) );

			Query( _QUERY_PLAYERS );
			ParsePlayers();
		}

		/// <summary>
		/// Gets the active modification
		/// </summary>
		public override string Mod
		{
			// I don't know if gamespy v1 already has this
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Gets if the server is password protected
		/// </summary>
		public override bool Passworded
		{
			get
			{
				if ( _params.ContainsKey( "password" ) && ( _params["password"] == "True" || _params["password"] == "1" ) )
				{
					return true;
				}
				return false;
			}
		}

		private void ParsePlayers()
		{
			if ( !IsOnline )
			{
				return;
			}

			if ( _params.ContainsKey("numplayers") )
			{
				_players = new PlayerCollection();// Int16.Parse( _params["numplayers"] ) );
			}

			string key, val;
			int pNr;
			string[] parts = ResponseString.Split( '\\' );
			for ( int i = 1; i < parts.Length; i++ )
			{
				if ( parts[i] == "queryid" || parts[i] == "final" )
				{
					i++;
					continue;
				}

				key = parts[i].Substring( 0, parts[i].IndexOf( "_" ) );
				pNr = Int16.Parse( parts[i].Substring( parts[i].IndexOf( "_" ) + 1 ) );
				val = parts[++i];

				if ( key == "teamname" )
				{
					_teams.Add( val );
				}

				try
				{
					if ( _players[pNr] == null )
					{
						_players.Insert( pNr, new Player() );
					}
				}
				catch ( ArgumentOutOfRangeException )
				{
					_players.Insert( pNr, new Player() );
				}

				switch ( key )
				{
					case "playername":
					case "player":
						_players[pNr].Name = val;
						break;
					
					case "score":
					case "frags":
						_players[pNr].Score = Int16.Parse( val );
						break;

					case "ping":
						_players[pNr].Ping = Int16.Parse( val );
						break;

					case "team":
						_players[pNr].Team = val;
						break;

					default:
						_players[pNr].Parameters.Add( key, val );
						break;
				}
			}
		}
	}
}
