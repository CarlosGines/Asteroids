using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CgfGames
{
	public class RedWeaponView : MonoBehaviour, IWeaponView
	{
		#region Constants
		//======================================================================

		public static readonly Color RED = new Color (252f / 255, 0, 13f / 255);

		#endregion 

		#region Public fields and properties
		//======================================================================

		public WeaponType Type { get { return WeaponType.RED; } }

		public float rayShotTime;

		#endregion

		#region External references
		//======================================================================

		public GameObject ray;
		public Text ammoText;

		#endregion

		#region Private fields
		//======================================================================

		int _ammo;

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
			ammoText.color = RED;
			_ammo = 0;
		}

		public void Unequip ()
		{
			ammoText.text = "";
		}

		public void Fire ()
		{
			ray.SetActive (true);
			_ammo--;
			ammoText.text = _ammo.ToString ();
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
			_ammo += amount;
			ammoText.text = _ammo.ToString ();
		}

		#endregion
	}
}
