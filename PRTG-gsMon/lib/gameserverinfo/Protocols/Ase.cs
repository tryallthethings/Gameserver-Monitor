#region Using directives
using System;
#endregion

namespace GameServerInfo
{
	internal class Ase : GameServerInfo.Protocol
	{
		#region Query strings
		private const string _QUERY_BASE = "s";

//		private string _QUERY_BASE = System.Text.Encoding.Default.GetString( new byte[] {
//			0x08, 0x80
//		} );
		#endregion

        public Ase(string host, int port)
            : base(host, port)
		{
			base._protocol = GameProtocol.Ase;
			Connect( host, port );
		}

		public override void GetServerInfo()
		{
            base.GetServerInfo();
			if ( !IsOnline )
			{
				return;
			}
			Query( _QUERY_BASE );
			ParseDetails();
		}

		private void ParseDetails()
		{
			Offset = 5;
			_params["gamename"] = ReadNextString();
			_params["port"] = ReadNextString();
			_params["hostname"] = ReadNextString();
			_params["gametype"] = ReadNextString();
			_params["mapname"] = ReadNextString();
			_params["version"] = ReadNextString();
			_params["passworded"] = ReadNextString();
			_params["players"] = ReadNextString();
			_params["maxplayers"] = ReadNextString();

			// params
			while ( Offset < Response.Length )
			{
				if ( Response[Offset - 1] == 1 )
				{
					break;
				}
				_params[ReadNextString()] = ReadNextString();
			}

			// players
			int pNr = 0, checkOffset = Offset;
			while ( Offset < Response.Length )
			{
				pNr = _players.Add( new Player() );
				Offset += 2;

				if ( ( Response[checkOffset] & 1 ) != 0 )
				{
					_players[pNr].Name = ReadNextString();
				}
				if ( ( Response[checkOffset] & 2 ) != 0 )
				{
					_players[pNr].Team = ReadNextString();
				}
				if ( ( Response[checkOffset] & 4 ) != 0 )
				{
					_players[pNr].Parameters.Add( "skin", ReadNextString() );
				}
				if ( ( Response[checkOffset] & 8 ) != 0 )
				{
					_players[pNr].Score = Int16.Parse( ReadNextString() );
				}
				if ( ( Response[checkOffset] & 16 ) != 0 )
				{
					_players[pNr].Ping = Int16.Parse( ReadNextString() );
				}
				if ( ( Response[checkOffset] & 32 ) != 0 )
				{
					int time = Int16.Parse( ResponseString[Offset++].ToString() );
					if ( (char)Response[Offset] == 'm' )
					{
						_players[pNr].Time = new TimeSpan( 0, time, 0 );
					}
					else
					{
						_players[pNr].Time = new TimeSpan( time, 0, 0 );
					}
				}

				try
				{
					Offset++;

					if ( Response[checkOffset] != Response[Offset] )
					{
						Offset++;
					}

					// just to be shure ...
					if ( ( Response[checkOffset] & 64 ) != 0 )
					{
						ReadNextString();
					}
					if ( ( Response[checkOffset] & 128 ) != 0 )
					{
						ReadNextString();
					}
				}
				catch ( ArgumentOutOfRangeException )
				{
					break;
				}
			}
		}

		private string ReadNextString()
		{
			int oldOffset = Offset;
			Offset += Response[Offset - 1];

			return ResponseString.Substring( oldOffset, ( Response[oldOffset - 1] - 1 ) );
		}

		public override string Mod
		{
			get
			{
				return null;
			}
		}


	}
}
