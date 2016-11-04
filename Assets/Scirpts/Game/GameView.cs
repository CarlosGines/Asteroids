using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

using Random = UnityEngine.Random;

namespace CgfGames
{
	/// <summary>
	/// View for the game
	/// </summary>
	public interface IGameView
	{
		/// <summary>
		/// Start timer to respawn the ship.
		/// </summary>
		/// <param name="respawn">Respawn ship action.</param>
		void WaitToRespawnShip (Action respawn);
	
		/// <summary>
		/// Spawns an asteroid.
		/// </summary>
		/// <returns>The asteroid.</returns>
		/// <param name="shipPos">Ship position.</param>
		IAsteroidView SpawnAsteroid (Vector3 shipPos);

		/// <summary>
		/// Start timer to spawn a saucer.
		/// </summary>
		/// <param name="spawnSacucer">Spawn sacucer action.</param>
		void WaitToSpawnSaucer (Action spawnSacucer);

		/// <summary>
		/// Spawns a saucer.
		/// </summary>
		/// <returns>The saucer.</returns>
		/// <param name="size">Size of the new saucer.</param>
		ISaucerView SpawnSaucer (int size);

		/// <summary>
		/// Stop timer to spawn a saucer.
		/// </summary>
		void CancelSpawnSaucer ();

		/// <summary>
		/// Perform actions needed when a level is finished.
		/// </summary>
		/// <param name="onComplete">On complete action.</param>
		void LevelFinished (Action onComplete);

		/// <summary>
		/// Update score on the view.
		/// </summary>
		/// <param name="oldScore">Score before update.</param>
		/// <param name="score">Score after update.</param>
		void UpdateScore (int oldScore, int score);

		/// <summary>
		/// Perform actions needed when a life is earned.
		/// </summary>
		void LifeUp ();

		/// <summary>
		/// Updates lives on the view.
		/// </summary>
		/// <param name="oldLives">Lives before update.</param>
		/// <param name="lives">Lives after update.</param>
		void UpdateLives (int oldLives, int lives);

		/// <summary>
		/// Perform actions needed when game is over.
		/// </summary>
		void GameOver ();
	}

	/// <summary>
	/// IGameView implementation.
	/// </summary>
	public class GameView : MonoBehaviour, IGameView
	{
		#region Constants
		//======================================================================

		/// <summary>
		/// Time to start a new level after previous on is finished.
		/// </summary>
		private const float NEW_LEVEL_TIME = 2f;

		/// <summary>
		/// Minimum time to respawn the ship after it is destroyed.
		/// </summary>
		private const float MIN_RESPAWN_SHIP_TIME = 2f;

		/// <summary>
		/// Minimum time to spawn a sacuer once we are told to do so.
		/// </summary>
		private const float SPAWN_SAUCER_MIN_TIME = 3f;

		/// <summary>
		/// Maximum time to spawn a sacuer once we are told to do so.
		/// </summary>
		private const float SPAWN_SAUCER_MAX_TIME = 30f;

		#endregion

		#region External references
		//======================================================================

		/// <summary>
		/// The asteroid game objects pool.
		/// </summary>
		public ObjectPool asteroidPool;

		/// <summary>
		/// The power up game objects pool.
		/// </summary>
		public ObjectPool powerupPool;

		/// <summary>
		/// The asteroid explosion game objects pool.
		/// </summary>
		public ObjectPool asteroidExplosionPool;

		/// <summary>
		/// The asteroid game objects pool.
		/// </summary>
		public SaucerView saucerView;

		/// <summary>
		/// The main text on the middle of the screen.
		/// </summary>
		public Text mainText;

		/// <summary>
		/// The score text.
		/// </summary>
		public Text scoreText;

		/// <summary>
		/// The lives text.
		/// </summary>
		public Text livesText;

		/// <summary>
		/// The audio source that plays life up fx.
		/// </summary>
		public AudioSource lifeUpAudio;

		/// <summary>
		/// The game over audio clip.
		/// </summary>
		public AudioClip gameOverClip;

		#endregion

		#region Cached components
		//======================================================================

		/// <summary>
		/// Audio souce component cached
		/// </summary>
		private AudioSource _audio;

		#endregion

		#region Private fields
		//======================================================================

		/// <summary>
		/// Routine to spawn a saucer based on time.
		/// </summary>
		private IEnumerator _waitToSpawnSaucerRoutine;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Check external references
			Assert.IsNotNull (this.asteroidPool);
			Assert.IsNotNull (this.powerupPool);
			Assert.IsNotNull (this.asteroidExplosionPool);
			Assert.IsNotNull (this.saucerView);
			Assert.IsNotNull (this.mainText);
			Assert.IsNotNull (this.scoreText);
			Assert.IsNotNull (this.livesText);
			Assert.IsNotNull (this.lifeUpAudio);
			Assert.IsNotNull (this.gameOverClip);

			// Cache components
			_audio = GetComponent<AudioSource> ();
		}

		#endregion

		#region IGameView Public methods
		//======================================================================

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.WaitToRespawnShip"/> on
		/// the next coroutine.
		/// </summary>
		public void WaitToRespawnShip (Action respawn)
		{
			StartCoroutine (WaitToRespawnShip2 (respawn));
		}
			
		/// <summary>
		/// Coroutine for the previous method.
		/// </summary>
		private IEnumerator WaitToRespawnShip2 (Action respawn)
		{
			yield return new WaitForSeconds (MIN_RESPAWN_SHIP_TIME);
			// Do not respawn while center position is not free.
			while (Physics2D.OverlapCircle (Vector2.zero, 1.5f) != null) {
				yield return 0;
			}
			respawn ();
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.SpawnAsteroid"/>
		/// </summary>
		public IAsteroidView SpawnAsteroid (Vector3 shipPos)
		{				
			// Get random asteroid init pos far from the ship.
			Vector3 pos = SpaceObjectMngr.RandomPos ();
			while (SpaceObjectMngr.SmartPath (pos, shipPos).sqrMagnitude < 
					2.5f * 2.5f) {
				pos = SpaceObjectMngr.RandomPos ();
			}
			// Spawn and init ship.
			AsteroidView asteroidView = this.asteroidPool
				.Get (
					pos, Quaternion.Euler (0, 0, Random.value * 360)
				).GetComponent<AsteroidView> ();
			asteroidView.Init (
				AsteroidCtrl.MAX_SIZE,
				this.asteroidPool,
				this.powerupPool,
				this.asteroidExplosionPool
			);
			return asteroidView;
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.WaitToSpawnSaucer"/>
		/// in the next coroutine.
		/// </summary>
		public void WaitToSpawnSaucer (Action spawnSacucer)
		{
			// Avoid starting timer twice.
			if (_waitToSpawnSaucerRoutine == null) {
				_waitToSpawnSaucerRoutine = WaitToSpawnSaucer2 (spawnSacucer);
				StartCoroutine (_waitToSpawnSaucerRoutine);
			}
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.SpawnAsteroid"/>
		/// </summary>
		private IEnumerator WaitToSpawnSaucer2 (Action spawnSacucer)
		{
			yield return new WaitForSeconds (
				SPAWN_SAUCER_MIN_TIME + Random.value * (
					SPAWN_SAUCER_MAX_TIME - SPAWN_SAUCER_MIN_TIME
				)
			);
			spawnSacucer ();
			_waitToSpawnSaucerRoutine = null;
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.SpawnSaucer"/>
		/// </summary>
		public ISaucerView SpawnSaucer (int size)
		{				
			this.saucerView.gameObject.SetActive (true);
			this.saucerView.Init (size, SaucerCtrl.SPEED [size]);
			return this.saucerView;
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.SpawnSaucer"/>
		/// </summary>
		public void CancelSpawnSaucer ()
		{
			if (_waitToSpawnSaucerRoutine != null) {
				StopCoroutine (_waitToSpawnSaucerRoutine);
				_waitToSpawnSaucerRoutine = null;
			}
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.LevelFinished"/>
		/// on the next coroutine
		/// </summary>
		public void LevelFinished (Action onComplete)
		{
			StartCoroutine (LevelFinished2 (onComplete));
		}

		/// <summary>
		/// Coroutine for the previous method.
		/// </summary>
		private IEnumerator LevelFinished2 (Action onComplete)
		{
			yield return new WaitForSeconds (NEW_LEVEL_TIME);
			onComplete ();
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.UpdateScore"/>
		/// </summary>
		public void UpdateScore (int oldScore, int score)
		{
			this.scoreText.text = score.ToString ();
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.LifeUp"/>
		/// </summary>
		public void LifeUp ()
		{
			this.lifeUpAudio.Play ();
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.UpdateLives"/>
		/// </summary>
		public void UpdateLives (int oldLives, int lives)
		{
			this.livesText.text = lives.ToString () + "  x  ";
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.GameOver"/> on the
		/// next coroutine.
		/// </summary>
		public void GameOver ()
		{
			StartCoroutine (this.GameOver2 ());
		}

		/// <summary>
		/// Coroutine for the previous method.
		/// </summary>
		private IEnumerator GameOver2 ()
		{
			_audio.clip = gameOverClip;
			_audio.Play ();
			this.mainText.enabled = true;
			yield return new WaitForSeconds (5f);
			SceneManager.LoadScene (0);
		}

		#endregion
	}
}
