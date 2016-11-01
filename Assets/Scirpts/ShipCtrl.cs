using UnityEngine;
using System;

namespace CgfGames
{
	public class ShipCtrl
	{
		#region Public fields
		//======================================================================

		public bool IsActive { get; private set; }
		public bool IsAlive { get; private set; }
		public Vector2 Pos { get { return _view.Pos; } }

		#endregion

		#region Events
		//======================================================================

		public event Action DestroyedEvent;

		#endregion

		#region Private fields
		//======================================================================
	
		private IShipView _view;

		#endregion

		#region Init
		//======================================================================

		public ShipCtrl (IShipView view)
		{
			_view = view;
			this.IsAlive = true;
			this.IsActive = true;
			_view.HitEvent += this.Destroyed;
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Rotate (float direction)
		{
			if (this.IsActive) {
				this._view.Rotate (direction);
			}
		}

		public void Thrust ()
		{
			if (this.IsActive) {
				_view.Thrust ();
			}
		}

		public void Fire ()
		{
			if (this.IsActive) {
				_view.Fire ();
			}
		}

		public void Teleport ()
		{
			if (this.IsActive) {
				this.IsActive = false;
				_view.Teleport (TeleportDone);
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
			_view.Destroyed ();
			if (this.DestroyedEvent != null) {
				this.DestroyedEvent ();
			}
		}

		public void Respawn ()
		{
			if (!this.IsAlive) {
				this.IsAlive = true;
				this.IsActive = true;
				_view.Respawn ();
			}
		}

		#endregion
	}
}