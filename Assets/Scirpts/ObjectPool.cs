using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CgfGames
{
	/// <summary>
	/// Object pool to avoid game object instantiation and destruciton.
	/// </summary>
	[Serializable]
	public class ObjectPool : MonoBehaviour
	{
		#region Public fields & properties
		//======================================================================

		/// <summary>
		/// The amount of game objects instantiated upfront.
		/// </summary>
		public int amount;

		#endregion

		#region External references
		//======================================================================

		/// <summary>
		/// The prefab to be pooled.
		/// </summary>
		public GameObject prefab;

		#endregion

		#region Private fields
		//======================================================================

		/// <summary>
		/// The list of instantiated game objects.
		/// </summary>
		private List<GameObject> _gobjs;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Instantiate game objects upfront.
			_gobjs = new List<GameObject> ();
			for (int i = 0; i < this.amount; i++) {
				GameObject gobj = UnityEngine.Object.Instantiate (this.prefab, transform) as GameObject;
				gobj.SetActive (false);
				_gobjs.Add (gobj);
			}
		}

		#endregion

		#region Public methods
		//======================================================================

		/// <summary>
		/// Get an object from pool.
		/// </summary>
		public GameObject Get ()
		{
			return this.Get (Vector3.zero, Quaternion.identity);
		}

		/// <summary>
		/// Get an object from pool.
		/// </summary>
		/// <param name="pos">Starting position for this object.</param>
		/// <param name="rot">Starting rotation for this object.</param>
		public GameObject Get (Vector3 pos, Quaternion rot)
		{
			// Find inactive object and return it
			foreach (GameObject gobj in _gobjs) {
				if (!gobj.activeInHierarchy) {
					gobj.SetActive (true);
					gobj.transform.position = pos;
					gobj.transform.rotation = rot;
					return gobj;
				}
			}

			// Grow pool if not enough objects instantiated.
			GameObject xGobj = UnityEngine.Object.Instantiate (this.prefab, transform) as GameObject;
			xGobj.transform.position = pos;
			xGobj.transform.rotation = rot;
			_gobjs.Add (xGobj);
			return xGobj;
		}

		#endregion
	}
}
