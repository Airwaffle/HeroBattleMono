using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.Screens
{
    class ScoreScreen : Screen
    {
        Texture2D[] m_Numbers = new Texture2D[11];
        Texture2D[] m_TeamIcons = new Texture2D[2];
        Texture2D m_Overlay;
        Texture2D m_Underlay;
        Texture2D m_TextDeath;
        Texture2D m_TextPlayer;
        Texture2D m_TextScore;
        Texture2D m_TextKill;
        Texture2D m_TextTeam;
        Texture2D m_WinnerStar;

        Vector2 m_OverlayPos = new Vector2(186, 202);
        Vector2 m_UnderlayWantedPos = new Vector2(150, 140);
        Vector2 m_UnderlayPos = new Vector2(-200, 140);
        bool m_UnderlayCorrectPosition = false;

        Vector2 m_IconPos = new Vector2(116,180);
        Vector2 m_StarPos = new Vector2(105, 225);

        Vector2 m_TextPlayerPos = new Vector2(246, 180);
        Vector2 m_TextTeamPos = new Vector2(402, 180);
        Vector2 m_TextScorePos = new Vector2(548, 180);
        Vector2 m_TextKillPos = new Vector2(652, 180);
        Vector2 m_TextDeathPos = new Vector2(756, 180);

        Vector2 m_NumberPlayerPos = new Vector2(274, 216);
        Vector2 m_NumberTeamPos = new Vector2(406, 216);
        Vector2 m_NumberScorePos = new Vector2(574, 216);
        Vector2 m_NumberKillPos = new Vector2(676, 216);
        Vector2 m_NumberDeathPos = new Vector2(788, 216);

        float m_IconDist = 0;
        float m_IconDistWanted = 76;

        float m_StarSize = 0;
        float m_StarSizeWanted = 1;

        public override void Initialize()
		{
			base.Initialize();

			m_Numbers[0] = Graphics.GetTexture("score_0");
            m_Numbers[1] = Graphics.GetTexture("score_1");
            m_Numbers[2] = Graphics.GetTexture("score_2");
            m_Numbers[3] = Graphics.GetTexture("score_3");
            m_Numbers[4] = Graphics.GetTexture("score_4");
            m_Numbers[5] = Graphics.GetTexture("score_5");
            m_Numbers[6] = Graphics.GetTexture("score_6");
            m_Numbers[7] = Graphics.GetTexture("score_7");
            m_Numbers[8] = Graphics.GetTexture("score_8");
            m_Numbers[9] = Graphics.GetTexture("score_9");
            m_Numbers[10] = Graphics.GetTexture("score_negative");

            m_TeamIcons[0] = Graphics.GetTexture("score_team_red");
            m_TeamIcons[1] = Graphics.GetTexture("score_team_blue");
            
            m_Overlay = Graphics.GetTexture("score_overlay");
            m_Underlay = Graphics.GetTexture("score_underlay");

            m_TextDeath = Graphics.GetTexture("score_death");
            m_TextKill = Graphics.GetTexture("score_kill");
            m_TextPlayer = Graphics.GetTexture("score_player");
            m_TextScore = Graphics.GetTexture("score_score");
            m_TextTeam = Graphics.GetTexture("score_team");
            m_WinnerStar = Graphics.GetTexture("score_winner");
		}

		public override void Update(float delta)
		{
			base.Update(delta);
            if (Input.AnyWasPressed(InputCommand.Attack) || Input.AnyWasPressed(InputCommand.MenuSelect) || Input.AnyWasPressed(InputCommand.MenuBack))
            {
                ScreenManager screenManager = ScreenManager.GetInstance();
                screenManager.ClearScreens(screenManager.NumScreens - 2);
            }
            if (!m_UnderlayCorrectPosition){
                m_UnderlayPos = m_UnderlayPos * (1 - 0.16f) + m_UnderlayWantedPos * 0.16f;
                if ((m_UnderlayPos - m_UnderlayWantedPos).Length() < 0.5f)
                    m_UnderlayCorrectPosition = true;
            } else {
                m_IconDist = m_IconDist * (1 - 0.20f) + m_IconDistWanted * 0.20f;
                m_StarSize = m_StarSize * (1 - 0.20f) + m_StarSizeWanted * 0.20f;
            }
		}

		public override void Draw()
		{
			base.Draw();
            List<PlayerInfo> PlayerInfos = GameMode.Instance.WinSortedPlayerInfos;
            List<GameObjects.Hero> Heroes = GameObjects.EntityManager.Heroes;
            bool isTeam = GameMode.Instance is GM_Team;

            Graphics.Draw(m_Underlay, m_UnderlayPos, null, 100, Color.White);

            Graphics.Draw(m_TextDeath, m_TextDeathPos, null, 110, Color.White);
            Graphics.Draw(m_TextKill, m_TextKillPos, null, 110, Color.White);
            Graphics.Draw(m_TextPlayer, m_TextPlayerPos, null, 110, Color.White);
            Graphics.Draw(m_TextScore, m_TextScorePos, null, 110, Color.White);
            Graphics.Draw(m_WinnerStar, m_StarPos, m_StarSize, null, 110, Color.White);

            // Draw team text only if team mode.
            if(isTeam)
                Graphics.Draw(m_TextTeam, m_TextTeamPos, null, 110, Color.White);
            if (m_UnderlayCorrectPosition)
            {
                for (int i = 0; i < Heroes.Count; i++)
                {
                    Vector2 extraDistance = new Vector2(0, i * m_IconDist);
                    int kills = GameMode.Instance is GM_Zombie ? (int)PlayerInfos[i].HeroKills + (int)PlayerInfos[i].OtherKills : (int)PlayerInfos[i].HeroKills;

                    Graphics.Draw(m_Overlay, m_OverlayPos + extraDistance, null, 105, Color.White);
                    Graphics.Draw(Heroes[PlayerInfos[i].Player].WinnerPortrait, m_IconPos + extraDistance, null, 106, Color.White);

                    if (isTeam)
                        Graphics.Draw(m_TeamIcons[Heroes[i].Team - 1], m_NumberTeamPos + extraDistance, null, 110, Color.White);

                    DrawNumber(PlayerInfos[i].Player.ToString(), m_NumberPlayerPos + extraDistance);
                    DrawNumber(PlayerInfos[i].Score.ToString(), m_NumberScorePos + extraDistance);
                    DrawNumber(kills.ToString(), m_NumberKillPos + extraDistance);
                    DrawNumber(PlayerInfos[i].Deaths.ToString(), m_NumberDeathPos + extraDistance);
                }
            }
		}

        // Draw digits.
        private void DrawNumber(String number, Vector2 pos){    
            

            // if first char is a minus sign, draw this to the left of the number
            if (number[0] == '-'){
                Graphics.Draw(m_Numbers[10], pos + new Vector2(-16, 0), null, 110, Color.White);
                number = number.Substring(1);
            }
            int digit = -1;
            for (int i = 0; i < number.Length; ++i)
            {
                digit = int.Parse(number[i].ToString()); ;
                Graphics.Draw(m_Numbers[digit], pos + new Vector2(16 * i, 0), null, 110, Color.White);
                
            }
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
