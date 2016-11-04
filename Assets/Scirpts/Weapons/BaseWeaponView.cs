using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

namespace CgfGames
{
	public class BaseWeaponView : MonoBehaviour, IWeaponView {

		#region Constants
		//======================================================================

		private const string SHIP_SHOT_TAG = "ShipShot";

		#endregion 

		#region Public fields and properties
		//======================================================================

		public WeaponType Type { get { return WeaponType.BASE; } }

		#endregion

		#region External references
		//======================================================================

		public ObjectPool shotPool;

		#endregion

		#region Cached components
		//======================================================================

		private Transform _trans;
		private AudioSource _audio;

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			Assert.IsNotNull (shotPool);

			_trans = transform;
			_audio = GetComponent <AudioSource> ();
		}
			
		#endregion

		#region IWeaponView Public methods
		//======================================================================

		public void Equip ()
		{
		}

		public void Unequip ()
		{
		}

		public void Fire ()
		{
			this.shotPool.Get (_trans.position, _trans.rotation);
			_audio.Play ();
		}

		public void FireHeld ()
		{
			// No-op.
		}

		public void Reload (int amount)
		{
		}

		#endregion
	}
}
