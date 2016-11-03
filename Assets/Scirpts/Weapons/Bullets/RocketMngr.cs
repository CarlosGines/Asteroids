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

		private Transform _trans;
		private CircleCollider2D _col;
		private SpriteRenderer _rend;
		private ParticleSystem _ps;

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
			this._trans = transform;
			this._col = GetComponent <CircleCollider2D> (); 
			this._rend = GetComponent <SpriteRenderer> (); 
			this._ps = GetComponent <ParticleSystem> ();
			_initSpeed = this.speed;
			_initColRadius = this._col.radius;
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
			_trans.Translate (speed * Time.deltaTime, 0, 0);
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
			this._rend.enabled = true;
			this._col.radius = _initColRadius;
		}

		#endregion

		#region Private methods
		//======================================================================

		private IEnumerator Explode ()
		{
			_exploded = true;
			this.speed = 0;
			this._rend.enabled = false;
			this._col.radius = explosionRadius;
			this._ps.Play ();
			yield return new WaitForSeconds (_ps.duration);
			gameObject.SetActive (false);
		}

		#endregion
	}
}