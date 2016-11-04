using UnityEngine;
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
				ShotMngr shot = this.shotPool.Get (
					cannons[i].position, cannons[i].rotation
				).GetComponent<ShotMngr> ();
				shot.Color = BLUE;
			}
		}

		public void FireHeld ()
		{
			this.Fire ();
		}

		public void Reload (int amount)
		{
		}

		#endregion
	}
}
