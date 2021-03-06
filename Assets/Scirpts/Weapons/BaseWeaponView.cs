﻿using UnityEngine;
using UnityEngine.Assertions;

namespace CgfGames
{
	public class BaseWeaponView : MonoBehaviour, IWeaponView {

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
			// No-op
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
			// No-op
		}

		#endregion
	}
}
