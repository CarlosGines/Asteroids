using UnityEngine;
using UnityEngine.Assertions;
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

		public GameObject rayGobj;
		public Text ammoText;

		#endregion

		#region Cached components
		//======================================================================

		private AudioSource _audio;

		#endregion

		#region Private fields
		//======================================================================

		int _ammo;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			Assert.IsNotNull (rayGobj);
			Assert.IsNotNull (ammoText);

			_audio = GetComponent<AudioSource> ();
		}

		void OnEnable ()
		{
			rayGobj.SetActive (false);
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
			rayGobj.SetActive (true);
			_audio.Play ();
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
			rayGobj.SetActive (false);
		}

		public void Reload (int amount)
		{
			_ammo += amount;
			ammoText.text = _ammo.ToString ();
		}

		#endregion
	}
}
