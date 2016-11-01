using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public class GameView : MonoBehaviour
	{
		#region Constants
		//======================================================================

		private const float NEW_LEVEL_TIME = 5f;
		private const float RESPAWN_SHIP_TIME = 2f;

		#endregion

		#region External references
		//======================================================================

		public GameObject shipPrefab;
		public GameObject asteroidPrefab;
		public GameObject saucerPrefab;
		public Text mainText;
		public Text scoreText;
		public Text livesText;

		#endregion

		#region Public methods
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
		
		public AsteroidCtrl SpawnAsteroid ()
		{				
			return Instantiate (asteroidPrefab).GetComponent<AsteroidCtrl> ();
		}

		public void WaitToSpawnSaucer (Action spawnSacucer)
		{
			StartCoroutine ("WaitAndSpawnSaucer2", spawnSacucer);
		}

		private IEnumerator WaitAndSpawnSaucer2 (Action spawnSacucer)
		{
			yield return new WaitForSeconds (3 + Random.value * 27);
			spawnSacucer ();
		}

		public void CancelSpawnSaucer ()
		{
			StopCoroutine ("WaitAndSpawnSaucer2");
		}

		public SaucerCtrl SpawnSaucer ()
		{				
			return Instantiate (saucerPrefab).GetComponent<SaucerCtrl> ();
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
			mainText.enabled = true;
		}

		#endregion
	}
}