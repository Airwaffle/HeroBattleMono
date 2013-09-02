using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class BossTravelingLaser : HitObject
    {
        private float m_SegmentsLength = 0;
        private AABB m_BoundingBox;
        private AnimationManager m_AnimationEnd;

        private Vector2 m_TravelDir;

        private float m_MaxRange = 1084;

        public BossTravelingLaser(Unit owner, Vector2 direction, Vector2 travelDir) :
            base(owner)
        {
            m_BoundingBox = new AABB();
            m_BoundingBox.Owner = this;
            AddAABB(m_BoundingBox);
            Direction = direction;
            m_TravelDir = travelDir;

            if (Direction.Y != 0)
            {
                m_MaxRange = 808;
            }
        }

        ~BossTravelingLaser()
        {
            m_AnimationEnd = null;
        }

        public override void Initialize()
        {
            base.Initialize();

            Layer = 2;
            Name = "Boss Laser";
            Texture = Graphics.GetTexture("puke_laser");
            Damage = Configuration.GetValue("Laser_Sword_Damage");
            Range = m_MaxRange;

            Vector2 position = Position;
            m_BoundingBox.LayerMask = AABBLayers.LayerProjectile;
            m_BoundingBox.CollisionMask = AABBLayers.CollisionProjectile;
            m_BoundingBox.MinX = position.X - 10;
            m_BoundingBox.MaxX = position.X + 10;
            m_BoundingBox.MinY = position.Y - 10;
            m_BoundingBox.MaxY = position.Y + 10;

            AnimationManager animations = new AnimationManager(75, 75, 3, 12);
            m_AnimationEnd = new AnimationManager(75, 75, 3, 12);

            Life = 7.0f;
            if (Direction.Y > 0)
            {
                animations.AddAnimation(new Animation(3, 0, 0, false, 0));
                m_AnimationEnd.AddAnimation(new Animation(3, 6, 0, false, 0));
                m_AnimationEnd.AddAnimation(new Animation(3, 10, 0, false, 0));
            }
            else if (Direction.Y < 0)
            {
                animations.AddAnimation(new Animation(3, 2, 0, false, 0));
                m_AnimationEnd.AddAnimation(new Animation(3, 4, 0, false, 0));
                m_AnimationEnd.AddAnimation(new Animation(3, 8, 0, false, 0));
            }
            else if (Direction.X > 0)
            {
                animations.AddAnimation(new Animation(3, 1, 0, false, 0));
                m_AnimationEnd.AddAnimation(new Animation(3, 7, 0, false, 0));
                m_AnimationEnd.AddAnimation(new Animation(3, 11, 0, false, 0));
            }
            else if (Direction.X < 0)
            {
                animations.AddAnimation(new Animation(3, 3, 0, false, 0));
                m_AnimationEnd.AddAnimation(new Animation(3, 5, 0, false, 0));
                m_AnimationEnd.AddAnimation(new Animation(3, 9, 0, false, 0));
            }
            Animations = animations;

            m_SegmentsLength = 35;
        }

        public override void Update(float delta)
        {
            Vector2 position = Position;
            position.X += m_TravelDir.X * delta;
            position.Y += m_TravelDir.Y * delta;
            Position = position;

            m_AnimationEnd.Update(delta);

            UpdateCollisionBox();

            base.Update(delta);
        }


        private void UpdateCollisionBox()
        {
            Vector2 position = Position;
            if (Direction.Y > 0)
            {
                m_BoundingBox.MinX = Position.X - 10 + 75;
                m_BoundingBox.MaxX = Position.X + 10 + 75;
                m_BoundingBox.MinY = Position.Y - 10 + 75 - 30;
                m_BoundingBox.MaxY = Position.Y + 10 + (Range - 30) + 75;
            }
            else if (Direction.Y < 0)
            {
                m_BoundingBox.MinX = Position.X - 10 + 75;
                m_BoundingBox.MaxX = Position.X + 10 + 75;
                m_BoundingBox.MinY = Position.Y - 10 - (Range - 30) + 75;
                m_BoundingBox.MaxY = Position.Y + 10 + 75 + 30;
            }
            else if (Direction.X > 0)
            {
                m_BoundingBox.MinX = Position.X - 10 + 75 - 30;
                m_BoundingBox.MaxX = Position.X + 10 + (Range - 30) + 75;
                m_BoundingBox.MinY = Position.Y - 10 + 75;
                m_BoundingBox.MaxY = Position.Y + 10 + 75;
            }
            else if (Direction.X < 0)
            {
                m_BoundingBox.MinX = Position.X - 10 - (Range - 30) + 75;
                m_BoundingBox.MaxX = Position.X + 10 + 75 + 30;
                m_BoundingBox.MinY = Position.Y - 10 + 75;
                m_BoundingBox.MaxY = Position.Y + 10 + 75;
            }
        }

        public override void Draw()
        {
            if (IsVisible)
            {
                Vector2 position = Position;
                int segments = (int)Math.Floor(Range / m_SegmentsLength);
                Vector2 destOffset = position;
                destOffset.X += Direction.X * m_SegmentsLength;
                destOffset.Y += Direction.Y * m_SegmentsLength;

                // Draw segments.
                for (int i = segments - 2; i >= 0; --i)
                {
                    Animations.Update(1.0f / 60.0f);
                    Graphics.Draw(
                        Texture,
                        destOffset,
                        2,
                        Animations.Rectangle,
                        Layer + position.Y / 768.0f,
                        Color.White);

                    destOffset.X += Direction.X * m_SegmentsLength;
                    destOffset.Y += Direction.Y * m_SegmentsLength;
                }

                destOffset.X = Direction.X * (Range - 30) + position.X;
                destOffset.Y = Direction.Y * (Range - 30) + position.Y;
                Graphics.Draw(
                    Texture,
                    destOffset,
                    2,
                    m_AnimationEnd.Rectangle,
                    Layer + position.Y / 768.0f,
                    Color.White);

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
            if (other.Owner is Hero)
            {
                Hero hero = other.Owner as Hero;
                if (hero.IsAlive)
                {
                    hero.ApplyDamage(Damage, Owner);
                }
            }

            // Laser should kill grenades.
            if (other.Owner is StickyGrenadeProjectile)
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
        public override void Remove()
        {
            base.Remove();
        }
    }
}
