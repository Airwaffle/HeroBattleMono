using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class Projectile : DamageObject
    {
        private float m_Speed = 0;
        private Vector2 m_Direction;

        public float Speed { get { return m_Speed; } set { m_Speed = value; } }
        public Vector2 Direction { get { return m_Direction; } protected set { m_Direction = value; } }
        public Vector2 Movement { get { return m_Direction * m_Speed; } }

        public Projectile(Unit owner, Vector2 direction) :
            base(owner)
        {
            m_Direction = direction;
        }

        public override void Initialize()
        {
            base.Initialize();

            Name = "Projectile";
            Layer = 2;
        }

        public override void Update(float delta)
        {
            base.Update(delta);
        }

        public override void OnCollide(AABB other)
        {
            base.OnCollide(other);


            if (other.Owner is Obstacle)
                Remove();
        }
    }
}
