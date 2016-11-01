using UnityEngine;
using NUnit.Framework;
using NSubstitute;

namespace CgfGames
{
	[TestFixture]
	[Category("Asteroids Tests")]
	public class UnitTests {

		[Test]
		public void CanLoseLives ()
		{
			ShipCtrl shipCtrl = new ShipCtrl (NSubstitute.Substitute.For<IShipView> ());
			GameCtrl gameCtrl = new GameCtrl (NSubstitute.Substitute.For<IGameView> (), shipCtrl);
			int lives = gameCtrl.GameState.Lives;

			shipCtrl.Destroyed ();

			Assert.AreEqual (gameCtrl.GameState.Lives, lives - 1);
		}
	}
}
