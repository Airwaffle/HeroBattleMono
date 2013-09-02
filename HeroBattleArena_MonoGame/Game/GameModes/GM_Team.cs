using System;
using System.Collections.Generic;
using HeroBattleArena.Game.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game
{
    public class GM_Team : GM_FFA
    {
        private TeamInfo m_WinningTeam;
        private bool m_bSeparateLives = false;

        public TeamInfo WinningTeam { get { return m_WinningTeam; } protected set { m_WinningTeam = value; } }
        public bool SeparateLives { get { return m_bSeparateLives; } set { m_bSeparateLives = value; } }

        //private List<PlayerInfo> m_TeamSortedPlayerInfos = new List<PlayerInfo>();
        //public override List<PlayerInfo> WinSortedPlayerInfos { get { return m_TeamSortedPlayerInfos; } set { m_TeamSortedPlayerInfos = value; } }

        public GM_Team() { }

        public override void PrepareGame()
        {
            base.PrepareGame();

            m_WinningTeam = null;
            TeamGame = true;
            SetTeamCount(2);
        }

        protected override void CheckGameWon()
        {
	        if(GameWon) return;

	        int frags = this.FragLimit;
	        int lives = this.LifeCount;

	        if(frags > 0) 
            {
                List<TeamInfo> teams = this.TeamInfos;
		        for(int i = 0; i < teams.Count; ++i) 
                {
			        if((int)teams[i].Score >= frags) 
                    {
				        this.WinningTeam = teams[i];
				        this.GameWon = true;
                        SortList();
			        }
		        }
	        }
	        if(lives > 0) 
            {		
		        List<TeamInfo> teams = this.TeamInfos;

		        int winningTeam = 0;
		        // Check all teams but the neutral team.
		        for(int i = 1; i < teams.Count; ++i) 
                {
			        if((int)teams[i].Deaths >= lives) 
                    {
				        if(i == 1)
					        winningTeam = 2;
				        else if(i == 2)
					        winningTeam = 1;
			        }
		        }
		
		        if(winningTeam > 0) 
                {
			        List<Hero> heroes = EntityManager.Heroes;
			        bool heroesDead = true;

			        for(int i = 0; i < heroes.Count; ++i) 
                    {
				        if(heroes[i].IsAlive && winningTeam != heroes[i].Team)
					        heroesDead = false;
			        }
			        if(heroesDead) 
                    {
				        this.m_WinningTeam = teams[winningTeam];
                        this.GameWon = true;
                        SortList();
			        }
		        }
	        }
        }

        public override bool RespawnHero(GameObjects.Hero hero)
        {
            if (this.m_bSeparateLives)
            {
                if (LifeCount > 0 && GetPlayerInfo(hero).Deaths >= LifeCount)
                    return false;
            }
            else
            {
                if (LifeCount > 0 && GetTeamInfo(hero).Deaths >= LifeCount)
                    return false;
            }
            if (SpawnHero(hero))
            {
                hero.Revive();
                return true;
            }

            return false;
        }
        public override void DrawScores()
        {
            /*bool first = true;
            int loserOffset = 0;
            List<GameObjects.Hero> heroes = GameObjects.EntityManager.Heroes;

            for (int i = 0; i < heroes.Count; i++)
            {
                if (heroes[i].Team == m_WinningTeam.TeamID)
                {
                    if (first)
                    {
                        Graphics.Draw(heroes[i].WinnerPortrait,
                        ScoreWinPos, null, 30, Color.White);
                        Graphics.DrawText("Your team got this many kills: " + m_WinningTeam.Score.ToString(), 105, new Vector2(ScoreWinPos.X + 135, ScoreWinPos.Y + 35), Color.Red);
                        Graphics.DrawText("Your team died this many times: " + m_WinningTeam.Deaths.ToString(), 105, new Vector2(ScoreWinPos.X + 135, ScoreWinPos.Y + 55), Color.White);
                        first = false;
                    } 
                    else 
                    {
                        Graphics.Draw(heroes[i].LoserPortrait,
                        new Vector2(ScoreLosePos[0].X, ScoreLosePos[0].Y + loserOffset), null, 30, Color.White);
                        Graphics.DrawText("Your team got this many kills: " + m_WinningTeam.Score.ToString(), 105, new Vector2(ScoreLosePos[0].X + 75, ScoreLosePos[0].Y + 35 + loserOffset), Color.Red);
                        Graphics.DrawText("Your team died this many times: " + m_WinningTeam.Deaths.ToString(), 105, new Vector2(ScoreLosePos[0].X + 75, ScoreLosePos[0].Y + 55 + loserOffset), Color.White);
                        loserOffset += 80;
                    }
                }
            }

            for (int j = 0; j < heroes.Count; j++)
            {
                if (heroes[j].Team != m_WinningTeam.TeamID)
                {
                    Graphics.Draw(heroes[j].LoserPortrait,
                        new Vector2(ScoreLosePos[0].X, ScoreLosePos[0].Y + loserOffset), null, 30, Color.White);
                    Graphics.DrawText("Your team got this many kills: " + GetTeamInfo(heroes[j]).Score.ToString(), 105, new Vector2(ScoreLosePos[0].X + 75, ScoreLosePos[0].Y + 35 + loserOffset), Color.Red);
                    Graphics.DrawText("Your team died this many times: " + GetTeamInfo(heroes[j]).Deaths.ToString(), 105, new Vector2(ScoreLosePos[0].X + 75, ScoreLosePos[0].Y + 55 + loserOffset), Color.White);
                    loserOffset += 80;
                }
            }
            */
            /*List<PlayerInfo> playerInfos = m_FFASortedPlayerInfos;

            Graphics.Draw(heroes[playerInfos[0].Player].WinnerPortrait,
                ScoreWinPos, null, 30, Color.White);

            for (int i = 1; i < heroes.Count; i++)
            {
                Graphics.Draw(heroes[playerInfos[i].Player].LoserPortrait,
                ScoreLosePos[i - 1], null, 30, Color.White);
            }*/
        }
    }
}
