using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace CgfGames
{
	/// <summary>
	/// Controller for the game.
	/// </summary>
	public interface IGameCtrl
	{
		/// <summary>
		/// Starts the game.
		/// </summary>
		void StartGame ();

		/// <summary>
		/// Starts next level.
		/// </summary>
		void StartLevel ();

		/// <summary>
		/// Perform needed game actions when the ship is destroyed.
		/// </summary>
		void ShipDestroyed ();

		/// <summary>
		/// Spawn the player ship again after it was destroyed.
		/// </summary>
		void RespawnShip ();

		/// <summary>
		/// Spawns an asteroid of max size at random position.
		/// </summary>
		/// <returns>The asteroid.</returns>
		IAsteroidCtrl SpawnAsteroid ();

		/// <summary>
		/// Perform needed game actions when an asteroid is destroyed.
		/// </summary>
		/// <param name="asteroid">Destroyed astroid.</param>
		/// <param name="childAsteroids">New child asteroids of the destroyed
		/// asteroid.</param>
		void AsteroidDestroyed (IAsteroidCtrl asteroid, 
            List<IAsteroidCtrl> childAsteroids);

		/// <summary>
		/// Spawns a saucer of a parameterized random size.
		/// </summary>
		void SpawnSaucer ();

		/// <summary>
		/// Spawns a saucer of a givem size.
		/// </summary>
		/// <param name="size">Size of the saucer.</param>
		void SpawnSaucer (int size);

		/// <summary>
		/// Perform needed game actions when a saucer is destroyed.
		/// </summary>
		/// <param name="asteroid">Destroyed saucer.</param>
		void SaucerDestroyed (ISaucerCtrl saucer);

		/// <summary>
		/// Perform needed game actions when a saucer leaves.
		/// </summary>
		/// <param name="asteroid">Destroyed saucer.</param>
		void SaucerGone (ISaucerCtrl saucer);

		/// <summary>
		/// Perform needed game actions when the score is updated.
		/// </summary>
		/// <param name="oldScore">Score before updater.</param>
		/// <param name="oldScore">Score after updater.</param>
		void ScoreUpdated (int oldScore, int score);

		/// <summary>
		/// Perform needed game actions when game over happnes.
		/// </summary>
		void GameOver ();

	}

	/// <summary>
	/// IGameCtrl implementation.
	/// </summary>
	[Serializable]
	public class GameCtrl
	{
		#region Constants
		//======================================================================

		/// <summary>
		/// Number of lives when the game starts.
		/// </summary>
		public const int INIT_LIVES = 2;

		/// <summary>
		/// Points needed to earn an extra life.
		/// </summary>
		public const int LIFE_POINTS = 10000;

		/// <summary>
		/// Points earn when an asteroid is destroyed. Indexed by size.
		/// </summary>
		public static readonly int[] ASTEROIDS_POINTS = new int[] {100, 50, 20};

		/// <summary>
		/// Points earn when an saucer is destroyed. Indexed by size.
		/// </summary>
		public static readonly int[] SAUCERS_POINTS = new int[] {1000, 200};

		/// <summary>
		/// Chance to spawn a small saucer when the game starts.
		/// </summary>
		private const float INIT_SMALL_SAUCER_CHANCE = 0.2f;

		/// <summary>
		/// After this much points, only small saucers are spawned.
		/// </summary>
		private const int ONLY_SMALL_SAUCER_POINTS = 40000;

		#endregion

		#region Public fields & properties
		//======================================================================

		/// <summary>
		/// The state of the game.
		/// </summary>
		[SerializeField]
		private IGameStateCtrl _gameState;
		public IGameStateCtrl GameState {
			get { return _gameState; } private set { _gameState = value; }
		}

		/// <summary>
		/// View associated to this controller.
		/// </summary>
		public IGameView View { get; private set; }

		/// <summary>
		/// The ship.
		/// </summary>
		public IShipCtrl Ship { get; private set; }

		/// <summary>
		/// Saucer currently on the game.
		/// </summary>
		public ISaucerCtrl Saucer { get; private set; } 

		/// <summary>
		/// List of spwned asteroids alive.
		/// </summary>
		public List<IAsteroidCtrl> AsteroidList { get; private set; }

		#endregion

		#region Init
		//======================================================================

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="CgfGames.GameCtrl"/> class.
		/// </summary>
		/// <param name="view">Associated view.</param>
		/// <param name="ship">The ship.</param>
		public GameCtrl (IGameView view, IShipCtrl ship) :
			this (new GameStateCtrl (0, INIT_LIVES, 0), view, ship)
		{
		}

		/// <summary>
		/// Initializes a new instance of the
		/// <see cref="CgfGames.GameCtrl"/> class.
		/// </summary>
		/// <param name="gameState">Initial game state.</param>
		/// <param name="view">Associated view.</param>
		/// <param name="ship">The ship.</param>
		public GameCtrl (IGameStateCtrl gameState, IGameView view, IShipCtrl ship)
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

		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.StartGame()"/>.
		/// </summary>
		public void StartGame ()
		{
			this.StartLevel ();
			this.View.UpdateScore (0, this.GameState.Score);
			this.View.UpdateLives (0, this.GameState.Lives);
		}
			
		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.StartLevel()"/>.
		/// </summary>
		public void StartLevel ()
		{
			this.GameState.Level++;
			// Spawn asteroids.
			int numAsteroids = this.GameState.Level + 3;
			for (int i = 0; i < numAsteroids ; i++) {
				this.SpawnAsteroid ();
			}
			// Start timer to spawn a saucer.
			this.View.WaitToSpawnSaucer(this.SpawnSaucer);
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.ShipDestroyed()"/>.
		/// </summary>
		public void ShipDestroyed ()
		{
			// Don't spawn saucers anymore.
			this.View.CancelSpawnSaucer ();
			if (this.GameState.Lives == 0) {
				// Game over :'(
				this.GameOver ();
			} else {
				this.GameState.Lives--;
				// Start timer to respawn the ship.
				this.View.WaitToRespawnShip (this.RespawnShip);
			}
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.RespawnShip()"/>.
		/// </summary>
		private void RespawnShip ()
		{
			// Perform view actions.
			this.Ship.Respawn ();
			// Start timer to spawn a new saucer.
			this.View.WaitToSpawnSaucer(this.SpawnSaucer);
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.SpawnAsteroid()"/>.
		/// </summary>
		public IAsteroidCtrl SpawnAsteroid ()
		{
			// Spawn asteroid
			AsteroidCtrl asteroid = new AsteroidCtrl (
				this.View.SpawnAsteroid (Ship.Pos), AsteroidCtrl.MAX_SIZE
			);
			// Add it to the list of living asteroid and register event
			// handlers.
			this.AsteroidList.Add (asteroid);
			asteroid.DestroyedEvent += AsteroidDestroyed;
			return asteroid;
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.AsteroidDestroyed"/>.
		/// </summary>
		public void AsteroidDestroyed (IAsteroidCtrl asteroid,
			List<IAsteroidCtrl> childAsteroids)
		{
			// Update score.
			this.GameState.Score += ASTEROIDS_POINTS[asteroid.Size];
			// Register child asteroids if any on the list.
			if (childAsteroids != null) {
				this.AsteroidList.AddRange (childAsteroids);
				foreach (AsteroidCtrl ac in childAsteroids) {
					ac.DestroyedEvent += this.AsteroidDestroyed;
				}
			}
			// Unregister this asteroid from it list.
			this.AsteroidList.Remove (asteroid as AsteroidCtrl);
			// Check if Level Finished.
			this.CheckLevelFinished ();
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.SpawnSaucer()"/>.
		/// </summary>
		public void SpawnSaucer ()
		{
			this.SpawnSaucer (this.GetNextSaucerSize ());
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.SpawnSaucer(int)"/>.
		/// </summary>
		public void SpawnSaucer (int size)
		{
			// Spawn saucer, register it on its field.
			this.Saucer = new SaucerCtrl (
				this.GameState, this.View.SpawnSaucer (size), this.Ship, size
			);
			// Spawn saucer, register event handlers.
			this.Saucer.DestroyedEvent += SaucerDestroyed;
			this.Saucer.GoneEvent += SaucerGone;
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.SaucerDestroyed"/>.
		/// </summary>
		public void SaucerDestroyed (ISaucerCtrl saucer)
		{
			// Update score
			this.GameState.Score += SAUCERS_POINTS [saucer.Size];
			// Unregister from its field.
			this.Saucer = null;
			// Start timer to spawn a new saucer.
			this.View.WaitToSpawnSaucer(this.SpawnSaucer);
			// Check if Level Finished.
			this.CheckLevelFinished ();
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.SaucerGone"/>.
		/// </summary>
		public void SaucerGone (ISaucerCtrl saucer)
		{
			// Unregister from its field.
			this.Saucer = null;
			// Start timer to spawn a new saucer if ship alive.
			if (this.Ship.IsAlive) {
				this.View.WaitToSpawnSaucer (this.SpawnSaucer);
			}
			// Check if Level Finished.
			this.CheckLevelFinished ();
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IGameCtrl.ScoreUpdated"/>.
		/// </summary>
		public void ScoreUpdated (int oldScore, int score)
		{
			// Check for life earned from points.
			if (oldScore / LIFE_POINTS < score / LIFE_POINTS) {
				this.GameState.Lives++;
				this.View.LifeUp ();
			}
			// Perform view actions.
			this.View.UpdateScore (oldScore, score);
		}

		public void GameOver ()
		{
			// Perform view actions.
			this.View.GameOver ();
		}

		#endregion

		#region Private methods
		//======================================================================

		/// <summary>
		/// Gets the size of the next saucer.
		/// </summary>
		/// <returns>The next saucer size.</returns>
		private int GetNextSaucerSize ()
		{
			// We calculate probability to spawn a big or small saucer.
			// We have an initial probability to spawn a small one, which
			// increases with score. From a certain score on, we only get small
			// saucers.
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

		/// <summary>
		/// Checks if the level is finished.
		/// </summary>
		private void CheckLevelFinished ()
		{
			if (AsteroidList.Count == 0 && Saucer == null) {
				// Don't spawn saucers anymore.
				View.CancelSpawnSaucer ();
				// Perform view actions.
				View.LevelFinished (this.StartLevel);
			}
		}

		#endregion
	}
}