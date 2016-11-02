using UnityEngine;
using System;

namespace CgfGames
{
	public interface IShipCtrl
	{
		bool IsActive { get; }

		bool IsAlive { get; }

		Vector2 Pos { get; }

		event Action DestroyedEvent;

		void Rotate (float direction);

		void Thrust ();

		void Fire ();

		void Teleport ();

		void Destroyed ();

		void Respawn ();
	}

	[Serializable]
	public class ShipCtrl : IShipCtrl
	{
		#region Public fields
		//======================================================================

		public bool IsActive { get; private set; }
		public bool IsAlive { get; private set; }
		public Vector2 Pos { get { return this.View.Pos; } }

		public ShipView View { get; private set; }


		#endregion

		#region Events
		//======================================================================

		public event Action DestroyedEvent;

		#endregion

		#region Private fields
		//======================================================================
	

		#endregion

		#region Init
		//======================================================================

		public ShipCtrl (IShipView view)
		{
			this.View = view as ShipView;
			this.IsAlive = true;
			this.IsActive = true;
			this.View.HitEvent += this.Destroyed;
		}

		#endregion

		#region IShipCtrl Public methods
		//======================================================================

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

		public void Fire ()
		{
			if (this.IsActive) {
				this.View.Fire ();
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