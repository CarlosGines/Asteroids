using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CgfGames
{
	[RequireComponent (typeof (SpaceObjectMngr))]
	public class ShotMngr : MonoBehaviour {

		#region Public properties
		//======================================================================

		public float speed;

		#endregion

		#region Cached components
		//======================================================================

		public Transform trans;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			trans = transform;
		}

		void OnEnable ()
		{
			StartCoroutine (this.TimedDisable ());
		}

		private IEnumerator TimedDisable ()
		{
			yield return new WaitForSeconds (SpaceObjectMngr.height * 0.8f / this.speed);
			gameObject.SetActive (false);
		}

		void Update ()
		{
			trans.Translate (speed * Time.deltaTime, 0, 0);
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (CompareTag ("ShipShot")) {
				if (other.CompareTag ("Asteroid") || other.CompareTag ("Saucer")) {
					gameObject.SetActive (false);
				}
			} else if (CompareTag ("SaucerShot") && other.CompareTag ("Ship")) {
				gameObject.SetActive (false);
			}
		}

		void OnDisable ()
		{
			StopAllCoroutines ();
		}

		#endregion
	}
}