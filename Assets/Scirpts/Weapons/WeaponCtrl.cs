using UnityEngine;
using System.Collections;

namespace CgfGames
{		
	public abstract class WeaponCtrl : IWeaponCtrl {

		#region Public fields & properties
		//======================================================================

		public IWeaponView View { get; private set; }

		public WeaponType Type { get { return this.View.Type; } }
		
		public abstract bool IsAvailable { get; }

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

		public virtual void Equip ()
		{
			this.View.Equip ();
		}

		public virtual void Unequip ()
		{
			this.View.Unequip ();
		}

		public virtual void Fire ()
		{
			this.View.Fire ();
		}

		public virtual void Reload (int amount)
		{
			this.View.Reload (amount);
		}

		#endregion
	}
}
