using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.Screens
{
    public class GameOptionsMenu : Screen
    {
        private static int s_DistBetweenOptions     = 26;
        private static int s_DistBetweenNumbers     = 16;
        private static Vector2 s_Offset = new Vector2(512 - 170, 300);

	    private int m_Selected      = 4;
        private int m_NumMenuItems  = 0;
	    private int m_CurrentMode   = 0;

	    private int m_CurrentLives      = 0;
	    private int m_MaxLives          = 0;
	    private int m_CurrentFrags      = 0;
	    private int m_MaxFrags          = 0;
	    private int m_FragsDifference   = 0;
	    private bool m_FriendlyFire     = false;
        /**/
        private bool m_Powerup          = true;
	    /**/
        private bool m_SeparateLives    = true;

        private Texture2D[] m_TexNumbers       = new Texture2D[10];
        private Texture2D[] m_TexFragSurvivor  = new Texture2D[2];
        private Texture2D[] m_TexOnOff         = new Texture2D[2];
        private Texture2D   m_TexFriendlyFire;
        private Texture2D   m_TexDone;
        private Texture2D   m_TexGameOptions;
        private Texture2D   m_TexSeparateLives;
        private Texture2D   m_TexBackground;
        private Texture2D   m_TexArrow;
        /**/
        private Texture2D   m_TexPowerup;
        /**/
	    public GameOptionsMenu() { }

	    public override void Initialize()
        {
            this.ExitOnEscape = false;

	        m_CurrentMode = (int)Configuration.GetValue("Options_Start_Mode");
            m_CurrentLives = (int)Configuration.GetValue("Options_Start_Lives");
	        m_MaxLives = (int)Configuration.GetValue("Options_Max_Lives");
            m_CurrentFrags = (int)Configuration.GetValue("Options_Start_Frags");
	        m_MaxFrags = (int)Configuration.GetValue("Options_Max_Frags");
	        m_FragsDifference = (int)Configuration.GetValue("Options_FragsDifference");
	        m_FriendlyFire = Configuration.GetValue("Options_Start_Friendly_Fire") > 0;
            /**/
            m_Powerup = Configuration.GetValue("Options_Start_Powerups_New") > 0;
	        /**/
            m_NumMenuItems = 5;
	        if(GameMode.Instance.TeamGame)
		        m_FriendlyFire = false;
            //if(GameMode.Instance is GM_Team)
            //{
            //    m_SeparateLives = ((GM_Team)GameMode.Instance).SeparateLives;
            //    m_NumMenuItems++;
            //}

            

            m_TexBackground = Graphics.GetTexture("game_options_background");
	        m_TexArrow      = Graphics.GetTexture("gray_arrow");

            for (int i = 0; i < 10; ++i)
                m_TexNumbers[i] = Graphics.GetTexture("gray_nr"+i);

	        m_TexFragSurvivor[0]    = Graphics.GetTexture("gray_frag");
	        m_TexFragSurvivor[1]    = Graphics.GetTexture("gray_survivor");
	        m_TexOnOff[0]           = Graphics.GetTexture("gray_on");
	        m_TexOnOff[1]           = Graphics.GetTexture("gray_off");
            m_TexFriendlyFire       = Graphics.GetTexture("gray_friendlyfire");
            /**/
            m_TexPowerup = Graphics.GetTexture("gray_powerups");
            /**/
            m_TexDone               = Graphics.GetTexture("gray_done");
            m_TexGameOptions        = Graphics.GetTexture("gray_gameoptions");
            m_TexSeparateLives      = Graphics.GetTexture("gray_separatelives");

        }

	    public override void Update(float delta)
        {
	        // Select next menu item.
            m_Selected = this.GetEntryChanged(m_Selected, m_NumMenuItems, delta);

	        // Input to menu item: left
	        if(Input.AnyWasPressed(InputCommand.Left))
	        {
				SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);
		
                switch(m_Selected)
                {
                    case 0:
                        m_FriendlyFire = !m_FriendlyFire;
                        break;
                    case 1:
                        m_Powerup = !m_Powerup;
                        break;
                    case 2:
                        if (!GameMode.Instance.TeamGame)
                        {
                            ++m_CurrentMode;
                            m_CurrentMode = m_CurrentMode % 2;
                        }
                        else
                        {
                            m_CurrentMode = 0;
                        }
                        break;
                    case 3:
                        if (m_CurrentMode == 0)
                        {
				            if (m_CurrentLives - 1 > 0)
                            {
					            --m_CurrentLives;
				            }
			            } 
                        else if (m_CurrentMode == 1)
                        {
				            if (m_CurrentFrags - m_FragsDifference > 0)
                            {
					            m_CurrentFrags -= m_FragsDifference;
				            }
			            }
                        break;
                    case 4:
                        if(GameMode.Instance.TeamGame)
                            m_SeparateLives = !m_SeparateLives;
                        break;
                }
	        }
	        // Input to menu item: right
 	        if(Input.AnyWasPressed(InputCommand.Right))
	        {
				SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);
                switch(m_Selected)
                {
                    case 0:
                        m_FriendlyFire = !m_FriendlyFire;
                        break;
                    case 1:
                        m_Powerup = !m_Powerup;
                        break;
                    case 2:
                        if (!GameMode.Instance.TeamGame)
                        {
                            ++m_CurrentMode;
                            m_CurrentMode = m_CurrentMode % 2;
                        }
                        else
                        {
                            m_CurrentMode = 0;
                        }
                        break;
                    case 3:
                        if (m_CurrentMode == 0)
                        {
				            if (m_CurrentLives + 1 <= m_MaxLives)
                            {
					            m_CurrentLives ++;
				            }
			            } 
                        else if (m_CurrentMode == 1)
                        {
				            if (m_CurrentFrags + m_FragsDifference <= m_MaxFrags)
                            {
					            m_CurrentFrags += m_FragsDifference;
				            }
			            }
                        break;
                    //case 4:
                    //    if(GameMode.Instance.TeamGame)
                    //        m_SeparateLives = !m_SeparateLives;
                    //    break;
                }
	        }

            if (this.GetEntrySelected() || GameMode.Instance is GM_Zombie || ScreenManager.IsTrial)
	        {
		        if(GameMode.Instance is GM_Team) 
                {
			        GM_Team game = (GM_Team)GameMode.Instance;

			        if(m_CurrentMode == 0)
                    {
                        game.LifeCount = m_CurrentLives;
                        game.FragLimit = 0;
			        } 
                    else 
                    {
                        game.LifeCount = 0;
                        game.FragLimit = m_CurrentFrags;
			        }
                    //game.SeparateLives = m_SeparateLives;
		        } 
                else if(GameMode.Instance is GM_FFA)
                {
			        GM_FFA game = (GM_FFA)GameMode.Instance;

			        if(m_CurrentMode == 0)
                    {
                        game.LifeCount = m_CurrentLives;
                        game.FragLimit = 0;
                        
			        } 
                    else 
                    {
                        game.LifeCount = 0;
				        game.FragLimit = m_CurrentFrags;
			        }

                }
                else if (GameMode.Instance is GM_Zombie)
                {
                    GM_Zombie game = (GM_Zombie)GameMode.Instance;
                    game.LifeCount = 0;
                    game.FragLimit = m_CurrentFrags;
                } 
		        GameMode.Instance.FriendlyFire = m_FriendlyFire;
                Exit();
	        }
        }

	    public override void Draw()
        {
            // Draw the option menu background.
	        Graphics.Draw(m_TexBackground, s_Offset, null, 0, Color.White);

            // Draw the game options label.
	        Graphics.Draw(m_TexGameOptions, 
                new Vector2(62+ s_Offset.X, 30 + s_Offset.Y), 
                null, 1, Color.White);

            // Draw the selection arrow.
	        Graphics.Draw(m_TexArrow, 
                new Vector2(
                    48 + s_Offset.X - 40 - 5, 
                    30 + s_DistBetweenOptions * (m_Selected + 1) + s_Offset.Y - 4), 
                null, 1, Color.White);

            // Draw friendly fire label.
	        Graphics.Draw(m_TexFriendlyFire, 
                new Vector2(48 + s_Offset.X, 30 + s_DistBetweenOptions + s_Offset.Y),
                null, 1, Color.White);

            // Draw on/off.
	        if (!m_FriendlyFire)
            {
		        Graphics.Draw(m_TexOnOff[1], 
                    new Vector2(254 + s_Offset.X, 30 + s_DistBetweenOptions + s_Offset.Y),
                    null, 1, Color.White);
	        } 
            else 
            {
		        Graphics.Draw(m_TexOnOff[0], 
                    new Vector2(254 + s_Offset.X, 30 + s_DistBetweenOptions + s_Offset.Y),
                    null, 1, Color.White);
	        }
	
            //Draw powerup label.
            Graphics.Draw(m_TexPowerup,
                new Vector2(48 + s_Offset.X, 55 + s_DistBetweenOptions + s_Offset.Y),
                null, 1, Color.White);

            // Draw on/off.
            if (!m_Powerup)
            {
                Graphics.Draw(m_TexOnOff[0],
                    new Vector2(254 + s_Offset.X, 55 + s_DistBetweenOptions + s_Offset.Y),
                    null, 1, Color.White);
                GameMode.Instance.UsePowerups = true;
                
            }
            else
            {
                Graphics.Draw(m_TexOnOff[1],
                    new Vector2(254 + s_Offset.X, 55 + s_DistBetweenOptions + s_Offset.Y),
                    null, 1, Color.White);
                GameMode.Instance.UsePowerups = false;
                
            }



	        if (m_CurrentMode == 0)
            {
		        Graphics.Draw(m_TexFragSurvivor[0], 
                    new Vector2(48 + s_Offset.X, 55 + s_DistBetweenOptions * 2 + s_Offset.Y),
                    null, 1, Color.White);
	        } 
            else 
            {
		        Graphics.Draw(m_TexFragSurvivor[1], 
                    new Vector2(48 + s_Offset.X, 55 + s_DistBetweenOptions * 2 + s_Offset.Y),
                    null, 1, Color.White);
	        }
	
	        string numberString = "";
	        if (m_CurrentMode == 0)
		        numberString = m_CurrentLives.ToString();
            else 
		        numberString = m_CurrentFrags.ToString();
            // Draw digits.
	        for (int i = 0; i < numberString.Length; ++i)
            {
		        int digit = int.Parse(numberString[i].ToString());
		        Graphics.Draw(m_TexNumbers[digit], 
                    new Vector2(
                        48 + s_Offset.X + s_DistBetweenNumbers * i, 
                        55 + s_DistBetweenOptions * 3 + s_Offset.Y),
                    null, 1, Color.White);
	        }

	
            //if(GameMode.Instance is GM_Team) 
            //{
            //    Graphics.Draw(m_TexSeparateLives, 
            //        new Vector2(48 + s_Offset.X, 30 + s_DistBetweenOptions * 4 + s_Offset.Y),
            //        null, 1, Color.White);
		        
            //    if(m_SeparateLives)
            //    {
            //        Graphics.Draw(m_TexOnOff[0], 
            //            new Vector2(48 + s_Offset.X + 225, 30 + s_DistBetweenOptions * 4 + s_Offset.Y),
            //            null, 1, Color.White);
            //    } 
            //    else 
            //    {
            //        Graphics.Draw(m_TexOnOff[1], 
            //            new Vector2(48 + s_Offset.X + 225, 30 + s_DistBetweenOptions * 4 + s_Offset.Y),
            //            null, 1, Color.White);

            //        Graphics.Draw(m_TexDone, 
            //            new Vector2(48 + s_Offset.X, 55 + s_DistBetweenOptions * 5 + s_Offset.Y),
            //            null, 1, Color.White);
            //    }
            //} 
            
            
              Graphics.Draw(m_TexDone, new Vector2(48 + s_Offset.X, 55 + s_DistBetweenOptions * 4 + s_Offset.Y), null, 1, Color.White);
            
        }
    }
}
