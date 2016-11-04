using UnityEngine;
using System;

namespace CgfGames
{
	public interface IShipCtrl
	{
		/// <summary>
		/// Whether the is on screen and can be controlled.
		/// </summary>
		bool IsActive { get; }

		/// <summary>
		/// Whether the ship is alive.
		/// </summary>
		bool IsAlive { get; }

		/// <summary>
		/// Current position pos the ship.
		/// </summary>
		Vector2 Pos { get; }

		/// <summary>
		/// Currently equipped weapon.
		/// </summary>
		IWeaponCtrl CurrentWeapon { get; }

		/// <summary>
		/// Occurs when the ship is destroyed.
		/// </summary>
		event Action DestroyedEvent;

		/// <summary>
		/// Equip the weapon specified by type.
		/// </summary>
		/// <param name="type">Type of weapon.</param>
		/// <param name="ammo">Ammo for the weapon.</param>
		void Equip (WeaponType type, int ammo);

		/// <summary>
		/// Fire.
		/// </summary>
		void Fire ();

		/// <summary>
		/// Fire in continuous mode.
		/// </summary>
		void FireHeld ();

		/// <summary>
		/// Rotate the ship.
		/// </summary>
		/// <param name="delta">Percentage of angular speed rotation and sense.
		/// </param>
		void Rotate (float delta);

		/// <summary>
		/// Rotate the hip.
		/// </summary>
		/// <param name="dir">New direction.</param>
		void Rotate (Vector2 dir);

		/// <summary>
		/// Thrust.
		/// </summary>
		void Thrust ();

		/// <summary>
		/// Use hyperspace to teleport the ship.
		/// </summary>
		void Teleport ();

		/// <summary>
		/// Perform actions needed when this ship is destroyed.
		/// </summary>
		void Destroyed ();

		/// <summary>
		/// Respawn the ship after being destroyed.
		/// </summary>
		void Respawn ();
	}

	[Serializable]
	public class ShipCtrl : IShipCtrl
	{
		#region Constants
		//======================================================================

		/// <summary>
		/// Time for blue weapon cool down and auto fire.
		/// </summary>
		float BLUE_SHOT_TIME = 0.1f;

		/// <summary>
		/// Time for red weapon cool down and auto fire.
		/// </summary>
		float RED_RAY_TIME = 0.3f;

		#endregion

		#region Public fields
		//======================================================================

		/// <summary>
		/// View associated to this controller.
		/// </summary>
		/// <value>The view.</value>
		public IShipView View { get; private set; }

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.IsActive"/>.
		/// </summary>
		public bool IsActive { get; private set; }

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.IsAlive"/>.
		/// </summary>
		public bool IsAlive { get; private set; }

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.Pos"/>.
		/// </summary>
		public Vector2 Pos { get { return this.View.Pos; } }

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.CurrentWeapon"/>.
		/// </summary>
		private IWeaponCtrl _currentWeapon;
		public IWeaponCtrl CurrentWeapon { 
			get { return _currentWeapon; }
			private set {
				_currentWeapon.Unequip ();
				_currentWeapon = value;
				_currentWeapon.Equip ();
			}
		}

		#endregion

		#region Events
		//======================================================================

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.DestroyedEvent"/>.
		/// </summary>
		public event Action DestroyedEvent;

		#endregion

		#region Private fields
		//======================================================================
	
		/// <summary>
		/// Weapons of this ship.
		/// </summary>
		private IWeaponCtrl[] _weapons;

		/// <summary>
		/// Base weapon of the ship (infinite).
		/// </summary>
		private IWeaponCtrl _baseWeapon;

		#endregion

		#region Init
		//======================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="CgfGames.ShipCtrl"/>
		/// class.
		/// </summary>
		/// <param name="view">View.</param>
		public ShipCtrl (IShipView view)
		{
			// Init properties
			this.View = view;
			this.IsAlive = true;
			this.IsActive = true;

			// Register vor view events
			this.View.HitEvent += this.Destroyed;
			this.View.NewWeaponEvent += this.Equip;

			// Init the weapons
			this.InitWeapons ();
		}

		/// <summary>
		/// Inits the weapons of this ship.
		/// </summary>
		private void InitWeapons ()
		{
			_weapons =
				new IWeaponCtrl [Enum.GetNames (typeof(WeaponType)).Length];
			// Instantiate weapons. Composite pattern used here.
			// Base weapon
			_weapons [(int)WeaponType.BASE] =  new WeaponCtrl (
				this.View.GetWeapon (WeaponType.BASE)
			);
			// Blue weapon
			_weapons [(int)WeaponType.BLUE] =  new TimedWeaponCtrl (
				BLUE_SHOT_TIME,
				BLUE_SHOT_TIME,
				new AmmoWeaponCtrl (
					new WeaponCtrl (
						this.View.GetWeapon (WeaponType.BLUE)
					)
				)
			);
			// Yellow weapon
			_weapons [(int)WeaponType.YELLOW] =  new TimedWeaponCtrl (
				0,
				0,
				new AmmoWeaponCtrl (
					new WeaponCtrl (
						this.View.GetWeapon (WeaponType.YELLOW)
					)
				)
			);
			// Red weapon
			_weapons [(int)WeaponType.RED] =  new TimedWeaponCtrl (
				RED_RAY_TIME,
				RED_RAY_TIME,
				new AmmoWeaponCtrl (
					new WeaponCtrl (
						this.View.GetWeapon (WeaponType.RED)
					)
				)
			);

			_baseWeapon = _weapons [(int)WeaponType.BASE];
			_currentWeapon = _baseWeapon;
			_currentWeapon.Equip ();
		}

		#endregion

		#region IShipCtrl Public methods
		//======================================================================

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.Equip"/>.
		/// </summary>
		public void Equip (WeaponType type, int ammo)
		{
			if (this.CurrentWeapon.Type != type) {				
				this.CurrentWeapon = _weapons [(int)type];
			}
			this.CurrentWeapon.Reload (ammo);
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.Fire"/>.
		/// </summary>
		public void Fire ()
		{
			if (this.IsActive) {
				if (!this.CurrentWeapon.IsAvailable) {
					this.CurrentWeapon = _baseWeapon;
				}
				this.CurrentWeapon.Fire ();
			}
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.FireHeld"/>.
		/// </summary>
		public void FireHeld ()
		{
			if (this.IsActive) {
				if (!this.CurrentWeapon.IsAvailable) {
					this.CurrentWeapon = _baseWeapon;
				}
				this.CurrentWeapon.FireHeld ();
			}
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.Rotate(Vector2)"/>.
		/// </summary>
		public void Rotate (Vector2 direction)
		{
			if (this.IsActive) {
				this.View.Rotate (direction);
			}
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.Rotate(float)"/>.
		/// </summary>
		public void Rotate (float direction)
		{
			if (this.IsActive) {
				this.View.Rotate (direction);
			}
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.Thrust"/>.
		/// </summary>
		public void Thrust ()
		{
			if (this.IsActive) {
				this.View.Thrust ();
			}
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.Teleport"/>.
		/// </summary>
		public void Teleport ()
		{
			if (this.IsActive) {
				// Make inactive while teleporting.
				this.IsActive = false;
				this.View.Teleport (this.TeleportDone);
			}
		}

		/// <summary>
		/// Actions to complete Teleport logic.
		/// </summary>
		private void TeleportDone ()
		{
			this.IsActive = true;
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.Destroyed"/>.
		/// </summary>
		public void Destroyed ()
		{
			this.IsAlive = false;
			this.IsActive = false;
			// Remove any special weapon.
			this.CurrentWeapon = _baseWeapon;
			// Notify
			this.View.Destroyed ();
			if (this.DestroyedEvent != null) {
				this.DestroyedEvent ();
			}
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipCtrl.IsActive"/>.
		/// </summary>
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