using UnityEngine;
using System;
using System.Collections;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public class SaucerCtrl {

		#region Constants
		//======================================================================

		public const int SMALL_SAUCER = 0;
		public const int BIG_SAUCER = 1;
		public static readonly float[] SPEED = new float[] {2f, 1f};
		private static readonly float[] SHOT_PERIOD = new float[] {1f, 2f};
		private const float MAX_SHOT_ANGLE_DISTORTION = 45f;
		private const int ACCURACY_LIMIT_POINTS = 40000;

		#endregion

		#region Public fields & properties
		//======================================================================

		public int Size { get; private set; }

		#endregion

		#region Events
		//======================================================================

		public event Action<SaucerCtrl> DestroyedEvent;
		public event Action<SaucerCtrl> GoneEvent;

		#endregion

		#region Private fields
		//======================================================================

		private GameState _gameState;
		private SaucerView _view;
		private ShipCtrl _shipCtrl;

		#endregion

		#region Init
		//======================================================================

		public SaucerCtrl (GameState gameState, SaucerView view, 
				ShipCtrl shipCtrl, int size)
		{
			_gameState = gameState;
			_view = view;
			_shipCtrl = shipCtrl;
			this.Size = size;
			_view.RepeatFire (this.Fire, SHOT_PERIOD [size]);
			_shipCtrl.DestroyedEvent += this.ShipDestroyed;
			_view.HitEvent += this.Destroyed;
			_view.GoneEvent += this.Gone;
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Fire ()
		{
			if (_shipCtrl.IsActive) {
				if (this.Size == BIG_SAUCER) {
					_view.Fire (Random.Range (0, 360));
				} else {
					_view.Fire (
						this.DistortShotAngle (this.GetShotAngleSimple ())
					);
				}
			}
		}

		public void ShipDestroyed ()
		{
			_view.ShipDestroyed ();
		}

		public void Destroyed ()
		{
			_shipCtrl.DestroyedEvent -= this.ShipDestroyed;
			if (this.DestroyedEvent != null) {
				this.DestroyedEvent (this);
			}
			_view.Destroyed ();
		}

		public void Gone ()
		{
			_shipCtrl.DestroyedEvent -= this.ShipDestroyed;
			if (this.GoneEvent != null) {
				this.GoneEvent (this);
			}
		}

		#endregion

		#region Private methods
		//======================================================================

		private float DistortShotAngle (float angle) {
			float maxDistortion = MAX_SHOT_ANGLE_DISTORTION *
				Mathf.Max ((1f -  1f * _gameState.Score / ACCURACY_LIMIT_POINTS), 0);
			float randomDistortion = Random.value * maxDistortion;
			float randomSign = Random.Range (0, 2) * 2 - 1;
			return angle + randomDistortion * randomSign;
		}

		private float GetShotAngleSimple ()
		{
			Vector2 dir = _shipCtrl.Pos - _view.Pos;
			return Vector2.Angle (Vector2.right, dir) * Mathf.Sign (dir.y);
		}

		private float GetShotAngleSmart ()
		{
			float h = SpaceObjectMngr.height;
			float w = SpaceObjectMngr.width;
			float targetX;
			float targetY;
			if (Mathf.Abs (_shipCtrl.Pos.x - _view.Pos.x) < w / 2) {
				targetX = _shipCtrl.Pos.x;
			} else if (_view.Pos.x > 0) {
				targetX = _shipCtrl.Pos.x + w;
			} else {
				targetX = _shipCtrl.Pos.x - w;
			}
			if (Mathf.Abs (_shipCtrl.Pos.y - _view.Pos.y) < h / 2) {
				targetY = _shipCtrl.Pos.y;
			} else if (_view.Pos.y > 0) {
				targetY = _shipCtrl.Pos.y + h;
			} else {
				targetY = _shipCtrl.Pos.y - h;
			}
			Vector2 dir = new Vector2 (targetX, targetY) - _view.Pos;
			return Vector2.Angle (Vector2.right, dir) * Mathf.Sign (dir.y);
		}

		#endregion
	}
}
