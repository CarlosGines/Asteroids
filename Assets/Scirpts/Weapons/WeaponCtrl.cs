using UnityEngine;

namespace CgfGames
{		
	public class WeaponCtrl : IWeaponCtrl {

		#region Public fields & properties
		//======================================================================

		public IWeaponView View { get; private set; }

		public WeaponType Type { get { return this.View.Type; } }
		
		public bool IsAvailable { get { return true; } }

		#endregion

		#region Init
		//======================================================================

		public WeaponCtrl (IWeaponView view)
		{
			this.View = view;
		}

		#endregion

		#region IWeapoCtrl Public methods
		//======================================================================

		public void Equip ()
		{
			this.View.Equip ();
		}

		public void Unequip ()
		{
			this.View.Unequip ();
		}

		public void Fire ()
		{
			this.View.Fire ();
		}

		public void FireHeld ()
		{
			this.View.FireHeld ();
		}

		public void Reload (int amount)
		{
			this.View.Reload (amount);
		}

		#endregion
	}
}
