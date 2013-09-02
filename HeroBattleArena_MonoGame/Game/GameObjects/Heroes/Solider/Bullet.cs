using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class Bullet : Projectile
    {
        public Bullet(Unit owner, Vector2 direction)
            : base(owner, direction)
		{
		}
        public override void Initialize()
        {
            base.Initialize();
            Name = "Bullet";
            Texture = Graphics.GetTexture("projectile_bullet");
            Vector2 position = Position;
            // Add BoundingBox
            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerProjectile;
            boundingBox.CollisionMask = AABBLayers.CollisionProjectile;
            boundingBox.MinY = position.Y - 7;
            boundingBox.MaxY = position.Y + 8;
            boundingBox.MinX = position.X - 7;
            boundingBox.MaxX = position.X + 8;
            AddAABB(boundingBox);

            DrawOffset = new Vector2(position.X - 25, position.Y - 25);

            Life = Configuration.GetValue("Solider_Bullet_Range") / Speed;

            AnimationManager animations = new AnimationManager(50, 50, 8, 1);
            animations.AddAnimation(new Animation(2, 0, 0, false, 0));
            animations.AddAnimation(new Animation(2, 0, 2, false, 0));
            animations.AddAnimation(new Animation(2, 0, 4, false, 0));
            animations.AddAnimation(new Animation(2, 0, 6, false, 0));
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
                Position += new Vector2(-90, 10);
            }
            else if (Direction.X > 0)
            {
                animations.ChangeAnimation(3);
                Position += new Vector2(90, 10);
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
	        foreach(AABB box in boxes) {
		        OnCollide(box);
	        }           
        }
        public override void Update(float delta)
        {
            Vector2 pos = Position;
            pos.X += Direction.X * Speed * delta;
            pos.Y += Direction.Y * Speed * delta;
            Position = pos;
	
            base.Update(delta);
        }
        public override void OnCollide(AABB other)
        {
            if (other.Owner == Owner) return;
            base.OnCollide(other);

            if (other.Owner is Unit)
            {
                Unit unit = other.Owner as Unit;
                if (unit.IsAlive)
                {
                    unit.ApplyDamage(Damage, Owner);
                    Remove();
                }
            }
        }
    }
}
