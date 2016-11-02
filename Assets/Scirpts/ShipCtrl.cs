using UnityEngine;
using System;

namespace CgfGames
{
	public interface IShipCtrl
	{
		bool IsActive { get; }

		bool IsAlive { get; }

		Vector2 Pos { get; }

		IWeaponCtrl CurrentWeapon { get; }

		event Action DestroyedEvent;

		void Equip (WeaponType type, int ammo);

		void Fire ();

		void Rotate (float direction);

		void Thrust ();

		void Teleport ();

		void Destroyed ();

		void Respawn ();
	}

	[Serializable]
	public class ShipCtrl : IShipCtrl
	{
		#region Public fields
		//======================================================================

		public IShipView View { get; private set; }

		public bool IsActive { get; private set; }

		public bool IsAlive { get; private set; }

		public Vector2 Pos { get { return this.View.Pos; } }

		public IWeaponCtrl CurrentWeapon { get; private set; }

		#endregion

		#region Events
		//======================================================================

		public event Action DestroyedEvent;

		#endregion

		#region Private fields
		//======================================================================
	
		private IWeaponCtrl[] _weapons;

		#endregion

		#region Init
		//======================================================================

		public ShipCtrl (IShipView view)
		{
			this.View = view as ShipView;
			this.IsAlive = true;
			this.IsActive = true;
			this.View.HitEvent += this.Destroyed;
			this.View.NewWeaponEvent += this.Equip;

			_weapons = new IWeaponCtrl [Enum.GetNames (typeof(WeaponType)).Length];
			_weapons [(int)WeaponType.BASE] =  new InfiniteWeaponCtrl (
				this.View.GetWeapon (WeaponType.BASE)
			);
			_weapons [(int)WeaponType.BLUE] =  new AmmoWeaponCtrl (
				this.View.GetWeapon (WeaponType.BLUE)
			);
			_weapons [(int)WeaponType.YELLOW] =  new InfiniteWeaponCtrl (
				this.View.GetWeapon (WeaponType.YELLOW)
			);
			_weapons [(int)WeaponType.RED] =  new AmmoWeaponCtrl (
				this.View.GetWeapon (WeaponType.RED)
			);

			this.CurrentWeapon = _weapons [(int)WeaponType.YELLOW];
			this.CurrentWeapon.Equip ();
		}

		#endregion

		#region IShipCtrl Public methods
		//======================================================================

		public void Equip (WeaponType type, int ammo)
		{
			if (this.CurrentWeapon.Type != type) {
				this.CurrentWeapon.Unequip ();
				this.CurrentWeapon = _weapons [(int)type];
				this.CurrentWeapon.Equip ();
			}
			this.CurrentWeapon.Reload (ammo);
		}

		public void Fire ()
		{
			if (this.IsActive) {
				if (!this.CurrentWeapon.IsAvailable) {
					this.CurrentWeapon = _weapons[(int)WeaponType.BASE];
				}
				this.CurrentWeapon.Fire ();
			}
		}

		public void Rotate (float direction)
		{
			if (this.IsActive) {
				this.View.Rotate (direction);
			}
		}

		public void Thrust ()
		{
			if (this.IsActive) {
				this.View.Thrust ();
			}
		}
			
		public void Teleport ()
		{
			if (this.IsActive) {
				this.IsActive = false;
				this.View.Teleport (TeleportDone);
			}
		}

		private void TeleportDone ()
		{
			this.IsActive = true;
		}

		public void Destroyed ()
		{
			this.IsAlive = false;
			this.IsActive = false;
			this.View.Destroyed ();
			if (this.DestroyedEvent != null) {
				this.DestroyedEvent ();
			}
		}

		public void Respawn ()
		{
			if (!this.IsAlive) {
				this.IsAlive = true;
				this.IsActive = true;
				this.View.Respawn ();
			}
		}

		#endregion
	}
}