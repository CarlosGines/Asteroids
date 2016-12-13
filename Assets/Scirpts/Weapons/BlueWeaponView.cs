using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace CgfGames
{
	public class BlueWeaponView : MonoBehaviour, IWeaponView {

		#region Constants
		//======================================================================

		public static readonly Color BLUE = new Color (0, 114f / 255, 188f / 255);

		#endregion 

		#region Public fields and properties
		//======================================================================

		public WeaponType Type { get { return WeaponType.BLUE; } }

		#endregion

		#region External references
		//======================================================================

		public ObjectPool shotPool;
		public Transform[] cannons;
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
			Assert.IsNotNull (this.shotPool);
			Assert.IsTrue (this.cannons.Length > 0);
			Assert.IsNotNull (ammoText);

			_audio = GetComponent <AudioSource> ();
		}

		#endregion

		#region IWeaponView Public methods
		//======================================================================

		public void Equip ()
		{
			this.ammoText.color = BLUE;
			_ammo = 0;
		}

		public void Unequip ()
		{
			this.ammoText.text = "";
		}

		public void Fire ()
		{
			_audio.Play ();
			for (int i = 0; i < cannons.Length; i++) {
				this.shotPool.Get (cannons [i].position, cannons [i].rotation);
			}
			_ammo--;
			this.ammoText.text = _ammo.ToString ();
		}

		public void FireHeld ()
		{
			this.Fire ();
		}

		public void Reload (int amount)
		{
			_ammo += amount;
			this.ammoText.text = _ammo.ToString ();
		}

		#endregion
	}
}
