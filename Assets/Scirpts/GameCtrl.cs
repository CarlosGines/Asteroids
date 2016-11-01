using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public class GameCtrl
	{
		#region Constants
		//======================================================================

		private const int INIT_LIVES = 2;
		private const int LIFE_POINTS = 10000;
		private static readonly int[] ASTEROIDS_POINTS = new int[] {100, 50, 20};
		private static readonly int[] SAUCERS_POINTS = new int[] {1000, 200};
		private const float INIT_SMALL_SAUCER_CHANCE = 0.2f;
		private const int ONLY_SMALL_SAUCER_POINTS = 40000;

		#endregion

		#region Private fields
		//======================================================================

		private GameView _view;
		private GameState _gameState;
		private ShipCtrl _shipCtrl;
		private SaucerCtrl _saucerCtrl;
		private List<AsteroidCtrl> _asteroidCtrlList;

		#endregion

		#region Init
		//======================================================================

		public GameCtrl (GameView view, ShipCtrl shipCtrl)
		{
			_view = view;
			_gameState = new GameState (0, INIT_LIVES, 0);
			_shipCtrl = shipCtrl;
			_asteroidCtrlList = new List<AsteroidCtrl> ();
			_gameState.ScoreUpdatedEvent += this.ScoreUpdated;
			_gameState.LivesUpdatedEvent += _view.UpdateLives;
			_shipCtrl.DestroyedEvent += ShipDestroyed;
		}

		#endregion

		#region Public methods
		//======================================================================

		public void StartGame ()
		{
			this.StartLevel ();
			_view.UpdateScore (0, _gameState.Score);
			_view.UpdateLives (0, _gameState.Lives);
		}


		public void StartLevel ()
		{
			_gameState.Level++;
			for (int i = 0; i < _gameState.Level + 3 ; i++) {
				this.SpawnAsteroid ();
			}
			_view.WaitToSpawnSaucer(this.SpawnSaucer);
		}

		public void SpawnAsteroid ()
		{
			AsteroidCtrl asteroidCtrl = new AsteroidCtrl (
				_view.SpawnAsteroid (),
				AsteroidCtrl.MAX_SIZE
			);
			_asteroidCtrlList.Add (asteroidCtrl);
			asteroidCtrl.DestroyedEvent += AsteroidDestroyed;
		}

		public void SpawnSaucer ()
		{
			this.SpawnSaucer (this.GetNextSaucerSize ());
		}

		public void SpawnSaucer (int size)
		{
			_saucerCtrl = _view.SpawnSaucer ();
			_saucerCtrl.Init (_gameState, _shipCtrl, size);
			_saucerCtrl.DestroyedEvent += SaucerDestroyed;
			_saucerCtrl.GoneEvent += SaucerGone;
		}

		public void GameOver ()
		{
			_view.GameOver ();
		}

		#endregion

		#region Private methods
		//======================================================================

		private void ScoreUpdated (int oldScore, int score)
		{
			if (oldScore / 10000 < score / 10000) {
				_gameState.Lives++;
			}
			_view.UpdateScore (oldScore, score);
		}

		private void ShipDestroyed ()
		{
			_view.CancelSpawnSaucer ();
			if (_gameState.Lives == 0) {
				this.GameOver ();
			} else {
				_gameState.Lives--;
				_view.WaitToRespawnShip (this.RespawnShip);
			}
		}

		private void RespawnShip ()
		{
			_shipCtrl.Respawn ();
			_view.WaitToSpawnSaucer(this.SpawnSaucer);
		}

		private void AsteroidDestroyed (AsteroidCtrl asteroidCtrl,
				List<AsteroidCtrl> childAsteroidCtrls)
		{
			_gameState.Score += ASTEROIDS_POINTS[asteroidCtrl.Size];
			if (childAsteroidCtrls != null) {
				_asteroidCtrlList.AddRange (childAsteroidCtrls);
				foreach (AsteroidCtrl ac in childAsteroidCtrls) {
					ac.DestroyedEvent += this.AsteroidDestroyed;
				}
			}
			_asteroidCtrlList.Remove (asteroidCtrl);
			this.CheckLevelFinished ();
		}

		// We calculate probability to spawn a big or small saucer.
		// We have an initial probability to spawn a small one, which
		// increases with score. From a certain score on, we only get small
		// saucers
		private int GetNextSaucerSize ()
		{
			float smallSaucerChance = INIT_SMALL_SAUCER_CHANCE + 
				(
					1f * _gameState.Score / ONLY_SMALL_SAUCER_POINTS * 
					(1 - INIT_SMALL_SAUCER_CHANCE)
				);
			int size;
			if (Random.value < smallSaucerChance) {
				size = SaucerCtrl.SMALL_SAUCER;
			} else {
				size = SaucerCtrl.BIG_SAUCER;
			}
			return size;
		}

		private void SaucerDestroyed (SaucerCtrl saucerCtrl)
		{
			_gameState.Score += SAUCERS_POINTS [saucerCtrl.Size];
			_saucerCtrl = null;
			_view.WaitToSpawnSaucer(this.SpawnSaucer);
			this.CheckLevelFinished ();
		}

		private void SaucerGone (SaucerCtrl saucerCtrl)
		{
			_saucerCtrl = null;
			_view.WaitToSpawnSaucer(this.SpawnSaucer);
			this.CheckLevelFinished ();
		}

		private void CheckLevelFinished ()
		{
			if (_asteroidCtrlList.Count == 0 && _saucerCtrl == null) {
				_view.CancelSpawnSaucer ();
				_view.LevelFinished (this.StartLevel);
			}
		}

		#endregion
	}
}