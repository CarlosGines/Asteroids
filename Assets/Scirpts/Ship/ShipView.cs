using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;

namespace CgfGames
{
	public interface IShipView
	{
		Vector2 Pos { get; }

		event Action HitEvent;

		event Action<WeaponType, int> NewWeaponEvent;

		IWeaponView GetWeapon (WeaponType type);

		void Rotate (float direction);

		void Thrust ();

		void Teleport (Action teleportDone);

		void Destroyed ();

		void Respawn ();
	}

	[RequireComponent (typeof (SpaceObjectMngr))]
	public class ShipView : MonoBehaviour, IShipView
	{
		#region Public fields & properties
		//======================================================================

		public float angSpeed;
		public float thrustForce;
		public Vector2 Pos { get { return _trans.position; } }

		#endregion

		#region Events
		//======================================================================

		public event Action HitEvent;
		public event Action<WeaponType, int> NewWeaponEvent;

		#endregion

		#region External references
		//======================================================================

		public BaseWeaponView baseWeaponView;
		public BlueWeaponView blueWeaponView;
		public YellowWeaponView yellowWeaponView;
		public RedWeaponView redWeaponView;

		public ParticleSystem enginePs;
		public ParticleSystem explosionPs;
		public ParticleSystem teleportPs;

		public AudioSource engineAudio;

		public AudioClip teleportClip;
		public AudioClip powerupClip;
		public AudioClip destroyedClip;

		#endregion


		#region Cached components
		//======================================================================

		private Transform _trans;
		private Rigidbody2D _rb;
		private Renderer _rend;
		private Collider2D _col;
		private AudioSource _audio;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			Assert.IsNotNull (baseWeaponView);
			Assert.IsNotNull (blueWeaponView);
			Assert.IsNotNull (yellowWeaponView);
			Assert.IsNotNull (redWeaponView);

			Assert.IsNotNull (enginePs);
			Assert.IsNotNull (explosionPs);
			Assert.IsNotNull (teleportPs);

			Assert.IsNotNull (engineAudio);

			Assert.IsNotNull (teleportClip);
			Assert.IsNotNull (powerupClip);
			Assert.IsNotNull (destroyedClip);

			_trans = transform;
			_rb = GetComponent<Rigidbody2D> ();
			_rend = GetComponent<Renderer> ();
			_col = GetComponent<Collider2D> ();
			_audio = GetComponent<AudioSource> ();
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			if (other.CompareTag ("Asteroid") || other.CompareTag ("Saucer") 
					|| other.CompareTag ("SaucerShot")) {
				if (this.HitEvent != null) {
					this.HitEvent ();
				}
			}
		}

		#endregion

		#region IShipView public methods
		//======================================================================

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
				throw new UnityException ("This weapon does not exist");
			}
		}

		public void Rotate (float direction)
		{
			_trans.Rotate (0, 0, angSpeed * Time.deltaTime * -direction);
		}

		public void Thrust ()
		{
			this.enginePs.Play ();
			if (!this.engineAudio.isPlaying) {
				this.engineAudio.Play ();
			}
			_rb.AddForce (_trans.right * thrustForce);
		}

		public void Teleport (Action teleportDone)
		{
			StartCoroutine (Teleport2 (teleportDone));
		}

		private IEnumerator Teleport2 (Action teleportDone)
		{
			this.teleportPs.Play ();
			_audio.PlayOneShot (this.teleportClip);
			_col.enabled = false;
			yield return new WaitForSeconds (0.25f);
			_rend.enabled = false;
			yield return new WaitForSeconds (1.5f);
			_rb.Sleep ();
			_trans.position = SpaceObjectMngr.RandomPos ();
			teleportPs.Play ();
			yield return new WaitForSeconds (0.25f);
			this.SetActiveSoft (true);
			_audio.PlayOneShot (this.teleportClip);
			teleportDone.Invoke ();
		}

		public void Destroyed ()
		{
			this.SetActiveSoft (false);
			_audio.PlayOneShot (this.destroyedClip, 1.5f);
			this.explosionPs.Play ();
		}

		public void Respawn ()
		{
			_trans.position = Vector3.zero;
			this.SetActiveSoft (true);
		}

		#endregion

		#region Custom public methods
		//======================================================================

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