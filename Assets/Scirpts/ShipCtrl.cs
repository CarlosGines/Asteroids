using UnityEngine;
using System;

namespace CgfGames
{
	public class ShipCtrl
	{
		#region Public fields
		//======================================================================

		public bool Active { get; private set; }
		public Vector2 Pos { get { return _view.Pos; } }

		#endregion

		#region Events
		//======================================================================

		public event Action DestroyedEvent;

		#endregion

		#region Private fields
		//======================================================================
	
		private ShipView _view;

		#endregion

		#region Init
		//======================================================================

		public ShipCtrl (ShipView view)
		{
			_view = view;
			this.Active = true;
			_view.HitEvent += this.Destroyed;
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Rotate (float direction)
		{
			if (this.Active) {
				this._view.Rotate (direction);
			}
		}

		public void Thrust ()
		{
			if (this.Active) {
				_view.Thrust ();
			}
		}

		public void Fire ()
		{
			if (this.Active) {
				_view.Fire ();
			}
		}

		public void Teleport ()
		{
			if (this.Active) {
				this.Active = false;
				_view.Teleport (TeleportDone);
			}
		}

		private void TeleportDone ()
		{
			this.Active = true;
		}

		public void Destroyed ()
		{
			this.Active = false;
			_view.Destroyed ();
			if (this.DestroyedEvent != null) {
				this.DestroyedEvent ();
			}
		}

		public void Respawn ()
		{
			this.Active = true;
			_view.Respawn ();
		}

		#endregion
	}
}