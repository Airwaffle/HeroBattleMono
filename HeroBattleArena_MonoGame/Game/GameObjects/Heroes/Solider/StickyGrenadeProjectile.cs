using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class StickyGrenadeProjectile : Projectile
    {
        float m_height = 10.0f;
        float m_groundPos = 0.0f;
        float m_gravity = Configuration.GetValue("Sticky_Grenade_Gravity");
        float m_bounce = Configuration.GetValue("Sticky_Grenade_Bounce");
        float m_dontHurtOwnerCounter = 0.5f;
        float m_aoe = Configuration.GetValue("Sticky_Grenade_AOE");
        Vector2 m_moveVector = new Vector2();
        Vector2 m_victimOffset = new Vector2();
        Unit m_victim = null;
        float m_stickcount = 0.0f;
        Unit m_protected = null;
        StickyGrenadeBuff m_buff = null;
        bool m_PlayStickSound = true;

        public StickyGrenadeProjectile(Unit owner, Vector2 direction, bool instantStuck)
            : base(owner, direction)
		{
            if (instantStuck)
            {
                m_dontHurtOwnerCounter = 0;
                m_PlayStickSound = false;
            }
		}

        public Unit ProtectedUnit { set { m_protected = value; } }

        public override void Initialize()
        {
            base.Initialize();
            Name = "Sticky Grenade Projectile";
            Texture = Graphics.GetTexture("projectile_grenade");
            Life = Configuration.GetValue("Sticky_Grenade_Life");
            Speed = Configuration.GetValue("Sticky_Grenade_Speed");
            Damage = Configuration.GetValue("Sticky_Grenade_Damage");

            if (Owner != null)
                m_groundPos = Owner.Position.Y;
            else
                m_groundPos = Position.Y + 50;
            m_moveVector.X = Configuration.GetValue("Sticky_Grenade_StartSpeedX");
            m_moveVector.Y = Configuration.GetValue("Sticky_Grenade_StartSpeedY");

            Vector2 position = Position;
            // Add BoundingBox
            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerSensitiveProjectile;
            boundingBox.CollisionMask = AABBLayers.CollisionSensitiveProjectile;
            boundingBox.MinY = position.Y - 10;
            boundingBox.MaxY = position.Y + 10;
            boundingBox.MinX = position.X - 10;
            boundingBox.MaxX = position.X + 10;
            AddAABB(boundingBox);

            DrawOffset = new Vector2(position.X - 20, position.Y - 20);

            AnimationManager animations = new AnimationManager(30, 30, 14, 1);
            animations.AddAnimation(new Animation(11, 0, 0, false, 0));
            animations.AnimationSpeed = 12;
            Animations = animations;
        }
        public override void Update(float delta)
        { 
            base.Update(delta);

            Vector2 position = new Vector2(Position.X, Position.Y);
            if (m_dontHurtOwnerCounter > 0)
            {
                m_dontHurtOwnerCounter -= delta;
            }
            if (m_victim == null)
		    {
		        if (Direction.Y != 0)
		        {
			        position.Y += Direction.Y * Speed * delta * m_height;
		        } 
		        else if (Direction.X != 0)
		        {
			        position.X +=  Direction.X * m_moveVector.X * Speed * delta;
			        position.Y = m_groundPos - m_height;
		        }

		        m_moveVector.Y -= delta * m_gravity;

                m_height += m_moveVector.Y * delta * 80;

		        if (m_height < 0){
			        m_moveVector.Y *= -1 * m_bounce;
			        m_moveVector.X *= m_bounce * 0.90f;
			        m_height = 0; 
			        if (m_moveVector.Y > 1){
						SoundCenter.Instance.Play(SoundNames.SoldierStickyGrenadeBounce);
			        }
		        }
		        if (position.X < 0 || position.Y < 0 || position.X > 1024 || position.Y > 768)
		        {
			        Bounce(null);
		        } else 
		        {
			        Position = position;
		        }
	        } 
	        else 
	        {
                if (!m_victim.IsAlive)
                {
                    Drop();
                }
                else
                {
                    Position = m_victim.Position - m_victimOffset;
                }
	        }
	        if (IsRemoved)
	        {
		        Vector2 ourPos = Position;
                List<Unit> units = EntityManager.Units;
		        foreach (Unit unit in units)
		        {
			        Vector2 unitPos = unit.Position;
			        Vector2 dif = ourPos - unitPos;
			        if (dif.Length() < m_aoe){
				        unit.ApplyDamage(Damage, Owner);
			        }
		        }
		        Effects.Spawn(Position, "Grenade_Explosion");

				SoundCenter.Instance.Play(SoundNames.SoldierStickyGrenadeExplosion);

                if (m_victim != null && m_buff != null){
			        m_buff.TellRemoved();
			        m_victim = null;
		        }
	        }
        }
        public override void LateUpdate(float delta)
        {
            if (m_victim != null)
            {
                if (m_victim.IsRemoved)
                {
                    m_moveVector.Y = 1;
                    m_moveVector.X = 50;
                    Direction = m_victim.Direction;
                    m_victim = null;
                }
            }
        }
        public override void OnCollide(AABB other)
        {
            if (m_victim != null) return;

	        if (other.Owner == Owner && m_dontHurtOwnerCounter > 0) return;
	        if (other.Owner == m_protected && m_stickcount > 0) return;

            if (other.Owner is Obstacle || other.Owner is Boss && Owner is Boss)
            {
                Bounce(other);
            }
            else if (other.Owner is Unit)
	        {
                Unit unit = other.Owner as Unit;
		        if (unit.IsAlive && unit.IsVisible)
		        {
                    if (m_PlayStickSound)
					    SoundCenter.Instance.Play(SoundNames.SoldierStickyGrenadeStick);
                    m_victim = unit;
			        Animations.Pause();

                    StickyGrenadeBuff buff = new StickyGrenadeBuff(this, m_victim);
			        m_victim.AddBuff(buff);
			        m_buff = buff;

			        m_victimOffset = m_victim.Position - Position;
		        }
            }

            
        }
        public void Bounce(AABB col)
        {
	        float epsilon = 0.05f;

			SoundCenter.Instance.Play(SoundNames.SoldierStickyGrenadeBounce); 
	        
			Vector2 position = new Vector2(Position.X, Position.Y);
        	
	        Vector2 dir = Direction;
            Direction = dir * -1;

	        if(col != null) {
		        if(dir.X < -epsilon)
			        position.X = col.MaxX + 10.1f;
		        else if(dir.X > epsilon)
			        position.X = col.MinX - 10.1f;
		        else if(dir.Y < -epsilon)
			        position.Y = col.MaxY + 10.1f;
		        else
			        position.Y = col.MinY - 10.1f;

		        if((position - Position).Length() > 75)
			        position = Position;
	        }

	        Position = position;
        }
        public void Drop(){
	        if (m_victim != null && Life > 0){
		        m_moveVector.Y = 1;
		        m_moveVector.X = 50;
		        Direction = m_victim.Direction;
		        m_victim = null;
		        Animations.Resume();
                m_buff.TellRemoved();
		        m_buff = null;
	        }
        }
    }
}
