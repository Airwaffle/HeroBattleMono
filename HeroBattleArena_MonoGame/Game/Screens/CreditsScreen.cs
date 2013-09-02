using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game
{
	public class CreditsScreen : Screen
	{
		private int		m_OffsetY	= 300;
		private float	m_CurrentY	= 768;
		private int		m_OffsetX	= 400;
		private int		m_LogoX		= 35;
		private float	m_Roll		= 0;

		private List<Texture2D> m_CreditsPortraits = new List<Texture2D>();
		private Texture2D m_Logo;

		public override void Initialize()
		{
			base.Initialize();

			m_Roll = Configuration.GetValue("Credits_Speed");
			m_Logo = (Graphics.GetTexture("credits_logo"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_jakob"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_joacim"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_william"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_karl"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_gustav"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_olle"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_Kris"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_emanuel"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_johan"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_filip"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_chris"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_simon"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_marcus"));
			m_CreditsPortraits.Add(Graphics.GetTexture("credits_hitmonlee"));
		}

		public override void Update(float delta)
		{
			base.Update(delta);

			m_CurrentY -= delta * m_Roll;

			if (m_CurrentY + (m_CreditsPortraits.Count - 1) * m_OffsetY < -300)
				Exit();
		}

		public override void Draw()
		{
			base.Draw();

			Graphics.Draw(m_Logo, new Vector2(m_LogoX, 200), null, 5, Color.White);

			for (int i = 0; i < m_CreditsPortraits.Count; ++i)
			{
				Graphics.Draw(
					m_CreditsPortraits[i], 
					new Vector2(
						m_OffsetX, 
						m_CurrentY+i*m_OffsetY), 
					null, 5, Color.White);
			}
		}
	}
}
