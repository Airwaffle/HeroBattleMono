using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeroBattleArena.Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game
{
    public class PlayerInfo
    {
		public int Player = 0;
        public uint Deaths = 0;
        public uint Suicides = 0;
        public uint HeroKills = 0;
        public uint OtherKills = 0;
        public float DamageDealt = 0;
        public uint KillingStreak = 0;
        public int Score { get { return (int)HeroKills * 200 + (int)OtherKills *25 - (int)Suicides * 100 - (int)Deaths * 200; } }
    }

    public class TeamInfo
    {
        public uint TeamID = 0;
        public uint Score = 0;
        public uint Deaths = 0;
    }

    public class GameMode
    {
        private Random m_Random = new Random(DateTime.Now.Millisecond*265);
        private static GameMode s_CurrentGame;

        private bool m_bFriendlyFire = true;
        private bool m_bTeamGame = false;
        private bool m_bGameOver = false;
        private bool m_bGameWon = false;
        private bool m_bPowerups = Configuration.GetValue("PowerUps") == 1;

        // Character portraits...
        

        private static Vector2 s_ScoreWinPos = new Vector2(240, 145);
        private static Vector2[] s_ScoreLosePos =  
		{ 
			new Vector2(240, 280),
			new Vector2(240, 360),
			new Vector2(240, 440),
		};

        protected Vector2 ScoreWinPos { get { return s_ScoreWinPos; } }
        protected Vector2[] ScoreLosePos { get { return s_ScoreLosePos; } }


        
 
        private float m_PowerupCounter = 0;
        private Powerup m_CurrentPowerup = null;
        private float m_PowerupSpawnTime;

        Map m_Map;
        Hero m_Winner;

        private List<PlayerInfo> m_PlayerInfos = new List<PlayerInfo>();
        private List<TeamInfo> m_TeamInfos = new List<TeamInfo>();

        public static GameMode Instance { get { return s_CurrentGame; } set { s_CurrentGame = value; } }
        public Map Map { get { return m_Map; } set { m_Map = value; } }
        public Hero Winner { get { return m_Winner; } protected set { m_Winner = value; } }
        public bool FriendlyFire { get { return m_bFriendlyFire; } set { m_bFriendlyFire = value; } }
        public bool TeamGame { get { return m_bTeamGame; } protected set { m_bTeamGame = value; } }
        public bool GameOver { get { return m_bGameOver; } protected set { m_bGameOver = value; } }
        public bool GameWon { get { return m_bGameWon; } protected set { m_bGameWon = value; } }
        public bool UsePowerups { get { return m_bPowerups; } set { m_bPowerups = value; } }

        public List<TeamInfo> TeamInfos { get { return m_TeamInfos; } }
        public List<PlayerInfo> PlayerInfos { get { return m_PlayerInfos; } }
		public virtual List<PlayerInfo> WinSortedPlayerInfos { get { return m_PlayerInfos; } set { m_PlayerInfos = value; } }

        public virtual void SortList() { }

        protected virtual void CheckGameWon() { }

        private float m_TimeModifier = 1.0f;
        public float TimeModifier { get { return m_TimeModifier; } set { m_TimeModifier = value; } }
        public Hero SafeTimePlayer { get; set; }

        protected void SetTeamCount(int numTeams)
        {
            m_TeamInfos = new List<TeamInfo>();
            for (int i = 0; i < numTeams + 1; ++i)
            {
				m_TeamInfos.Add(new TeamInfo());
                m_TeamInfos[i].TeamID = (uint)i;
            }
        }

        public GameMode()
        {
			for (int i = 0; i < 4; ++i)
			{
				PlayerInfo PI = new PlayerInfo();
				PI.Player = i;
				m_PlayerInfos.Add(PI);
			}
        }

        public virtual void PrepareGame()
        {
            m_PowerupCounter = 0;
            m_CurrentPowerup = null;
            m_bGameWon = false;
            m_bGameOver = false;
            m_Winner = null;
            for (int i = 0; i < m_TeamInfos.Count; ++i)
            {
                m_TeamInfos[i].Score = 0;
                m_TeamInfos[i].Deaths = 0;
            }

            for (int i = 0; i < m_PlayerInfos.Count; ++i)
            {
                m_PlayerInfos[i].DamageDealt = 0;
                m_PlayerInfos[i].Deaths = 0;
                m_PlayerInfos[i].HeroKills = 0;
                m_PlayerInfos[i].OtherKills = 0;
                m_PlayerInfos[i].Suicides = 0;
            }

            m_PowerupSpawnTime = Configuration.GetValue("Powerup_Spawn_Time");
        }

        public virtual void StartGame(){ }

        public virtual void OnUnitDamaged(Unit source, float damage)
        {
            if (source == null) return;
            if (source.GetType() == typeof(Hero))
            {
                PlayerInfo info = GetPlayerInfo((Hero)source);
                if(info != null)
                    info.DamageDealt += damage;
            }
        }

        public virtual void OnUnitKilled(Unit killer, Unit killed) { }

        public virtual bool SpawnHero(Hero hero) { return false; }
        public virtual bool SpawnHero(Hero hero, int pos) { return false; }
        public virtual bool RespawnHero(Hero hero) { return false; }

        public virtual void DrawGUI() { }
        public virtual void DrawScores() { }

        public TeamInfo GetTeamInfo(Unit unit)
        {
            int team = unit.Team;
            if (team < 1)
                return null;
            else if (team > (int)m_TeamInfos.Count)
                return null;
            return m_TeamInfos[team];
        }

        public PlayerInfo GetPlayerInfo(Hero hero)
        {
            return m_PlayerInfos[hero.PlayerOwner];
        }

        public PlayerInfo GetPlayerInfo(int playerID)
        {
            if (playerID < 0 || playerID > 3)
                throw new Exception("Invalid GetPlayerInfo argument.");
            return m_PlayerInfos[playerID];
        }

        public virtual void Update(float delta) 
        {
                if (m_bPowerups && !m_bGameWon)
                {
                    if (m_CurrentPowerup == null)
                    {
                        m_PowerupCounter += delta;
                        if (m_PowerupCounter >= m_PowerupSpawnTime)
                        {
                            m_PowerupCounter = 0;

                            bool done = false;
                            Vector2 spawnPos;
                            do
                            {
                                spawnPos = new Vector2(
                                    (float)(m_Random.NextDouble() * 900 + 62),
                                    (float)(m_Random.NextDouble() * 706 + 62));
                                AABB aabb = new AABB(
                                spawnPos.X - 10,
                                spawnPos.Y - 10,
                                spawnPos.X + 10,
                                spawnPos.Y + 10);
                                aabb.CollisionMask = AABBLayers.CollisionProjectile;
                                aabb.LayerMask = AABBLayers.LayerProjectile;
                                aabb.Owner = null;
                                if (Collision.GetCollidingBoxes(aabb).Count == 0)
                                    done = true;
                            } while (!done);

                            if (m_Random.NextDouble() < 0.5)
                                m_CurrentPowerup = new SlowPowerup();
                            else
                                m_CurrentPowerup = new DamagePowerup();
                            m_CurrentPowerup.Position = spawnPos;
                            EntityManager.Spawn(m_CurrentPowerup);
                        }
                    }
                    else
                    {
                        if (m_CurrentPowerup.IsRemoved)
                        {
                            m_CurrentPowerup = null;
                            m_PowerupCounter = 0;
                        }
                    }
                }
            
            }

    }
}
