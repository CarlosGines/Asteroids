using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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

		#region Private fields
		//======================================================================

		int _ammo;

		#endregion

		#region IWeaponView Public methods
		//======================================================================

		public void Equip ()
		{
			ammoText.color = BLUE;
			_ammo = 0;
		}

		public void Unequip ()
		{
			ammoText.text = "";
		}

		public void Fire ()
		{
			for (int i = 0; i < cannons.Length; i++) {
				this.shotPool.Get (cannons [i].position, cannons [i].rotation);
			}
			_ammo--;
			ammoText.text = _ammo.ToString ();
		}

		public void FireHeld ()
		{
			this.Fire ();
		}

		public void Reload (int amount)
		{
			_ammo += amount;
			ammoText.text = _ammo.ToString ();
		}

		#endregion
	}
}
