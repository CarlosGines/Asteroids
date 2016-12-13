namespace CgfGames
{		
	public class AmmoWeaponCtrl : WeaponCtrlWrap {

		#region Public fields & properties
		//======================================================================

		public int Ammo { get; private set; }

		public override bool IsAvailable { get { return Ammo > 0; } }

		#endregion

		#region Init
		//======================================================================

		public AmmoWeaponCtrl (IWeaponCtrl origin) : base (origin)
		{
		}

		#endregion

		#region IWeapoCtrl Public methods
		//======================================================================

		public override void Unequip ()
		{
			this.Ammo = 0;
			this.origin.Unequip ();
		}

		public override void Reload (int amount)
		{
			this.Ammo += amount;
			this.origin.Reload (amount);
		}

		public override void Fire ()
		{
			if (this.Ammo > 0) {
				this.Ammo--;
				this.origin.Fire ();
			}
		}

		public override void FireHeld ()
		{
			if (this.Ammo > 0) {
				this.Ammo--;
				this.origin.FireHeld ();
			}
		}

		#endregion
	}
}
