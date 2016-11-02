using UnityEngine;
using System.Collections;

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

		void Reload (int amount);
	}
}
