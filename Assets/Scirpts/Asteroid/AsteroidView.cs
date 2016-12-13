using UnityEngine;
using System;

using Random = UnityEngine.Random;

namespace CgfGames
{
    /// <summary>
    /// View for an asteroid.
    /// </summary>
    public interface IAsteroidView
	{
		/// <summary>
		/// Occurs when hit by shots.
		/// </summary>
		event Action HitEvent;

		/// <summary>
		/// Init the view. Use after obtaining object from object pool.
		/// </summary>
		/// <param name="size">Size.</param>
		/// <param name="asteroidPool">Asteroid game object pool.</param>
		/// <param name="powerupPool">Powerup game object pool.</param>
		/// <param name="explosionPool">Explosion game object pool.</param>
		void Init (int size, ObjectPool asteroidPool, ObjectPool powerupPool,
			ObjectPool explosionPool);

		/// <summary>
		/// Perform actions needed when the asteroid is destroyed.
		/// </summary>
		void Destroyed ();

		/// <summary>
		/// Spawns a child asteroid.
		/// </summary>
		/// <returns>The child asteroid.</returns>
		/// <param name="childNum">Child number (more than one spawned).</param>
		/// <param name="size">Size.</param>
		IAsteroidView SpawnChild (int childNum, int size);

		/// <summary>
		/// Tries to spawn powerup, depending on probability.
		/// </summary>
		void TrySpawnPowerup ();
	}

	/// <summary>
	/// IAsteroidView implementation.
	/// </summary>
	[RequireComponent (typeof (SpaceObjectMngr))]
	public class AsteroidView : MonoBehaviour, IAsteroidView
	{
		#region Constants
		//======================================================================

		/// <summary>
		/// Number of children spawned when an asteroid is destroyed.
		/// </summary>
		private const int NUM_CHILDREN = 2;

		/// <summary>
		/// Angle between child asteroids when spawned.
		/// </summary>
		private const float CHILDREN_DELTA_ANGLE = 15f;

		#endregion

		#region Public fields & properties
		//======================================================================

		/// <summary>
		/// Speed of the slowest asteroid.
		/// </summary>
		public float baseSpeed;

		/// <summary>
		/// The speed of this asteroid.
		/// </summary>
		public float speed;

		#endregion

		#region Events
		//======================================================================

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.HitEvent"/>.
		/// </summary>
		public event Action HitEvent;

		#endregion

		#region Cached fields
		//======================================================================

		/// <summary>
		/// Transform component cached.
		/// </summary>
		public Transform trans;

		#endregion

		#region private fields
		//======================================================================

		/// <summary>
		/// The asteroid game object pool.
		/// </summary>
		private ObjectPool _asteroidPool;

		/// <summary>
		/// The power up game object pool.
		/// </summary>
		public ObjectPool _powerupPool;

		/// <summary>
		/// The asteroid explosion game object pool.
		/// </summary>
		public ObjectPool _explosionPool;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Cache components.
			this.trans = transform;
		}

		void Update ()
		{
			// Translate asteroid.
			this.trans.Translate (this.speed * Time.deltaTime, 0, 0);
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			// Detect if collides with a ship shot.
			if (other.CompareTag (Tags.SHIP_SHOT)) {
				this.HitEvent ();
			}
		}

		#endregion

		#region Public methods
		//======================================================================

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.Init"/>.
		/// </summary>
		public void Init (int size, ObjectPool asteroidPool,
			ObjectPool powerupPool, ObjectPool explosionPool)
		{
			this.trans.localScale = Vector3.one * 0.4f *
				(int)Math.Pow (2, size);
			_asteroidPool = asteroidPool;
			_powerupPool = powerupPool;
			_explosionPool = explosionPool;
			this.speed = this.baseSpeed * (AsteroidCtrl.MAX_SIZE + 1 - size);
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.Destroyed"/>.
		/// </summary>
		public void Destroyed ()
		{
			gameObject.SetActive (false);
			// Spawn explosion.
			_explosionPool.Get (this.trans.position, Quaternion.identity);
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.SpawnChild"/>.
		/// </summary>
		public IAsteroidView SpawnChild (int childNum, int childSize)
		{
			// Calculate delta angle for this child.
			float angle = CHILDREN_DELTA_ANGLE / 2 * 
				Math.Sign (childNum - 0.5f);
			Quaternion rot = Quaternion.Euler (0, 0, angle) *
            	this.trans.rotation;
			// Obtain and init asteoid.
			IAsteroidView asteroiView = _asteroidPool
				.Get (this.trans.position, rot)
				.GetComponent<AsteroidView> ();
			asteroiView.Init (
				childSize,
				_asteroidPool,
				_powerupPool,
				_explosionPool
			);
			return asteroiView;
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IAsteroidView.TrySpawnPowerup"/>.
		/// </summary>
		public void TrySpawnPowerup ()
		{
			float value = Random.value;
			// Chance to get a power up.
			if (value < 0.5f) {
				WeaponPowerupMngr powerup = 
					_powerupPool.Get (trans.position, trans.rotation)
					.GetComponent<WeaponPowerupMngr> ();
				// Random power up type.
				if (value < 1f / 6f) {
					powerup.Init (WeaponType.BLUE);
				} else if (value < 2f / 6f) {
					powerup.Init (WeaponType.YELLOW);
				} else if (value < 3f / 6f) {
					powerup.Init (WeaponType.RED);
				} 
			}
		}

		#endregion
	}
}