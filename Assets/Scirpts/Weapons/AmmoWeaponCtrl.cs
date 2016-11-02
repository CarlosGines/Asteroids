using UnityEngine;
using System.Collections;

namespace CgfGames
{		
	public class AmmoWeaponCtrl : WeaponCtrl {

		#region Public fields & properties
		//======================================================================

		public int Ammo { get; private set; }

		public override bool IsAvailable { get { return Ammo > 0; } }

		#endregion

		#region Init
		//======================================================================

		public AmmoWeaponCtrl (IWeaponView view) : base (view)
		{
		}

		#endregion

		#region IWeapoCtrl Public methods
		//======================================================================

		public override void Unequip ()
		{
			this.Ammo = 0;
			this.View.Unequip ();
		}

		public override void Reload (int amount)
		{
			this.Ammo += amount;
			this.View.Reload (amount);
		}

		public override void Fire ()
		{
			if (this.Ammo > 0) {
				this.Ammo--;
				this.View.Fire ();
			}
		}

		#endregion
	}
}
