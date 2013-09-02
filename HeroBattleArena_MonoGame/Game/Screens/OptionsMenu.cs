using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.Screens
{
	public class OptionsMenu : Screen
	{
        private static Vector2 s_Offset = new Vector2(512 - 174, 768 / 2 - 235);
        private static int s_DistBetweenOptions = 26;
        private static int s_DistBetweenNumbers = 16;

		private const int MENU_ITEMS = 3;
        private int m_NumMenuItems = 0;

		private int m_Selected = 0;

        private Texture2D m_Soundvolume;
        private Texture2D m_MusicVolume;
        private Texture2D m_Done;
        private Texture2D m_ArrowOptionsMenu;
        private Texture2D[] m_Numbers = new Texture2D[10];
        private Vector2 m_arrowOffset = new Vector2(45, 5);

        public override void Initialize()
        {
            base.Initialize();
            m_Soundvolume = Graphics.GetTexture("gray_soundvolume");
            m_MusicVolume = Graphics.GetTexture("gray_musicvolume");
            m_Done = Graphics.GetTexture("gray_done");
            m_ArrowOptionsMenu = Graphics.GetTexture("gray_arrow");

            for (int i = 0; i < 10; i++)
            {
                m_Numbers[i] = Graphics.GetTexture("gray_nr" + i);
            }
            m_NumMenuItems = 3;

        }

		public override void Update(float delta)
		{
            base.Update(delta);

            m_Selected = this.GetEntryChanged(m_Selected, m_NumMenuItems, delta);

            //if(Input.AnyWasPressed(InputCommand.Down))
            //{
            //    SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);
            //    m_Selected++;
            //    if(m_Selected >= MENU_ITEMS)
            //    {
            //        m_Selected = 0;
            //    }
            //}
            //if(Input.AnyWasPressed(InputCommand.Up))
            //{
            //    SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);
            //    m_Selected--;
            //    if(m_Selected < 0)
            //    {
            //        m_Selected = MENU_ITEMS - 1;
            //    }
            //}

			if(Input.AnyIsPressed(InputCommand.Left))
			{
				if (m_Selected == 0)
				{
					if (SoundCenter.Instance.MasterSoundVolume > 0)
						SoundCenter.Instance.MasterSoundVolume -= 0.01f;
					if (SoundCenter.Instance.MasterSoundVolume < 0)
						SoundCenter.Instance.MasterSoundVolume = 0;
				} 
				else if (m_Selected == 1)
				{
					if (SoundCenter.Instance.MasterMusicVolume > 0)
						SoundCenter.Instance.MasterMusicVolume -= 0.01f;
					if (SoundCenter.Instance.MasterMusicVolume < 0)
						SoundCenter.Instance.MasterMusicVolume = 0;
				}
			}
			if(Input.AnyIsPressed(InputCommand.Right))
			{
				if (m_Selected == 0)
				{
					Console.WriteLine("WTF, Increase!");
					if (SoundCenter.Instance.MasterSoundVolume < 1)
					{
						Console.WriteLine("...");
						SoundCenter.Instance.MasterSoundVolume += 0.01f;
					}
					if (SoundCenter.Instance.MasterSoundVolume > 1)
						SoundCenter.Instance.MasterSoundVolume = 1;
				} 
				else if (m_Selected == 1)
				{
					if (SoundCenter.Instance.MasterMusicVolume < 1)
						SoundCenter.Instance.MasterMusicVolume += 0.01f;
					if (SoundCenter.Instance.MasterMusicVolume > 1)
						SoundCenter.Instance.MasterMusicVolume = 1;
				}
			}

			if (Input.AnyWasPressed(InputCommand.MenuSelect))
			{
				SoundCenter.Instance.Play(SoundNames.InterfaceClickConfirm);
				Exit();
			}

			/*if(Input.AnyWasPressed(InputCommand.MenuSelect))
			{
				SoundCenter.playSound(SoundCenter.SOUND_interfaceClickConfirm);
				switch(m_Selected)
				{
				case 0:
					ScreenManager.getInstance().addScreen(new KeyboardMenu());
					break;
				case 1:
					ScreenManager.getInstance().addScreen(new JoystickMenu());
					break;
				case 2:
					exitScreen();
					break;
				}
			}*/
		}

        public override void Exit()
        {
            HighScoreList.Save();
            base.Exit();
        }
		public override void Draw()
		{
			base.Draw();

			Graphics.DrawText("Options", 1, new Vector2(20, 20), Color.White);
	
			Color color = Color.White;
            Graphics.Draw(m_ArrowOptionsMenu, s_Offset + new Vector2(m_arrowOffset.X-310, m_arrowOffset.Y-30 + m_Selected * 70), 1.0f, null, 10.0f, Color.White);
			
           

                Graphics.Draw(m_Soundvolume, new Vector2(128, 128), null, 1, color);
                           
                Graphics.Draw(m_MusicVolume, new Vector2(128, 200), null, 2, color);
                
                Graphics.Draw(m_Done, new Vector2(128, 270), null, 1, color);



                string numberString = ((int)(SoundCenter.Instance.MasterMusicVolume*100)).ToString();
                for (int i = 0; i < numberString.Length; ++i)
                {
                    int digit = int.Parse(numberString[i].ToString());
                    Graphics.Draw(m_Numbers[digit],
                        new Vector2(
                            48 + s_Offset.X + s_DistBetweenNumbers * i,
                            55 + s_DistBetweenOptions * 3 + s_Offset.Y-75),
                        null, 1, Color.White);
                }

                // Draw digits.
                numberString = ((int)(SoundCenter.Instance.MasterSoundVolume*100)).ToString();
                for (int i = 0; i < numberString.Length; ++i)
                {
                    int digit = int.Parse(numberString[i].ToString());
                    Graphics.Draw(m_Numbers[digit],
                        new Vector2(
                            48 + s_Offset.X + s_DistBetweenNumbers * i,
                            55 + s_DistBetweenOptions * 3 + s_Offset.Y-155),
                        null, 1, Color.White);
                }
        }
	}
}
