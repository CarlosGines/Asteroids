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
				Destroy (other.gameObject);
				this.HitEvent ();
			}
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Init (int size, Vector3 pos, Vector3 dir)
		{
			this.trans.position = pos;
			this.trans.localScale = Vector3.one * (int) (Math.Pow (2, size));
			this.translation = dir;
			this.translation.z = 0;
			this.translation = this.translation.normalized;
			this.translation *= this.baseSpeed * (AsteroidCtrl.MAX_SIZE + 1 - size);

		}

		public void UpdateSize (int size)
		{
		}

		public void SetPos (Vector3 pos)
		{
		}

		public void SetTranslation (Vector3 dir, int size)
		{

		}

		public void Destroyed ()
		{
			Destroy (gameObject);
		}

		public List<AsteroidCtrl> SpawnChildren (int size)
		{
			List<AsteroidCtrl> asteroidCtrlList = new List<AsteroidCtrl> ();
			float startAngle = -CHILDREN_DELTA_ANGLE * (NUM_CHILDREN - 1) / 2;
			Vector3 childDir = Quaternion.Euler (0, 0, startAngle) * this.translation;
			for (int i = 0; i < NUM_CHILDREN; i++) {
				AsteroidCtrl asteroidCtrl = 
					Instantiate (gameObject).GetComponent<AsteroidCtrl> ();
				asteroidCtrl.Init (size, this.trans.position, childDir);
				asteroidCtrlList.Add (asteroidCtrl);
				childDir = Quaternion.Euler (0, 0, CHILDREN_DELTA_ANGLE) * childDir;
			}
			return asteroidCtrlList;
		}

		#endregion
	}
}