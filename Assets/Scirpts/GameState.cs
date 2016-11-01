using UnityEngine;
using System;

namespace CgfGames
{
	[Serializable]
	public class GameState
	{
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

		public event Action<int, int> ScoreUpdatedEvent;
		public event Action<int, int> LivesUpdatedEvent;

		public GameState (int score, int lives, int level)
		{
			_score = score;
			_lives = lives;
			Level = level;
		}
	}
}
