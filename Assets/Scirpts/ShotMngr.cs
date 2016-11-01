using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CgfGames
{
	[RequireComponent (typeof (SpaceObjectMngr))]
	public class ShotMngr : MonoBehaviour {

		#region Public properties
		//======================================================================

		public float speed;

		#endregion

		#region Cached components
		//======================================================================

		public Transform trans;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			trans = transform;
			Destroy (gameObject, SpaceObjectMngr.height * 0.8f / this.speed);
		}

		void Update ()
		{
			trans.Translate (Vector3.right * speed * Time.deltaTime);
		}

		#endregion
	}
}