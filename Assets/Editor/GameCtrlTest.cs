using NUnit.Framework;
using NSubstitute;

namespace CgfGames
{
    [TestFixture]
	[Category("Asteroids Tests")]
	public class GameCtrlTest
	{
		//======================================================================

		[Test]
		public void CanSpawnAsteroids ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);

			game.SpawnAsteroid ();

			Assert.AreEqual (game.AsteroidList.Count, 1);

			game.SpawnAsteroid ();

			Assert.AreEqual (game.AsteroidList.Count, 2);
		}

		//======================================================================

		[Test]
		public void CanSpawnSaucers ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);

			game.SpawnSaucer ();

			Assert.NotNull (game.Saucer);
		}

		//======================================================================

		[Test]
		public void CanEarnPointsDestroyingAsteroids ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);
			var asteroid = Substitute.For<IAsteroidCtrl> ();
			asteroid.Size.Returns (AsteroidCtrl.MAX_SIZE);

			game.AsteroidDestroyed (asteroid, null);

			Assert.AreEqual (
				game.GameState.Score,
				GameCtrl.ASTEROIDS_POINTS[AsteroidCtrl.MAX_SIZE]
			);
		}

		//======================================================================

		[Test]
		public void CanEarnPointsDestroyingSaucers ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);
			var saucer = Substitute.For<ISaucerCtrl> ();
			saucer.Size.Returns (SaucerCtrl.SMALL_SAUCER);

			game.SaucerDestroyed (saucer);

			Assert.AreEqual (
				game.GameState.Score, 
				GameCtrl.SAUCERS_POINTS[SaucerCtrl.SMALL_SAUCER]
			);
		}

		//======================================================================

		[Test]
		public void CanEarnLivesWithPoints ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);
			int initLives = game.GameState.Lives;

			game.ScoreUpdated (0, GameCtrl.LIFE_POINTS);

			Assert.AreEqual (game.GameState.Lives, initLives + 1);
		}

		//======================================================================

		[Test]
		public void CanLoseLives ()
		{
			GameCtrl game = new GameCtrl (
				Substitute.For<IGameView> (), Substitute.For<IShipCtrl> ()
			);
			int initLives = game.GameState.Lives;

			game.ShipDestroyed ();

			Assert.AreEqual (game.GameState.Lives, initLives - 1);
		}

		//======================================================================

		[Test]
		public void CanRespawnShip ()
		{
			var gameView = Substitute.For<IGameView> ();
			var ship = Substitute.For<IShipCtrl> ();
			gameView.WaitToRespawnShip (Arg.Invoke ());
			GameCtrl game = new GameCtrl (gameView, ship);

			game.ShipDestroyed ();

			ship.Received (1).Respawn ();
		}

		//======================================================================

		[Test]
		public void CanGameOver ()
		{
			var gameView = Substitute.For<IGameView> ();
			GameCtrl game = new GameCtrl (
				new GameState (), gameView, Substitute.For<IShipCtrl> ()
			);

			game.ShipDestroyed ();

			gameView.Received (1).GameOver ();
		}

		//======================================================================

		[Test]
		public void CanSpawnAsteroidsWithNewLevel ()
		{
			var gameView = Substitute.For<IGameView> ();
			gameView.LevelFinished (Arg.Invoke ());
			GameCtrl game = new GameCtrl (
				gameView, Substitute.For<IShipCtrl> ()
			);

			game.StartGame ();

			Assert.AreEqual (game.GameState.Level, 1);
			Assert.AreEqual (game.AsteroidList.Count, 4);

			for (int i = 0; i < this.TotalAsteroids(1); i++) {
				game.AsteroidList [0].Destroyed ();
			}

			Assert.AreEqual (game.GameState.Level, 2);
			Assert.AreEqual (game.AsteroidList.Count, 5);
		}

		private int TotalAsteroids (int level)
		{
			int init = level + 3;
			return init + init * 2 + init * 2 * 2;
		}

		//======================================================================

		[Test]
		public void CannnotFinishLevelWithSaucerAlive ()
		{
			var gameView = Substitute.For<IGameView> ();
			gameView.LevelFinished (Arg.Invoke ());
			GameCtrl game = new GameCtrl (
				gameView, Substitute.For<IShipCtrl> ()
			);

			game.StartGame ();

			Assert.AreEqual (game.GameState.Level, 1);

			game.SpawnSaucer ();

			for (int i = 0; i < this.TotalAsteroids(1); i++) {
				game.AsteroidList [0].Destroyed ();
			}

			Assert.AreEqual (game.GameState.Level, 1);
		}

		//======================================================================

		[Test]
		public void CanFinishLevelAfterSaucerDead ()
		{
			var gameView = Substitute.For<IGameView> ();
			gameView.LevelFinished (Arg.Invoke ());
			GameCtrl game = new GameCtrl (
				gameView, Substitute.For<IShipCtrl> ()
			);

			game.StartGame ();

			Assert.AreEqual (game.GameState.Level, 1);

			game.SpawnSaucer ();
			for (int i = 0; i < this.TotalAsteroids(1); i++) {
				game.AsteroidList [0].Destroyed ();
			}
			game.Saucer.Destroyed ();

			Assert.AreEqual (game.GameState.Level, 2);
		}

		//======================================================================
	}
}
