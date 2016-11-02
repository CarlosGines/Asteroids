using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public interface IGameCtrl
	{
		void StartGame ();

		void StartLevel ();

		void ShipDestroyed ();

		IAsteroidCtrl SpawnAsteroid ();

		void AsteroidDestroyed (IAsteroidCtrl asteroidCtrl, 
            List<IAsteroidCtrl> childAsteroidCtrls);

		void SpawnSaucer ();

		void SpawnSaucer (int size);

		void SaucerDestroyed (ISaucerCtrl saucerCtrl);

		void SaucerGone (ISaucerCtrl saucerCtrl);

		void ScoreUpdated (int oldScore, int score);

		void GameOver ();

	}

	public class GameCtrl
	{
		#region Constants
		//======================================================================

		public const int INIT_LIVES = 2;
		public const int LIFE_POINTS = 10000;
		public static readonly int[] ASTEROIDS_POINTS = new int[] {100, 50, 20};
		public static readonly int[] SAUCERS_POINTS = new int[] {1000, 200};
		private const float INIT_SMALL_SAUCER_CHANCE = 0.2f;
		private const int ONLY_SMALL_SAUCER_POINTS = 40000;

		#endregion

		#region Public fields & properties
		//======================================================================

		public GameState GameState { get; private set; }
		public IGameView View { get; private set; }
		public IShipCtrl Ship { get; private set; }
		public ISaucerCtrl Saucer { get; private set; } 
		public List<IAsteroidCtrl> AsteroidList { get; private set; }

		#endregion

		#region Init
		//======================================================================

		public GameCtrl (IGameView view, IShipCtrl ship) :
			this (new GameState (0, INIT_LIVES, 0), view, ship)
		{
		}

		public GameCtrl (GameState gameState, IGameView view, IShipCtrl ship)
		{
			this.View = view;
			this.GameState = gameState;
			this.Ship = ship;
			this.AsteroidList = new List<IAsteroidCtrl> ();
			this.GameState.ScoreUpdatedEvent += this.ScoreUpdated;
			this.GameState.LivesUpdatedEvent += this.View.UpdateLives;
			this.Ship.DestroyedEvent += this.ShipDestroyed;
		}

		#endregion

		#region Public methods
		//======================================================================

		public void StartGame ()
		{
			this.StartLevel ();
			this.View.UpdateScore (0, this.GameState.Score);
			this.View.UpdateLives (0, this.GameState.Lives);
		}
			
		public void StartLevel ()
		{
			this.GameState.Level++;
			for (int i = 0; i < this.GameState.Level + 3 ; i++) {
				this.SpawnAsteroid ();
			}
			this.View.WaitToSpawnSaucer(this.SpawnSaucer);
		}

		public void ShipDestroyed ()
		{
			this.View.CancelSpawnSaucer ();
			if (this.GameState.Lives == 0) {
				this.GameOver ();
			} else {
				this.GameState.Lives--;
				this.View.WaitToRespawnShip (this.RespawnShip);
			}
		}

		public IAsteroidCtrl SpawnAsteroid ()
		{
			AsteroidCtrl asteroidCtrl = new AsteroidCtrl (
				this.View.SpawnAsteroid (), AsteroidCtrl.MAX_SIZE
			);
			this.AsteroidList.Add (asteroidCtrl);
			asteroidCtrl.DestroyedEvent += AsteroidDestroyed;
			return asteroidCtrl;
		}

		public void AsteroidDestroyed (IAsteroidCtrl asteroidCtrl,
			List<IAsteroidCtrl> childAsteroidCtrls)
		{
			this.GameState.Score += ASTEROIDS_POINTS[asteroidCtrl.Size];
			if (childAsteroidCtrls != null) {
				this.AsteroidList.AddRange (childAsteroidCtrls);
				foreach (AsteroidCtrl ac in childAsteroidCtrls) {
					ac.DestroyedEvent += this.AsteroidDestroyed;
				}
			}
			this.AsteroidList.Remove (asteroidCtrl);
			this.CheckLevelFinished ();
		}

		public void SpawnSaucer ()
		{
			this.SpawnSaucer (this.GetNextSaucerSize ());
		}

		public void SpawnSaucer (int size)
		{
			this.Saucer = new SaucerCtrl (
				this.GameState, this.View.SpawnSaucer (size), this.Ship, size
			);
			this.Saucer.DestroyedEvent += SaucerDestroyed;
			this.Saucer.GoneEvent += SaucerGone;
		}

		public void SaucerDestroyed (ISaucerCtrl saucerCtrl)
		{
			this.GameState.Score += SAUCERS_POINTS [saucerCtrl.Size];
			this.Saucer = null;
			this.View.WaitToSpawnSaucer(this.SpawnSaucer);
			this.CheckLevelFinished ();
		}

		public void SaucerGone (ISaucerCtrl saucerCtrl)
		{
			this.Saucer = null;
			this.View.WaitToSpawnSaucer(this.SpawnSaucer);
			this.CheckLevelFinished ();
		}

		public void ScoreUpdated (int oldScore, int score)
		{
			if (oldScore / LIFE_POINTS < score / LIFE_POINTS) {
				this.GameState.Lives++;
			}
			this.View.UpdateScore (oldScore, score);
		}

		public void GameOver ()
		{
			this.View.GameOver ();
		}

		#endregion

		#region Private methods
		//======================================================================
			
		private void RespawnShip ()
		{
			this.Ship.Respawn ();
			this.View.WaitToSpawnSaucer(this.SpawnSaucer);
		}

		// We calculate probability to spawn a big or small saucer.
		// We have an initial probability to spawn a small one, which
		// increases with score. From a certain score on, we only get small
		// saucers
		private int GetNextSaucerSize ()
		{
			float smallSaucerChance = INIT_SMALL_SAUCER_CHANCE + 
				(
					1f * this.GameState.Score / ONLY_SMALL_SAUCER_POINTS * 
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

		private void CheckLevelFinished ()
		{
			if (AsteroidList.Count == 0 && Saucer == null) {
				View.CancelSpawnSaucer ();
				View.LevelFinished (this.StartLevel);
			}
		}

		#endregion
	}
}