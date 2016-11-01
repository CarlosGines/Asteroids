using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace CgfGames
{
	[RequireComponent (typeof (SpaceObjectMngr))]
	public class AsteroidView : MonoBehaviour
	{
		#region Constants
		//======================================================================

		private const int NUM_CHILDREN = 2;
		private const float CHILDREN_DELTA_ANGLE = 15f;

		#endregion

		#region Public fields, properties & events
		//======================================================================

		public float baseSpeed;
		public Vector3 translation;

		public event Action HitEvent;

		#endregion

		#region External references
		//======================================================================

		public ObjectPool asteroidPool;

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
			this.trans.Translate (this.translation * Time.deltaTime);
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

		public void Init (int size, Vector3 pos, Vector3 dir, ObjectPool asteroidPool)
		{
			this.trans.position = pos;
			this.trans.localScale = Vector3.one * (int) (Math.Pow (2, size));
			this.translation = dir;
			this.translation.z = 0;
			this.translation = this.translation.normalized;
			this.translation *= 
				this.baseSpeed * (AsteroidCtrl.MAX_SIZE + 1 - size);
			this.asteroidPool = asteroidPool;
		}

		public void Destroyed ()
		{
			gameObject.SetActive (false);
		}

		public List<AsteroidView> SpawnChildren (int size)
		{
			List<AsteroidView> asteroidViewList = new List<AsteroidView> ();
			float startAngle = -CHILDREN_DELTA_ANGLE * (NUM_CHILDREN - 1) / 2;
			Vector3 childDir = 
				Quaternion.Euler (0, 0, startAngle) * this.translation;
			for (int i = 0; i < NUM_CHILDREN; i++) {
				AsteroidView asteroiView = 
					asteroidPool.Get ().GetComponent<AsteroidView> ();
				asteroiView.Init (
					size,
					this.trans.position,
					childDir,
					this.asteroidPool
				);
				asteroidViewList.Add (asteroiView);
				childDir = 
					Quaternion.Euler (0, 0, CHILDREN_DELTA_ANGLE) * childDir;
			}
			return asteroidViewList;
		}

		#endregion
	}
}