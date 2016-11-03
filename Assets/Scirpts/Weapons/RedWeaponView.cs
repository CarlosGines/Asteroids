using UnityEngine;
using System.Collections;

namespace CgfGames
{
	public class RedWeaponView : MonoBehaviour, IWeaponView
	{
		#region Public fields and properties
		//======================================================================

		public WeaponType Type { get { return WeaponType.RED; } }

		public float rayShotTime;

		#endregion

		#region External references
		//======================================================================

		public GameObject ray;

		#endregion

		#region Unity callbacks
		//======================================================================

		void OnEnable ()
		{
			ray.SetActive (false);
		}

		void OnDisable ()
		{
			StopAllCoroutines ();
		}

		#endregion

		#region IWeaponView Public methods
		//======================================================================

		public void Equip ()
		{
		}

		public void Unequip ()
		{
		}

		public void Fire ()
		{
			ray.SetActive (true);
			StopAllCoroutines ();
			StartCoroutine (this.RayOff ());
		}

		public void FireHeld ()
		{
			this.Fire ();
		}

		private IEnumerator RayOff ()
		{
			yield return new WaitForSeconds (rayShotTime);
			ray.SetActive (false);
		}

		public void Reload (int amount)
		{
		}

		#endregion
	}
}
