using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    class BossHit : HitObject
    {
        public BossHit(Unit owner, Vector2 direction)
			: base(owner)
		{
			Direction = direction;
            Range = Configuration.GetValue("Zombie_Attack_Range") + 20;
		}

        public override void Initialize()
        {
            base.Initialize();

            Name = "BossAttackHit";

            Hide();

            AnimationManager animations = new AnimationManager(20, 30, 1, 1);
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
                boundingBox.MinX = position.X - 20 - 20;
                boundingBox.MaxX = position.X + 20;
            }
            else if (Direction.Y < 0)
            {
                boundingBox.MinY = position.Y - Range;
                boundingBox.MaxY = position.Y - 0;
                boundingBox.MinX = position.X - 20 - 20;
                boundingBox.MaxX = position.X + 20;
            }
            else if (Direction.X > 0)
            {
                boundingBox.MinX = position.X + 0;
                boundingBox.MaxX = position.X + Range;
                boundingBox.MinY = position.Y - 20;
                boundingBox.MaxY = position.Y + 20 + 15;
            }
            else if (Direction.X < 0)
            {
                boundingBox.MinX = position.X - Range;
                boundingBox.MaxX = position.X - 0;
                boundingBox.MinY = position.Y - 20;
                boundingBox.MaxY = position.Y + 20 + 15;
            }
            AddAABB(boundingBox);
        }

        public override void Update(float delta)
        {
            base.Update(delta);
        }

        public override void OnCollide(AABB other)
        {
            if (other.Owner is Hero)
            {
                Unit unit = other.Owner as Unit;
                if (unit.IsAlive)
                {
                    unit.ApplyDamage(Damage, Owner);
                    unit.RemoveStun();
                    Remove();
                }
            }
        }
    }
}
