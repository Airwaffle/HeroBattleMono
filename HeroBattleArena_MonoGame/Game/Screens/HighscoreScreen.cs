using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.Screens
{
    class HighscoreScreen : Screen
    {
        Texture2D[] m_Numbers = new Texture2D[11];
        Texture2D[] m_TeamIcons = new Texture2D[2];
        Texture2D[] m_HeroIcons = new Texture2D[5];
        Texture2D m_Overlay;
        Texture2D m_Underlay;
        Texture2D m_TextWave;
        Texture2D m_TextPlayer;
        Texture2D m_TextScore;
        Texture2D m_TextKill;
        Texture2D m_TextTeam;
        Texture2D m_WinnerStar;
        Texture2D m_Letters;
        Texture2D m_LettersYellow;

        Vector2 m_OverlayPos = new Vector2(186, 202);
        Vector2 m_UnderlayWantedPos = new Vector2(150, 140);
        Vector2 m_UnderlayPos = new Vector2(-200, 140);
        bool m_UnderlayCorrectPosition = false;

        Vector2 m_IconPos = new Vector2(116, 180);
        Vector2 m_StarPos = new Vector2(105, 225);

        Vector2 m_TextPlayerPos = new Vector2(280, 180);
        Vector2 m_TextTeamPos = new Vector2(402, 180);
        Vector2 m_TextScorePos = new Vector2(756, 180);
        Vector2 m_TextKillPos = new Vector2(652, 180); 
        Vector2 m_TextDeathPos = new Vector2(548, 180);
            

        Vector2 m_NumberPlayerPos = new Vector2(274, 216);
        Vector2 m_NumberTeamPos = new Vector2(406, 216);
        Vector2 m_NumberScorePos = new Vector2(788, 216);
        Vector2 m_NumberKillPos = new Vector2(676, 216);
        Vector2 m_NumberWavePos = new Vector2(574, 216); 

        float m_IconDist = 0;
        float m_IconDistWanted = 76;

        float m_StarSize = 0;
        float m_StarSizeWanted = 1;

        bool m_IsInputingName = false;
        string m_InputString = "";
        int m_CurrentInputSign = 0;
        int m_MaxInputLetters = 5;
        string m_Alphabet = "ABCDEFGHIJKLMNOPQRSTUVXYZ0123456789";
        int m_LetterOffset = 39; // The offset between the letters on the spritesheet.
        int m_LettersWidth = 10; // The number of letters on the width of the spritesheet.

        int m_NrOfPlayers = -1;
        ScoreComponent m_Component = null;
        int m_AddingPosition = -1;

        public HighscoreScreen(int nrPlayers)
        {
            m_NrOfPlayers = nrPlayers;
        }

        public void InputName(ScoreComponent newComponent, int position) { 
            m_IsInputingName = true;
            m_Component = newComponent;
            m_AddingPosition = position;
        }

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

            m_HeroIcons[0] = Graphics.GetTexture("score_icon_arthur");
            m_HeroIcons[1] = Graphics.GetTexture("score_icon_stryker");
            m_HeroIcons[2] = Graphics.GetTexture("score_icon_aztec");
            m_HeroIcons[3] = Graphics.GetTexture("score_icon_nano");
            m_HeroIcons[4] = Graphics.GetTexture("score_icon_zombie");

            m_Overlay = Graphics.GetTexture("score_overlay");
            m_Underlay = Graphics.GetTexture("score_underlay");

            m_TextWave = Graphics.GetTexture("score_wave");
            m_TextKill = Graphics.GetTexture("score_kill");
            m_TextPlayer = Graphics.GetTexture("score_player");
            m_TextScore = Graphics.GetTexture("score_score");
            m_TextTeam = Graphics.GetTexture("score_team");
            m_WinnerStar = Graphics.GetTexture("score_winner");

            m_Letters = Graphics.GetTexture("score_letters");
            m_LettersYellow = Graphics.GetTexture("score_letters_win");

            ExitOnEscape = false;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if ((Input.AnyWasPressed(InputCommand.Attack) || Input.AnyWasPressed(InputCommand.MenuSelect) || Input.AnyWasPressed(InputCommand.MenuBack)) && m_IsInputingName == false)
            {
                ScreenManager screenManager = ScreenManager.GetInstance();
                screenManager.ClearScreens(screenManager.NumScreens - 2);
            }
            if (!m_UnderlayCorrectPosition)
            {
                m_UnderlayPos = m_UnderlayPos * (1 - 0.16f) + m_UnderlayWantedPos * 0.16f;
                if ((m_UnderlayPos - m_UnderlayWantedPos).Length() < 0.5f)
                    m_UnderlayCorrectPosition = true;
            }
            else
            {
                m_IconDist = m_IconDist * (1 - 0.20f) + m_IconDistWanted * 0.20f;
                m_StarSize = m_StarSize * (1 - 0.20f) + m_StarSizeWanted * 0.20f;

                if (m_IsInputingName)
                {
                    m_CurrentInputSign = GetEntryChanged(m_CurrentInputSign, m_Alphabet.Length, delta);

                    //Agree to letter
                    if (Input.AnyWasPressed(InputCommand.Attack) || Input.AnyWasPressed(InputCommand.Right))
                    {
                        if (m_InputString.Length > m_MaxInputLetters)
                        {
                            m_InputString += m_Alphabet[m_CurrentInputSign].ToString();
                            InputDone();
                        }
                        else
                        {
                            m_InputString += m_Alphabet[m_CurrentInputSign].ToString();
                            m_CurrentInputSign = 0;
                        }
                    }
                    else if (Input.AnyWasPressed(InputCommand.Special3) || Input.AnyWasPressed(InputCommand.Left)) 
                    {
                        if (m_InputString.Length > 0)
                        {
                            m_InputString = m_InputString.Remove(m_InputString.Length - 1);
                        }
                        m_CurrentInputSign = 0;
                    }
                    else if (Input.AnyWasPressed(InputCommand.MenuSelect))
                    {
                        m_InputString += m_Alphabet[m_CurrentInputSign].ToString();
                        InputDone();
                    }
                    else if (Input.AnyWasPressed(InputCommand.MenuBack))
                    {
                        m_InputString = "";
                    }
                }
            }
        }

        public override void Draw()
        {
            base.Draw();
            List<PlayerInfo> PlayerInfos = GameMode.Instance.WinSortedPlayerInfos;
            List<GameObjects.Hero> Heroes = GameObjects.EntityManager.Heroes;

            Graphics.Draw(m_Underlay, m_UnderlayPos, null, 100, Color.White);

            Graphics.Draw(m_TextWave, m_TextDeathPos, null, 110, Color.White);
            Graphics.Draw(m_TextKill, m_TextKillPos, null, 110, Color.White);
            Graphics.Draw(m_TextPlayer, m_TextPlayerPos, null, 110, Color.White);
            Graphics.Draw(m_TextScore, m_TextScorePos, null, 110, Color.White);
            Graphics.Draw(m_WinnerStar, m_StarPos + new Vector2((m_NrOfPlayers - 1) * 20, 0), m_StarSize, null, 110, Color.White);

            if (m_UnderlayCorrectPosition)
            {
                for (int i = 0; i < 5; i++)
                {
                    ScoreComponent sc = HighScoreList.GetComponent(m_NrOfPlayers - 1, i);
                    Vector2 extraDistance = new Vector2(0, i * m_IconDist);

                    for (int j = 0; j < sc.Heroes.Length; j++)
                    {
                        Graphics.Draw(m_HeroIcons[int.Parse(sc.Heroes[j].ToString())], m_IconPos + extraDistance + new Vector2(j * 40 - 20 * sc.Heroes.Length, 0), null, 106, Color.White);
                    }

                    if (i != m_AddingPosition)
                    {
                        DrawLetters(sc.Name, m_NumberPlayerPos + extraDistance, Color.White, m_Letters);
                    }
                    else
                    {
                        DrawLetters(m_InputString, m_NumberPlayerPos + extraDistance, Color.White, m_LettersYellow);
                        if (m_IsInputingName)
                        {
                            DrawLetters(m_Alphabet[m_CurrentInputSign].ToString(), m_NumberPlayerPos + extraDistance + new Vector2(m_InputString.Length * m_LetterOffset, 0), Color.Yellow, m_Letters);
                        }
                    }
                    DrawNumber(sc.Kills.ToString(), m_NumberKillPos + extraDistance - new Vector2((sc.Kills.ToString().Length - 1) * 8, 0));
                    DrawNumber(sc.Wave.ToString(), m_NumberWavePos + extraDistance - new Vector2((sc.Wave.ToString().Length - 1) * 8, 0));
                    DrawNumber(sc.Score.ToString(), m_NumberScorePos + extraDistance - new Vector2((sc.Score.ToString().Length - 1) * 8, 0)); 
                }
            }
        }


        private void InputDone()
        {
            m_IsInputingName = false;
            //m_Component.Name = m_InputString;
            //HighScoreList.ReplaceComponent(m_NrOfPlayers-1, m_Component, m_AddingPosition);
            HighScoreList.ReplaceComponentName(m_NrOfPlayers - 1, m_InputString, m_AddingPosition);
        }

        // Draw digits.
        private void DrawNumber(String number, Vector2 pos)
        {
            // if first char is a minus sign, draw this to the left of the number
            if (number[0] == '-')
            {
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

        // Draw letters.
        private void DrawLetters(String letters, Vector2 pos, Color color, Texture2D sheet)
        {
            for (int i = 0; i < letters.Length; ++i)
            {
                Graphics.Draw(sheet, pos + new Vector2(m_LetterOffset * i, 0), GetLetter(letters[i]), 110, color);
            }
        }

        private Rectangle GetLetter(char character)
        {

            int xOffset = 0;
            int yOffset = 0;
            int index = m_Alphabet.IndexOf(character);

            // Check if we found the character
            if (index != -1)
            {
                xOffset = (index % m_LettersWidth);
                yOffset = (int)Math.Floor(index * 1.0f / m_LettersWidth);
            }

            return new Rectangle(xOffset * m_LetterOffset, yOffset * m_LetterOffset, m_LetterOffset, m_LetterOffset);
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
}
