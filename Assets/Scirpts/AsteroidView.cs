using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public interface IAsteroidView
	{
		event Action HitEvent;

		void Init (int size, ObjectPool asteroidPool, ObjectPool powerupPool);

		void Destroyed ();

		IAsteroidView SpawnChild (int childNum, int size);

		void TrySpawnPowerup ();
	}

	[RequireComponent (typeof (SpaceObjectMngr))]
	public class AsteroidView : MonoBehaviour, IAsteroidView
	{
		#region Constants
		//======================================================================

		private const int NUM_CHILDREN = 2;
		private const float CHILDREN_DELTA_ANGLE = 15f;

		#endregion

		#region Public fields, properties & events
		//======================================================================

		public float baseSpeed;
		public float speed;

		public event Action HitEvent;

		#endregion

		#region Scene references
		//======================================================================

		public ObjectPool asteroidPool;
		public ObjectPool powerupPool;

		#endregion

		#region Cached fields
		//======================================================================

		public Transform trans;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			this.trans = transform;
		}

		void Update ()
		{
			this.trans.Translate (this.speed * Time.deltaTime, 0, 0);
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.CompareTag ("ShipShot")) {
				this.HitEvent ();
			}
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Init (int size, ObjectPool asteroidPool,
			ObjectPool powerupPool)
		{
			this.trans.localScale = Vector3.one * 1.6f *
				(int)Math.Pow (2, size);
			this.asteroidPool = asteroidPool;
			this.powerupPool = powerupPool;
			this.speed = this.baseSpeed * (AsteroidCtrl.MAX_SIZE + 1 - size);
		}

		public void Destroyed ()
		{
			gameObject.SetActive (false);
		}

		public IAsteroidView SpawnChild (int childNum, int childSize)
		{
			float angle = CHILDREN_DELTA_ANGLE / 2 * 
				Math.Sign (childNum - 0.5f);
			Quaternion rot = Quaternion.Euler (0, 0, angle) *
            	this.trans.rotation;
			Debug.Log ("Dame asteroide child size " + childSize);
			IAsteroidView asteroiView = asteroidPool
				.Get (this.trans.position, rot)
				.GetComponent<AsteroidView> ();
			asteroiView.Init (
				childSize,
				this.asteroidPool,
				this.powerupPool
			);
			return asteroiView;
		}

		public void TrySpawnPowerup ()
		{
			float value = Random.value;
			if (value < 0.5f) {
				WeaponPowerupMngr powerup = 
					powerupPool.Get (trans.position, trans.rotation)
					.GetComponent<WeaponPowerupMngr> ();
				if (value < 1f / 6f) {
					powerup.Init (WeaponType.BLUE);
				} else if (value < 2f / 6f) {
					powerup.Init (WeaponType.YELLOW);
				} else if (value < 3f / 6f) {
					powerup.Init (WeaponType.RED);
				} 
			}
		}

		#endregion
	}
}