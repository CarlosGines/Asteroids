using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CgfGames
{
	[RequireComponent (typeof (SpaceObjectMngr))]
	public class RocketMngr : MonoBehaviour {


		#region Public properties
		//======================================================================

		public float speed;
		public float explosionRadius;

		#endregion

		#region Cached components
		//======================================================================

		public Transform trans;
		public CircleCollider2D col;
		public SpriteRenderer rend;
		public ParticleSystem ps;

		#endregion

		#region Private vars
		//======================================================================

		private bool _exploded;
		private float _initSpeed;
		private float _initColRadius;
			

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			this.trans = transform;
			this.col = GetComponent <CircleCollider2D> (); 
			this.rend = GetComponent <SpriteRenderer> (); 
			this.ps = GetComponent <ParticleSystem> ();
			_initSpeed = this.speed;
			_initColRadius = this.col.radius;
		}

		void OnEnable ()
		{
			StartCoroutine (this.TimedExplode ());
		}

		private IEnumerator TimedExplode ()
		{
			yield return new WaitForSeconds (SpaceObjectMngr.height * 0.6f / this.speed);
			if (!_exploded) {
				StartCoroutine (this.Explode ());
			}
		}

		void Update ()
		{
			trans.Translate (speed * Time.deltaTime, 0, 0);
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (!_exploded && (other.CompareTag ("Asteroid") || 
					other.CompareTag ("Saucer"))) {
				StartCoroutine (this.Explode ());
			}
		}

		void OnDisable ()
		{
			StopAllCoroutines ();
			_exploded = false;
			this.speed =_initSpeed;
			this.rend.enabled = true;
			this.col.radius = _initColRadius;
		}

		#endregion

		#region Private methods
		//======================================================================

		private IEnumerator Explode ()
		{
			_exploded = true;
			this.speed = 0;
			this.rend.enabled = false;
			this.col.radius = explosionRadius;
			this.ps.Play ();
			yield return new WaitForSeconds (ps.duration);
			gameObject.SetActive (false);
		}

		#endregion
	}
}