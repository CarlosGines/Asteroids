using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CgfGames
{
	[RequireComponent (typeof (SpaceObjectMngr))]
	[RequireComponent (typeof (SpriteRenderer))]
	public class ShotMngr : MonoBehaviour {

		#region Public properties & properties
		//======================================================================

		public float speed;

		public Color Color { 
			get { return rend.color; } 
			set { rend.color = value; }
		}

		#endregion

		#region Cached components
		//======================================================================

		private Transform trans;
		private SpriteRenderer rend;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			trans = transform;
			rend = GetComponent<SpriteRenderer> ();
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