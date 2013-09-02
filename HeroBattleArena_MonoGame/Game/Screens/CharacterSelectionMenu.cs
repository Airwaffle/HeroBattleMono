using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HeroBattleArena.Game.GameObjects;

class PlayerSelection
{
    public int CurrentSelection = -1;
    public int Team = 0;
    public int Activated = 0;
    public int Rect = 0;
    public int Owner = 0;
}

class HeroPortrait
{
    public Texture2D Big = null;
    public Texture2D Small = null;
    public int X = 0;
    public int Y = 0;
}

namespace HeroBattleArena.Game.Screens
{
    public class HeroesToBeCreated
    {
        public int Hero;
        public int Player;
        public int Owner;
        public int Team;

        public HeroesToBeCreated(int hero, int player, int owner, int team)
        {
            this.Hero = hero;
            this.Player = player;
            this.Owner = owner;
            this.Team = team;
        }
    }

    public class CharacterSelectionMenu : Screen
    {
#region Fields
        private const int _NUM_CHARACTERS = 6;

        //private int     m_Selected                  = 0;
        private int     m_MaxSelections             = 2;
        private bool    m_bOptionsDone              = false;
        private int     m_NumPlayers                = 0;
        private int     m_NumPortraits              = 4;
        private float   m_DistBetweenPortraits      = 227;
        private float   m_OrgDistBetweenPortraits   = 227;
        private float   m_MaxDistBetweenPortraits   = 250;
        private float   m_PortOffset                = 0;

        private PlayerSelection[]   m_Players           = new PlayerSelection[4];
        private HeroPortrait[]      m_Portraits         = new HeroPortrait[_NUM_CHARACTERS];
        private Rectangle[]         m_Rects             = new Rectangle[4];
        private bool[]              m_RectsAvailability = new bool[4];
        private bool[]              m_PortraitsVisible  = { true, true, true, true, false, false };

        private Texture2D[]     m_PlayerStats   = new Texture2D[_NUM_CHARACTERS];
        private Texture2D[]     m_TeamTextures  = new Texture2D[2];
        private Texture2D[]     m_SelectBoxes   = new Texture2D[4];
        private Texture2D[]     m_PlayerColors  = new Texture2D[4];
        private Texture2D       m_TexMetalBack;
        private Texture2D[]     m_TexTeamMetalBack = new Texture2D[2];

        private Vector2[]   m_InfoPositions = new Vector2[4];

        private float m_PortraitSpeed;
        private float m_PortraitAcc;
        private float m_PortraitDelay;
        private float m_OrgPortraitDelay;

        private AnimationManager[] m_SelectionAnimations    = new AnimationManager[4];
        private AnimationManager[] m_SelectionMapAnimations = new AnimationManager[_NUM_CHARACTERS];

        private Texture2D[] m_HandGraphics = new Texture2D[4];
        private Vector2[] m_HandPositions = new Vector2[4];
        private AnimationManager[] m_HandAnimations = new AnimationManager[4];
        private int m_HandSpeed = 500;

        private int m_NumbersUnlocked = 4;


#endregion

#region Initialization

        public CharacterSelectionMenu(int firstID)
        {
            for (int i = 0; i < 4; ++i)
            {
                m_Rects[i] = new Rectangle();
                m_Rects[i].Width = 200;
                m_Rects[i].Height = 200;
                m_RectsAvailability[i] = true;

                m_Players[i] = new PlayerSelection();
            }
            for (int i = 0; i < _NUM_CHARACTERS; ++i)
                m_Portraits[i] = new HeroPortrait();

            // Activate first player (The one who entered the screen.)
            m_Players[firstID].Activated = 1;
	        m_Players[firstID].Rect      = 0;

	        m_RectsAvailability[0] = false;
	        m_NumPlayers = 1;

            if (GameMode.Instance is GM_Team)
            {
                m_MaxSelections = 3;
            }
            else
            {
                m_MaxSelections = 2;
            }
            GameScreen.SetTrainingMode();
        }

        public override void Initialize()
        {
            base.Initialize();

            // Setup screen behavior
            ExitOnEscape = false;
            DrawInBackground = true;

            // Setup rects
	        m_Rects[0].X = 38;
	        m_Rects[0].Y = 18;
	        m_Rects[1].X = 1024 - m_Rects[1].Width - 217;
	        m_Rects[1].Y = 18;
	        m_Rects[2].X = 38;
	        m_Rects[2].Y = 768 - m_Rects[2].Height - 68;
	        m_Rects[3].X = 1024 - m_Rects[1].Width - 217;
	        m_Rects[3].Y = 768 - m_Rects[3].Height - 68;

            // ????
            int x = 0;
            int y = 0;
            for (int i = 0; i < 4; i++)
            {
                m_Portraits[i].X = x;
                m_Portraits[i].X = y;
                if (i % 2 == 0)
                {
                    y++;
                    x = 0;
                }
                x++;
            }

            // The metal plate begind selections.
            m_TexMetalBack = Graphics.GetTexture("menu_player_plate");

            // Team texture overlays
            m_TexTeamMetalBack[0] = Graphics.GetTexture("menu_player_blue_plate");
            m_TexTeamMetalBack[1] = Graphics.GetTexture("menu_player_red_plate");

            // Hero statistic textures.
	        m_PlayerStats[0] = Graphics.GetTexture("arthur_stats");
            m_PlayerStats[1] = Graphics.GetTexture("soldier_stats");
            m_PlayerStats[2] = Graphics.GetTexture("aztek_stats");
            m_PlayerStats[3] = Graphics.GetTexture("nano_stats");
            m_PlayerStats[4] = Graphics.GetTexture("zombie_stats");
            m_PlayerStats[5] = Graphics.GetTexture("nano_stats");

            // Hero portraits.
	        m_Portraits[0].Big = Graphics.GetTexture("menu_port_arthur");
	        m_Portraits[0].Small = Graphics.GetTexture("menu_port_sm_arthur");
	        m_Portraits[1].Big = Graphics.GetTexture("menu_port_soldier");
	        m_Portraits[1].Small = Graphics.GetTexture("menu_port_sm_soldier");

            if (ScreenManager.IsTrial)
            {
                m_Portraits[2].Small = Graphics.GetTexture("menu_port_buy");
                m_Portraits[3].Small = Graphics.GetTexture("menu_port_buy");
                m_Portraits[4].Small = Graphics.GetTexture("menu_port_buy");
                m_Portraits[5].Small = Graphics.GetTexture("menu_port_buy");
            }
            else
            {
                m_Portraits[2].Big = Graphics.GetTexture("menu_port_aztec");
                m_Portraits[2].Small = Graphics.GetTexture("menu_port_sm_aztec");
                m_Portraits[3].Big = Graphics.GetTexture("menu_port_mage");
                m_Portraits[3].Small = Graphics.GetTexture("menu_port_sm_mage");
                m_Portraits[4].Big = Graphics.GetTexture("menu_port_unknown");
                m_Portraits[4].Small = Graphics.GetTexture("menu_port_sm_unknown");
                m_Portraits[5].Big = Graphics.GetTexture("menu_port_unknown2");
                m_Portraits[5].Small = Graphics.GetTexture("menu_port_sm_unknown2");

            }

            // Selection boxes.
	        m_SelectBoxes[0] = Graphics.GetTexture("menu_select_player1");
	        m_SelectBoxes[1] = Graphics.GetTexture("menu_select_player2");
	        m_SelectBoxes[2] = Graphics.GetTexture("menu_select_player3");
	        m_SelectBoxes[3] = Graphics.GetTexture("menu_select_player4");

            // Player color indicators.
	        m_PlayerColors[0] = Graphics.GetTexture("Player_1_color");
	        m_PlayerColors[1] = Graphics.GetTexture("Player_2_color");
	        m_PlayerColors[2] = Graphics.GetTexture("Player_3_color");
	        m_PlayerColors[3] = Graphics.GetTexture("Player_4_color");

            m_HandGraphics[0] = Graphics.GetTexture("green_marker");
            m_HandGraphics[1] = Graphics.GetTexture("yellow_marker");
            m_HandGraphics[2] = Graphics.GetTexture("blue_marker");
            m_HandGraphics[3] = Graphics.GetTexture("red_marker");

            m_HandPositions[0] = new Vector2(200, 200);
            m_HandPositions[1] = new Vector2(1024 - 200, 200);
            m_HandPositions[2] = new Vector2(1024 - 200, 768 - 200);
            m_HandPositions[3] = new Vector2(200, 768 - 200);

            for (int i = 0; i < 4; i++)
            {
                m_HandAnimations[i] = new AnimationManager(114, 114, 8, 1);
                m_HandAnimations[i].AddAnimation(new Animation(3, 0, 0, false, 0));
                m_HandAnimations[i].AddAnimation(new Animation(5, 0, 3, true, 0));
                m_HandAnimations[i].AddAnimation(new Animation(1, 0, 0, false, 0));
                m_HandAnimations[i].StandardAnimationSpeed = 8;
            }

            // Positions for the character info texts.
            m_InfoPositions[0] = new Vector2(35, 10);
            m_InfoPositions[1] = new Vector2(600, 10);
            m_InfoPositions[2] = new Vector2(35, 490);
            m_InfoPositions[3] = new Vector2(600, 490);

            // Load config values.
	        m_PortraitSpeed     = Configuration.GetValue("General_Menu_Port_Speed");
	        m_PortraitAcc       = Configuration.GetValue("General_Menu_Port_Acc");
	        m_PortraitDelay     = Configuration.GetValue("General_Menu_Port_Move_Delay");
	        m_OrgPortraitDelay  = Configuration.GetValue("General_Menu_Port_Move_Delay");

            // Setup animations.
            for (int i = 0; i < 4; i++)
            {
                m_SelectionAnimations[i] = new AnimationManager(185, 185, 8, 5);
                m_SelectionAnimations[i].AddAnimation(new Animation(13, 0, 0, false, 0));
                m_SelectionAnimations[i].AddAnimation(new Animation(13, 0, 0, true, 2));
                m_SelectionAnimations[i].AddAnimation(new Animation(21, 2, 0, true, 3));
                m_SelectionAnimations[i].AddAnimation(new Animation(1, 4, 4, false, 0));
                m_SelectionAnimations[i].StandardAnimationSpeed = 14;
            }

            for (int i = 0; i < _NUM_CHARACTERS; i++)
            {
                m_SelectionMapAnimations[i] = new AnimationManager(300, 300, 6, 2);
                m_SelectionMapAnimations[i].AddAnimation(new Animation(1, 0, 0, false, 0));
                m_SelectionMapAnimations[i].AddAnimation(new Animation(11, 0, 0, true, 2));
                m_SelectionMapAnimations[i].AddAnimation(new Animation(1, 1, 4, false, 0));
                m_SelectionMapAnimations[i].StandardAnimationSpeed = 14;
            }	
        }

#endregion

        public override void OnBecomeActive()
        {
            base.OnBecomeActive();

            m_NumPortraits = 5;

            if (ScreenManager.IsTrial)
            {
                m_Portraits[2].Small = Graphics.GetTexture("menu_port_buy");
                m_Portraits[3].Small = Graphics.GetTexture("menu_port_buy");
                m_Portraits[4].Small = Graphics.GetTexture("menu_port_buy");
                m_NumbersUnlocked = 2;
            } else {
                m_Portraits[2].Small = Graphics.GetTexture("menu_port_sm_aztec");
                m_Portraits[3].Small = Graphics.GetTexture("menu_port_sm_mage");
                if (EntityManager.GetUnlockedChar(0))
                {
                    m_Portraits[4].Small = Graphics.GetTexture("menu_port_sm_unknown");
                    m_NumbersUnlocked = 5;
                }
                else
                {
                    m_Portraits[4].Small = Graphics.GetTexture("menu_port_locked");
                    m_NumbersUnlocked = 4;
                }
            }

            if (GameMode.Instance is GM_Team)
            {
                m_MaxSelections = 3;
            }
            else
            {
                m_MaxSelections = 2;
            }

            m_DistBetweenPortraits = (m_OrgDistBetweenPortraits / m_NumPortraits) * 4;

            GameScreen.SetTrainingMode();

        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (m_bOptionsDone)
            {
                MovePortraits(delta);
                for (int i = 0; i < 4; ++i)
                {
                    m_SelectionMapAnimations[i].Update(delta);
                }
                return;
            }

	        // Checks if all the player has selected their hero and somebody is pressing start
	        if(Input.AnyWasPressed(InputCommand.MenuSelect))
	        {
				SoundCenter.Instance.Play(SoundNames.InterfaceClickConfirm);

		        int done = 0;
		        for (int i = 0; i < 4; i++)
                {
			        if (m_Players[i].Activated >= m_MaxSelections/* && m_SelectionAnimations[i].GetCurrentAnimation() == 3*/)
                    {
				        ++done;
			        }
		        }
		
		        bool team1 = false;
		        bool team2 = false;
		        bool differentTeams = false;
		        if (GameMode.Instance.TeamGame)
                {
			        for (int i = 0; i < 4; ++i)
                    {
				        if (m_Players[i].Activated >= m_MaxSelections && m_Players[i].Team == 0)
                        {
					        team1 = true;
				        } 
                        else if (m_Players[i].Activated >= m_MaxSelections && m_Players[i].Team == 1)
                        {
					        team2 = true;
				        }
			        }
			        if (team1 && team2)
                    {
				        differentTeams = true;
			        }
		        } 
                else 
                {
			        differentTeams = true;
		        }

		        for (int i = 0; i < 4; i++)
                {
                    if (Input.GetPlayerState(i).WasPressed(InputCommand.MenuSelect) || Input.GetPlayerState(i).WasPressed(InputCommand.Attack))
                    {


                        if (m_Players[i].Activated <= 0)
                        {
                            ++m_Players[i].Activated;
                            for (int j = 0; j < 4; j++)
                            {
                                if (m_RectsAvailability[j])
                                {
                                    m_Players[i].Rect = j;
                                    m_RectsAvailability[j] = false;
                                    ++m_NumPlayers;
                                    break;
                                }
                            }
                        }
                        else if (m_Players[i].Activated == 1)
                        {
                            m_HandAnimations[m_Players[i].Rect].ChangeAnimation(1);
                            if (m_Players[i].CurrentSelection != -1)
                            {
                                ++m_Players[i].Activated;
                            }
                        }
                        else if (m_Players[i].Activated == 2 && m_MaxSelections == 3)
                        {
                            ++m_Players[i].Activated;
                        }
                        else if (done >= m_NumPlayers && m_NumPlayers > 1 && differentTeams || done >= m_NumPlayers && m_NumPlayers == 1 && GameMode.Instance is GM_Zombie)
                        {
                            if (Configuration.GetValue("Debug_Skip_Screens") == 0)
                            {
                                DrawInBackground = true;
                                ScreenManager.GetInstance().Add(new GameOptionsMenu());
                                m_bOptionsDone = true;
                                for (int g = 0; g < 4; ++g)
                                {
                                    m_SelectionMapAnimations[g].ChangeAnimation(1);
                                }
                            }
                            else
                            {
                                MovePortraits(delta);
                            }
                            return;
                        }

                        /*
                        
				        if (m_Players[i].Activated <= 0)
                        {
 					        ++m_Players[i].Activated;
					        for (int j = 0; j < 4; j++)
                            {
						        if (m_RectsAvailability[j])
                                {
							        m_Players[i].Rect = j;
							        m_RectsAvailability[j] = false;
							        ++m_NumPlayers;
							        break;
						        }
					        }
				        } 
                        else if (m_Players[i].Activated == 1)
                        {
					        ++m_Players[i].Activated;
					        m_SelectionAnimations[i].ChangeAnimation(2);
				        } 
                        else if (m_Players[i].Activated == 2 && m_MaxSelections == 3)
                        {
					        ++m_Players[i].Activated;
				        }
                        else if (done >= m_NumPlayers && m_NumPlayers > 1 && differentTeams || done >= m_NumPlayers && m_NumPlayers == 1 && GameMode.Instance is GM_Zombie)
                        {
					        if (Configuration.GetValue("Debug_Skip_Screens") == 0)
                            {
						        DrawInBackground = true;
                                ScreenManager.GetInstance().Add(new GameOptionsMenu());
						        m_bOptionsDone = true;
 						        for(int g = 0; g < 4; ++g)
                                {
							        m_SelectionMapAnimations[g].ChangeAnimation(1);
						        }
					        } 
                            else
                            {
						        MovePortraits(delta);
					        }
					        return;
				        }*/
			        }
                    
		        }
	        }

            if (Input.AnyWasPressed(InputCommand.MenuBack) || Input.AnyWasPressed(InputCommand.Special3))
            {
				SoundCenter.Instance.Play(SoundNames.InterfaceClickBack);

		        int exit = 0;
		        for (int i = 0; i < 4; i++)
                {
			        if (m_Players[i].Activated <= 0)
                    {
				        ++exit;
			        }
		        }

		        for (int i = 0; i < 4; i++)
                {
			        if (Input.GetPlayerState(i).WasPressed(InputCommand.MenuBack))
                    {
				        if (m_Players[i].Activated > 0)
                        {
					        --m_Players[i].Activated;
					        if (m_Players[i].Activated == 1)
                            {
						        m_SelectionAnimations[i].ChangeAnimation(0);
					        }
					        if (m_Players[i].Activated == 0)
                            {
						        m_RectsAvailability[m_Players[i].Rect] = true;
						        --m_NumPlayers;
					        }
				        } 
                        else 
                        {
					        if (exit >= 4)
                            {
						        Exit();
					        }
				        }
			        }
		        }
	        }

	        for (int i = 0; i < 4; i++)
            {
		        if (m_Players[i].Activated == 1)
                {
                    Vector2 handDir = new Vector2(0,0);
                    if (Input.GetPlayerState(i).IsPressed(InputCommand.Up) && m_HandPositions[m_Players[i].Rect].Y > 0)
                    {
                        handDir.Y--;
			        }
                    if (Input.GetPlayerState(i).IsPressed(InputCommand.Down) && m_HandPositions[m_Players[i].Rect].Y < 768 - 114)
                    {
                        handDir.Y++;
			        }
                    if (Input.GetPlayerState(i).IsPressed(InputCommand.Right) && m_HandPositions[m_Players[i].Rect].X < 1024 - 114)
                    {
                        handDir.X++;
			        }
                    if (Input.GetPlayerState(i).IsPressed(InputCommand.Left) && m_HandPositions[m_Players[i].Rect].X > 0)
                    {
                        handDir.X--;
			        }
                    if (!(handDir.X == 0 && handDir.Y == 0))
                        handDir.Normalize();
                    if (m_HandAnimations[m_Players[i].Rect].GetCurrentAnimation() != 1)
                        m_HandPositions[m_Players[i].Rect] += handDir * m_HandSpeed * delta;

                    if (m_HandPositions[m_Players[i].Rect].X > 20 && m_HandPositions[m_Players[i].Rect].X < 918 && m_HandPositions[m_Players[i].Rect].Y > 290 && m_HandPositions[m_Players[i].Rect].Y < 445)
                    {
                        int selected = (int)Math.Floor(m_HandPositions[m_Players[i].Rect].X / m_DistBetweenPortraits);
                        if (selected >= m_NumbersUnlocked) selected = -1;
                        m_Players[i].CurrentSelection = selected;
                    }
                    else
                    {
                        m_Players[i].CurrentSelection = -1;
                    }
		        } 
                else if (m_Players[i].Activated == 2 && GameMode.Instance is GM_Team)
                {
			        if (Input.GetPlayerState(i).WasPressed(InputCommand.Up))
                    {
						SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);

                        ++m_Players[i].Team;
				        m_Players[i].Team = m_Players[i].Team % 2;
			        }
			        if (Input.GetPlayerState(i).WasPressed(InputCommand.Down))
                    {
						SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);

                        --m_Players[i].Team;
				        m_Players[i].Team = m_Players[i].Team % 2;
				        if (m_Players[i].Team < 0)
                        {
					        m_Players[i].Team += 2;
				        }
			        }
			        if (Input.GetPlayerState(i).WasPressed(InputCommand.Right))
                    {
						SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);

                        ++m_Players[i].Team;
				        m_Players[i].Team = m_Players[i].Team % 2;
			        }
			        if (Input.GetPlayerState(i).WasPressed(InputCommand.Left))
                    {
						SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);

                        --m_Players[i].Team;
				        m_Players[i].Team = m_Players[i].Team % 2;
				        if (m_Players[i].Team < 0)
                        {
					        m_Players[i].Team += 2;
				        }
			        }
		        }
	        }
	        for (int i = 0; i < 4; i++)
            {
		        //m_SelectionAnimations[i].Update(delta);
                m_HandAnimations[i].Update(delta);
	        }
        }

        /// <summary>
        /// Draw the character selection menu...
        /// </summary>
        public override void Draw()
        {
            base.Draw();

	        for (int i = 0; i < 4; ++i)
            {
                // ????
		        //Rectangle metalRect(s_rects[i].x  - 8, s_rects[i].y - 8, s_rects[i].w, s_rects[i].h);
                Graphics.Draw(m_TexMetalBack, new Vector2(m_Rects[i].X, m_Rects[i].Y), null, 0.1f, Color.White);
	        }

	        for (int i = 0; i < 4; ++i)
            {
		        if (m_Players[i].Activated != 0 && m_Players[i].Activated < 4) 
		        {
                    Color selectedColor = Color.White;
                    if (m_Players[i].Activated >= 2)
                    {
                        selectedColor = Color.DarkGray;
                    }
                    if (m_Players[i].CurrentSelection != -1){
                        
                        // Draw portrait.
                        Graphics.Draw(
                            m_Portraits[m_Players[i].CurrentSelection].Big, 
                            new Vector2(m_Rects[m_Players[i].Rect].X + 10, m_Rects[m_Players[i].Rect].Y + 15),
                            null, 0.3f, selectedColor);
                        // Draw character info.
                        Graphics.Draw(
                            m_PlayerStats[m_Players[i].CurrentSelection],
                            new Vector2(m_InfoPositions[m_Players[i].Rect].X, m_InfoPositions[m_Players[i].Rect].Y + 5),
                            null, 0.2f, selectedColor);
                    }
                    // Draw color indicator.
                    Graphics.Draw(
                        m_PlayerColors[m_Players[i].Rect],
                        m_InfoPositions[m_Players[i].Rect] + new Vector2(350, 0),
                        null, 0.2f, Color.White);

			        if (GameMode.Instance is GM_Team)
				    {
                        if (m_Players[i].Activated < 3)
                        {
                            selectedColor = Color.White;
                        }
                        // Draw team indicator.
                        Graphics.Draw(m_TexTeamMetalBack[m_Players[i].Team], new Vector2(m_Rects[m_Players[i].Rect].X, m_Rects[m_Players[i].Rect].Y),
                            null, 0.1f, selectedColor);
				    } 
				    else 
				    {
                        Graphics.Draw(m_TexMetalBack, new Vector2(m_Rects[m_Players[i].Rect].X, m_Rects[m_Players[i].Rect].Y),
                            null, 0.1f, selectedColor);
				    }
		        }
	        }

	        for (int i = 0; i < m_NumPortraits; ++i)
            {
                // Draw available characters.
                //if (i==4 && EntityManager.GetUnlockedChar()
                Graphics.Draw(
                    m_Portraits[i].Small,
                    new Vector2(i * m_DistBetweenPortraits + 20 - m_PortOffset, 264 - 30),
                    m_SelectionMapAnimations[i].Rectangle,
                    0.5f, Color.White);
		        
                if (!m_bOptionsDone)
                {
			        for (int h = 0; h < 4; ++h)
                    {
                        
				        if (m_Players[h].Activated != 0)
                        {
                            Graphics.Draw(m_HandGraphics[m_Players[h].Rect], m_HandPositions[m_Players[h].Rect], m_HandAnimations[m_Players[h].Rect].Rectangle, 50, Color.White);

					        /*if (i == m_Players[h].CurrentSelection)
                            {
                                // Draw something...
                                Graphics.Draw(
                                    m_SelectBoxes[m_Players[h].Rect],
                                    new Vector2(i * m_DistBetweenPortraits + 110 - 70 + 40, 300 - 30 + 25),
                                    m_SelectionAnimations[h].Rectangle,
                                    0.6f, Color.White);
					        }*/
				        }
			        }
		        }
	        }
        }

        /// <summary>
        /// We are done with the selecting...
        /// </summary>
        private void DoneSelecting()
        {
            List<HeroesToBeCreated> uncreatedHeroes = new List<HeroesToBeCreated>();

	        /*for (int i = 0; i < 4; i++)
	        {
		        if (m_Players[i].activated >= 2){
			        m_Players[i].owner = i;
		        }
	        }	
	
	        for (int i = 0; i < 4; i++)
	        {
		        if (m_Players[i].activated >= 2){
			        uncreatedHeroes.push_back(new HeroesToBeCreated(m_Players[i].currentSelected, i, i, m_Players[i].team + 1));
		        }
	        }*/

	        for (int i = 0; i < 4; i++)
	        {	
		        for (int k = 0; k < 4; k++)
                {
			        if (m_Players[k].Activated >= 2 && m_Players[k].Rect == i)
                    {
				        uncreatedHeroes.Add(new HeroesToBeCreated(
                            m_Players[k].CurrentSelection, i, k, 
                            m_Players[k].Team + 1));
			        }
		        }
	        }

	        // Nollställer stuffs ifall man skulle vilja backa
	        for (int i = 0; i < 4; i++)
	        {
		        if (m_Players[i].Activated >= 2)
                {
			        m_Players[i].Activated = 1;
		        }
		        m_SelectionAnimations[i].ChangeAnimation(0);
		        m_SelectionMapAnimations[i].ChangeAnimation(0);
	        }
	        m_PortraitDelay = Configuration.GetValue("General_Menu_Port_Move_Delay");
	        m_PortraitSpeed = Configuration.GetValue("General_Menu_Port_Speed");
	        m_bOptionsDone = false;

            // Setup rects
	        m_Rects[0].X = 38;
	        m_Rects[0].Y = 18;
	        m_Rects[1].X = 1024 - m_Rects[1].Width - 217;
	        m_Rects[1].Y = 18;
	        m_Rects[2].X = 38;
	        m_Rects[2].Y = 768 - m_Rects[2].Height - 68;
	        m_Rects[3].X = 1024 - m_Rects[1].Width - 217;
	        m_Rects[3].Y = 768 - m_Rects[3].Height - 68;

            // Positions for the character info texts.
            m_InfoPositions[0] = new Vector2(35, 10);
            m_InfoPositions[1] = new Vector2(600, 10);
            m_InfoPositions[2] = new Vector2(35, 490);
            m_InfoPositions[3] = new Vector2(600, 490);

	        m_DistBetweenPortraits = (m_OrgDistBetweenPortraits/m_NumPortraits) * 4;
	        m_PortOffset = 0;
	        //exitScreen();
	        DrawInBackground = false;

	        ScreenManager.GetInstance().Add(new MapSelectionMenu(uncreatedHeroes));
        }

        /// <summary>
        /// Moves the portraits.
        /// </summary>
        /// <param name="delta">Time elapsed in seconds since last frame.</param>
        private void MovePortraits(float delta)
        {
            m_DistBetweenPortraits = m_OrgDistBetweenPortraits + (m_MaxDistBetweenPortraits - m_OrgDistBetweenPortraits) / (m_OrgPortraitDelay - m_PortraitDelay);
            m_PortOffset = 30 * (m_OrgPortraitDelay - m_PortraitDelay);
            for (int i = 0; i < 4; i++)
            {
                if (i % 2 == 0)
                {
                    m_Rects[i].X -= (int)(m_PortraitSpeed * delta);
                    m_InfoPositions[i].X -= (int)(m_PortraitSpeed * delta);
                }
                else
                {
                    // Same as above...
                    m_Rects[i].X += (int)(m_PortraitSpeed * delta);
                    m_InfoPositions[i].X += (int)(m_PortraitSpeed * delta);
                }
            }
            m_PortraitSpeed += m_PortraitAcc * delta;
            m_PortraitDelay -= delta;
            if (m_PortraitDelay <= 0 || ScreenManager.IsTrial)
            {
                DoneSelecting();
            }
        }
    }
}
