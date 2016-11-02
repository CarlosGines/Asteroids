using UnityEngine;
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

		#endregion

		#region Unity callbacks
		//======================================================================

		void Awake ()
		{
			_trans = transform;
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
			GameObject shotGobj = this.shotPool.Get (
				_trans.position, _trans.rotation
			);
			shotGobj.tag = SHIP_SHOT_TAG;
		}

		public void Reload (int amount)
		{
		}

		#endregion
	}
}
