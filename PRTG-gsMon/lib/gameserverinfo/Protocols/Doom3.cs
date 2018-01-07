#region Using directives
using System;
#endregion

namespace GameServerInfo
{
	internal class Doom3 : GameServerInfo.Protocol
	{
		private const string _QUERY_STATUS = @"ÿÿgetInfo";

        public Doom3(string host, int port)
            : base(host, port)
		{
			base._protocol = GameProtocol.Doom3;
			Connect( host, port );
		}

		public override void GetServerInfo()
		{
            base.GetServerInfo();
			if ( !IsOnline )
			{
				return;
			}
			Query( _QUERY_STATUS );

			Offset = 23;
			string key, val;
			int playerOffset = ResponseString.IndexOf( System.Text.Encoding.Default.GetString( new byte[] {
				0, 0, 0 ,0
			} ) );

			playerOffset += 3;

			while ( Offset < playerOffset )
			{
				key = ReadNextParam();
				val = ReadNextParam();
			
				if ( key.Length == 0 )
				{
					continue;
				}
				_params[key] = val;
			}

			Offset = playerOffset;

			int pNr;
			while ( Offset < Response.Length )
			{
				// ok that's ugly, i fix this when someone playes doom3 online ;)
				try
				{
					pNr = _players.Add( new Player() );
					_players[pNr].Parameters.Add( "playernr", Response[Offset++].ToString() );
					_players[pNr].Ping = BitConverter.ToInt16( Response, Offset );
					Offset += 2;
					_players[pNr].Score = BitConverter.ToInt16( Response, Offset );
					Offset += 4; // we skip two empty bytes
					_players[pNr].Name = ReadNextParam();

					// is there a next playername?
					if ( ( Offset + 7 ) > Response.Length )
					{
						break;
					}
				}
				catch ( ArgumentException )
				{
					break;
				}
			}
		}

		public override string Name
		{
			get
			{
				return _params["si_name"];
			}
		}

		public override string Mod
		{
			get
			{
				return _params["gamename"];
			}
		}

		public override string Map
		{
			get
			{
				return _params["si_map"];
			}
		}

		public override int MaxPlayers
		{
			get
			{
				return Int16.Parse( _params["si_maxPlayers"] );
			}
		}

		public override bool Passworded
		{
			get
			{
				if ( _params.ContainsKey("si_usepass") && _params["si_usepass"] == "0" )
				{
					return false;
				}
				return true;
			}
		}
	}
}
