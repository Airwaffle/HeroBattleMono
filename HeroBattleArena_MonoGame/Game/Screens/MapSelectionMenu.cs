using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HeroBattleArena.Game.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.Screens
{
    public class MapSelectionMenu : Screen
    {
        private static readonly float[] _OFFSETS = { 290, 200, 260, 290 };
        private const float _OFFSET_Y = 120;

        private int m_Selected = 0;

        private List<HeroesToBeCreated> m_CurrentHeroes;
        private AnimationManager m_Selection;

        private Texture2D[] m_TexLevels = new Texture2D[4];
	    private Texture2D[] m_TexLevelGraphics = new Texture2D[4];
        private Texture2D m_TexSelectGraphic;
	        

        public MapSelectionMenu(List<HeroesToBeCreated> uncreatedHeroes)
        {
            m_CurrentHeroes = uncreatedHeroes;
        }

        public override void Initialize()
        {
            base.Initialize();

            m_Selection = new AnimationManager(185, 185, 8, 2);
	        m_Selection.AddAnimation(new Animation(13, 0, 0, false, 0));
	        m_Selection.StandardAnimationSpeed = 14;

            m_TexLevels[0] = Graphics.GetTexture("camelot_square_map");
            m_TexLevels[1] = Graphics.GetTexture("checkpoint_map");
            m_TexLevels[2] = Graphics.GetTexture("xclass_cruiser_map");
	        m_TexLevels[3] = Graphics.GetTexture("sacred_chamber_map");

            m_TexSelectGraphic = Graphics.GetTexture("menu_select_player1");

            m_TexLevelGraphics[0] = Graphics.GetTexture("menu_port_sm_arthur");
            m_TexLevelGraphics[1] = Graphics.GetTexture("menu_port_sm_soldier");
            m_TexLevelGraphics[2] = Graphics.GetTexture("menu_port_sm_aztec");
            m_TexLevelGraphics[3] = Graphics.GetTexture("menu_port_sm_mage"); 
        }

        public override void Update(float delta)
        {
            List<Map> maps = Map.Maps;

	        if (Input.GetPlayerState(m_CurrentHeroes[0].Owner).WasPressed(InputCommand.Down) || 
                Input.GetPlayerState(m_CurrentHeroes[0].Owner).WasPressed(InputCommand.Right)) 
            {
				SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);

		        ++m_Selected;
		        if(m_Selected >= maps.Count)
                {
			        m_Selected = 0;
		        }
	        }
	        if (Input.GetPlayerState(m_CurrentHeroes[0].Owner).WasPressed(InputCommand.Up) || 
                Input.GetPlayerState(m_CurrentHeroes[0].Owner).WasPressed(InputCommand.Left)) 
            {
				SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);

		        --m_Selected;
		        if(m_Selected < 0) 
                {
			        m_Selected = maps.Count - 1;
		        }
	        }

            if (Input.AnyWasPressed(InputCommand.MenuSelect) || GameMode.Instance is GM_Zombie || ScreenManager.IsTrial)
	        {
                if (!ScreenManager.IsTrial)
				    SoundCenter.Instance.Play(SoundNames.InterfaceClickConfirm);

                EntityManager.Clear();

		        int arthurs = 0;
		        int soldiers = 0;
		        int aztecs = 0;
		        int mages = 0;

		        for (int i = 0; i < m_CurrentHeroes.Count; ++i)
                {
			        Hero hero = new Hero();

			        if (m_CurrentHeroes[i].Hero == 0)
                    {
				        hero = new Arthur();
				        if(GameMode.Instance.TeamGame)
                        {
					        if (m_CurrentHeroes[i].Team == 1)
                                hero.HeroColor = 0; //Blue
                            else 
                                hero.HeroColor = 2; //Red
				        } 
                        else 
					        hero.HeroColor = arthurs;
                        // Give different arthurs different colors...
				        ++arthurs;
			        } 
                    else if (m_CurrentHeroes[i].Hero == 1)
			        {
				        hero = new Soldier();
				        if(GameMode.Instance.TeamGame)
                        {
					        if (m_CurrentHeroes[i].Team == 1)
                                hero.HeroColor = 0; //Blue
                            else 
                                hero.HeroColor = 3; //Red
				        } 
                        else 
					        hero.HeroColor = soldiers;
                        // Give different soldiers different colors...
				        ++soldiers;
			        } 
                    else if (m_CurrentHeroes[i].Hero == 2)
			        {
				        hero = new Aztec();
				        if(GameMode.Instance.TeamGame)
                        {
					        if (m_CurrentHeroes[i].Team == 1)
                                hero.HeroColor = 2; //Blue
                            else
                                hero.HeroColor = 0; //Red
				        } 
                        else 
                            hero.HeroColor = aztecs;
                        // Give different aztecs different colors...
				        ++aztecs;
			        } 
                    else if (m_CurrentHeroes[i].Hero == 3)
			        {
				        hero = new Mage();
                        if(GameMode.Instance.TeamGame)
                        {
					        if (m_CurrentHeroes[i].Team == 1)
                                hero.HeroColor = 0; // Blue
                            else
                                hero.HeroColor = 3; // Red
				        } 
                        else 
                            hero.HeroColor = mages;
                        // Give different mages different colors...
				        ++mages;
			        } 
                    else if (m_CurrentHeroes[i].Hero == 4)
                    {
				        hero = new ZombieHero();
			        }

                    hero.PlayerOwner    = m_CurrentHeroes[i].Owner;
                    hero.Team           = m_CurrentHeroes[i].Team;
                    //GameMode.Instance.PlayerInfos[i].Player = i;
                    EntityManager.Spawn(hero);
		        }

		        SkylineScreen.Visible = false;
		        ScreenManager.GetInstance().Add(new GameScreen(maps[m_Selected]));
                EntityManager.SortHeroesAfterOwner();
                if (GameMode.Instance is GM_Zombie)
                {
                    List<Hero> heroes = EntityManager.Heroes;
                    foreach (Hero hero in heroes)
                        hero.Team = 1;
                    ScreenManager.GetInstance().Add(new MovieScreen(0));
                }
                
               
	        } 
            else if(Input.AnyWasPressed(InputCommand.MenuBack))
	        {
				SoundCenter.Instance.Play(SoundNames.InterfaceClickBack);

		        Exit();
	        }
            m_Selection.Update(delta);
        }

        public override void Draw()
        {
            for (int i = 0; i < 4; ++i)
            {
		        Graphics.Draw(m_TexLevelGraphics[i], 
                    new Vector2(i*250 + 20 - 30, 264 - 30), 
                    new Rectangle(300*4, 300, 300, 300), 5, Color.White);

		        if (i == m_Selected)
                {
			        Graphics.Draw(m_TexSelectGraphic, 
                        new Vector2(i*250 + 110 - 70 + 40 - 30, 300 - 30 + 25), 
                        m_Selection.Rectangle, 6, Color.White);
		        }
	        }
#warning Do we really need a loop here?
	        for(int i =0; i<4; i++)
	        {
		        if(i==m_Selected)
		        {
                    // Draw the current levels title.
                    Graphics.Draw(m_TexLevels[m_Selected],
                        new Vector2(512 - _OFFSETS[i], _OFFSET_Y),
                        null, 5, Color.White);
		        }
	        }
        }

        public override void OnBecomeActive()
        {
            base.OnBecomeActive();

            GameMode.Instance.PrepareGame();
            SkylineScreen.Visible = true;

	        for (int i = 0; i < m_CurrentHeroes.Count; ++i)
	        {
		        //m_CurrentHeroes[i]->
	        }
        }
    }
}
