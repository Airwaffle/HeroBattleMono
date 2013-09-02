// spelare rör sig när dom kör winposes
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeroBattleArena.Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace HeroBattleArena.Game.Screens
{
    public class GameScreen : Screen
    {
        private static Random rand = new Random(DateTime.Now.Millisecond);
        private float[] m_RespawnTimers = { 0, 0, 0, 0 };
        private float m_RespawnTime = 0;
        private static bool TRAININGMODE = Configuration.GetValue("PracticeMode") == 1 && !ScreenManager.IsTrial;
        private static bool m_TrainingMode = TRAININGMODE;
        private bool[] m_HasAgreedToStart;
        private PracticeOverlay practiceScreen;
#if DEBUG
        private bool m_bShowSpawnpoints = false;
        private Texture2D m_TexSpawnpoint;
#endif
        private Map m_Map;
        private Color m_BackgroundColor = Color.White;

        public static void SetTrainingMode(){
            m_TrainingMode = TRAININGMODE;
        }
        
        public GameScreen(Map map)
        {
            m_Map = map;
        }
        public void ChangeMap(Map map)
        {
            m_Map = map;
        }

        public void StartGame(int player)
        {
            m_HasAgreedToStart[player] = true;
            practiceScreen.PressedStart(player);

            for (int i = 0; i < m_HasAgreedToStart.Length; i++)
                if (m_HasAgreedToStart[i] == false) return;

            practiceScreen.MoveOut();
            m_TrainingMode = false;
            Reset();
            Effects.Spawn(new Vector2(512, 384), "ex_fight");
        }

        public void Reset()
        {
            List<Hero> heroes = EntityManager.Heroes;

            List<IEntity> entities = EntityManager.Entities;

            for (int i = 0; i < entities.Count; i++ )
            {
                if (entities[i] is GameEffect ||
                    entities[i] is Enemy || 
                    (entities[i] is DamageObject && !(entities[i] is SacredTorch))
                    )
                {
                    entities[i].Remove();
                }
            }
           
            for (int i = 0; i < heroes.Count; i++)
            {
                heroes[i].Revive();
                GameMode.Instance.SpawnHero(heroes[i],i);
                GameMode.Instance.PrepareGame();
            }
        }

		public override void OnBecomeActive()
		{
			GUI.Instance.Active = true;
		}

        public override void Initialize()
        {
            base.Initialize();

            this.DrawInBackground = true;
            this.UpdateInBackground = false;
            this.ExitOnEscape = false;

            m_Map.Load();
            GameMode.Instance.Map = m_Map;
            GameMode.Instance.StartGame();
            if (GameMode.Instance is GM_Zombie)
                m_BackgroundColor = Color.CornflowerBlue;

	        // Spawn heroes.
            List<Hero> heroes = EntityManager.Heroes;
            for (int i = 0; i < heroes.Count; i++ )
                GameMode.Instance.SpawnHero(heroes[i], i);

            if (GameMode.Instance is GM_Zombie)
            {
                if (WaveController.CurrentWaveNumber == 0)
                    Effects.Spawn(new Vector2(512, 384), "ex_survive");
                m_TrainingMode = false;
            }
            else
            {
                if (m_TrainingMode)
                {
                    practiceScreen = new PracticeOverlay(heroes.Count);
                    EntityManager.Spawn(practiceScreen);
                }
                else
                {
                    Effects.Spawn(new Vector2(512, 384), "ex_fight");
                }
            }

            // Sets so that everybody has to agree to start the game 
            m_HasAgreedToStart = new bool[heroes.Count];

            // Get the respawn time.
            m_RespawnTime = Configuration.GetValue("General_Respawn_Time");

#if DEBUG
            // Detect if we should show spawn points.
            m_bShowSpawnpoints = Configuration.GetValue("Debug_Show_Spawns") > 0;
            m_TexSpawnpoint = Graphics.GetTexture("debug_spawnpoint");
#endif

	        // Create the bounding boxes around the level.
			ObstacleParameters obstacleParams = new ObstacleParameters();
	        obstacleParams.BoundingBoxes.Add(new AABB(-40, -40, 0, 768 + 40));
	        obstacleParams.BoundingBoxes.Add(new AABB(-40, -40, 1024 + 40, 0));
	        obstacleParams.BoundingBoxes.Add(new AABB(-40, 768, 1024 + 40, 768 + 40));
	        obstacleParams.BoundingBoxes.Add(new AABB(1024, -40, 1024 + 40, 768 + 40));
	        Obstacle obstacle = new Obstacle(obstacleParams);
	        EntityManager.Spawn(obstacle);
	
	        //To randomize ingame music. Perhaps a good idea to move this code into soundCenter.cpp?
            if (GameMode.Instance is GM_Zombie)
            {
                SoundCenter.Instance.PlayMusic(MusicNames.Burial);
            } 
            else
            {
                int snd = rand.Next(0, 3);

                if (snd == 0)
		            SoundCenter.Instance.PlayMusic(MusicNames.Charge);
                else if (snd == 1)
                    SoundCenter.Instance.PlayMusic(MusicNames.Ciak);
                else
                    SoundCenter.Instance.PlayMusic(MusicNames.RoadsideFragging);
            }
        }

        public override void Update(float delta)
        {            
            base.Update(delta);
            
            bool bGameOver = GameMode.Instance.GameOver;
	        if(!bGameOver) 
            {
		        EntityManager.Update(delta);
                ScreenBounce.Update(delta);

		        List<Hero> heroes = EntityManager.Heroes;

                if (m_TrainingMode)
                {
                    for(int i = 0; i < heroes.Count && i < 4; ++i)
                    heroes[i].MaximizeEverything(false); 
                }

                for(int i = 0; i < heroes.Count && i < 4; ++i)
		        {
                    // Quick accessor to hero.
			        Hero hero = heroes[i];
			
			        if(!hero.IsAlive) 
                    {
				        m_RespawnTimers[i] += delta;
				        if(m_RespawnTimers[i] > m_RespawnTime) 
                        {
					        if(GameMode.Instance.RespawnHero(hero))
                            {
						        m_RespawnTimers[i] = 0;
					        }
				        }
			        }
		        }

		        Collision.Update(delta);

		        EntityManager.LateUpdate(delta);
		        for (int i = 0; i < heroes.Count; ++i)
                {
			        if(Input.GetPlayerState(heroes[i].PlayerOwner).WasPressed(InputCommand.MenuSelect))
                    {

                        if (m_TrainingMode)
                        {
                            StartGame(i);
                        }
                        else
                        {
                            GUI.Instance.Active = false;
                            ScreenManager.GetInstance().Add(new PauseScreen(this, heroes[i].PlayerOwner, heroes[i].CharacterID));
                        }
			        }
		        }
	        }

#if DEBUG
            if (Input.AnyWasPressed(Microsoft.Xna.Framework.Input.Keys.F5))
                m_bShowSpawnpoints = !m_bShowSpawnpoints;
#endif

			GUI.Instance.Update(delta);

            // Exit the game screen if the game is over.
            GameMode.Instance.Update(delta);
	        //if(GameMode.Instance.GameOver)
		    //    Exit();
        }

        public override void Draw()
        {
            base.Draw();

			GUI.Instance.Draw();

            // Draw the map background image.
            //if (GameMode.Instance is GM_Zombie)
                Graphics.Draw(m_Map.BackgroundTexture, Vector2.Zero, null, 1, m_BackgroundColor);

#if DEBUG
	        if(m_bShowSpawnpoints) 
            {
                // Draw spawn points.
                List<Vector2> spawnpoints = m_Map.SpawnPoints;
                foreach(Vector2 spawn in spawnpoints)
			        Graphics.Draw(m_TexSpawnpoint, spawn - new Vector2(16,16), null, 20, Color.White);
	        }
#endif

	        // Draw all entities.
            List<IEntity> entities = EntityManager.Entities;
            foreach(IEntity entity in entities)
		        entity.Draw();
        }
    }
}
