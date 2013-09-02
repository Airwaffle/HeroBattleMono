using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    class BossFollowHit : HitObject
    {
        private int m_Thickness = 5;
        private bool m_Disabled = true;

        public bool Disabled {set {m_Disabled = value;}}

        public BossFollowHit(Unit owner)
			: base(owner)
		{
		}

        public override void Initialize(){

            Name = "BossFollowHit";

            Hide();

            Damage = 10;
            Life = Int16.MaxValue;
            Vector2 position = Position;

            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerProjectile;
            boundingBox.CollisionMask = AABBLayers.CollisionProjectile;

            boundingBox.MinX = position.X - 20 - 20 + 20 - 10 - m_Thickness;
            boundingBox.MaxX = position.X + 20 - 20 + 20 + 10 + m_Thickness;
            boundingBox.MinY = position.Y - 10 - 20 + 66 - 10 - 50 - m_Thickness;
            boundingBox.MaxY = position.Y + 10 - 20 + 66 + m_Thickness;
           
            AddAABB(boundingBox);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            Position = Owner.Position;
        }

        public override void OnCollide(AABB other)
        {
            if (other.Owner == Owner || m_Disabled) return;
            base.OnCollide(other);

            if (other.Owner is Hero)
            {
                Hero hero = other.Owner as Hero;
                if (hero.IsAlive)
                {
                    hero.ApplyDamage(Damage, Owner);
                }
            }
        }
    }
}
