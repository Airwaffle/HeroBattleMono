using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public enum Directions
    {
        Up      = 0,
        Left    = 1,
        Down    = 2,
        Right   = 3,
    }

    public class Unit : Entity
    {
        private static float s_InvincibleHitTime = 0;
        private static float s_HealthBarShowTime = 1;
#region Fields
        private List<Buff> m_Buffs      = new List<Buff>();
        private List<Buff> m_TempBuffs  = new List<Buff>();

        private float m_Health      = 100;
        private float m_MaxHealth   = 100;
        private bool m_bAlive       = true;
        private float m_Invincible  = 0.0f;
        private float m_Stunned     = 0.0f;
        private float m_Blink       = 0.0f;
        private float m_BlinkSpeed;
        private float m_InvisibleBlinkSpeed;
        private bool m_bControl     = true;
        private bool m_bShadow      = true;

#if DEBUG
        private bool m_bDrawFeet    = false;
        private Texture2D m_TexFeet;
#endif

        private AABB m_MovementBB = new AABB();

        private float m_AttackDamage = 0;
        private float m_AttackSpeed = 0;
        private float m_Armor = 0;
        private float m_MoveSpeed = 0;

        // Default properties for the unit
        private float m_DefAttackDamage = 0;
        private float m_DefAttackSpeed = 0;
        private float m_DefArmor = 0;
        private float m_DefMoveSpeed = 0;

        private Directions  m_AnimationDirection = Directions.Down;
        private Vector2     m_Direction = new Vector2(0, 1);

        private int m_Team = 0;

        private Vector2 m_Movement  = Vector2.Zero;
        private Vector2 m_Center    = Vector2.Zero;

        private Texture2D m_TexShadow = null;

        private UnitBar m_HealthBar = null;
#endregion

#region Properties

        /// <summary>
        /// Gets the units healthbar.
        /// The damage will inflicted depends on the attacked units armor.
        /// </summary>
        protected UnitBar HealthBar { get { return m_HealthBar; } }
        /// <summary>
        /// Gets or sets the default attack damage
        /// for the unit.
        /// The damage will inflicted depends on the attacked units armor.
        /// </summary>
        public float DefaultAttackDamage { get { return m_DefAttackDamage; } protected set { m_DefAttackDamage = value; } }
        /// <summary>
        /// Gets or sets the default attack speed of the unit.
        /// The attack speed is the delay in seconds before the next attack.
        /// </summary>
        public float DefaultAttackSpeed { get { return m_DefAttackSpeed; } protected set { m_DefAttackSpeed = value; } }
        /// <summary>
        /// Gets or sets the default armor of the unit.
        /// The armor is applied to the received damage with the following formula:
        /// totalDamage = incomingDamage - armor
        /// totalDamage can't be less then 1.
        /// </summary>
        public float DefaultArmor { get { return m_DefArmor; } protected set { m_DefArmor = value; } }
        /// <summary>
        /// Gets or sets the defulat movement speed of the unit.
        /// This speed is in pixels per second.
        /// </summary>
        public float DefaultMoveSpeed { get { return m_DefMoveSpeed; } protected set { m_DefMoveSpeed = value; } }
        /// <summary>
        /// Gest or sets the attack damage of the unit for one frame,
        /// then it reverts to its default value.
        /// </summary>
        public float AttackDamage { get { return m_AttackDamage; } set { m_AttackDamage = value; } }
        /// <summary>
        /// Gets or sets the attack speed of the unit for one frame,
        /// then it reverts to its default value.
        /// </summary>
        public float AttackSpeed { get { return m_AttackSpeed; } set { m_AttackSpeed = value; } }
        /// <summary>
        /// Gets or sets the armor for the unit for one frame,
        /// then it reverts to its default value.
        /// </summary>
        public float Armor { get { return m_Armor; } set { m_Armor = value; } }
        /// <summary>
        /// Gets or sets the movement speed for the unit for one frame,
        /// then it reverts to its default value.
        /// </summary>
        public float MoveSpeed { get { return m_MoveSpeed; } set { m_MoveSpeed = value; } }
        /// <summary>
        /// Gets or sets the current health of the unit.
        /// Only inherited objects have access to set.
        /// </summary>
        public float Health { get { return m_Health; } protected set { m_Health = value; } }
        /// <summary>
        /// Gets or sets the max health of the unit.
        /// Use ApplyDamage instead of setting this directly.
        /// </summary>
        public float MaxHealth { get { return m_MaxHealth; } protected set { m_MaxHealth = value; } }
        /// <summary>
        /// Gets the units health in the range 0.0-1.0.
        /// </summary>
        public float HealthFactor { get { return m_Health / m_MaxHealth; } }
        /// <summary>
        /// Offset from the position, could be
        /// used by weapons to have as a center.
        /// </summary>
        public Vector2 Center { get { return m_Center; } protected set { m_Center = value; } }
        /// <summary>
        /// Gets or sets the current position for
        /// the unit, this is usually the center of
        /// the feets.
        /// </summary>
        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                Vector2 difference = value - base.Position;
                m_Center += difference;
                base.Position = value;
            }
        }
        /// <summary>
        /// Gets the feet collision box for the unit.
        /// </summary>
        public AABB MovementBB { get { return m_MovementBB; } }
        /// <summary>
        /// Gets whether the unit is alive, a dead unit
        /// doesn't necessarily have to be removed from
        /// the game, but can be respawned.
        /// </summary>
        public bool IsAlive { get { return m_bAlive; } }
        /// <summary>
        /// Checks if the unit is currently invincible,
        /// which means it won't take damage.
        /// </summary>
        public bool IsInvincible { get { return m_Invincible > 0; } }
        /// <summary>
        /// Gets the time remaining in invincibility mode.
        /// </summary>
        public float Invincible { get { return m_Invincible; } }
        /// <summary>
        /// Gets whether the unit is stunned or not,
        /// if it's stunned, it can't do a thing.
        /// </summary>
        public bool IsStunned { get { return m_Stunned > 0; } }
        /// <summary>
        /// Gets the remaining stun duration.
        /// </summary>
        public float StunDuration { get { return m_Stunned; } }
        /// <summary>
        /// Gets whether the unit can control its movement.
        /// </summary>
        public bool Control { get { return m_bControl; } set { m_bControl = value; } }
        /// <summary>
        /// Gets the current animation direction.
        /// </summary>
        public Directions AnimationDirection { get { return m_AnimationDirection; } }
        /// <summary>
        /// Gets the unit vector direction.
        /// </summary>
        public Vector2 Direction { get { return m_Direction; } }
        /// <summary>
        /// Gets or sets the movement vector for the unit.
        /// </summary>
        public Vector2 Movement { get { return m_Movement; } set { m_Movement = value; } }
        /// <summary>
        /// Gets or sets whether the unit has a shadow.
        /// </summary>
        public bool Shadow { get { return m_bShadow; } set { m_bShadow = value; } }
        /// <summary>
        /// Gets or sets the units team.
        /// </summary>
        public int Team { get { return m_Team; } set { m_Team = value; } }
        /// <summary>
        /// Gets the buffs attached to this unit.
        /// </summary>
        public List<Buff> Buffs { get { return m_Buffs; } }

#endregion

#region Initialization

        public override void Initialize()
        {
            base.Initialize();

            Name = "Unit";

            m_MovementBB.Owner = this;
            m_MovementBB.LayerMask = AABBLayers.LayerHeroFeet;
            m_MovementBB.CollisionMask = AABBLayers.CollisionHeroFeet;
            AddAABB(m_MovementBB);

#if DEBUG
            m_bDrawFeet     = Configuration.GetValue("Debug_Show_Feet_Collision") > 0;
            m_TexFeet       = Graphics.GetTexture("debug_feet");
#endif
            m_BlinkSpeed    = Configuration.GetValue("General_Blink_Speed");
            m_InvisibleBlinkSpeed = Configuration.GetValue("General_Blink_Speed")/2;
            m_TexShadow = Graphics.GetTexture("shadow_below_unit");
            m_HealthBar = new UnitBar(Graphics.GetTexture("life_bar"));

            if (s_InvincibleHitTime == 0)
                s_InvincibleHitTime = Configuration.GetValue("General_Invincible_After_Hit");

            ChangeDirection(Directions.Down);
        }

#endregion

#region Methods

        public override void Update(float delta)
        {
	        // Reset combative properties.
	        m_AttackDamage	= m_DefAttackDamage;
	        m_AttackSpeed	= m_DefAttackSpeed;
	        m_Armor			= m_DefArmor;
	        m_MoveSpeed		= m_DefMoveSpeed;

	        base.Update(delta);
            m_HealthBar.Update(delta);
	
	        // Update stun duration.
	        if(m_Stunned > 0) {
		        m_Stunned -= delta;
		        if(m_Stunned < 0)
			        m_Stunned = 0;
	        }

            if (m_Blink > 0)
            {
                m_Blink -= delta;
                if ((int)(m_Blink * m_BlinkSpeed) % 2 == 0)
                {
					Color = Color.White;
                }
                else
                {
					Color = Color.Red;
                }
                if (m_Blink <= 0)
                {
					Color = Color.White;
                    m_Blink = 0;
                }
            }

            if (m_Invincible > 0)
            {
                m_Invincible -= delta;
                if (m_Invincible > s_InvincibleHitTime)
                {
                    if ((int)(m_Invincible * m_InvisibleBlinkSpeed) % 2 == 0)
                    {
                        Color = Color.Silver;
                    }
                    else
                    {
                        Color = Color.White;
                    }
                }
                if (m_Invincible <= 0)
                {
                    Color = Color.White;
                    m_Invincible = 0;
                }
            }
            else if (Color == Color.Silver)
            {
                Color = Color.White;
            }

	        // Add buffs
            while(m_TempBuffs.Count > 0)
            {
		        m_Buffs.Add(m_TempBuffs[0]);
                m_TempBuffs.RemoveAt(0);
	        }
            // Update buffs.
	        for(int i = 0; i < m_Buffs.Count; ++i)
            {
		        m_Buffs[i].Update(delta);
	        }
            // Remove buffs.
            for (int i = 0; i < m_Buffs.Count; )
            {
                if (m_Buffs[i].IsRemoved)
                {
                    m_Buffs[i].Deactivate();
                    m_Buffs.RemoveAt(i);
                }
                else ++i;
            }
        }

        public override void Draw()
        {
            base.Draw();
            m_HealthBar.Draw(Position, m_Health, m_MaxHealth);
	        if (m_bShadow && IsAlive && IsVisible) 
            {
		        Graphics.Draw(
                    m_TexShadow,
                    Position + new Vector2(-12, 4),
                    null, 1.5f, Color.White);
	        }
#if DEBUG
	        if(m_bDrawFeet) 
            {
		        Graphics.Draw(
			        m_TexFeet, 
			        new Vector2(m_MovementBB.MinX, m_MovementBB.MinY),
                    new Rectangle(0,0, (int)m_MovementBB.Width, (int)m_MovementBB.Height),
			        50, Color.White);
	        }
#endif
        }

        public void AddBuff(Buff buff)
        {
            if (m_bAlive)
            {
                buff.Activate();
                m_TempBuffs.Add(buff);
            }
        }

        public void SetInvincibility(float duration)
        {
            if (duration > m_Invincible)
                m_Invincible = duration;
        }

        public void RemoveInvincibility()
        {
            m_Invincible = 0;
        }

        public void SetStun(float duration)
        {
            if (duration > m_Stunned)
                m_Stunned = duration;
        }

        public void RemoveStun()
        {
            m_Stunned = 0;
        }

        public void SetBlink(float duration)
        {
            if(duration > m_Blink)
                m_Blink = duration;
        }

        public void RemoveBlink()
        {
            m_Blink = 0;
        }

        public void ChangeDirection(Directions direction)
        {
            switch (direction)
            {
                case Directions.Up:
                    m_Direction.X = 0;
                    m_Direction.Y = -1;
                    break;
                case Directions.Left:
                    m_Direction.X = -1;
                    m_Direction.Y = 0;
                    break;
                case Directions.Right:
                    m_Direction.X = 1;
                    m_Direction.Y = 0;
                    break;
                case Directions.Down:
                    m_Direction.X = 0;
                    m_Direction.Y = 1;
                    break;
            }
            m_AnimationDirection = direction;
        }

        public virtual void Attack()
        {
            foreach (Buff buff in m_Buffs)
            {
                buff.OnTargetAttack();
            }
        }

        public void StopMoving()
        {
            m_Movement.X = m_Movement.Y = 0;
        }

        public virtual void ApplyDamage(float damage, Unit source)
        {
	        if(!IsAlive) return;
	        if(IsInvincible) return;

            // Check if it's a team game and then if the damage source
            // is the same team, we should not damage our friends if
            // friendly fire is off.
            if (GameMode.Instance.TeamGame || GameMode.Instance is GM_Zombie && source != null)
            {
                if (!GameMode.Instance.FriendlyFire)
                {
                    if (m_Team == source.Team)
                        return;
                }
            }

		    damage -= m_Armor;
		    if(damage < 1)
			    damage = 1;
            Console.WriteLine(damage + " damage dealth on " + Name);
			GameMode.Instance.OnUnitDamaged(source, damage);
           
		    m_Health -= damage;
            ShowHp(s_HealthBarShowTime);

            //SoundCenter.Instance.Play(SoundNames.AllImpact);

		    if (m_Health <= 0)
		    {
				GameMode.Instance.OnUnitKilled(source, this);

			    m_Health = 0;
			    Die();
		    } else {
			    SetInvincibility(s_InvincibleHitTime);
			    SetBlink(s_InvincibleHitTime);
		    }
        }

        public void ShowHp(float time)
        {
            m_HealthBar.Show(time);
        }

        public bool ReduceLife(float damage, Unit source)
        {
            if (!IsAlive) return false;
            if (IsInvincible) return false;

            if (m_Health > 1){
                m_Health -= damage;
                m_HealthBar.Show(s_HealthBarShowTime);
                GameMode.Instance.OnUnitDamaged(source, damage);
                return true;
            }
            return false;            
        }

        public void Heal(float healAmmount)
        {
            if (healAmmount <= 0) return; // Avoid getting damaged.
            m_Health += healAmmount;
            m_HealthBar.Show(s_HealthBarShowTime);
            if (m_Health > m_MaxHealth)
                m_Health = m_MaxHealth;
        }

        public void DispellBuffs()
        {
            while (m_Buffs.Count > 0)
            {
                m_Buffs[0].Deactivate();
                m_Buffs.RemoveAt(0);
            }
        }

        public virtual void Die()
        {
            DispellBuffs();

            m_bAlive = false;
            m_Health = 0;
            m_Invincible = 0;
            m_Blink = 0;
            m_Movement = Vector2.Zero;
            IsSolid = false;
        }

        public virtual void Revive()
        {
            DispellBuffs();

            m_Health = m_MaxHealth;
            m_Movement = Vector2.Zero;

            m_Invincible = 0;
            m_Stunned = 0;
            m_Blink = 0;

            IsSolid = true;
            m_bAlive = true;
            m_bControl = true;

            ChangeDirection(Directions.Down);

            Show();
        }

        public override void OnCollide(AABB other)
        {
            base.OnCollide(other);

            foreach (Buff buff in m_Buffs)
                buff.OnTargetCollide(other);
        }

#endregion

    }
}
