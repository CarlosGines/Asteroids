using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public class GameCtrl : MonoBehaviour
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

		#region External references
		//======================================================================

		public GameView view;
		public ShipCtrl shipCtrl;

		#endregion

		#region Private properties
		//======================================================================

		[SerializeField]
		private GameState _gameState;
		private SaucerCtrl _saucerCtrl;
		private List<AsteroidCtrl> _asteroidCtrlList;

		#endregion

		#region Init
		//======================================================================

		void Start ()
		{
			this.Init ();
			this.StartGame ();
		}

		void Init ()
		{
			_gameState = new GameState (0, INIT_LIVES, 0);
			_asteroidCtrlList = new List<AsteroidCtrl> ();
			_gameState.ScoreUpdatedEvent += this.ScoreUpdated;
			_gameState.LivesUpdatedEvent += this.view.UpdateLives;
			shipCtrl.DestroyedEvent += ShipDestroyed;
		}

		#endregion

		#region Public methods
		//======================================================================

		public void StartGame ()
		{
			this.StartLevel ();
			view.UpdateScore (0, _gameState.Score);
			view.UpdateLives (0, _gameState.Lives);
		}


		public void StartLevel ()
		{
			_gameState.Level++;
			for (int i = 0; i < _gameState.Level + 3 ; i++) {
				this.SpawnAsteroid ();
			}
			this.view.WaitToSpawnSaucer(this.SpawnSaucer);
		}

		public void SpawnAsteroid ()
		{
			AsteroidCtrl asteroidCtrl = this.view.SpawnAsteroid ();
			asteroidCtrl.Init (AsteroidCtrl.MAX_SIZE);
			_asteroidCtrlList.Add (asteroidCtrl);
			asteroidCtrl.DestroyedEvent += AsteroidDestroyed;
		}

		public void SpawnSaucer ()
		{
			this.SpawnSaucer (this.GetNextSaucerSize ());
		}

		public void SpawnSaucer (int size)
		{
			_saucerCtrl = this.view.SpawnSaucer ();
			_saucerCtrl.Init (_gameState, shipCtrl, size);
			_saucerCtrl.DestroyedEvent += SaucerDestroyed;
			_saucerCtrl.GoneEvent += SaucerGone;
		}

		public void GameOver ()
		{
			view.GameOver ();
		}

		#endregion

		#region Private methods
		//======================================================================

		private void ScoreUpdated (int oldScore, int score)
		{
			if (oldScore / 10000 < score / 10000) {
				_gameState.Lives++;
			}
			this.view.UpdateScore (oldScore, score);
		}

		private void ShipDestroyed ()
		{
			this.view.CancelSpawnSaucer ();
			if (_gameState.Lives == 0) {
				this.GameOver ();
			} else {
				_gameState.Lives--;
				this.view.WaitToRespawnShip (this.RespawnShip);
			}
		}

		private void RespawnShip ()
		{
			shipCtrl.Respawn ();
			this.view.WaitToSpawnSaucer(this.SpawnSaucer);
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
			this.view.WaitToSpawnSaucer(this.SpawnSaucer);
			this.CheckLevelFinished ();
		}

		private void SaucerGone (SaucerCtrl saucerCtrl)
		{
			_saucerCtrl = null;
			this.view.WaitToSpawnSaucer(this.SpawnSaucer);
			this.CheckLevelFinished ();
		}

		private void CheckLevelFinished ()
		{
			if (_asteroidCtrlList.Count == 0 && _saucerCtrl == null) {
				this.view.CancelSpawnSaucer ();
				this.view.LevelFinished (this.StartLevel);
			}
		}

		#endregion
	}
}