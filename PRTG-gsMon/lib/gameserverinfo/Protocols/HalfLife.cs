#region Using directives
using System;
#endregion

namespace GameServerInfo
{
	internal class HalfLife : GameServerInfo.Protocol
	{
		#region Querystrings
		// FF FF FF FF 64 65 74 61 69 6C 73
		private const string _QUERY_DETAILS = @"ÿÿÿÿdetails"; // Details

		// FF FF FF FF 72 75 6C 65 73
		private const string _QUERY_RULES = @"ÿÿÿÿrules"; // Rules

		// FF FF FF FF 70 6C 61 79 65 72 73
		private const string _QUERY_PLAYERS = @"ÿÿÿÿplayers"; // Players

		// not used yet
		private const string _QUERY_PING = @"ÿÿÿÿping";
		private const string _QUERY_INFO = @"ÿÿÿÿinfostring";
		#endregion

		/// <param name="host">Serverhost address</param>
		/// <param name="port">Serverport</param>
        public HalfLife(string host, int port)
            : base(host, port)
		{
			base._protocol = GameProtocol.HalfLife;
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
			Query( _QUERY_DETAILS );
			ParseDetails();

			//Query( _QUERY_RULES );
			//ParseRules();

			//Query( _QUERY_PLAYERS );
			//ParsePlayers();
		}

		/// <summary>
		/// Gets the active modification
		/// </summary>
		public override string Mod
		{
			get
			{
				return _params["mod"];
			}
		}

		#region Private methods
		private void ParseDetails()
		{
			Offset = 6;
			_params["serveraddress"] = ReadNextParam();
			_params["hostname"] = ReadNextParam();
			_params["mapname"] = ReadNextParam();
			_params["mod"] = ReadNextParam();
			_params["modname"] = ReadNextParam();
            _params["numplayers"] = Response[Offset++].ToString();
			_params["maxplayers"] = Response[Offset++].ToString();
			_params["protocolver"] = Response[Offset++].ToString();

			_params["servertype"] = ( (char)Response[Offset++] ).ToString();
			_params["serveros"] = ( (char)Response[Offset++] ).ToString();
			_params["passworded"] = Response[Offset++].ToString();
			_params["modded"] = Response[Offset].ToString();

			// not used
			if ( Response[Offset++] == 1 )
			{
//				_params["modwebpage"] = ReadNextParam();
//				_params["moddlserver"] = ReadNextParam();
			}
		}

		private void ParseRules()
		{
			string key, val;
			Offset = 7;

			for ( int i = 0; i < ( BitConverter.ToInt16( Response, 5 ) * 2 ); i++ )
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

		private void ParsePlayers()
		{
			_params["numplayers"] = Response[5].ToString();
			Offset = 6;

			int pNr;
			for ( int i = 0; i < Response[5]; i++ )
			{
				pNr = _players.Add( new Player() );
				_players[pNr].Parameters.Add( "playernr", Response[Offset++].ToString() );
				_players[pNr].Name = ReadNextParam();
				_players[pNr].Score = BitConverter.ToInt32( Response, Offset );
				Offset += 4;
				_players[pNr].Time = new TimeSpan( 0, 0, (int)BitConverter.ToSingle( Response, Offset ) );
				Offset += 4;
			}
		}
		#endregion
	}
}
