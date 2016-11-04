using UnityEngine;
using System;

namespace CgfGames
{
	/// <summary>
	/// Current state of the game.
	/// </summary>
	[Serializable]
	public class GameState
	{
		#region Public fields & properties
		//======================================================================

		/// <summary>
		/// The current player score.
		/// </summary>
		[SerializeField]
		private int _score;
		public int Score {
			get { return _score; }
			set {
				int oldScore = _score;
				_score = value;
				if (this.ScoreUpdatedEvent != null) {
					this.ScoreUpdatedEvent (oldScore, _score);
				}
			}
		}

		/// <summary>
		/// The current player lives.
		/// </summary>
		[SerializeField]
		private int _lives;
		public int Lives {
			get { return _lives; }
			set {
				int oldLives = _lives;
				_lives = value;
				if (this.LivesUpdatedEvent != null) {
					this.LivesUpdatedEvent (oldLives, _lives);
				}
			}
		}

		/// <summary>
		/// The current level.
		/// </summary>
		[SerializeField]
		private int _level;
		public int Level {
			get { return _level; }
			set { _level = value; }
		}

		#endregion

		#region Public fields & properties
		//======================================================================

		/// <summary>
		/// Occurs when score is updated.
		/// </summary>
		public event Action<int, int> ScoreUpdatedEvent;

		/// <summary>
		/// Occurs when lives are updated.
		/// </summary>
		public event Action<int, int> LivesUpdatedEvent;

		#endregion

		#region Public fields & properties
		//======================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="CgfGames.GameState"/>
		/// class.
		/// </summary>
		public GameState ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CgfGames.GameState"/> class.
		/// </summary>
		/// <param name="score">Initial score.</param>
		/// <param name="lives">Initial lives.</param>
		/// <param name="level">Initial level.</param>
		public GameState (int score, int lives, int level)
		{
			_score = score;
			_lives = lives;
			Level = level;
		}

		#endregion
	}
}
