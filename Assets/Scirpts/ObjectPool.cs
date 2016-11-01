using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CgfGames
{
	[Serializable]
	public class ObjectPool : MonoBehaviour
	{
		public GameObject prefab;
		public int amount;
		private List<GameObject> _gobjs;

		void Awake ()
		{
			_gobjs = new List<GameObject> ();
			for (int i = 0; i < this.amount; i++) {
				GameObject gobj = UnityEngine.Object.Instantiate (this.prefab, transform) as GameObject;
				gobj.SetActive (false);
				_gobjs.Add (gobj);
			}
		}

		public GameObject Get ()
		{
			return this.Get (Vector3.zero, Quaternion.identity);
		}

		public GameObject Get (Vector3 pos, Quaternion rot)
		{
			foreach (GameObject gobj in _gobjs) {
				if (!gobj.activeInHierarchy) {
					gobj.SetActive (true);
					gobj.transform.position = pos;
					gobj.transform.rotation = rot;
					return gobj;
				}
			}

			// Grow pool
			GameObject xGobj = UnityEngine.Object.Instantiate (this.prefab, transform) as GameObject;
			xGobj.transform.position = pos;
			xGobj.transform.rotation = rot;
			_gobjs.Add (xGobj);
			return xGobj;
		}
	}
}
