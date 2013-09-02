using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class ZombieAttackHit : HitObject
    {
        public ZombieAttackHit(Unit owner, Vector2 direction)
			: base(owner)
		{
			Direction = direction;
            Range = Configuration.GetValue("Zombie_Attack_Range");
		}

        public override void Initialize()
        {
            base.Initialize();

            Name = "ZombieAttackHit";

            Hide();

            AnimationManager animations = null;
            if (Direction.Y != 0)
                animations = new AnimationManager(20, 30, 1, 1);
            else
                animations = new AnimationManager(30, 20, 1, 1);
            animations.AddAnimation(new Animation(1, 0, 0, false, 0));
            Animations = animations;

            Vector2 position = Position;

            // Add BoundingBox.
            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerProjectile;
            boundingBox.CollisionMask = AABBLayers.CollisionProjectile;
            if (Direction.Y > 0)
            {
                boundingBox.MinY = position.Y - 0;
                boundingBox.MaxY = position.Y + Range;
                boundingBox.MinX = position.X - 10;
                boundingBox.MaxX = position.X + 10;
            }
            else if (Direction.Y < 0)
            {
                boundingBox.MinY = position.Y - Range;
                boundingBox.MaxY = position.Y - 0;
                boundingBox.MinX = position.X - 10;
                boundingBox.MaxX = position.X + 10;
            }
            else if (Direction.X > 0)
            {
                boundingBox.MinX = position.X + 0;
                boundingBox.MaxX = position.X + Range;
                boundingBox.MinY = position.Y - 10;
                boundingBox.MaxY = position.Y + 10;
            } 
            else if (Direction.X < 0)
            {
                boundingBox.MinX = position.X - Range;
                boundingBox.MaxX = position.X - 0;
                boundingBox.MinY = position.Y - 10;
                boundingBox.MaxY = position.Y + 10;
            }
            AddAABB(boundingBox);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            Position = Owner.Position;
        }

        public override void OnCollide(AABB other)
        {
            if (other.Owner == Owner) return;
            base.OnCollide(other);

            bool zombieAttackEachOther = false;
            if (Owner is Enemy){
                
                if ((Owner as Enemy).Owner != null){
                    zombieAttackEachOther = true;
                }
                if (other.Owner is Enemy)
                {
                    if ((other.Owner as Enemy).Owner != null)
                    {
                        zombieAttackEachOther = true;
                    }
                }
            }

            if (Owner is Hero || zombieAttackEachOther)
            {
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
            else
            {
                if (other.Owner is Hero)
                {
                    Hero hero = other.Owner as Hero;
                    if (hero.IsAlive)
                    {
                        hero.ApplyDamage(Damage, Owner);
                        Remove();
                    }
                }
            }
        }
    }
}
