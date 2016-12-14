using UnityEngine;
using System;

namespace CgfGames
{
/// <summary>
	/// Current state of the game.
	/// </summary>
	public interface IGameStateCtrl
	{
		/// <summary>
		/// The current player score.
		/// </summary>
		int Score {	get; set; }

		/// <summary>
		/// The current player lives.
		/// </summary>
		int Lives {	get; set; }


		/// <summary>
		/// The current level.
		/// </summary>
		int Level {	get; set; }

		/// <summary>
		/// Occurs when score is updated.
		/// </summary>
		event Action<int, int> ScoreUpdatedEvent;

		/// <summary>
		/// Occurs when lives are updated.
		/// </summary>
		event Action<int, int> LivesUpdatedEvent;
	}

	/// <summary>
	/// Current state of the game.
	/// </summary>
	[Serializable]
	public class GameStateCtrl : IGameStateCtrl
	{
		#region Public fields & properties
		//======================================================================

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

		[SerializeField]
		private int _level;
		public int Level {
			get { return _level; }
			set { _level = value; }
		}

		#endregion

		#region Events
		//======================================================================

		public event Action<int, int> ScoreUpdatedEvent;

		public event Action<int, int> LivesUpdatedEvent;

		#endregion

		#region Constructors
		//======================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="CgfGames.GameState"/>
		/// class.
		/// </summary>
		public GameStateCtrl ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CgfGames.GameState"/> class.
		/// </summary>
		/// <param name="score">Initial score.</param>
		/// <param name="lives">Initial lives.</param>
		/// <param name="level">Initial level.</param>
		public GameStateCtrl (int score, int lives, int level)
		{
			_score = score;
			_lives = lives;
			Level = level;
		}

		#endregion
	}
}
