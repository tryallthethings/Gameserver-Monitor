#region Using directives

using System;
using System.Collections.Specialized;

#endregion

namespace GameServerInfo
{
	/// <summary>
	/// Represents a player on a server
	/// </summary>
	public class Player
	{
		private string _name, _team;
		private int _score, _ping;
		private TimeSpan _time;
		private StringDictionary _params;

		/// <summary>
		/// Represents a player on a server
		/// </summary>
		public Player()
		{
			_params = new StringDictionary();
		}

		/// <param name="name">Playername</param>
		/// <param name="score">Playerscore</param>
		public Player( string name, int score ) : this()
		{
			_name = name;
			_score = score;
		}

		/// <param name="name">Playername</param>
		/// <param name="score">Playerscore</param>
		/// <param name="ping">Playerping</param>
		public Player( string name, int score, int ping ) : this( name, score )
		{
			_ping = ping;
		}

		/// <param name="name">Playername</param>
		/// <param name="team">Teamname</param>
		/// <param name="score">Playerscore</param>
		/// <param name="ping">Playerping</param>
		public Player( string name, string team, int score, int ping ) : this( name, score, ping )
		{
			_team = team;
		}

		/// <summary>
		/// Gets the playername
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets the playerscore
		/// </summary>
		public int Score
		{
			get
			{
				return _score;
			}
			set
			{
				_score = value;
			}
		}

		/// <summary>
		/// Gets the ping of the player
		/// </summary>
		public int Ping
		{
			get
			{
				return _ping;
			}
			set
			{
				_ping = value;
			}
		}

		/// <summary>
		/// Gets the team of the player
		/// </summary>
		public string Team
		{
			get
			{
				return _team;
			}
			set
			{
				_team = value;
			}
		}

		/// <summary>
		/// Gets the time that the player is on the server
		/// </summary>
		public TimeSpan Time
		{
			get
			{
				return _time;
			}
			set
			{
				_time = value;
			}
		}

		/// <summary>
		/// Gets extended parameters
		/// </summary>
		public StringDictionary Parameters
		{
			get
			{
				return _params;
			}
			set
			{
				_params = value;
			}
		}
	}
}
