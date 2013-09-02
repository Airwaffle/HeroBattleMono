using System;
using System.Collections.Generic;
using HeroBattleArena.Game.GameObjects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using HeroBattleArena.Game.Screens;

namespace HeroBattleArena.Game
{
    public class GM_Zombie : GM_FFA
    {
        private bool actuallyLost = false;

        private List<PlayerInfo> m_ZombieSortedPlayerInfos = new List<PlayerInfo>();
        public override List<PlayerInfo> WinSortedPlayerInfos { get { return m_ZombieSortedPlayerInfos; } set { m_ZombieSortedPlayerInfos = value; } }
        public GM_Zombie() { }

        public override void SortList()
        {
            List<Hero> heroes = EntityManager.Heroes;
            List<PlayerInfo> infos = new List<PlayerInfo>();
            for (int j = 0; j < PlayerInfos.Count; j++)
                infos.Add(PlayerInfos[j]);
            

            int i = 0;
            while (i < heroes.Count)
            {
                if (i <= 0)
                {
                    i++;
                }
                else if (infos[i].OtherKills > infos[i - 1].OtherKills)
                {
                    //SWAP
                    PlayerInfo tmp = infos[i];
                    infos[i] = infos[i - 1];
                    infos[i - 1] = tmp;
                    i--;
                }
                else
                {
                    i++;
                }

            }

            m_ZombieSortedPlayerInfos = infos;
        }

        public override void OnUnitKilled(Unit other, Unit killed)
        {
            base.OnUnitKilled(other, killed);
        }

        public override void PrepareGame()
        {
            AIManager.Initialize();
            /*if (!FriendlyFire)
            {
                List<Hero> heroes = EntityManager.Heroes;
                for (int i = 0; i < heroes.Count; ++i)
                {
                    heroes[i].Team = 1;
                }
             TeamGame = true;
            }*/
            base.PrepareGame();           
        }

        public override void StartGame()
        {
            //LifeCount = 1;
        }

        protected override void CheckGameWon()
        {
            if (GameWon) return;

            if (AIManager.GameWon)
            {
                GameWon = true;
            }

            List<Hero> heroes = EntityManager.Heroes;
            bool heroesDead = true;

            for (int i = 0; i < heroes.Count; ++i)
            {
                PlayerInfo info = GetPlayerInfo(heroes[i]);
                if (info.Deaths < LifeCount)
                    heroesDead = false;
            }
            if (heroesDead)
            {
                GameWon = true;
            }

            if (GameWon)
            {
                SortList();
                actuallyLost = true;
                CheckHighScore();
                WaveController.ResetWaves();
                AIManager.Initialize();
            }
        }

        

        public override void Update(float delta)
        {
            base.Update(delta);
            if(!GameWon)
                AIManager.Update(delta);
        }

        public void CheckHighScore()
        {
            int score = 0;
            int kills = 0;
            List<Hero> heroes = EntityManager.Heroes;
            for (int i = 0; i < heroes.Count; ++i)
            {
                PlayerInfo info = GetPlayerInfo(heroes[i]);
                score += info.Score;
                kills += (int)info.OtherKills;
            }
            ScoreComponent comp = new ScoreComponent("AAA", score, WaveController.CurrentWaveNumber, "", kills);
            int scorePlace = HighScoreList.CheckNewComponent(heroes.Count, comp);

            if (scorePlace != -1)
            {
                string heroString = "";
                for (int i = 0; i < heroes.Count; ++i)
                {
                    if (heroes[i] is Arthur) heroString += "0";
                    else if (heroes[i] is Soldier) heroString += "1";
                    else if (heroes[i] is Aztec) heroString += "2";
                    else if (heroes[i] is Mage) heroString += "3";
                    else if (heroes[i] is ZombieHero) heroString += "4";
                }
                comp.Heroes = heroString;

                HighscoreScreen screen = new HighscoreScreen(heroes.Count);
                screen.InputName(comp, scorePlace);
                ScreenManager.GetInstance().Add(screen);
                HighScoreList.AddNewComponent(heroes.Count - 1, comp, scorePlace);
            }
            else
            {
                ScreenManager.GetInstance().Add(new HighscoreScreen(heroes.Count));
            }
        }

        public override void DrawScores()
        {
            /*
            List<GameObjects.Hero> heroes = GameObjects.EntityManager.Heroes;
            List<PlayerInfo> playerInfos = m_ZombieSortedPlayerInfos;

            Graphics.Draw(heroes[playerInfos[0].Player].WinnerPortrait,
                ScoreWinPos, null, 30, Color.White);
            Graphics.DrawText("Zombies Killed: " + playerInfos[0].OtherKills.ToString(), 105, new Vector2(ScoreWinPos.X + 135, ScoreWinPos.Y + 15), Color.Red);
            Graphics.DrawText("You killed this many heroes: " + playerInfos[0].HeroKills.ToString(), 105, new Vector2(ScoreWinPos.X + 135, ScoreWinPos.Y + 35), Color.White);
            Graphics.DrawText("You died this many times: " + playerInfos[0].Deaths.ToString(), 105, new Vector2(ScoreWinPos.X + 135, ScoreWinPos.Y + 55), Color.White);
            Graphics.DrawText("Total damage dealt: " + playerInfos[0].DamageDealt.ToString(), 105, new Vector2(ScoreWinPos.X + 135, ScoreWinPos.Y + 75), Color.White);

            for (int i = 1; i < heroes.Count; i++)
            {
                Graphics.DrawText("Zombies Killed: " + playerInfos[i].OtherKills.ToString(), 105, new Vector2(ScoreLosePos[i - 1].X + 75, ScoreLosePos[i - 1].Y), Color.Red);
                Graphics.DrawText("You killed this many heroes: " + playerInfos[i].HeroKills.ToString(), 105, new Vector2(ScoreLosePos[i - 1].X + 75, ScoreLosePos[i - 1].Y + 15), Color.White);
                Graphics.DrawText("You died this many times: " + playerInfos[i].Deaths.ToString(), 105, new Vector2(ScoreLosePos[i - 1].X + 75, ScoreLosePos[i - 1].Y + 35), Color.White);
                Graphics.DrawText("Total damage dealt: " + playerInfos[i].DamageDealt.ToString(), 105, new Vector2(ScoreLosePos[i - 1].X + 75, ScoreLosePos[i - 1].Y + 55), Color.White);
                
                Graphics.Draw(heroes[playerInfos[i].Player].LoserPortrait,
                ScoreLosePos[i - 1], null, 30, Color.White);
            }
             */
        }
    }
}
