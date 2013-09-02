using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class Ability
    {
        private Hero m_Owner;
        private float m_Cooldown = 1;
        private float m_CooldownCount = 0;
        private float m_ManaCost = 0;
        private Texture2D m_Icon;

        /// <summary>
        /// Gets the hero owner of the ability.
        /// </summary>
        public Hero Owner { get { return m_Owner; } }
        /// <summary>
        /// Gets or sets the mana cost for activating the ability.
        /// </summary>
        public float ManaCost { get { return m_ManaCost; } protected set { m_ManaCost = value; } }
        /// <summary>
        /// Gets the cooldown percentage in range 0.0-1.0.
        /// Sets the cooldown in seconds.
        /// </summary>
        public float Cooldown { get { return m_CooldownCount / m_Cooldown; } protected set { m_Cooldown = value; } }
        /// <summary>
        /// Gets or sets the icon for the ability which can be seen
        /// in the GUI.
        /// </summary>
        public Texture2D Icon { get { return m_Icon; } protected set { m_Icon = value; } }

        /// <summary>
        /// Creates a new ability with default values.
        /// </summary>
        /// <param name="owner">The hero which the ability belongs to.</param>
        public Ability(Hero owner)
        {
            m_Owner = owner;
        }

        /// <summary>
        /// Initialize the ability so that it can
        /// be used.
        /// </summary>
        public virtual void Initialize() { }

        /// <summary>
        /// Mainly used to update the cooldown.
        /// </summary>
        /// <param name="delta">Time elapsed since last frame.</param>
        public virtual void Update(float delta)
        {
            if (m_CooldownCount > 0)
            {
                m_CooldownCount -= delta;

                if (m_CooldownCount < 0)
                    m_CooldownCount = 0;
            }
        }

        /// <summary>
        /// Try to activate the ability, if it succeeds,
        /// mana will be drained from the owner.
        /// </summary>
        /// <returns></returns>
        public virtual bool Activate()
        {
            if (m_CooldownCount == 0 && m_Owner.Mana >= m_ManaCost)
            {
                Use();
                m_Owner.DecreaseMana(m_ManaCost);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Reset the ability to it can be used again.
        /// </summary>
        public virtual void Reset()
        {
            m_CooldownCount = 0;
        }

        /// <summary>
        /// Effectivly sets the cooldown to max,
        /// which result in the ability not being
        /// able to be used.
        /// </summary>
        public virtual void Use()
        {
            m_CooldownCount = m_Cooldown;
        }
    }
}
