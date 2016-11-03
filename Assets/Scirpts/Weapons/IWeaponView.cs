namespace CgfGames
{
	public interface IWeaponView
	{
		WeaponType Type { get; }

		void Equip ();

		void Unequip ();

		void Fire ();

		void FireHeld ();

		void Reload (int amount);
	}
}
