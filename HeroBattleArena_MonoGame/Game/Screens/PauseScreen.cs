using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HeroBattleArena.Game.GameObjects;
using Microsoft.Xna.Framework.GamerServices;

namespace HeroBattleArena.Game.Screens
{
    public class PauseScreen : Screen
    {
        private const int _ITEMS = 5;
        private static Vector2 s_Offset = new Vector2(512-174, 768/2-235);

        private int m_Selected = 0;
        private GameScreen m_GameScreen;
        private Texture2D m_TexBackground;
        private Texture2D m_ResumeGame;
        private Texture2D m_QuitGame;
        
        private Texture2D m_ChangeMap;
        private Texture2D m_Restartwave;
        private Texture2D m_ChangeCharacter;
        private Texture2D m_Tutorial;
        private Texture2D m_Arrow;
        private Vector2 m_arrowOffset = new Vector2(45,5);
		private Texture2D m_TexSnapshot;

        private int m_PlayerPaused = 0;
        private int m_CharacterPaused = 0;

        private InputCommand[] m_Code = { InputCommand.Up, InputCommand.Up, InputCommand.Down, InputCommand.Down, InputCommand.Left, InputCommand.Right, InputCommand.Left, InputCommand.Right, InputCommand.Special3, InputCommand.Attack};
        private int m_CurrentCodeIndex = 0;

        public PauseScreen(GameScreen gameScreen, int player, int character)
        {
            m_GameScreen = gameScreen;
            m_PlayerPaused = player;
            m_CharacterPaused = character;
        }

        public override void Initialize()
        {
            base.Initialize();
			//m_TexSnapshot = Graphics.Snapshot();
            m_TexSnapshot = Graphics.GetTexture("transparent_black");
            m_TexBackground = Graphics.GetTexture("paus_background");
            m_ResumeGame = Graphics.GetTexture("paus_resume_game");
            m_QuitGame = Graphics.GetTexture("paus_quit_game");
            m_ChangeMap = Graphics.GetTexture("paus_change_map");
            m_Restartwave = Graphics.GetTexture("paus_restart");
           
            m_ChangeCharacter = Graphics.GetTexture("paus_change_character");
            m_Tutorial = Graphics.GetTexture("paus_showtutorial");
            m_Arrow = Graphics.GetTexture("gray_arrow");

        }

        public bool PressedWrong()
        {
            if (Input.AnyWasPressed(InputCommand.Attack) && m_Code[m_CurrentCodeIndex] != InputCommand.Attack)
                return true;
            else if (Input.AnyWasPressed(InputCommand.Down) && m_Code[m_CurrentCodeIndex] != InputCommand.Down)
                return true;
            else if (Input.AnyWasPressed(InputCommand.Left) && m_Code[m_CurrentCodeIndex] != InputCommand.Left)
                return true;
            else if (Input.AnyWasPressed(InputCommand.LeftShoulder) && m_Code[m_CurrentCodeIndex] != InputCommand.LeftShoulder)
                return true;
            else if (Input.AnyWasPressed(InputCommand.LeftTrigger) && m_Code[m_CurrentCodeIndex] != InputCommand.LeftTrigger)
                return true;
            else if (Input.AnyWasPressed(InputCommand.MenuBack) && m_Code[m_CurrentCodeIndex] != InputCommand.MenuBack)
                return true;
            else if (Input.AnyWasPressed(InputCommand.MenuSelect) && m_Code[m_CurrentCodeIndex] != InputCommand.MenuSelect)
                return true;
            else if (Input.AnyWasPressed(InputCommand.Right) && m_Code[m_CurrentCodeIndex] != InputCommand.Right)
                return true;
            else if (Input.AnyWasPressed(InputCommand.RightShoulder) && m_Code[m_CurrentCodeIndex] != InputCommand.RightShoulder)
                return true;
            else if (Input.AnyWasPressed(InputCommand.RightTrigger) && m_Code[m_CurrentCodeIndex] != InputCommand.RightTrigger)
                return true;
            else if (Input.AnyWasPressed(InputCommand.Special1) && m_Code[m_CurrentCodeIndex] != InputCommand.Special1)
                return true;
            else if (Input.AnyWasPressed(InputCommand.Special2) && m_Code[m_CurrentCodeIndex] != InputCommand.Special2)
                return true;
            else if (Input.AnyWasPressed(InputCommand.Special3) && m_Code[m_CurrentCodeIndex] != InputCommand.Special3)
                return true;
            else if (Input.AnyWasPressed(InputCommand.Up) && m_Code[m_CurrentCodeIndex] != InputCommand.Up)
                return true;

            return false;
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            m_Selected = GetEntryChanged(m_Selected, _ITEMS, delta);

            if (Input.AnyWasPressed(m_Code[m_CurrentCodeIndex]))
            {
                if (++m_CurrentCodeIndex >= m_Code.Length)
                {
                    GM_FFA game = (GM_FFA)GameMode.Instance;
                    game.LifeCount = 999;
                    m_CurrentCodeIndex = 0;
                }
            }
            else if (PressedWrong())
            {
                m_CurrentCodeIndex = 0;
            }

            if (GetEntrySelected())
            {
                switch(m_Selected)
                {
                case 0:
			        // Back to the game.
			        Exit();
                    break;
                case 1:
			        // Back to map selection.
                    SoundCenter.Instance.PlayMusic(MusicNames.Slugwar);
                    WaveController.ResetWaves();
			        m_GameScreen.Exit();
			        Exit();
                    break;
                case 2:
			        // Back to character selection.
                    SoundCenter.Instance.PlayMusic(MusicNames.Slugwar);
                    WaveController.ResetWaves();
			        ScreenManager.GetInstance().ClearScreens(4);
			        SkylineScreen.Visible = true;
                    break;
                case 3:
                    // Show Tutorial
                    EntityManager.HideEverything();
                    ScreenManager.GetInstance().Add(new TutorialScreen(m_PlayerPaused, m_CharacterPaused));
                    break;
                case 4:
                    // Back to main menu.
                    if (ScreenManager.IsTrial)
                    {
#if XBOX
                        Guide.ShowMarketplace((PlayerIndex)Input.LastSelectID);
#endif
                    }

                    SoundCenter.Instance.PlayMusic(MusicNames.Slugwar);
                    WaveController.ResetWaves();
                    ScreenManager.GetInstance().ClearScreens(1);
                    SkylineScreen.Visible = true;
                    ScreenManager.GetInstance().Add(new MainMenu());
                    break;
		        }
            }
        }

        public override void Draw()
        {
            base.Draw();

            /*m_ResumeGame = Graphics.GetTexture("Resume_game");
            m_QuitGame = Graphics.GetTexture("Quit_game");
            m_ChangeMap = Graphics.GetTexture("Change_map");
            m_ChangeCharacter = Graphics.GetTexture("Change_character");*/

			Graphics.Draw(m_TexSnapshot, Vector2.Zero, null, 0, Color.White);
	        Graphics.Draw(m_TexBackground, s_Offset, null, 0.5f, Color.White);

            if (m_Selected == 0)
            {
                Graphics.Draw(m_ResumeGame, s_Offset + new Vector2(48, 90), 1.0f, null, 10.0f, Color.White);
                Graphics.Draw(m_Arrow, s_Offset + new Vector2(48 - m_arrowOffset.X, 90 - m_arrowOffset.Y), 1.0f, null, 10.0f, Color.White);
            }
            else
                Graphics.Draw(m_ResumeGame, s_Offset + new Vector2(48, 90), 1.0f, null, 10.0f, Color.White);

            if (m_Selected == 1)
            {
                if (GameMode.Instance is GM_Zombie || ScreenManager.IsTrial)
                {
                    Graphics.Draw(m_Restartwave, s_Offset + new Vector2(48, 150), 1.0f, null, 10.0f, Color.White);

                }
                else
                {
                    Graphics.Draw(m_ChangeMap, s_Offset + new Vector2(48, 150), 1.0f, null, 10.0f, Color.White);
                }
                    Graphics.Draw(m_Arrow, s_Offset + new Vector2(48 - m_arrowOffset.X, 150 - m_arrowOffset.Y), 1.0f, null, 10.0f, Color.White);
            }
            else
            {   
                if(GameMode.Instance is GM_Zombie || ScreenManager.IsTrial){
                    Graphics.Draw(m_Restartwave, s_Offset + new Vector2(48, 150), 1.0f, null, 10.0f, Color.White);
                }
                else
                    Graphics.Draw(m_ChangeMap, s_Offset + new Vector2(48, 150), 1.0f, null, 10.0f, Color.White);

            }

            if (m_Selected == 2)
            {
                Graphics.Draw(m_ChangeCharacter, s_Offset + new Vector2(48, 180), 1.0f, null, 10.0f, Color.White);
                Graphics.Draw(m_Arrow, s_Offset + new Vector2(48 - m_arrowOffset.X, 180 - m_arrowOffset.Y), 1.0f, null, 10.0f, Color.White);
            }
            else
                Graphics.Draw(m_ChangeCharacter, s_Offset + new Vector2(48, 180), 1.0f, null, 10.0f, Color.White);

            if (m_Selected == 3)
            {
                Graphics.Draw(m_Tutorial, s_Offset + new Vector2(48, 210), 1.0f, null, 10.0f, Color.White);
                Graphics.Draw(m_Arrow, s_Offset + new Vector2(48 - m_arrowOffset.X, 210 - m_arrowOffset.Y), 1.0f, null, 10.0f, Color.White);
            }
            else
                Graphics.Draw(m_Tutorial, s_Offset + new Vector2(48, 210), 1.0f, null, 10.0f, Color.White);
            
            if (m_Selected == 4)
            {
                Graphics.Draw(m_QuitGame, s_Offset + new Vector2(48, 270), 1.0f, null, 10.0f, Color.White);
                Graphics.Draw(m_Arrow, s_Offset + new Vector2(48 - m_arrowOffset.X, 270 - m_arrowOffset.Y), 1.0f, null, 10.0f, Color.White);
            }
            else
                Graphics.Draw(m_QuitGame, s_Offset + new Vector2(48, 270), 1.0f, null, 10.0f, Color.White);
        }
        public override void Exit()
        {
            EntityManager.ShowHidden();
            base.Exit();
        }
    }
}
