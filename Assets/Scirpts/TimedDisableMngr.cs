using UnityEngine;
using System.Collections;

namespace CgfGames
{
	public class TimedDisableMngr : MonoBehaviour
	{
		public float disableTime;

		void OnEnable ()
		{
			StartCoroutine (this.TimedDisable ());
		}

		private IEnumerator TimedDisable ()
		{
			yield return new WaitForSeconds (this.disableTime);
			gameObject.SetActive (false);
		}

		void OnDisable ()
		{
			StopAllCoroutines ();
		}
	}
}
