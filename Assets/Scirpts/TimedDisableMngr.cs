using UnityEngine;
using System.Collections;

namespace CgfGames
{
	/// <summary>
	/// Disable the object in a specific time in the future.
	/// </summary>
	public class TimedDisableMngr : MonoBehaviour
	{
		// Time to disable object.
		public float disableTime;

		void OnEnable ()
		{
			StartCoroutine (this.TimedDisable ());
		}

		/// <summary>
		/// Disable object after the specified time.
		/// </summary>
		/// <returns>The disable.</returns>
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
