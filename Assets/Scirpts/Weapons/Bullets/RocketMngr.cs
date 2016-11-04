using UnityEngine;
using UnityEngine.Assertions;
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

		#region External references
		//======================================================================

		public AudioClip explosion;


		#endregion

		#region Cached components
		//======================================================================

		private Transform _trans;
		private CircleCollider2D _col;
		private SpriteRenderer _rend;
		private ParticleSystem _ps;
		private AudioSource _audio;

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
			Assert.IsNotNull (this.explosion);

			_trans = transform;
			_col = GetComponent <CircleCollider2D> (); 
			_rend = GetComponent <SpriteRenderer> (); 
			_ps = GetComponent <ParticleSystem> ();
			_audio = GetComponent <AudioSource> ();

			_initSpeed = this.speed;
			_initColRadius = this._col.radius;
		}

		void OnEnable ()
		{
			StartCoroutine (this.TimedExplode ());
			_audio.Play ();
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
			if (!_exploded && (other.CompareTag (Tags.ASTEROID) || 
					other.CompareTag (Tags.SAUCER))) {
				StartCoroutine (this.Explode ());
			}
		}

		void OnDisable ()
		{
			StopAllCoroutines ();
			this.speed =_initSpeed;
			_exploded = false;
			_rend.enabled = true;
			_col.radius = _initColRadius;
		}

		#endregion

		#region Private methods
		//======================================================================

		private IEnumerator Explode ()
		{
			this.speed = 0;
			_exploded = true;
			_rend.enabled = false;
			_col.radius = explosionRadius;
			_ps.Play ();
			_audio.Stop ();
			_audio.PlayOneShot (this.explosion);
			yield return new WaitForSeconds (_ps.duration);
			gameObject.SetActive (false);
		}

		#endregion
	}
}