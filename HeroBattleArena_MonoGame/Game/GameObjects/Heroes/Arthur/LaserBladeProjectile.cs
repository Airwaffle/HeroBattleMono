using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class LaserBladeProjectile : HitObject
    {
        private float m_Speed               = 0;
        private float m_SegmentsLength      = 0;
        private float m_ManaDrain           = 0;
        private Ability m_Ability;
        private AABB m_BoundingBox;
        private AnimationManager m_AnimationEnd;
        private float m_SoundTimer		= 0;
        private Cue m_LoopInstance = null;

        private float m_RecoilDistance = 15;
        private float m_RecoilCounter = 0;

        public LaserBladeProjectile(Unit owner, Vector2 direction, Ability ability) :
            base(owner)
        {
			m_Ability = ability;
			m_BoundingBox = new AABB();
			m_BoundingBox.Owner = this;
			AddAABB(m_BoundingBox);
			Direction = direction;
        }

		~LaserBladeProjectile()
		{
			((Hero)Owner).CanAttack = true;
			((Hero)Owner).HeroState = HeroStates.IDLE;
            if (m_SoundTimer > 0.5f)
                m_LoopInstance.Stop(AudioStopOptions.AsAuthored);
			m_AnimationEnd = null;
		}

		public override void Initialize()
		{
			base.Initialize();

			Layer = 2;
			Name = "LaserBladeProjectile";

			Texture = Graphics.GetTexture("laser_blade");
			m_Speed = Configuration.GetValue("Laser_Sword_Speed");
			m_ManaDrain = Configuration.GetValue("Laser_Sword_ManaDrain");
			Damage = Configuration.GetValue("Laser_Sword_Damage");
			Range = 10;

			Vector2 position = Position;
			m_BoundingBox.LayerMask = AABBLayers.LayerProjectile;
			m_BoundingBox.CollisionMask = AABBLayers.CollisionProjectile;
			m_BoundingBox.MinX = position.X - 10;
			m_BoundingBox.MaxX = position.X + 10;
			m_BoundingBox.MinY = position.Y - 10;
			m_BoundingBox.MaxY = position.Y + 10;

			DrawOffset = new Vector2(position.X - 75, position.Y - 75);

			AnimationManager animations = new AnimationManager(150, 150, 3, 12);
			m_AnimationEnd = new AnimationManager(150, 150, 3, 12);

			Life = 1000.0f;
			if (Direction.Y > 0)
			{
				animations.AddAnimation(new Animation(3, 0, 0, false, 0));
				m_AnimationEnd.AddAnimation(new Animation(3, 6, 0, false, 0));
				m_AnimationEnd.AddAnimation(new Animation(3, 10, 0, false, 0));

				DrawOffset = new Vector2(position.X - 69, position.Y - 30);
			} 
			else if (Direction.Y < 0)
			{
				animations.AddAnimation(new Animation(3, 2, 0, false, 0));
				m_AnimationEnd.AddAnimation(new Animation(3, 4, 0, false, 0));
				m_AnimationEnd.AddAnimation(new Animation(3, 8, 0, false, 0));

				DrawOffset = new Vector2(position.X - 95, position.Y - 170);

			} 
			else if (Direction.X > 0)
			{
				animations.AddAnimation(new Animation(3, 1, 0, false, 0));
				m_AnimationEnd.AddAnimation(new Animation(3, 7, 0, false, 0));
				m_AnimationEnd.AddAnimation(new Animation(3, 11, 0, false, 0));

				DrawOffset = new Vector2(position.X - 15, position.Y - 81);
			} 
			else if (Direction.X < 0)
			{
				animations.AddAnimation(new Animation(3, 3, 0, false, 0));
				m_AnimationEnd.AddAnimation(new Animation(3, 5, 0, false, 0));
				m_AnimationEnd.AddAnimation(new Animation(3, 9, 0, false, 0));

				DrawOffset = new Vector2(position.X - 135, position.Y - 81);
			}
			Animations = animations;
			
			m_SegmentsLength = 35;
		}
		
		public override void Update(float delta)
		{
			Owner.StopMoving();

            float movement = (float)(m_RecoilDistance * 0.15 + m_RecoilCounter * (1 - 0.15));
            m_RecoilDistance -= movement;
            Owner.Movement += Owner.Direction * movement * -1;

			m_Ability.Use();

			if(m_SoundTimer < 0.5f) 
			{
				m_SoundTimer += delta;
				if(m_SoundTimer > 0.5f) 
				{
                    //D10JOHTHTODO Fade out LaserBladeProjectile sound once key is released
                    m_LoopInstance = SoundCenter.Instance.Play(SoundNames.ArthurLaserBladeProjectile);
                    //m_LoopInstance.IsLooped = true;
				}
			}
            #if XBOX
                    if (!((Hero)Owner).GetSpecialDown(1) || ((Hero)Owner).Mana < 5 || Owner.IsStunned || !Owner.IsAlive){
				        Remove();
			        }
            #else
                    if (!((Hero)Owner).GetSpecialDown(2) || ((Hero)Owner).Mana < 5 || Owner.IsStunned || !Owner.IsAlive){
                        Remove();
                    }
            #endif

			
			
			Position = Owner.Position;
			((Hero)Owner).DecreaseMana(m_ManaDrain*delta);
			Range += delta * m_Speed;
			m_AnimationEnd.Update(delta);

			UpdateCollisionBox();
			CheckTipCollision();

			base.Update(delta);
		}
		
		private void CheckTipCollision()
		{
			AABB tip = m_BoundingBox.Copy();
	
			if (Direction.Y < 0)
				tip.MinY += (tip.MaxY - tip.MinY) - 50;
			else if (Direction.Y > 0)
				tip.MaxY -= (tip.MaxY - tip.MinY) + 50;
			else if (Direction.X < 0)
				tip.MinX +=( tip.MaxX - tip.MinX) - 50;
			else if (Direction.X > 0)
				tip.MaxX -= (tip.MaxX - tip.MinX) + 50;

			//Checks if the tip collides with something else then the lasers collision box.
			List<AABB> boxes = Collision.GetCollidingBoxes(tip);
			if (boxes.Count > 1)
			{
				m_AnimationEnd.ChangeAnimation(1);
			} 
			else 
			{
				m_AnimationEnd.ChangeAnimation(0);
			}
		}

		private void UpdateCollisionBox()
		{
			Vector2 position = Position;
			if (Direction.Y > 0)
			{
				m_BoundingBox.MinX = DrawOffset.X - 10 + 75;
				m_BoundingBox.MaxX = DrawOffset.X + 10 + 75;
				m_BoundingBox.MinY = DrawOffset.Y - 10 + 75 - 30;
				m_BoundingBox.MaxY = DrawOffset.Y + 10 + (Range - 30) + 75;
			}
			else if (Direction.Y < 0)
			{
				m_BoundingBox.MinX = DrawOffset.X - 10 + 75;
				m_BoundingBox.MaxX = DrawOffset.X + 10 + 75;
				m_BoundingBox.MinY = DrawOffset.Y - 10 - (Range - 30) + 75;
				m_BoundingBox.MaxY = DrawOffset.Y + 10 + 75 + 30;
			}
			else if (Direction.X > 0)
			{
				m_BoundingBox.MinX = DrawOffset.X - 10 + 75 - 30;
				m_BoundingBox.MaxX = DrawOffset.X + 10 + (Range - 30) + 75;
				m_BoundingBox.MinY = DrawOffset.Y - 10 + 75;
				m_BoundingBox.MaxY = DrawOffset.Y + 10 + 75;
			}
			else if (Direction.X < 0)
			{
				m_BoundingBox.MinX = DrawOffset.X - 10 - (Range - 30) + 75;
				m_BoundingBox.MaxX = DrawOffset.X + 10 + 75 + 30;
				m_BoundingBox.MinY = DrawOffset.Y - 10 + 75;
				m_BoundingBox.MaxY = DrawOffset.Y + 10 + 75;
			}
		}

		public override void Draw()
		{
			if(IsVisible)
			{
				Vector2 position = Position;
				int segments = (int)Math.Floor(Range/m_SegmentsLength);
				Vector2 destOffset = DrawOffset;
				destOffset.X += Direction.X * m_SegmentsLength;
				destOffset.Y += Direction.Y * m_SegmentsLength;

				// Draw segments.
				for (int i = segments -2; i >= 0; --i)
				{
                    Animations.Update(1.0f/60.0f);
					Graphics.Draw(
						Texture,
						destOffset,
						Animations.Rectangle,
						Layer + position.Y/768.0f,
						Color.White);

					destOffset.X += Direction.X * m_SegmentsLength;
					destOffset.Y += Direction.Y * m_SegmentsLength;
				}

				//int extraSpace = m_segmentsLength*segments;
				destOffset.X = Direction.X * (Range - 30) + DrawOffset.X;
				destOffset.Y = Direction.Y * (Range - 30) + DrawOffset.Y;
				Graphics.Draw(
					Texture,
					destOffset,
					m_AnimationEnd.Rectangle,
					Layer + position.Y/768.0f,
					Color.White);

				//destOffset.X = position.X + Range * Direction.X;
				//destOffset.Y = position.Y + Range * Direction.Y;

#if DEBUG
				List<AABB> boundingBoxes = BoundingBoxes;
				for (int i = 0; i < boundingBoxes.Count; ++i)
					Graphics.DrawAABB(boundingBoxes[i], DebugAABBMode.Body);
				Graphics.DrawAABB(m_BoundingBox, DebugAABBMode.Body);
#endif
			}
		}

		public override void OnCollide(AABB other)
		{
			if (other.Owner == Owner) return;
			if (other.Owner is Unit)
			{
				Unit unit = other.Owner as Unit;
				if (unit.IsAlive)
				{
					unit.ApplyDamage(Damage, Owner);
				}
			}

			// Laser should kill grenades.
			if(other.Owner is StickyGrenadeProjectile)
			{
				StickyGrenadeProjectile grenade = other.Owner as StickyGrenadeProjectile;
				grenade.Life = 0;
#warning StickyGrenadeProjectile needs Victim accessor.
				//grenade.->setVictim(0);
			}

			// Laser kills aztec spears...
			if (other.Owner is AztecSpear)
			{
				((AztecSpear)other.Owner).Life = 0;
			}
		}
	}
}
