namespace CgfGames
{		
	public enum WeaponType {BASE, BLUE, YELLOW, RED}
	
	public interface IWeaponCtrl
	{
		WeaponType Type { get; }

		bool IsAvailable { get; }

		void Equip ();

		void Unequip ();

		void Fire ();

		void FireHeld ();

		void Reload (int amount);
	}

	public class WeaponCtrlWrap : IWeaponCtrl
	{
		protected IWeaponCtrl origin;

		public virtual WeaponType Type { get { return this.origin.Type; } }

		public virtual bool IsAvailable { get { return this.origin.IsAvailable; } }

		public  WeaponCtrlWrap (IWeaponCtrl origin)
		{
			this.origin = origin;
		}
		
		public virtual void Equip ()
		{
			this.origin.Equip ();
		}

		public virtual void Unequip ()
		{
			this.origin.Unequip ();
		}

		public virtual void Fire ()
		{
			this.origin.Fire ();
		}

		public virtual void FireHeld ()
		{
			this.origin.FireHeld ();
		}

		public virtual void Reload (int amount)
		{
			this.origin.Reload (amount);
		}
	}
}
