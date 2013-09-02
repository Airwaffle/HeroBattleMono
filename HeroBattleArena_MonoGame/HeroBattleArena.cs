using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using HeroBattleArena.Game;
using HeroBattleArena.Game.GameObjects;
using HeroBattleArena.Game.Screens;

namespace HeroBattleArena
{
	public class HeroBattleArena : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		SpriteFont defaultFont;

		int frameCount = 0;
		float frameTimeCount = 0;
		float frameRate = 60;

		bool bShowFPS = false;

		public HeroBattleArena()
		{
			//Guide.SimulateTrialMode = false;
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = 1024;
			graphics.PreferredBackBufferHeight = 768;
			graphics.ApplyChanges();
			Content.RootDirectory = "Content";
#if XBOX
			Components.Add(new GamerServicesComponent(this));
#endif
		}

		protected override void Initialize()
		{
			base.Initialize();

			Configuration.Initialize();
            Game.Input.Initialize();
			Map.Initialize();
			HighScoreList.Initialize();
			FileSaver.Initialize();
			SoundCenter.Instance.Initialize();

			bShowFPS = Configuration.GetValue("System_ShowFPS") > 0;
			//if (Configuration.GetValue("System_CapFPS") == 0)
			{
				graphics.SynchronizeWithVerticalRetrace = false;
				this.IsFixedTimeStep = false;
				graphics.ApplyChanges();
			}
			if (Configuration.GetValue("System_Fullscreen") > 0)
			{
				graphics.IsFullScreen = true;
				graphics.ApplyChanges();
			}
		}

		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			Graphics.Initialize(this, Content, spriteBatch);

			//testTex = Content.Load<Texture2D>(@"Textures\Menus\CharacterSelection\p1");
			//animations = new AnimationManager(185, 185, 21, 2);
			//animations.AddAnimation(new Animation(13, 0, 0, false, 0));
			//animations.AddAnimation(new Animation(21, 1, 0, true, 2));
			//animations.AddAnimation(new Animation(1, 1, 20, false, 2));

			Entity.SpriteBatch = spriteBatch;
			Console.Write("Loading...");
			Graphics.LoadNextLevel();
			Console.Write(" Done!\n");
			Effects.Initialize();
			SoundCenter.Instance.Load(Content);

			ScreenManager.GetInstance().Add(new SkylineScreen());

			defaultFont = Content.Load<SpriteFont>(@"Fonts\DefaultFont");

			GUI.Instance.Load(Content);
			
		}

		protected override void UnloadContent()
		{
			EntityManager.Clear();
			Graphics.Quit();
		}

		protected override void Update(GameTime gameTime)
		{
			float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
			// Calculate frame rate.
			frameCount++;
			frameTimeCount += delta;
			if (frameTimeCount > 1.0)
			{
				frameRate = frameCount / frameTimeCount;
				frameTimeCount = 0;
				frameCount = 0;
			}
            Game.Input.Update();

#if DEBUG
			if (Game.Input.AnyWasPressed(Keys.F6))
				Graphics.ToggleAABB();
            if (Game.Input.AnyWasPressed(Keys.F7))
			{
				graphics.SynchronizeWithVerticalRetrace = !graphics.SynchronizeWithVerticalRetrace;
				this.IsFixedTimeStep = !this.IsFixedTimeStep;
				graphics.ApplyChanges();
			}
#endif

			if (ScreenManager.GetInstance().NumScreens == 0)
				this.Exit();
			if (Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.F))
			{
				graphics.ToggleFullScreen();
			}

			/*
			if (Input.AnyWasPressed(Keys.P))
			{
				Map.Maps[0].Load();
				foreach (Vector2 vec in Map.Maps[0].SpawnPoints)
					Console.WriteLine(vec);
			}
			*/

			//if (Input.AnyWasPressed(Keys.C))
			//  animations.ChangeAnimation(1);
			//animations.Update(delta);

			ScreenManager.GetInstance().Update(delta);
			FileSaver.Update();
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			if (GUI.Instance.Active)
				GUI.Instance.PreDraw();
			GraphicsDevice.SetRenderTarget(null);
			//GraphicsDevice.SetRenderTarget(0, null);
			GraphicsDevice.Clear(Color.Black);

			ScreenManager.GetInstance().Draw();

			if (true)
			{
				spriteBatch.Begin();

				//spriteBatch.Draw(testTex, new Vector2(400, 400), animations.Rectangle, Color.White);
				spriteBatch.DrawString(defaultFont, "Fps: " + frameRate, new Vector2(5, 5), Color.White);
				spriteBatch.End();
			}

			base.Draw(gameTime);
		}
	}

#region Entry Point
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			using (HeroBattleArena game = new HeroBattleArena())
			{
				game.Run();
			}
		}
	}
#endregion
}
