using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CgfGames
{
	[RequireComponent (typeof (SpaceObjectMngr))]
	[RequireComponent (typeof (SpriteRenderer))]
	[RequireComponent (typeof (TrailRenderer))]
	public class ShotMngr : MonoBehaviour {

		#region Public properties & properties
		//======================================================================

		public float speed;

		#endregion

		#region Cached components
		//======================================================================

		[HideInInspector]
		public Transform trans;
		private TrailRenderer _tr;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			this.trans = transform;
			_tr = GetComponent<TrailRenderer> ();
			GetComponent <SpaceObjectMngr> ().OffScreenEvent += _tr.Clear;
		}

		void OnEnable ()
		{
			_tr.Clear ();
			StartCoroutine (this.TimedDisable ());
		}

		private IEnumerator TimedDisable ()
		{
			yield return new WaitForSeconds (SpaceObjectMngr.height * 0.8f / this.speed);
			gameObject.SetActive (false);
		}

		void Update ()
		{
			this.trans.Translate (this.speed * Time.deltaTime, 0, 0);
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