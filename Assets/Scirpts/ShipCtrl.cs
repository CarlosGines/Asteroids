using UnityEngine;
using System;

namespace CgfGames
{
	public class ShipCtrl : MonoBehaviour
	{
		#region Public fields
		//======================================================================

		public bool Active { get; private set; }

		#endregion

		#region Events
		//======================================================================

		public event Action DestroyedEvent;

		#endregion

		#region External references
		//======================================================================
	
		public ShipView view;

		#endregion

		#region Init
		//======================================================================

		void Awake ()
		{
			this.Active = true;
			this.view.HitEvent += this.Destroyed;
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Rotate (float direction)
		{
			if (this.Active) {
				this.view.Rotate (direction);
			}
		}

		public void Thrust ()
		{
			if (this.Active) {
				this.view.Thrust ();
			}
		}

		public void Fire ()
		{
			if (this.Active) {
				this.view.Fire ();
			}
		}

		public void Teleport ()
		{
			if (this.Active) {
				this.Active = false;
				this.view.Teleport (TeleportDone);
			}
		}

		private void TeleportDone ()
		{
			this.Active = true;
		}

		public void Destroyed ()
		{
			this.Active = false;
			this.view.Destroyed ();
			if (this.DestroyedEvent != null) {
				this.DestroyedEvent ();
			}
		}

		public void Respawn ()
		{
			this.Active = true;
			this.view.Respawn ();
		}

		#endregion
	}
}