using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HeroBattleArena.Game.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using HeroBattleArena.Game.Screens;

namespace HeroBattleArena.Game
{
    public class GM_FFA : GameMode
    {
        private float m_GameOverCount = 0;
        private float m_GameOverDuration = 1.0f;
        private int m_FragLimit = 0;
        private int m_LifeCount = 0;
        private int m_KillingStreakNeeded = (int)Configuration.GetValue("KillingstreakNeededUnlock");

        public int FragLimit { get { return m_FragLimit; } set { m_FragLimit = value; } }
        public int LifeCount { get { return m_LifeCount; } set { m_LifeCount = value; } }

        private List<PlayerInfo> m_FFASortedPlayerInfos = new List<PlayerInfo>();

        public override List<PlayerInfo> WinSortedPlayerInfos { get { return m_FFASortedPlayerInfos; } set { m_FFASortedPlayerInfos = value; } }

        public GM_FFA() { }

        public override void SortList()
        {
            List<Hero> heroes = EntityManager.Heroes;
            List<PlayerInfo> infos = new List<PlayerInfo>();
            for (int j = 0; j < PlayerInfos.Count; j++)
                infos.Add(PlayerInfos[j]);

            int i = 0;
            while (i < heroes.Count)
            {
                if (i <= 0){
                    i++;
                } else if (infos[i].HeroKills > infos[i-1].HeroKills)
                {
                    //SWAP
                    PlayerInfo tmp = infos[i];
                    infos[i] = infos[i-1];
                    infos[i-1] = tmp;
                    i--;
                } else {
                    i++;
                }

            }

            m_FFASortedPlayerInfos = infos;
        }

        public override void PrepareGame()
        {
            m_GameOverDuration = Configuration.GetValue("General_GameOverWait");

            m_GameOverCount = 0;
            this.TeamGame = false;
            SetTeamCount(1);

			base.PrepareGame();
        }

        protected override void CheckGameWon()
        {
	        if(this.GameWon) return;

	        if(this.m_FragLimit > 0) 
            {
		        for(int i = 0; i < 4; ++i) 
                {
			        PlayerInfo info = this.GetPlayerInfo(i);
			        if((int)info.HeroKills >= this.m_FragLimit)
                    {
                        List<Hero> heroes = EntityManager.Heroes;
				        for(int k = 0; k < heroes.Count; ++k) 
                        {
					        if(heroes[k].PlayerOwner == i) 
                            {
						        this.Winner = heroes[k];
                                Effects.Spawn(new Vector2(400, 300), heroes[k].VictoryExclamation);

						        break;
					        }
				        }
				        this.GameWon = true;
                        SortList();
			        }
		        }
	        }
	        if(m_LifeCount > 0) 
            {
		        List<Hero> heroes = EntityManager.Heroes;
		        int livingHeroes = heroes.Count;
		        Hero winningHero = null;

		        for(int k = 0; k < heroes.Count; ++k) 
                {
			        PlayerInfo info = GetPlayerInfo(heroes[k]);
			        if((int)info.Deaths >= m_LifeCount) 
                    {
				        --livingHeroes;
				        if(livingHeroes == 1) 
                        {
					        break;
				        }
			        }
			        else 
                    {
				        winningHero = heroes[k];
			        }
		        }
		        if(livingHeroes == 1) 
                {
			        this.Winner = winningHero;
                    this.GameWon = true;
#warning Hittar inte följande hero (vvem det var som vann om jag kör last man standing, WTF?)
                    //Effects.Spawn(new Vector2(400, 300), winningHero.VictoryExclamation);
                    SortList();
		        }
	        }
        }

        public override void OnUnitKilled(Unit other, Unit killed)
        {
            // Checks if the one killing the unit is a minion to any other player
            // in that case, make that player responsible for the killing.
            if (other is Zombie)
            {
                if ((other as Zombie).Owner != null)
                {
                    other = (other as Zombie).Owner;
                }
            }

	        if(killed != null) 
            {
		        if(killed is Hero) 
                {
			        OnHeroKilled(other, killed as Hero);
		        }
	        }

	        if(other is Hero && !(killed is Hero))
            {
		        PlayerInfo killerInfo = GetPlayerInfo(other as Hero);
		        ++killerInfo.OtherKills;
                ++killerInfo.KillingStreak;

                // Check if new characters are unlocked.
                if (killerInfo.KillingStreak == m_KillingStreakNeeded && !EntityManager.GetUnlockedChar(0))
                {
                    EntityManager.UnlockChar(0);
                    HighScoreList.Save();
                    //ADDSOUND UNLOCK ZOMBIECHARACTER
                    Effects.Spawn(new Vector2(512, 768 / 2), "ex_unlocked_green");
                }
	        }
        }

        public virtual void OnHeroKilled(Unit other, Hero killed)
        {
		    PlayerInfo killedInfo = GetPlayerInfo(killed);
            // Add to deaths.
		    ++killedInfo.Deaths;
            // Check if someone killed him/herself.
		    if(other == killed)
			    ++killedInfo.Suicides;
            // Add deaths to the killed hero's team.
		    TeamInfo killedTeam = GetTeamInfo(killed);
		    ++killedTeam.Deaths;

            // Removes the players killingstreak
            killedInfo.KillingStreak = 0;

            // Check if the other (killer) is a hero.
            // Also, don't do this if other and killed
            // is the same guy.            
            if(other is Hero && other != killed)
            {
                Hero killer = other as Hero;
                Console.WriteLine(killer.Name + " got score!");
                PlayerInfo killerInfo = GetPlayerInfo(killer);		
		        ++killerInfo.HeroKills;

		        TeamInfo killerTeam = GetTeamInfo(killer);
		        killerTeam.Score++;
            }
        }

        public override bool SpawnHero(Hero hero)
        {
            List<Vector2> spawnPoints = Map.SpawnPoints;
	        List<Hero> heroes = EntityManager.Heroes;

	        int bestSpawnPoint = 0;
	        float bestValue = -1;

	        // Find the best spawn point.
	        for(int i = 0; i < spawnPoints.Count; ++i) 
            {
		        float value = 0;
		        for(int k = 0; k < heroes.Count; ++k) 
                {
			        float length = (spawnPoints[i] - heroes[k].Position).Length();
			        if(length < 64) 
                    {
				        value = 0;
				        break;
			        }
			        value += length;
		        }
		        if(value > 0 && value > bestValue) 
                {
			        bestValue = value;
			        bestSpawnPoint = i;	
		        }
	        }

	        if(bestValue >= 0) 
            {
		        hero.Position = spawnPoints[bestSpawnPoint];
		        return true;
	        }
	        return false;
        }

        public override bool SpawnHero(Hero hero, int pos)
        {
            List<Vector2> spawnPoints = Map.SpawnPoints;

            if (pos >= spawnPoints.Count) return false;

            hero.Position = spawnPoints[pos];
            return true;
        }

        public override bool RespawnHero(Hero hero)
        {
            if (m_LifeCount > 0 && GetPlayerInfo(hero).Deaths >= m_LifeCount)
                return false;
            if (SpawnHero(hero))
            {
                Effects.Spawn(hero.Position, "General_Respawn");
                SoundCenter.Instance.Play(SoundNames.AllRespawn);
                hero.Revive();
                return true;
            }

            return false;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            CheckGameWon();

	        // Countdown for game termination.
	        if(GameWon && !GameOver) 
            {
		        m_GameOverCount += delta;
                if (m_GameOverCount > m_GameOverDuration)
                {
                    GameOver = true;
                    if (!(GameMode.Instance is GM_Zombie))
                    {
                        ScreenManager.GetInstance().Add(new ScoreScreen());
                    }
                }
	        }
        }

        public override void DrawScores() 
        {
            /*List<GameObjects.Hero> heroes = GameObjects.EntityManager.Heroes;
            List<PlayerInfo> playerInfos = m_FFASortedPlayerInfos;

            Graphics.Draw(heroes[playerInfos[0].Player].WinnerPortrait,
                ScoreWinPos, null, 30, Color.White);

            Graphics.DrawText("You killed this many heroes: " + playerInfos[0].HeroKills.ToString(), 105, new Vector2(ScoreWinPos.X + 135, ScoreWinPos.Y  + 15), Color.Red);
            Graphics.DrawText("You died this many times: " + playerInfos[0].Deaths.ToString(), 105, new Vector2(ScoreWinPos.X + 135, ScoreWinPos.Y + 35), Color.White);
            Graphics.DrawText("Total damage dealt: " + playerInfos[0].DamageDealt.ToString(), 105, new Vector2(ScoreWinPos.X + 135, ScoreWinPos.Y + 55), Color.White);
            for (int i = 1; i < heroes.Count; i++)
            {
                Graphics.DrawText("You killed this many heroes: " + playerInfos[i].HeroKills.ToString(), 105, new Vector2(ScoreLosePos[i - 1].X + 75, ScoreLosePos[i - 1].Y + 15), Color.Red);
                Graphics.DrawText("You died this many times: " + playerInfos[i].Deaths.ToString(), 105, new Vector2(ScoreLosePos[i - 1].X + 75, ScoreLosePos[i - 1].Y + 35), Color.White);
                Graphics.DrawText("Total damage dealt: " + playerInfos[i].DamageDealt.ToString(), 105, new Vector2(ScoreLosePos[i - 1].X + 75, ScoreLosePos[i - 1].Y + 55), Color.White);
                
                Graphics.Draw(heroes[playerInfos[i].Player].LoserPortrait,
                ScoreLosePos[i-1], null, 30, Color.White);
             
            }*/     
        }
    }
}
