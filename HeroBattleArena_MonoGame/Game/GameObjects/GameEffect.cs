using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
	public struct EffectParameters
	{
		public Texture2D Texture;
		public int DrawWidth, DrawHeight;
		public int FramesX, FramesY;
        public int NumberOfFrames;
		public float Layer;
		public int FrameRate;
		public int Loops;
		public float OffsetX, OffsetY;
	}

	public class GameEffect : Entity
	{
		EffectParameters m_Params;
		float m_LifeTime			= 1;
		Vector2 m_FollowOffset		= Vector2.Zero;
		Unit m_Follow				= null;

		public GameEffect(EffectParameters param)
		{
			m_Params = param;
		}

		public override void Initialize()
		{
			base.Initialize();

			Name = "Effect";

			Layer = m_Params.Layer;

			Vector2 position = Position;

			m_FollowOffset.X = position.X + m_Params.OffsetX;
			m_FollowOffset.Y = position.Y + m_Params.OffsetY;
            uint _NumberOfFrames = (uint)m_Params.NumberOfFrames;
            if (_NumberOfFrames == 0)
            {
                _NumberOfFrames = (uint)m_Params.FramesX * (uint)m_Params.FramesY;
            }

			DrawOffset = new Vector2(
				position.X - (float)m_Params.DrawWidth/2,
				position.Y - (float)m_Params.DrawHeight/2);
			if(m_Follow == null)
				DrawOffset += new Vector2(m_Params.OffsetX, m_Params.OffsetY);

			AnimationManager animations = new AnimationManager(
				m_Params.DrawWidth, m_Params.DrawHeight,
				(uint)m_Params.FramesX, (uint)m_Params.FramesY);
            animations.AddAnimation(new Animation(_NumberOfFrames, 0, 0, false, 0));
			animations.AnimationSpeed = ((float)m_Params.FrameRate);
			Animations = animations;

            m_LifeTime = (float)(m_Params.Loops * (float)(((float)_NumberOfFrames) / m_Params.FrameRate));
			Texture = m_Params.Texture;	
		}

		public override void Update(float delta)
		{
			if (m_Follow != null)
			{
				Position = m_Follow.Position + m_FollowOffset;
				if (!m_Follow.IsAlive || m_Follow.IsRemoved)
					Remove();
			}

			m_LifeTime -= delta;
			if (m_LifeTime <= 0)
				Remove();

			base.Update(delta);
		}

		public override void LateUpdate(float delta)
		{
			if (m_Follow != null)
			{
				if (m_Follow.IsRemoved)
					Remove();
			}
		}

		public Vector2 GetOffset()
		{
			return m_FollowOffset;
		}

		public void SetFollow(Unit unit)
		{
			m_Follow = unit;
		}

		public void SetLifeTime(float life)
		{
			m_LifeTime = life;
		}
	}
}
