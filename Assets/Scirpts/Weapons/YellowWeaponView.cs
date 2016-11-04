using UnityEngine;
using System.Collections;

namespace CgfGames
{
	public class YellowWeaponView : MonoBehaviour, IWeaponView {

		#region Constants
		//======================================================================

		public static readonly Color YELLOW = new Color (255f / 255, 209f / 255, 0);

		#endregion 

		#region Public fields and properties
		//======================================================================

		public WeaponType Type { get { return WeaponType.YELLOW; } }

		#endregion

		#region External references
		//======================================================================

		public ObjectPool rocketPool;

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
			GameObject rocketGobj = this.rocketPool.Get (
				_trans.position, _trans.rotation
			);
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
