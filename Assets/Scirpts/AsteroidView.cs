using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public interface IAsteroidView
	{
		event Action HitEvent;

		void Init (int size, Vector3 pos, Quaternion rot, ObjectPool asteroidPool,
           ObjectPool powerupPool);

		void Destroyed ();

		List<IAsteroidView> SpawnChildren (int size);
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

		public void Init (int size, Vector3 pos, Quaternion rot,
			ObjectPool asteroidPool, ObjectPool powerupPool)
		{
			this.trans.position = pos;
			this.trans.rotation = rot;
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

		public List<IAsteroidView> SpawnChildren (int childSize)
		{
			List<IAsteroidView> asteroidViewList = new List<IAsteroidView> ();
			float startAngle = -CHILDREN_DELTA_ANGLE * (NUM_CHILDREN - 1) / 2;
			Quaternion childRot = Quaternion.Euler (0, 0, startAngle) *
            	this.trans.rotation;
			for (int i = 0; i < NUM_CHILDREN; i++) {
				IAsteroidView asteroiView = asteroidPool.Get ()
					.GetComponent<AsteroidView> ();
				asteroiView.Init (
					childSize,
					this.trans.position,
					childRot,
					this.asteroidPool,
					this.powerupPool
				);
				asteroidViewList.Add (asteroiView);
				childRot = Quaternion.Euler (0, 0, CHILDREN_DELTA_ANGLE) *
					childRot;
			}
			return asteroidViewList;
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