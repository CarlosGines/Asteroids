using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;

namespace CgfGames
{
	/// <summary>
	/// View for the ship
	/// </summary>
	public interface IShipView
	{
		/// <summary>
		/// Current position of the ship.
		/// </summary>
		Vector2 Pos { get; }

		/// <summary>
		/// Occurs when the ship is hit.
		/// </summary>
		event Action HitEvent;

		/// <summary>
		/// Occurs when a new weapon is picked up.
		/// </summary>
		event Action<WeaponType, int> NewWeaponEvent;

		/// <summary>
		/// Obtains weapon view by type.
		/// </summary>
		/// <returns>The weapon.</returns>
		/// <param name="type">The weapon type.</param>
		IWeaponView GetWeapon (WeaponType type);

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
		/// <param name="onComplete">On Complete action.</param>
		void Teleport (Action onComplete);

		/// <summary>
		/// Perform actions needed when this ship is destroyed.
		/// </summary>
		void Destroyed ();

		/// <summary>
		/// Respawn the ship.
		/// </summary>
		void Respawn ();
	}

	/// <summary>
	/// IShipView implementation.
	/// </summary>
	[RequireComponent (typeof (SpaceObjectMngr))]
	[RequireComponent (typeof (Rigidbody2D))]
	[RequireComponent (typeof (Collider2D))]
	[RequireComponent (typeof (SpriteRenderer))]
	[RequireComponent (typeof (AudioSource))]
	public class ShipView : MonoBehaviour, IShipView
	{
		#region Public fields & properties
		//======================================================================

		/// <summary>
		/// Angular speed for rotation.
		/// </summary>
		public float angSpeed;

		/// <summary>
		/// The thrust force.
		/// </summary>
		public float thrustForce;

		/// <summary>
		/// Implements <see cref="CgfGames.IShipView.Pos"/>
		/// </summary>
		public Vector2 Pos { get { return _trans.position; } }

		#endregion

		#region Events
		//======================================================================

		/// <summary>
		/// Implements <see cref="CgfGames.IShipView.HitEvent"/>
		/// </summary>
		public event Action HitEvent;

		/// <summary>
		/// Implements <see cref="CgfGames.IShipView.NewWeaponEvent"/>
		/// </summary>
		public event Action<WeaponType, int> NewWeaponEvent;

		#endregion

		#region External references
		//======================================================================

		/// <summary>
		/// The base weapon view.
		/// </summary>
		public BaseWeaponView baseWeaponView;

		/// <summary>
		/// The blue weapon view.
		/// </summary>
		public BlueWeaponView blueWeaponView;

		/// <summary>
		/// The yellow weapon view.
		/// </summary>
		public YellowWeaponView yellowWeaponView;

		/// <summary>
		/// The red weapon view.
		/// </summary>
		public RedWeaponView redWeaponView;

		/// <summary>
		/// The engine particle system.
		/// </summary>
		public ParticleSystem enginePs;

		/// <summary>
		/// The ship explosion particle system.
		/// </summary>
		public ParticleSystem explosionPs;

		/// <summary>
		/// The teleport particle system.
		/// </summary>
		public ParticleSystem teleportPs;

		/// <summary>
		/// The audio source that plays the engine fx.
		/// </summary>
		public AudioSource engineAudio;

		/// <summary>
		/// The teleport fx audio clip.
		/// </summary>
		public AudioClip teleportClip;

		/// <summary>
		/// The powerup pick up audio clip.
		/// </summary>
		public AudioClip powerupClip;

		/// <summary>
		/// The ship explosion audio clip.
		/// </summary>
		public AudioClip explosionClip;

		#endregion


		#region Cached components
		//======================================================================

		/// <summary>
		/// Transform component cached.
		/// </summary>
		private Transform _trans;

		/// <summary>
		/// Rigidbody2D component cached.
		/// </summary>
		private Rigidbody2D _rb;

		/// <summary>
		/// Collider2D component cached.
		/// </summary>
		private Collider2D _col;

		/// <summary>
		/// SpriteRenderer component cached.
		/// </summary>
		private SpriteRenderer _rend;

		/// <summary>
		/// Audio source component cached.
		/// </summary>
		private AudioSource _audio;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Check external references
			Assert.IsNotNull (this.baseWeaponView);
			Assert.IsNotNull (this.blueWeaponView);
			Assert.IsNotNull (this.yellowWeaponView);
			Assert.IsNotNull (this.redWeaponView);
			Assert.IsNotNull (this.enginePs);
			Assert.IsNotNull (this.explosionPs);
			Assert.IsNotNull (this.teleportPs);
			Assert.IsNotNull (this.engineAudio);
			Assert.IsNotNull (this.teleportClip);
			Assert.IsNotNull (this.powerupClip);
			Assert.IsNotNull (this.explosionClip);

			// Cache components
			_trans = transform;
			_rb = GetComponent<Rigidbody2D> ();
			_rend = GetComponent<SpriteRenderer> ();
			_col = GetComponent<Collider2D> ();
			_audio = GetComponent<AudioSource> ();
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			// Check for killing collisions.
			if (other.CompareTag (Tags.ASTEROID) || 
					other.CompareTag (Tags.SAUCER) || 
					other.CompareTag (Tags.SAUCER_SHOT)) {
				if (this.HitEvent != null) {
					this.HitEvent ();
				}
			}
		}

		#endregion

		#region IShipView public methods
		//======================================================================

		/// <summary>
		/// Implements <see cref="CgfGames.IShipView.GetWeapon"/>
		/// </summary>
		public IWeaponView GetWeapon (WeaponType type)
		{
			switch (type) {
			case WeaponType.BASE:
				return this.baseWeaponView;
			case WeaponType.BLUE:
				return this.blueWeaponView;
			case WeaponType.YELLOW:
				return this.yellowWeaponView;
			case WeaponType.RED:
				return this.redWeaponView;
			default:
				throw new UnityException ("This weapon does not exist! :S");
			}
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipView.Rotate(dfloat)"/>
		/// </summary>
		public void Rotate (float delta)
		{
			_trans.Rotate (0, 0, angSpeed * Time.deltaTime * -delta);
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipView.Rotate(Vector2)"/>
		/// </summary>
		public void Rotate (Vector2 dir)
		{
			_trans.rotation = Quaternion.FromToRotation (Vector2.right, dir);
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipView.Thrust"/>
		/// </summary>
		public void Thrust ()
		{
			this.enginePs.Play ();
			if (!this.engineAudio.isPlaying) {
				this.engineAudio.Play ();
			}
			_rb.AddForce (_trans.right * thrustForce);
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipView.Teleport"/> on the next
		/// coroutine.
		/// </summary>
		public void Teleport (Action teleportDone)
		{
			StartCoroutine (Teleport2 (teleportDone));
		}
			
		/// <summary>
		/// Coroutine for the previous method.
		/// </summary>
		private IEnumerator Teleport2 (Action teleportDone)
		{
			// Complex sequence for teleporting:
			// Step 1:
			this.teleportPs.Play ();
			_audio.PlayOneShot (this.teleportClip);
			_col.enabled = false;
			yield return new WaitForSeconds (0.25f);
			// Step 2:
			_rend.enabled = false;
			yield return new WaitForSeconds (1.5f);
			// Step 3:
			_trans.position = SpaceObjectMngr.RandomPos ();
			_rb.Sleep ();
			teleportPs.Play ();
			yield return new WaitForSeconds (0.25f);
			// Step 4:
			this.SetActiveSoft (true);
			_audio.PlayOneShot (this.teleportClip);
			teleportDone.Invoke ();
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipView.Destroyed"/>
		/// </summary>
		public void Destroyed ()
		{
			this.SetActiveSoft (false);
			_audio.PlayOneShot (this.explosionClip, 1.5f);
			this.explosionPs.Play ();
		}

		/// <summary>
		/// Implements <see cref="CgfGames.IShipView.Respawn"/>
		/// </summary>
		public void Respawn ()
		{
			_trans.position = Vector3.zero;
			this.SetActiveSoft (true);
		}

		#endregion

		#region Custom public methods
		//======================================================================

		/// <summary>
		/// Perform actions needed when a new weapon is picked up.
		/// </summary>
		/// <param name="type">Type of weapon.</param>
		/// <param name="ammo">Amount of ammo for the weapon.</param>
		public void NewWeapon (WeaponType type, int ammo)
		{
			_audio.PlayOneShot (this.powerupClip);
			if (this.NewWeaponEvent != null)
			{
				this.NewWeaponEvent (type, ammo);
			}
		}

		#endregion

		#region Private methods
		//======================================================================

		/// <summary>
		/// Emulates GameObject.SetActive(bool), so that coroutines are not
		/// stopped.
		/// </summary>
		/// <param name="active">If set to <c>true</c> active.</param>
		public void SetActiveSoft (bool active)
		{
			if (active) {
				_rend.enabled = true;
				_col.enabled = true;
			} else {
				_rb.Sleep ();
				_rend.enabled = false;
				_col.enabled = false;
			}
		}

		#endregion
	}
}