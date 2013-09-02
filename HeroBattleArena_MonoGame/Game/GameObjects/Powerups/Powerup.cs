using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace HeroBattleArena.Game.GameObjects
{
    public class Powerup : Entity
    {
        private float m_Unpickable = 0.5f;

        public override void Initialize()
        {
            base.Initialize();

            Show();
            Layer = 2;
            Name = "Powerup";

            AABB collision = new AABB();
            collision.MinX = Position.X - 10;
            collision.MinY = Position.Y - 10;
            collision.MaxX = Position.X + 10;
            collision.MaxY = Position.Y + 10;
            collision.Owner = this;
            collision.CollisionMask = AABBLayers.CollisionPickup;
            collision.LayerMask = AABBLayers.LayerPickup;
            AddAABB(collision);
        }

        public override void Update(float delta)
        {
            base.Update(delta);

            if (m_Unpickable > 0)
            {
                m_Unpickable -= delta;
                if (m_Unpickable < 0)
                    m_Unpickable = 0;
            }
        }

        public virtual void OnPickedUp(Hero hero)
        {

            SoundCenter.Instance.Play(SoundNames.InterfacePowerUpPickup);
#if DEBUG
            Console.WriteLine(hero.Name + " picked up powerup: " + Name);
#endif
        }

        public override void OnCollide(AABB other)
        {
            base.OnCollide(other);

            if (m_Unpickable > 0) return;

            if (other.Owner is Hero)
            {
                OnPickedUp(other.Owner as Hero);
                Remove();
            }
        }
    }
}
