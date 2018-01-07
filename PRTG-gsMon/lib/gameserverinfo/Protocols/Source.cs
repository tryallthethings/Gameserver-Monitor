#region Using directives
using System;
using System.Text;
#endregion

namespace GameServerInfo
{
	internal class Source : GameServerInfo.Protocol
	{
		
		// FF FF FF FF 55 FF FF FF FF
		private const string _QUERY_GET_CHALLENGE = @"ÿÿÿÿUÿÿÿÿ";
		
		// FF FF FF FF 54 Source Engine Query + 0x00
		private const string _QUERY_DETAILS = @"ÿÿÿÿTSource Engine Query";

		// FF FF FF FF 56
		private const string _QUERY_RULES = @"ÿÿÿÿV";

		// FF FF FF FF 55
		private const string _QUERY_PLAYERS = @"ÿÿÿÿU";
		
		private string challenge;

		/// <param name="host">Serverhost address</param>
		/// <param name="port">Serverport</param>
		public Source( string host, int port ) : base(host,port)
		{
			base._protocol = GameProtocol.Source;
			Connect( host, port );
		}

		/// <summary>
		/// Querys the serverinfos
		/// </summary>
		public override void GetServerInfo()
		{
            base.GetServerInfo();
            if (!IsOnline)
            {
                return;
            }

            //Query( _QUERY_GET_CHALLENGE );
            //ParseChallenge();

			Query( _QUERY_DETAILS+Encoding.Default.GetString(new Byte[] { 0x00 }));
			ParseDetails();

			//Query( _QUERY_RULES+this.challenge );
			//ParseRules();
			
			//Query( _QUERY_PLAYERS+this.challenge );
			//ParsePlayers(true);
		}
		private void ParseChallenge(){
			this.challenge = System.Text.Encoding.Default.GetString(base.Response, 5, 4);
		}
		private void ParseDetails()
		{
			_params["protocolver"] = Response[5].ToString();
			_params["hostname"] = ReadNextParam( 6 );
			_params["mapname"] = ReadNextParam();
			_params["mod"] = ReadNextParam();
			_params["modname"] = ReadNextParam();
			byte[] appid = new byte[2];
			Array.Copy(Response,this.Offset++,appid,0,2);
			_params["appid"] = System.BitConverter.ToInt16(appid,0).ToString();
			this.Offset++;
			_params["numplayers"] = Response[this.Offset++].ToString();
			_params["maxplayers"] = Response[this.Offset++].ToString();
			_params["botcount"] = Response[this.Offset++].ToString();
			_params["servertype"] = Response[this.Offset++].ToString();
			_params["serveros"] = Response[this.Offset++].ToString();
			_params["passworded"] = Response[this.Offset++].ToString();
			_params["secureserver"] = Response[this.Offset++].ToString();
			_params["version"] = ReadNextParam();
		}

		private void ParseRules()
		{
			string key, val;
			int ruleCount = BitConverter.ToInt16( Response, 5 );
			Offset = 7;

			for ( int i = 0; i < ( ruleCount * 2 ); i++ )
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

		private void ParsePlayers(bool autoConvertUnicode)
		{
			_params["numplayers"] = Response[5].ToString();
			Offset = 6;

			int pNr; int players=Response[5];
			
			for ( int i = 0; i < players; i++ )
			{
				pNr = _players.Add( new Player() );
				_players[pNr].Parameters.Add( "playernr", Response[Offset++].ToString() );
				if(autoConvertUnicode){
					//_players[pNr].Name = 
					
					byte[] ba=Encoding.GetEncoding(1251).GetBytes(ReadNextParam());
				
					char[] ca=new char[Encoding.UTF8.GetDecoder().GetCharCount(ba,0,ba.Length)];

					Encoding.UTF8.GetDecoder().GetChars(ba,0,ba.Length,ca,0);

				}else{
					_players[pNr].Name = ReadNextParam();
				}
				
				_players[pNr].Score = BitConverter.ToInt32( Response, Offset );
				Offset += 4;
				_players[pNr].Time = new TimeSpan( 0, 0, (int)BitConverter.ToSingle( Response, Offset ) );
				Offset += 4;
			}
		
			for ( int i = 0; i < Response[5]; i++ )
			{
				pNr = _players.Add( new Player() );
				try
				{
					_players[pNr].Parameters.Add( "playernr", Response[Offset++].ToString() );
					_players[pNr].Name = ReadNextParam();
					_players[pNr].Score = BitConverter.ToInt32( Response, Offset );
					Offset += 4;
					_players[pNr].Time = new TimeSpan( 0, 0, (int)BitConverter.ToSingle( Response, Offset ) );
					Offset += 4;
				}
				catch(Exception)
				{
					Offset += 8;
					_players.RemoveAt(pNr);
				}
			}
		}
	}
}