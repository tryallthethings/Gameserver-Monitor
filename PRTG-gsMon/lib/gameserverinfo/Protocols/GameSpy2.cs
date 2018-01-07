#region Using directives
using System;
#endregion

namespace GameServerInfo
{
	internal class GameSpy2 : GameServerInfo.Protocol
	{
		#region Query strings
		private string _QUERY_BASE = System.Text.Encoding.Default.GetString( new byte[] {
			0xFE, 0xFD, 0x00, 0x04, 0x05, 0x06, 0x07
		} );

		private string _SERVERINFOS = System.Text.Encoding.Default.GetString( new byte[] {
			0xFF, 0x00, 0x00
		} );

		private string _PLAYERINFOS = System.Text.Encoding.Default.GetString( new byte[] {
			0x00, 0xFF, 0x00
		} );

		private string _TEAMINFOS = System.Text.Encoding.Default.GetString( new byte[] {
			0x00, 0x00, 0xFF
		} );

		private string _SEPERATOR = System.Text.Encoding.Default.GetString( new byte[] {
			0x00, 0x00
		} );
		#endregion

		/// <param name="host">Serverhost address</param>
		/// <param name="port">Serverport</param>
        public GameSpy2(string host, int port)
            : base(host, port)
		{
			base._protocol = GameProtocol.GameSpy2;
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
			Query( _QUERY_BASE + _SERVERINFOS );
			ParseDetails();

			Query( _QUERY_BASE + _PLAYERINFOS );
			ParsePlayers();

			Query( _QUERY_BASE + _TEAMINFOS );
			ParseTeam();
		}

		/// <summary>
		/// Gets the active modification
		/// </summary>
		public override string Mod
		{
			get
			{
				return _params["all_active_mods"];
			}
		}

		/// <summary>
		/// Gets if the server is password protected
		/// </summary>
		public override bool Passworded
		{
			get
			{
				return ( _params.ContainsKey( "password" ) && _params["password"] == "0" );
			}
		}

		#region Private methods
		private void ParseDetails()
		{
			if ( !IsOnline )
			{
				return;
			}
			Offset = 5;
			string key, val;

			while ( Offset < Response.Length )
			{
				key = ReadNextParam();
				val = ReadNextParam();
				if ( key.Length == 0 )
				{
					continue;
				}
				_params[key] = val;
			}
		}

		// TODO make this generic would be much nicer...
		private void ParsePlayers()
		{
			if ( !IsOnline )
			{
				return;
			}
			Offset = ResponseString.IndexOf( _SEPERATOR ) + 2;

			int pNr;
			while ( Offset < Response.Length )
			{
				pNr = _players.Add( new Player() );
				_players[pNr].Name = ReadNextParam();
				_players[pNr].Score = Int32.Parse( ReadNextParam() );
				_players[pNr].Parameters.Add( "deaths", ReadNextParam() );
				_players[pNr].Ping = Int32.Parse( ReadNextParam() );
				_players[pNr].Team = ReadNextParam();
				_players[pNr].Parameters.Add( "kills", ReadNextParam() );
			}
		}

		// TOTO only tested on BFV
		private void ParseTeam()
		{
			if ( !IsOnline )
			{
				return;
			}
			Offset = ResponseString.IndexOf( _SEPERATOR, 7 ) + 2;

			_teams.Add( ReadNextParam() );
			ReadNextParam();
			_teams.Add( ReadNextParam() );
		}
		#endregion
	}
}
