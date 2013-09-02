using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class VampyricHit : HitObject
    {
        float m_boostSpeed = Configuration.GetValue("Vampyric_Boost_Speed");
        float m_boostSlowdown = Configuration.GetValue("Vampyric_Slowdown");
        float m_stuckAtSpeed = Configuration.GetValue("Vampyric_Stuck_Speed");
        float m_StuckTime = Configuration.GetValue("Vampyric_Stuck_Time");
        List<Vector2> m_dragBehind = new List<Vector2>();
        float m_dragDelay = Configuration.GetValue("Vampyric_Drag_Graphic_Delay");
        float m_dragCounter = 0;

        public VampyricHit(Unit owner)
			: base(owner)
		{
            //Texture = Graphics.GetTexture("zombie_sprite2");
            Texture = Graphics.LoadTexture("Characters/Zombie/Sprite_Sheet_Zombie_2");
        }

        public override void Initialize()
        {
            // Get configuraton settings.
            Damage = Configuration.GetValue("Vampyric_Impact_Damage");
            Range = 20;
            Layer = 1.98f;
            Vector2 position = Position;

            DrawOffset = new Vector2(position.X - 20, position.Y - 20);
            Life = 1000;

            // Create the attack AABB.
            AABB boundingBox = new AABB(-20 + Range * Owner.Direction.X, -20 + Range * Owner.Direction.Y, 20 + Range * Owner.Direction.X, 20 + Range * Owner.Direction.Y);
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerProjectile;
            boundingBox.CollisionMask = AABBLayers.CollisionProjectile;
            AddAABB(boundingBox);
        }
        public override void Update(float delta)
        {
            base.Update(delta);
            m_dragCounter += delta;
            if (m_dragCounter > m_dragDelay)
            {
                m_dragCounter = 0;
                m_dragBehind.Add(new Vector2(Owner.DrawOffset.X, Owner.DrawOffset.Y));
            }
            
            Owner.Movement = Owner.Direction;
            Owner.MoveSpeed = m_boostSpeed;
            m_boostSpeed *= (1.0f - (1.0f/(delta * m_boostSlowdown)));

            Position = Owner.Position;
           
            if (m_boostSpeed < 10)
            {
                (Owner as Hero).HeroState = HeroStates.IDLE;
                Remove();
            }
            
        }

        public override void Draw()
        {
            for (int i = 0; i < m_dragBehind.Count; i++)
            {
                Graphics.Draw(
                    Texture,
                    m_dragBehind[i],
                    2.0f,
                    Owner.Animations.Rectangle,
                    Layer + (float)Position.Y / 768.0f, Color.DarkRed);
                
            }

            Graphics.DrawAABB(BoundingBoxes[0], DebugAABBMode.Body);
        }

        public override void OnCollide(AABB other)
        {
            if (other.Owner == Owner) return;
            if (other.Owner is Unit && m_boostSpeed <= m_stuckAtSpeed)
            {
                Unit otherUnit = other.Owner as Unit;
                if (!otherUnit.IsAlive) return;

                otherUnit.ApplyDamage(Damage, Owner);
                if (!otherUnit.IsAlive)
                {
                    Remove();
                    (Owner as Hero).HeroState = HeroStates.IDLE;
                    return;
                };

                otherUnit.AddBuff(new VampyricDrain(otherUnit, m_StuckTime, Configuration.GetValue("Vampyric_Life_Drain"), Owner));
                otherUnit.SetBlink(m_StuckTime);
                otherUnit.SetStun(m_StuckTime);
                (Owner as Hero).HeroState = HeroStates.SPECIAL7;
                Remove();
            }
        }
    }
}