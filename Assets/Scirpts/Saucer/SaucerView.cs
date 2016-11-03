﻿using UnityEngine;
using System;
using System.Collections;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public interface ISaucerView
	{
		Vector2 Pos { get; }

		event Action HitEvent;

		event Action GoneEvent;

		void Init (int size, float speed);

		void RepeatFire (Action fire, float period);

		void Fire (float angle);

		void ShipDestroyed ();

		void Destroyed ();
	}

	public class SaucerView : MonoBehaviour, ISaucerView
	{
		#region Constants
		//======================================================================

		private const string SAUCER_SHOT_TAG = "SaucerShot";
		private const float LEAVE_TIME = 1f;
		private static readonly Vector3 SMALL_SCALE = Vector3.one * 0.5f;

		#endregion

		#region Public fields & properties
		//======================================================================

		public float speed;
		public Vector2 Pos { get { return trans.position; } }

		#endregion

		#region Events
		//======================================================================

		public event Action HitEvent;
		public event Action GoneEvent;

		#endregion

		#region External references
		//======================================================================

		public ShotMngr saucerShot;

		#endregion

		#region Cached components
		//======================================================================

		[HideInInspector]
		public Transform trans;

		#endregion

		#region Private fields
		//======================================================================

		private int _sense;
		private Vector3 _vTranslate;

		#endregion


		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			this.trans = transform;
			_vTranslate = Vector3.zero;
		}
		
		// Update is called once per frame
		void Update () {
			this.trans.Translate (
				(Vector3.right + _vTranslate) * _sense * speed * Time.deltaTime
			);
			if (Mathf.Abs (this.trans.position.x) > SpaceObjectMngr.width / 2) {
				this.Leave ();
			}
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.CompareTag ("ShipShot")) {
				if (this.HitEvent != null) {
					this.HitEvent ();
				}
			}
		}

		void OnDisable ()
		{
			StopAllCoroutines ();
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Init (int size, float speed)
		{
			this.trans.position = SpaceObjectMngr.LateralRandomPos ();
			this.trans.localScale = SMALL_SCALE * (size + 1);
			_sense = this.trans.position.x > 0 ? -1 : 1;
			this.speed = speed;
			StartCoroutine (this.SetRandomDirection ());
		}

		public void RepeatFire (Action fire, float period)
		{
			StartCoroutine (this.RepeatFire2 (fire, period));
		}

		private IEnumerator RepeatFire2 (Action fire, float period)
		{
			yield return new WaitForSeconds (period);
			fire ();
			StartCoroutine (this.RepeatFire2 (fire, period));
		}

		public void Fire (float angle)
		{
			saucerShot.gameObject.SetActive (true);
			saucerShot.trans.position = trans.position;
			saucerShot.trans.rotation = Quaternion.Euler (0, 0, angle);
		}

		public void ShipDestroyed ()
		{
			StartCoroutine (this.ShipDestroyed2 ());
		}

		private IEnumerator ShipDestroyed2 ()
		{
			yield return new WaitForSeconds (LEAVE_TIME);
			this.Leave ();
		}

		public void Destroyed ()
		{
			gameObject.SetActive (false);
		}

		#endregion

		#region Public methods
		//======================================================================

		private IEnumerator SetRandomDirection ()
		{
			// Change 3 direction 3 times
			yield return new WaitForSeconds (SpaceObjectMngr.width / speed / 3);
			_vTranslate = new Vector3 (0, Random.Range (0, 3) - 1, 0);
			StartCoroutine (SetRandomDirection ());
		}

		private void Leave ()
		{
			gameObject.SetActive (false);
			if (this.GoneEvent != null) {
				this.GoneEvent ();
			}
		}

		#endregion
	}
}
