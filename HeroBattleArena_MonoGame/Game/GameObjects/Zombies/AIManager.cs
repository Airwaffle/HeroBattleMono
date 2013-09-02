using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HeroBattleArena.Game.Screens;

namespace HeroBattleArena.Game.GameObjects
{
    public class AIManager
    {
        #region Fields
        private const int s_SegmentHeight = 20;
        private const int s_SegmentWidth = 25;
        private static float s_SpawnCounter = 0;
        private static float m_SpawnRate = 1;
        private static bool s_ShowTiles = false;
        private static bool s_GameWon = false;
        private static Wave s_ThisWave;
        private static bool s_IsBoss = false;
        private static bool s_FirstUpdate = true;
        private static int s_BossLevel = (int)Configuration.GetValue("Boss_Start_Level"); 

        private static string[] m_WaveNumbers = { "ex_wave0", "ex_wave1", "ex_wave2", "ex_wave3", "ex_wave4", "ex_wave5", "ex_wave6", "ex_wave7", "ex_wave8", "ex_wave9" };
        private static int s_CurrentMap = 0;

        private static List<int> s_NextMapSwitch = new List<int>();  // Decides at which map the map will switch
        private static List<int> s_MapOrder = new List<int>();
        #endregion
      
        #region Properties

        public static int SegmentHeight { get { return s_SegmentHeight; } }
        public static int SegmentWidth { get { return s_SegmentWidth; } }
        public static bool GameWon { get { return s_GameWon; } }
        public static float SpawnRate { get { return m_SpawnRate; }  set { m_SpawnRate = value; } }
        #endregion

        #region Methods

        public static int GetMapNumber()
        {
            int index = s_CurrentMap - 1;
            if (index < 0)
            {
                index = s_NextMapSwitch.Count - 1;
            }
            return s_MapOrder[index];

        }
        public static void AddMapChange(int atWave, int whichMap)
        {
            s_NextMapSwitch.Add(atWave);
            s_MapOrder.Add(whichMap);
        }

        public AIManager()
        {

        }

        public static void Reset()
        {
            s_CurrentMap = 0;
            s_IsBoss = false;
            s_BossLevel = (int)Configuration.GetValue("Boss_Start_Level");
            s_FirstUpdate = true;
        }

        public static void Initialize()
        {
            s_GameWon = false;
	        s_ShowTiles = Configuration.GetValue("Debug_Show_Tile_Collision") > 0;

            WaveController.Initialize();
            s_ThisWave = WaveController.CurrentWave;
            m_SpawnRate = 1 / WaveController.CurrentWave.spawn_per_second;
        }

        public static void CalculateUnWalkable()
        {
            List<Obstacle> obstacles = EntityManager.Obstacles;
	        List<AABB> boundingBoxes = new List<AABB>();
	        foreach (Obstacle obstacle in obstacles)
	        {
		        List<AABB> aabbs = obstacle.BoundingBoxes;
		        foreach (AABB aabb in aabbs)
		        {
                    boundingBoxes.Add(aabb);
		        }
	        }

            AABB square = new AABB(0, 0, 0, 0);
            square.CollisionMask = AABBLayers.CollisionProjectile;
            square.LayerMask = AABBLayers.LayerProjectile;
        }


        public static void Update(float delta)
        {
            if (s_FirstUpdate)
            {
                CheckNewMap();
                s_FirstUpdate = false;
            }
            if (!GameMode.Instance.GameWon)
            {
                SpawnZombies(delta);
            }
            if (!s_ThisWave.isSpawning)
            {
                UpdateWave();
            }

            Random rand = new Random();
            if (rand.Next(0, 1000) <= 0)
            {
                if (s_IsBoss == true)
                    SoundCenter.Instance.Play(SoundNames.bossZombie);
                else
                    SoundCenter.Instance.Play(SoundNames.Zombie);
            }
        }

        public static void SpawnZombies(float delta)
        {
            if (m_SpawnRate <= 0 || delta > 1.0f/30.0f) return;
            if (s_ThisWave.isSpawning)
            {
                s_SpawnCounter += delta;
                if (s_SpawnCounter >= m_SpawnRate)
                {
                    Zombie zombie = new Zombie(null);
                    EntityManager.Spawn(zombie);

                    // Calculates what kind of zombies needs to be created and randomizes one of them (think this may be a quite neat little algorithm!)
                    int zombiesLeft = 0;

                    for (int i = 0; i < 4; i++)
                    {
                        if (s_ThisWave.zombie_amount[i] > 0)
                            zombiesLeft += s_ThisWave.zombie_amount[i];
                    }
                    Random rnd = new Random();
                    int randomZombie = rnd.Next(1, zombiesLeft);

                    for (int i = 0; i < 4; i++)
                    {
                        if (randomZombie <= s_ThisWave.zombie_amount[i])
                        {
                            zombie.setStats(i, s_ThisWave.zombie_health[i], s_ThisWave.zombie_speed[i], s_ThisWave.zombie_damage[i], s_ThisWave.zombie_attackspeed[i], s_ThisWave.zombie_unborrow[i]);
                            s_ThisWave.zombie_amount[i]--;
                            break;
                        }
                        else
                        {
                            randomZombie -= s_ThisWave.zombie_amount[i];
                        }
                    }

                    // Desides if the zombie will spawn outside or inside the screen
                    float random = rnd.Next(0, 100) / 100.0f;

                    if (random < s_ThisWave.spawning_below_ground)
                        zombie.CreateInside();
                    else
                        zombie.CreateOutside();


                    // if there are no more zombies to spawn
                    if (zombiesLeft - 1 <= 0)
                    {
                        s_ThisWave.isSpawning = false;
                    }

                    s_SpawnCounter -= m_SpawnRate;
                }
            }
        }

        private static void UpdateWave()
        {
            // Checks if its time for the next wave
            List<Enemy> enemies = EntityManager.Enemies;
            if (enemies.Count == 0)
            {
                // Gets if there is a next wave availible
                WaveController.Message nextMessage = WaveController.NextWave();
               
                // If WaveController returns END it means that there is no next wave availible
                if (nextMessage == WaveController.Message.End)
                {
                    // End zombiemode
                    //s_GameWon = true;
                    //WaveController.ResetWaves();
                }
                else if (nextMessage == WaveController.Message.Boss)
                {
                    SoundCenter.Instance.Play(SoundNames.bossZombie);
                    s_IsBoss = true;
                    Boss boss = new Boss(s_BossLevel, new Vector2(612, 0));
                    EntityManager.Spawn(boss);
                    //WaveController.NextRevolution();
                    s_BossLevel++;
                    Effects.Spawn(new Vector2(1024 / 2, 768 / 2),"ex_wave_boss");
                }
                else if (nextMessage == WaveController.Message.Next)
                {
                    NextWave();
                    s_IsBoss = false;
                    string waveNr = (WaveController.CurrentWaveNumber + 1).ToString();
                    int nrDist = 64;

                    Effects.Spawn(new Vector2((1024 / 2) - 32 * waveNr.Length, 768 / 2), "ex_wave");

                    for (int i = 0; i < waveNr.Length; i++)
                    {
                        int nr = int.Parse(waveNr[i].ToString());
                        Effects.Spawn(new Vector2(1024 / 2 + 215 - 32 * waveNr.Length + i * nrDist, 768 / 2), m_WaveNumbers[nr]);
                    }
                }
            }
        }
        

        public static void NextWave()
        {
            s_ThisWave = WaveController.CurrentWave;
            m_SpawnRate = 1 / WaveController.CurrentWave.spawn_per_second;

            List<Hero> heroes = EntityManager.Heroes;
            foreach(Hero hero in heroes)
            {
                PlayerInfo info = GameMode.Instance.GetPlayerInfo(hero);
                if (info.Deaths >= (GameMode.Instance as GM_FFA).LifeCount)
                {
                    info.Deaths--;
                    GameMode.Instance.RespawnHero(hero);
                }
            }

            CheckNewMap();
        }

        private static void CheckNewMap()
        {
            List<Hero> heroes = EntityManager.Heroes;

            if (s_CurrentMap < s_NextMapSwitch.Count)
            {
                if (s_NextMapSwitch[s_CurrentMap] == WaveController.ModuluWave)
                {
                    ScreenManager.GetInstance().GetScreen(ScreenManager.GetInstance().NumScreens - 1).Exit();

                    ScreenManager.GetInstance().Add(new GameScreen(Map.Maps[s_MapOrder[s_CurrentMap]]));
                    ScreenManager.GetInstance().Add(new MovieScreen(s_MapOrder[s_CurrentMap]));

                    List<IEntity> entities = EntityManager.Entities;

                    foreach (Hero hero in heroes)
                        hero.MaximizeEverything(true);

                    for (int i = 0; i < entities.Count; i++)
                        if (!(entities[i] is Hero))
                            entities[i].Remove();

                    Map.Maps[s_MapOrder[s_CurrentMap]].Load();

                    GameMode.Instance.Map = Map.Maps[s_MapOrder[s_CurrentMap]];

                    s_CurrentMap = (s_CurrentMap + 1) % s_NextMapSwitch.Count;
                }
            }
        }
        #endregion
    }
}
