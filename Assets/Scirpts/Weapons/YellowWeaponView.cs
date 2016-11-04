using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CgfGames
{
	public class YellowWeaponView : MonoBehaviour, IWeaponView {

		#region Constants
		//======================================================================

		public static readonly Color YELLOW = new Color (255f / 255, 209f / 255, 0);

		#endregion 

		#region Public fields and properties
		//======================================================================

		public WeaponType Type { get { return WeaponType.YELLOW; } }

		#endregion

		#region External references
		//======================================================================

		public ObjectPool rocketPool;
		public Text ammoText;

		#endregion

		#region Cached components
		//======================================================================

		private Transform _trans;

		#endregion

		#region Private fields
		//======================================================================

		int _ammo;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			_trans = transform;
		}
			
		#endregion

		#region IWeaponView Public methods
		//======================================================================

		public void Equip ()
		{
			ammoText.color = YELLOW;
			_ammo = 0;
		}

		public void Unequip ()
		{
			ammoText.text = "";
		}

		public void Fire ()
		{
			this.rocketPool.Get (_trans.position, _trans.rotation);
			_ammo--;
			ammoText.text = _ammo.ToString ();
		}

		public void FireHeld ()
		{
			// No-op.
		}

		public void Reload (int amount)
		{
			_ammo += amount;
			ammoText.text = _ammo.ToString ();
		}

		#endregion
	}
}
