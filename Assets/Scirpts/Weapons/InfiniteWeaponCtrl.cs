using UnityEngine;
using System.Collections;

namespace CgfGames
{		
	public class InfiniteWeaponCtrl : WeaponCtrl {

		#region Public fields & properties
		//======================================================================
		
		public override bool IsAvailable { get { return true; } }

		#endregion

		#region Init
		//======================================================================

		public InfiniteWeaponCtrl (IWeaponView view) : base (view)
		{
		}

		#endregion

		#region IWeapoCtrl Public methods
		//======================================================================

		public override void Reload (int amount)
		{
			// No-op
		}

		#endregion
	}
}
