using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game
{
	public class TutorialScreenOLD : Screen
	{
		private int m_Selected = 0;
		private float m_Pressed = 0;
		private int m_CurrentArrow = 0;
		private float m_OffsetX = 360;
		private float m_OffsetY = 30;

		private Texture2D[] m_TexCharacters = new Texture2D[4];
		private Texture2D[] m_TexArrows = new Texture2D[3];

		public override void Initialize()
		{
			base.Initialize();

			m_TexCharacters[0] = Graphics.GetTexture("arthur_tutorial");
			m_TexCharacters[1] = Graphics.GetTexture("aztek_tutorial");
			m_TexCharacters[2] = Graphics.GetTexture("nano_tutorial");
			m_TexCharacters[3] = Graphics.GetTexture("soldier_tutorial");

			m_TexArrows[0] = Graphics.GetTexture("tva_pil");
			m_TexArrows[1] = Graphics.GetTexture("hoger_pil");
			m_TexArrows[2] = Graphics.GetTexture("vanster_pil");
		}

		public override void Update(float delta)
		{
			base.Update(delta);

			if (m_Pressed > 0)
			{
				m_Pressed -= delta;
				if (m_Pressed <= 0)
				{
					m_CurrentArrow = 0;
					m_Pressed = 0;
				}

			}

			if(Input.AnyWasPressed(InputCommand.Right)) 
			{
				m_Selected ++;
				m_Selected = m_Selected % 4;
				m_Pressed  = 0.1f;
				m_CurrentArrow = 2;
			}
			if(Input.AnyWasPressed(InputCommand.Left)) 
			{
				m_Selected --;
				m_Selected += 4;
				m_Selected = m_Selected % 4;
				m_Pressed  = 0.1f;
				m_CurrentArrow = 1;
			}
		}

		public override void Draw()
		{
			base.Draw();

			Graphics.Draw(
				m_TexArrows[m_CurrentArrow],
				new Vector2(m_OffsetX, m_OffsetY), 
				null, 5, Color.White);
			Graphics.Draw(
				m_TexCharacters[m_Selected], 
				Vector2.Zero, null, 5, Color.White);
		}
	}
}
