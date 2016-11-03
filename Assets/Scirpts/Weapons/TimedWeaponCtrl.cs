using UnityEngine;
using UnityEngine.Assertions;

namespace CgfGames
{		
	public class TimedWeaponCtrl : WeaponCtrlWrap {

		#region Private fields
		//======================================================================

		private float _lastFireTime;
		private float _coolDownTime;
		private float _autoFireTime;

		#endregion

		#region Init
		//======================================================================

		public TimedWeaponCtrl (float coolDownTime, float autoFireTime,
			IWeaponCtrl origin) : base (origin)
		{
			Assert.IsTrue (autoFireTime >= coolDownTime);
			_coolDownTime = coolDownTime;
			_autoFireTime = autoFireTime;
		}

		#endregion

		#region IWeapoCtrl Public methods
		//======================================================================

		public override void Fire ()
		{
			if (_coolDownTime == 0 || Time.time > _lastFireTime +
			    	_coolDownTime) {
				_lastFireTime = Time.time;
				this.origin.Fire ();
			}
		}

		public override void FireHeld ()
		{
			if (_autoFireTime != 0 && Time.time > _lastFireTime + _autoFireTime) {
				_lastFireTime = Time.time;
				this.origin.FireHeld ();
			}
		}

		#endregion
	}
}
