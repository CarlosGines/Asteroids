using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace CgfGames
{
	/// <summary>
	/// Controller for an asteroid
	/// </summary>
	public interface IAsteroidCtrl
	{
		/// <summary>
		/// Size of the astroid. 0 is the smallest.
		/// </summary>
		/// <value>The size.</value>
		int Size { get; }

		/// <summary>
		/// Occurs when the asteroid is destroyed.
		/// </summary>
		event Action<IAsteroidCtrl, List<IAsteroidCtrl>> DestroyedEvent;

		/// <summary>
		/// Perform actions needed when the asteroid is destroyed.
		/// </summary>
		void Destroyed ();
	}

	/// <summary>
	/// IAsteroidCtrl implementation.
	/// </summary>
	[Serializable]
	public class AsteroidCtrl : IAsteroidCtrl
	{
		#region Constants
		//======================================================================

		/// <summary>
		/// Maximum size for an asteroid.
		/// </summary>
		public const int MAX_SIZE = 2;

		#endregion

		#region Public fields & properties
		//======================================================================

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidCtrl.Size"/>.
		/// </summary>
		public int Size { get; private set; }

		#endregion

		#region Events
		//======================================================================

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidCtrl.DestroyedEvent"/>.
		/// </summary>
		public event Action<IAsteroidCtrl, List<IAsteroidCtrl>> DestroyedEvent;
		
		#endregion

		#region External references
		//======================================================================

		/// <summary>
		/// View associated to this controller.
		/// </summary>
		public IAsteroidView View;

		#endregion

		#region Init
		//======================================================================

		/// <summary>
		/// Initializes a new instance of the <see cref="CgfGames.AsteroidCtrl"/> class.
		/// </summary>
		/// <param name="view">Associated view.</param>
		/// <param name="size">Size.</param>
		public AsteroidCtrl (IAsteroidView view, int size)
		{
			this.View = view ;
			this.Size = size;
			this.View.HitEvent += this.Destroyed;
		}

		#endregion

		#region Public methods
		//======================================================================

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidCtrl.Destroyed()"/>.
		/// </summary>
		public void Destroyed ()
 		{
			// Biggest asteroids might spawn power ups.
			if (this.Size == MAX_SIZE) {
				this.View.TrySpawnPowerup ();
			}
			// Spawn child asteroids if not already smallest size.
			List<IAsteroidCtrl> asteroidList = null;
			if (this.Size > 0) {
				asteroidList = this.SpawnChildren ();
			}
			// Notify Destruction
			if (DestroyedEvent != null) {
				this.DestroyedEvent (this, asteroidList);
			}
			// Perform view actions
			this.View.Destroyed ();
			// Clean up event handlers
			this.View.HitEvent -= this.Destroyed;
		}

		#endregion

		#region Private methods
		//======================================================================

		/// <summary>
		/// Spawns child asteroids.
		/// </summary>
		/// <returns>The child asteroids.</returns>
		private List<IAsteroidCtrl> SpawnChildren ()
		{
			List<IAsteroidCtrl> asteroidList = new List<IAsteroidCtrl> ();
			int childSize = this.Size - 1;
			for (int i = 0; i < 2; i++) {
				asteroidList.Add (
					new AsteroidCtrl (
						this.View.SpawnChild (i, childSize),
						childSize
					)
				);
			}
			return asteroidList;
		}

		#endregion
	}
}