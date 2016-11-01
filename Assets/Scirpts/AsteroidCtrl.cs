using UnityEngine;
using System;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public class AsteroidCtrl : MonoBehaviour
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
			private set {
				_size = value;
				this.view.UpdateSize (value);
			}
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

		public void Init (int size)
		{
			this.Init (size, SpaceObjectMngr.RandomPos (), Random.onUnitSphere);
		}

		public void Init (int size, Vector3 pos, Vector3 dir)
		{
			this.Size = size;
			this.view.Init (size, pos, dir);
			this.view.HitEvent += this.Destroyed;
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Destroyed ()
		{
			List<AsteroidCtrl> asteroidCtrlList = null;
			if (this.Size > 0) {
				asteroidCtrlList = this.view.SpawnChildren (this.Size - 1);
			}
			if (DestroyedEvent != null) {
				this.DestroyedEvent (this, asteroidCtrlList);
			}
			this.view.Destroyed ();
		}

		#endregion
	}
}