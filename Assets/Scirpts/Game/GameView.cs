﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public interface IGameView
	{
		void WaitToRespawnShip (Action respawn);
	
		IAsteroidView SpawnAsteroid ();

		void WaitToSpawnSaucer (Action spawnSacucer);

		ISaucerView SpawnSaucer (int size);

		void CancelSpawnSaucer ();

		void LevelFinished (Action onComplete);

		void UpdateScore (int oldScore, int score);

		void UpdateLives (int oldLives, int lives);

		void GameOver ();
	}

	public class GameView : MonoBehaviour, IGameView
	{
		#region Constants
		//======================================================================

		private const float NEW_LEVEL_TIME = 2f;
		private const float RESPAWN_SHIP_TIME = 2f;
		private const float SPAWN_SAUCER_MIN_TIME = 3f;
		private const float SPAWN_SAUCER_MAX_TIME = 30f;

		#endregion

		#region External references
		//======================================================================

		public GameObject shipPrefab;
		public ObjectPool asteroidsPool;
		public ObjectPool powerupPool;
		public SaucerView saucerView;
		public Text mainText;
		public Text scoreText;
		public Text livesText;

		#endregion

		#region Private fields
		//======================================================================

		private IEnumerator _waitToSpawnSaucerRoutine;

		#endregion

		#region IGameView Public methods
		//======================================================================

		public void WaitToRespawnShip (Action respawn)
		{
			StartCoroutine (WaitToRespawnShip2 (respawn));
		}

		private IEnumerator WaitToRespawnShip2 (Action respawn)
		{
			yield return new WaitForSeconds (RESPAWN_SHIP_TIME);
			respawn ();
		}
		
		public IAsteroidView SpawnAsteroid ()
		{				
			Debug.Log ("Dame asteroide pare");
			AsteroidView asteroidView = this.asteroidsPool
				.Get (
					SpaceObjectMngr.RandomPos (),
					Quaternion.Euler (0, 0, Random.value * 360)
				).GetComponent<AsteroidView> ();
			asteroidView.Init (
				AsteroidCtrl.MAX_SIZE,
				this.asteroidsPool,
				this.powerupPool
			);
			return asteroidView;
		}

		public void WaitToSpawnSaucer (Action spawnSacucer)
		{
			if (_waitToSpawnSaucerRoutine == null) {
				_waitToSpawnSaucerRoutine = WaitToSpawnSaucer2 (spawnSacucer);
				StartCoroutine (_waitToSpawnSaucerRoutine);
			}
		}

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

		public ISaucerView SpawnSaucer (int size)
		{				
			this.saucerView.gameObject.SetActive (true);
			this.saucerView.Init (size, SaucerCtrl.SPEED [size]);
			return this.saucerView;
		}
		
		public void CancelSpawnSaucer ()
		{
			if (_waitToSpawnSaucerRoutine != null) {
				StopCoroutine (_waitToSpawnSaucerRoutine);
				_waitToSpawnSaucerRoutine = null;
			}
		}

		public void LevelFinished (Action onComplete)
		{
			StartCoroutine (LevelFinished2 (onComplete));
		}

		private IEnumerator LevelFinished2 (Action onComplete)
		{
			yield return new WaitForSeconds (NEW_LEVEL_TIME);
			onComplete ();
		}

		public void UpdateScore (int oldScore, int score)
		{
			scoreText.text = score.ToString ();
		}

		public void UpdateLives (int oldLives, int lives)
		{
			livesText.text = lives.ToString ();
		}

		public void GameOver ()
		{
			StartCoroutine (this.GameOver2 ());
		}

		private IEnumerator GameOver2 ()
		{
			mainText.enabled = true;
			yield return new WaitForSeconds (3f);
			SceneManager.LoadScene (0);
		}

		#endregion
	}
}