using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public class AsteroidCtrl
	{
		#region Constants
		//======================================================================

		public const int MAX_SIZE = 2;

		#endregion

		#region Public fields & properties
		//======================================================================

		private int _size;
		public int Size {
			get { return _size; }
			private set { _size = value; }
		}

		#endregion

		#region Events
		//======================================================================

		public event Action<AsteroidCtrl, List<AsteroidCtrl>> DestroyedEvent;
		
		#endregion

		#region External references
		//======================================================================

		public AsteroidView view;

		#endregion

		#region Init
		//======================================================================

		public AsteroidCtrl (AsteroidView view, int size)
		{
			this.view = view;
			this.Size = size;
			this.view.HitEvent += this.Destroyed;
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Destroyed ()
 		{
 			List<AsteroidCtrl> asteroidCtrlList = null;
			if (this.Size > 0) {
				int childSize = this.Size - 1;
				asteroidCtrlList = new List<AsteroidCtrl> ();
				List<AsteroidView> asteroidViewList = this.view.SpawnChildren (childSize);
				foreach (AsteroidView asteroidView in asteroidViewList) {
					AsteroidCtrl asteroidCtrl = new AsteroidCtrl (asteroidView, childSize);
					asteroidCtrlList.Add (asteroidCtrl);
				}
			}
			if (DestroyedEvent != null) {
				this.DestroyedEvent (this, asteroidCtrlList);
			}
			this.view.Destroyed ();
			this.view.HitEvent -= this.Destroyed;
		}

		#endregion
	}
}