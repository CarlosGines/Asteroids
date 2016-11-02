using UnityEngine;
using System.Collections;

namespace CgfGames
{
	public class BlueWeaponView : MonoBehaviour, IWeaponView {

		#region Constants
		//======================================================================

		private const string SHIP_SHOT_TAG = "ShipShot";

		#endregion 

		#region Public fields and properties
		//======================================================================

		public WeaponType Type { get { return WeaponType.BLUE; } }

		#endregion

		#region External references
		//======================================================================

		public ObjectPool shotPool;
		public Transform[] cannons;

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
			for (int i = 0; i < cannons.Length; i++) {
				GameObject shotGobj = this.shotPool.Get (
					cannons[i].position, cannons[i].rotation
				);
				shotGobj.tag = SHIP_SHOT_TAG;
			}
		}

		public void Reload (int amount)
		{
		}

		#endregion
	}
}
