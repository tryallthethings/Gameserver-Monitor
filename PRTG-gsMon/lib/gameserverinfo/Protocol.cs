#region Using directives
using System;
using System.Net;
using System.Collections.Specialized;
using System.Net.Sockets;
#endregion

namespace GameServerInfo
{
	internal abstract class Protocol
	{
		private Socket _serverConnection;
		private IPEndPoint _remoteIpEndPoint;
		private byte[] _sendBuffer, _readBuffer;
		private int _timeout = 1500, _offset;
		private DateTime _scanTime;
		private bool _debugMode;
        private bool _queryInProgress = false;

		protected string _requestString = "", _responseString = "";
		protected bool _isOnline = true;
		protected int _packages;
		protected GameProtocol _protocol;
		protected PlayerCollection _players;
		protected StringDictionary _params;
		protected StringCollection _teams;

        internal string _host;
        internal int _port;

		public Protocol()
		{
			_players = new PlayerCollection();
			_params = new StringDictionary();
			_teams = new StringCollection();
		}
        public Protocol(string host, int port) : this() {
            _host = host;
            _port = port;
        }

		#region Protocted members
		protected void Connect( string host, int port )
		{
			_serverConnection = new Socket( AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp );
			_serverConnection.SetSocketOption( SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, _timeout );

			IPAddress ip;
			try
			{
				ip = IPAddress.Parse( host );
			}
			catch ( System.FormatException )
			{
				ip = Dns.GetHostEntry( host ).AddressList[0];
				//ip = Dns.Resolve( host ).AddressList[0];
			}
			_remoteIpEndPoint = new IPEndPoint( ip, port );
            _isOnline = true;
		}
        protected void Connect() {
            Connect(_host, _port);
        }

		protected void Query( string request )
		{
            if (_queryInProgress) { throw new InvalidOperationException("Another query for this server is in progress"); }
            _queryInProgress = true;
			_readBuffer = new byte[100 * 1024]; // 100kb should be enought
			EndPoint _remoteEndPoint = (EndPoint)_remoteIpEndPoint;
			_packages = 0;
			int read = 0, bufferOffset = 0;

			// Request
			_sendBuffer = System.Text.Encoding.Default.GetBytes( request );
			_serverConnection.SendTo( _sendBuffer, _remoteIpEndPoint );
			
			// Response
			do
			{
				read = 0;
				try
				{
					// Multipackage check
					if ( _packages > 0 )
					{
						switch ( _protocol )
						{
							case GameProtocol.HalfLife:
							case GameProtocol.Source:
								byte[] _tempBuffer = new byte[100 * 1024];
								read = _serverConnection.ReceiveFrom( _tempBuffer, ref _remoteEndPoint );

								int packets = ( _tempBuffer[8] & 15 );
								int packetNr = ( _tempBuffer[8] >> 4 ) + 1;

								if ( packetNr < packets )
								{
									Array.Copy( _readBuffer, 9, _tempBuffer, read, bufferOffset );
									_readBuffer = _tempBuffer;
								}
								else
								{
									Array.Copy( _tempBuffer, 9, _readBuffer, bufferOffset, read );
								}
								_tempBuffer = null;
								break;
							
							case GameProtocol.GameSpy:
							case GameProtocol.GameSpy2:
								read = _serverConnection.ReceiveFrom( _readBuffer, bufferOffset, ( _readBuffer.Length - bufferOffset ), SocketFlags.None, ref _remoteEndPoint );
								break;

							default:
							case GameProtocol.Ase:
							case GameProtocol.Quake3:
								// these protocols don't support multi packages (i guess)
								break;
						}
					}
					// first package
					else
					{
						read = _serverConnection.ReceiveFrom( _readBuffer, ref _remoteEndPoint );
					}
					bufferOffset += read;
					_packages++;
				}
				catch ( System.Net.Sockets.SocketException e)
				{
#if DEBUG_QUERY
                    System.Diagnostics.Trace.TraceError("Socket exception " + e.SocketErrorCode + " " + e.Message);
#endif
                    break;
				}
			} while ( read > 0 );

			_scanTime = DateTime.Now;

			if ( bufferOffset > 0 && bufferOffset < _readBuffer.Length )
			{
				byte[] temp = new byte[bufferOffset];
				for ( int i = 0; i < temp.Length; i++ )
				{
                    temp[i] = _readBuffer[i];
				}
				_readBuffer = temp;
				temp = null;
			}
			else
			{
#if DEBUG_QUERY
                System.Diagnostics.Trace.TraceError("Answer is either zero-length or exceeds buffer length");
#endif
				_isOnline = false;
			}
			_responseString = System.Text.Encoding.Default.GetString( _readBuffer );
            _queryInProgress = false;

			if ( _debugMode )
			{
				System.IO.FileStream stream = System.IO.File.OpenWrite( ".dat" );
				stream.Write( _readBuffer, 0, _readBuffer.Length );
				stream.Close();
			}
		}

		protected void AddParams( string[] parts )
		{
			if ( !IsOnline )
			{
				return;
			}
			string key, val;
			for ( int i = 0; i < parts.Length; i++ )
			{
				if ( parts[i].Length == 0 )
				{
					continue;
				}
				key = parts[i++];
				val = parts[i];

				// Gamespy uses this
				if ( key == "final" )
				{
					break;
				}
				if ( key == "querid" )
				{
					continue;
				}

				_params[key] = val;
			}
		}

		protected byte[] Response
		{
			get
			{
				return _readBuffer;
			}
		}

		protected string ResponseString
		{
			get
			{
				return _responseString;
			}
		}

		protected int Offset
		{
			get
			{
				return _offset;
			}
			set
			{
				_offset = value;
			}
		}

		protected string ReadNextParam( int offset )
		{
			if ( offset > _readBuffer.Length )
			{
				throw new IndexOutOfRangeException();
			}
			_offset = offset;
			return ReadNextParam();
		}

		protected string ReadNextParam()
		{
			string temp = "";
			for ( ; _offset < _readBuffer.Length; _offset++ )
			{
				if ( _readBuffer[_offset] == 0 )
				{
					_offset++;
					break;
				}
				temp += (char)_readBuffer[_offset];
			}
			return temp;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the connectiontimeout
		/// </summary>
		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				_timeout = value;
			}
		}

		/// <summary>
		/// Gets the parsed parameters
		/// </summary>
		public StringDictionary Parameters
		{
			get
			{
				return _params;
			}
		}

		/// <summary>
		/// Gets the teamnames, not always set
		/// </summary>
		public StringCollection Teams
		{
			get
			{
				return _teams;
			}
		}

		/// <summary>
		/// Gets the players on the server
		/// </summary>
		public PlayerCollection Players
		{
			get
			{
				return _players;
			}
		}

		/// <summary>
		/// Gets the number of players on the server
		/// </summary>
		public int NumPlayers
		{
			get
			{
				return (_players.Count==0 && _params["numplayers"]!=null)? 
							Int16.Parse(_params["numplayers"]) 
					: _players.Count;
			}
		}

		/// <summary>
		/// Gets the time the last scan
		/// </summary>
		public DateTime ScanTime
		{
			get
			{
				return _scanTime;
			}
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
				_debugMode = value;
			}
		}
		#endregion

		#region Abstract and virtual members
		/// <summary>
		/// Querys the serverinfos
		/// </summary>
        public virtual void GetServerInfo()
        {
            if (!IsOnline)
            {
                this.Connect(); //try reconnect
            }
        }

		/// <summary>
		/// Gets the server name
		/// </summary>
		public virtual string Name
		{
			get
			{
				if ( !_isOnline )
				{
					return null;
				}
				return _params["hostname"];
			}
		}

		/// <summary>
		/// Gets the active modification
		/// </summary>
		public virtual string Mod
		{
			get
			{
				if ( !_isOnline )
				{
					return null;
				}
				return _params["modname"];
			}
		}

		/// <summary>
		/// Gets the mapname
		/// </summary>
		public virtual string Map
		{
			get
			{
				if ( !_isOnline )
				{
					return null;
				}
				return _params["mapname"];
			}
		}

		/// <summary>
		/// Gets if the server is password protected
		/// </summary>
		public virtual bool Passworded
		{
			get
			{
				if ( _params.ContainsKey( "passworded" ) && ( _params["passworded"] != "0" ) )
				{
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Gets if the server is online
		/// </summary>
		public virtual bool IsOnline
		{
			get
			{
				return _isOnline;
			}
		}

		/// <summary>
		/// Gets the maximal player number
		/// </summary>
		public virtual int MaxPlayers
		{
			get
			{
                try
                {
                    return Int16.Parse(_params["maxplayers"]);
                }
                catch {
                    return -1;
                }
			}
		}
		#endregion
	}
}