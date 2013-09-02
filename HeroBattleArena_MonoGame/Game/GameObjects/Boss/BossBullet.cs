using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class BossBullet : Projectile
    {
        public BossBullet(Unit owner, Vector2 direction)
            : base(owner, direction) { }

        public override void Initialize()
        {
            base.Initialize();
            Name = "BossProjectile";
            Texture = Graphics.GetTexture("boss_bullet");

            Life = 1024/Speed;

            Vector2 position = Position;

            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerProjectile;
            boundingBox.CollisionMask = AABBLayers.CollisionProjectile;
            boundingBox.MinX = position.X - 7;
            boundingBox.MaxX = boundingBox.MinX + 15;
            boundingBox.MinY = position.Y - 7;
            boundingBox.MaxY = boundingBox.MinY + 15;

            AddAABB(boundingBox);

            DrawOffset = new Vector2(position.X - 25, position.Y - 25);

            AnimationManager animations = new AnimationManager(50, 50, 4, 4);
            animations.AddAnimation(new Animation(4, 0, 0, false, 0));
            animations.AddAnimation(new Animation(4, 1, 0, false, 0));
            animations.AddAnimation(new Animation(4, 2, 0, false, 0));
            animations.AddAnimation(new Animation(4, 3, 0, false, 0));
            animations.AnimationSpeed = 26;
            Animations = animations;

            if (Direction.Y < 0)
            {
                animations.ChangeAnimation(0);
                Position += new Vector2(-5, -40);
            }
            else if (Direction.Y > 0)
            {
                animations.ChangeAnimation(2);
                Position += new Vector2(-3, 30);
            }
            else if (Direction.X < 0)
            {
                animations.ChangeAnimation(1);
                Position += new Vector2(-50, 10);
            }
            else
            {
                animations.ChangeAnimation(3);
                Position += new Vector2(50, 10);
            }


            // Creational collision test.
            Vector2 pPos = Vector2.Zero;
            pPos.X = Owner.Position.X;
            pPos.Y = Owner.Position.Y;
            AABB testRect = boundingBox.Copy();

            Vector2 deltaPos = position - pPos;

            // Left side of player
            if (Direction.X < 0)
            {
                testRect.MaxX = pPos.X + 10;
            }
            // Right side of player
            else if (Direction.X > 0)
            {
                testRect.MinX = pPos.X - 10;
            }
            // Above player
            if (Direction.Y < 0)
            {
                testRect.MaxY = pPos.Y + 10;
            }
            // Below player
            else if (Direction.Y > 0)
            {
                testRect.MinY = pPos.Y - 10;
            }
            List<AABB> boxes = Collision.GetCollidingBoxes(testRect);
            foreach (AABB box in boxes)
            {
                OnCollide(box);
            }

        }
        public override void Update(float delta)
        {
            base.Update(delta);

            Vector2 position = Position;
            position.X += Direction.X * Speed * delta;
            position.Y += Direction.Y * Speed * delta;
            Position = position;

        }

        public override void OnCollide(AABB otherCol)
        {
            base.OnCollide(otherCol);
            if (otherCol.Owner == Owner) return;

            Vector2 pos = Position;
            Vector2 dir = Direction;
            // Adjusting pos to hit location.
            if (dir.X < 0)
                pos = new Vector2(otherCol.MaxX, pos.Y);
            else if (dir.X > 0)
                pos = new Vector2(otherCol.MinX, pos.Y);
            else if (dir.Y < 0)
                pos = new Vector2(pos.X, otherCol.MaxY);
            else
                pos = new Vector2(pos.X, otherCol.MinY);

            if ((pos - Position).Length() > 75)
                pos = Position;

            if (otherCol.Owner is Hero)
            {
                Hero hero = otherCol.Owner as Hero;
                if (hero.IsAlive)
                {
                    hero.ApplyDamage(Damage, Owner);

                    if ((Owner as Boss).Level < 7)
                    {
                        hero.AddBuff(new SlowdownBuff(hero));
                    }
                    else
                    {
                        hero.SetStun(2);
                        Effects.SpawnFollowing(hero, "Stunned_Effect", 2);
                    }

                    Remove();
                    if (Direction.Y > 0)
                    {
                        Effects.Spawn(pos, "projectile_mage_hit_down");
                    }
                    else if (Direction.Y < 0)
                    {
                        Effects.Spawn(pos, "projectile_mage_hit_up");
                    }
                    else if (Direction.X > 0)
                    {
                        Effects.Spawn(pos, "projectile_mage_hit_right");
                    }
                    else
                    {
                        Effects.Spawn(pos, "projectile_mage_hit_left");
                    }
                }
            }

            if (otherCol.Owner is Obstacle && (Owner as Boss).Level < 6)
            {
                if (Direction.Y > 0)
                {
                    Effects.Spawn(pos, "projectile_mage_hit_down");
                }
                else if (Direction.Y < 0)
                {
                    Effects.Spawn(pos, "projectile_mage_hit_up");
                }
                else if (Direction.X > 0)
                {
                    Effects.Spawn(pos, "projectile_mage_hit_right");
                }
                else
                {
                    Effects.Spawn(pos, "projectile_mage_hit_left");
                }

                Remove();
            }
        }
    }
}


