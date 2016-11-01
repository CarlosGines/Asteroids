using UnityEngine;
using System;
using System.Collections;

using Random = UnityEngine.Random;

namespace CgfGames
{
	public class SaucerCtrl : MonoBehaviour {

		#region Constants
		//======================================================================

		public const int SMALL_SAUCER_SIZE = 0;
		public const int BIG_SAUCER_SIZE = 1;
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

		#region External references
		//======================================================================

		public SaucerView view;

		#endregion

		#region Private fields
		//======================================================================

		private GameState _gameState;
		private ShipCtrl _shipCtrl;

		#endregion

		#region Init
		//======================================================================

		public void Init (GameState gameState,  ShipCtrl shipCtrl, int size)
		{
			_gameState = gameState;
			_shipCtrl = shipCtrl;
			this.Size = size;
			this.view.Init (size);
			this.view.HitEvent += this.Destroyed;
			this.view.GoneEvent += () => {
				if (this.GoneEvent != null) {
					this.GoneEvent (this);
				}
			};
			InvokeRepeating ("Fire", 1, 1);
		}

		#endregion

		#region Public methods
		//======================================================================

		public void Fire ()
		{
			if (_shipCtrl.Active) {
				if (this.Size == BIG_SAUCER_SIZE) {
					this.view.Fire (Random.Range (0, 360));
				} else {
					this.view.Fire (
						this.DistortShotAngle (this.GetShotAngleSimple ())
					);
				}
			}
		}

		public void Destroyed ()
		{
			if (this.DestroyedEvent != null) {
				this.DestroyedEvent (this);
			}
			this.view.Destroyed ();
		}

		#endregion

		#region Private methods
		//======================================================================

		private float DistortShotAngle (float angle) {
			float maxDistortion = MAX_SHOT_ANGLE_DISTORTION *
				Mathf.Max ((1f -  1f * _gameState.Score / ACCURACY_LIMIT_POINTS), 0);
			print (maxDistortion);
			float randomDistortion = Random.value * maxDistortion;
			float randomSign = Random.Range (0, 2) * 2 - 1;
			return angle + randomDistortion * randomSign;
		}

		private float GetShotAngleSimple ()
		{
			Vector2 dir = _shipCtrl.view.Pos - view.Pos;
			return Vector2.Angle (Vector2.right, dir) * Mathf.Sign (dir.y);
		}

		private float GetShotAngleSmart ()
		{
			float h = SpaceObjectMngr.height;
			float w = SpaceObjectMngr.width;
			float targetX;
			float targetY;
			if (Mathf.Abs (_shipCtrl.view.Pos.x - view.Pos.x) < w / 2) {
				targetX = _shipCtrl.view.Pos.x;
			} else if (view.Pos.x > 0) {
				targetX = _shipCtrl.view.Pos.x + w;
			} else {
				targetX = _shipCtrl.view.Pos.x - w;
			}
			if (Mathf.Abs (_shipCtrl.view.Pos.y - view.Pos.y) < h / 2) {
				targetY = _shipCtrl.view.Pos.y;
			} else if (view.Pos.y > 0) {
				targetY = _shipCtrl.view.Pos.y + h;
			} else {
				targetY = _shipCtrl.view.Pos.y - h;
			}
			Vector2 dir = new Vector2 (targetX, targetY) - view.Pos;
			return Vector2.Angle (Vector2.right, dir) * Mathf.Sign (dir.y);
		}

		#endregion
	}
}
