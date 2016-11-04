using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CgfGames
{
	/// <summary>
	/// Manager for a power up object.
	/// </summary>
	public class WeaponPowerupMngr : MonoBehaviour
	{
		#region Public fields & properties
		//======================================================================

		/// <summary>
		/// The speed for translation.
		/// </summary>
		public float speed;

		/// <summary>
		/// The type of weapon it provides.
		/// </summary>
		public WeaponType type;

		/// <summary>
		/// The amount of ammo it provides.
		/// </summary>
		public int ammo;

		#endregion

		#region External references
		//======================================================================

		/// <summary>
		/// The halo sprite renderer for the power up.
		/// </summary>
		public SpriteRenderer haloRend;

		/// <summary>
		/// The sprite for blue weapon power up.
		/// </summary>
		public Sprite blueSprite;

		/// <summary>
		/// The sprite for yellow weapon power up.
		/// </summary>
		public Sprite yellowSprite;

		/// <summary>
		/// The sprite for red weapon power up.
		/// </summary>
		public Sprite redSprite;

		#endregion

		#region Cached components
		//======================================================================

		/// <summary>
		/// Transform component cached.
		/// </summary>
		private Transform _trans;

		/// <summary>
		/// Sprite Renderer component cached.
		/// </summary>
		private SpriteRenderer _rend;

		#endregion

		#region Private fields
		//======================================================================

		/// <summary>
		/// Direction for translation.
		/// </summary>
		private Vector3 _dir;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			// Check external references
			Assert.IsNotNull (this.haloRend);
			Assert.IsNotNull (this.blueSprite);
			Assert.IsNotNull (this.yellowSprite);
			Assert.IsNotNull (this.redSprite);

			// Cache components.
			_trans = transform;
			_rend = GetComponent<SpriteRenderer> ();
		}

		void OnEnable ()
		{
			StartCoroutine (this.TimedDisable ());
		}

		/// <summary>
		/// Routine to disable game object after a certain time.
		/// </summary>
		/// <returns>The disable.</returns>
		private IEnumerator TimedDisable ()
		{
			yield return new WaitForSeconds (10);
			gameObject.SetActive (false);
		}

		void Update ()
		{
			// Translate power up
			_trans.Translate (_dir  * this.speed * Time.deltaTime);
		}

		void OnTriggerEnter2D (Collider2D other)
		{
			// Check if player picks it up.
			if (other.CompareTag (Tags.SHIP)) {
				// Notify the ship.
				other.GetComponent<ShipView> ().NewWeapon (this.type, this.ammo);
				gameObject.SetActive (false);
			}
		}

		void OnDisable ()
		{
			StopAllCoroutines ();
		}

		#endregion

		#region Public methods
		//======================================================================

		/// <summary>
		/// Init the manager. Use after obtaining object from object pool.
		/// </summary>
		/// <param name="type">Type of weapon provided by the power up.</param>
		public void Init (WeaponType type)
		{			
			this.type = type;

			// Update weapon type dependant fields and components.	
			switch (this.type) {
			case WeaponType.BLUE:
				this.ammo = 50;
				_rend.sprite = blueSprite;
				this.haloRend.color = BlueWeaponView.BLUE;
				break;
			case WeaponType.YELLOW:
				this.ammo = 5;
				_rend.sprite = yellowSprite;
				this.haloRend.color = YellowWeaponView.YELLOW;
				break;
			case WeaponType.RED:
				this.ammo = 15;
				_rend.sprite = redSprite;
				this.haloRend.color = RedWeaponView.RED;
				break;
			}

			// Set direction for translation from rotation and reset rotation.
			_dir = _trans.right;
			_trans.rotation = Quaternion.identity;
		}

		#endregion
	}
}
