using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
	public class ObstacleParameters
	{
		public AnimationManagerParameters AnimationParameters = null;

		public Texture2D Texture = null;
		public float OffsetX = 0, OffsetY = 0;
		public float DepthOffset = 0;
		public float Layer = 0;
		public List<AABB> BoundingBoxes = new List<AABB>();
	}

	public class Obstacle : Entity
	{
		private ObstacleParameters m_Params;
        private Color m_Color = Color.White;

		public Obstacle(ObstacleParameters param)
		{
			m_Params = param;
		}

		public override void Initialize()
		{
			base.Initialize();

			Name = "Obstacle";

            if (GameMode.Instance is GM_Zombie)
                m_Color = Color.CornflowerBlue;

			Layer = m_Params.Layer + (Position.Y + m_Params.DepthOffset)/768.0f;

			// Create boundin boxes from parameters.
			for (int i = 0; i < m_Params.BoundingBoxes.Count; ++i)
			{
				AABB aabb = new AABB();
				aabb.MinX = m_Params.BoundingBoxes[i].MinX + Position.X;
				aabb.MaxX = m_Params.BoundingBoxes[i].MaxX + Position.X;
				aabb.MinY = m_Params.BoundingBoxes[i].MinY + Position.Y;
				aabb.MaxY = m_Params.BoundingBoxes[i].MaxY + Position.Y;
				aabb.Owner = this;
				aabb.CollisionMask = AABBLayers.CollisionStaticObject;
				aabb.LayerMask = AABBLayers.LayerStaticObject;
				AddAABB(aabb);
			}

			DrawOffset = new Vector2(
				Position.X + m_Params.OffsetX,
				Position.Y + m_Params.OffsetY);

			if (m_Params.AnimationParameters != null && m_Params.Texture != null)
			{
				AnimationManager animations = new AnimationManager(
					m_Params.AnimationParameters.FrameWidth,
					m_Params.AnimationParameters.FrameHeight,
					(uint)m_Params.AnimationParameters.FramesX,
					(uint)m_Params.AnimationParameters.FramesY);

				// Loop through cycles.
				foreach (AnimationCycleParameters cycle in m_Params.AnimationParameters.Cycles)
				{
					animations.AddAnimation(new Animation((uint)cycle.NumFrames, (uint)cycle.StartY, (uint)cycle.StartX, false, 0));
				}

				animations.StandardAnimationSpeed = m_Params.AnimationParameters.FrameRate;
				//animations.AddAnimation(new Animation((uint)m_Params.FramesX * (uint)m_Params.FramesY, 0, 0, false, 0));

				Animations = animations;
			}
			else if (m_Params.Texture != null)
			{
				AnimationManager animations = new AnimationManager(
					m_Params.Texture.Width,
					m_Params.Texture.Height,
					1, 1);
				Animations = animations;
			}
			else
				Hide();

			Texture = m_Params.Texture;

			m_Params = null;
		}

		public override void OnCollide(AABB other)
		{
		}

		public override void Draw()
		{
			if (IsVisible && Texture != null)
			{
				Graphics.Draw(
					Texture,
					DrawOffset,
					Scale,
					Animations.Rectangle,
                    Layer, m_Color);
			}
#if DEBUG
			foreach (AABB aabb in BoundingBoxes)
				Graphics.DrawAABB(aabb, DebugAABBMode.Body);
#endif
		}
	}
}
