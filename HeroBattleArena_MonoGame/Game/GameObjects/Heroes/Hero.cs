using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HeroBattleArena.Game.GameObjects;

namespace HeroBattleArena.Game.GameObjects
{
    public enum HeroStates
    {
        IDLE,
        BLINK,
        WALKING,
        ATTACKING,
        DYING,
        DEAD,
        SPECIAL1,
        SPECIAL2,
        SPECIAL3,
        SPECIAL4,
        SPECIAL5,
        SPECIAL6,
        SPECIAL7,
        SPECIAL8,
        SPECIAL9,
        SPECIAL10,
        SPECIAL11,
        SPECIAL12,
        NOTHING
    }

    public class Hero : Unit
    {

#region Fields

        private static float s_InvincibleDuration = 0;

        private List<Ability> m_Abilities = new List<Ability>();

        private int m_PlayerID = 0;
        private int m_characterID = 0;
        private float m_Mana = 1;
        private float m_MaxMana = 1;
        private float m_ManaRegenRate = 1;
        private bool m_bCanAttack = true;
        private bool m_bStrafing = false;
        private bool[] m_bSpecialDown = { false, false, false };
        private float m_Silenced = 0;
        private HeroStates m_HeroState = HeroStates.IDLE;
        private float m_AttackCounter = 0;
        private Vector2 m_WeaponOffset = Vector2.Zero;
        private Texture2D m_Portrait;
        private Texture2D m_WinnerPortrait = null;

        private float m_AnimationSync = 0;
        private int m_HeroColor = 0;
        private UnitBar m_ManaBar = null;

        protected int VictoryAnimation = 0;
        private string m_victoryExclamation = "";
        private bool m_IsTutorial = false;

        private float m_RumbleTakeDamage = Configuration.GetValue("Rumble_Take_Damage");
        private float m_RumbleGiveDamage = Configuration.GetValue("Rumble_Give_Damage");

#endregion

#region Properties

        /// <summary>
        /// Gets which type of character the hero is.
        /// </summary>
        public int CharacterID { get { return m_characterID; } protected set { m_characterID = value; } }
		/// <summary>
		/// Gets the heroes abilities, should not be used
		/// to add new abilities, use AddAbility instead.
		/// </summary>
		public List<Ability> Abilities { get { return m_Abilities; } }
        /// <summary>
        /// Gets or sets the portrait of the hero which
        /// will show up in the GUI.
        /// </summary>
        public Texture2D Portrait { get { return m_Portrait; } protected set { m_Portrait = value; } }
        /// <summary>
        /// Gets or sets the hero state.
        /// </summary>
        /// 

        public Texture2D WinnerPortrait { get { return m_WinnerPortrait; } protected set { m_WinnerPortrait = value; } }
        public string VictoryExclamation { get { return m_victoryExclamation; } protected set { m_victoryExclamation = value; } }

        public HeroStates HeroState { get { return m_HeroState; } set { m_HeroState = value; } }
        /// <summary>
        /// Gets or sets the hero's current mana, use DecreaseMana()
        /// if you want to reduce the mana.
        /// </summary>
        public float Mana { get { return m_Mana; } protected set { m_Mana = value; } }
        /// <summary>
        /// Gets or sets the hero's max mana.
        /// </summary>
        public float MaxMana { get { return m_MaxMana; } protected set { m_MaxMana = value; } }
        /// <summary>
        /// Gets the mana factor, which is m_Mana/m_MaxMana which will be in
        /// the range of 0.0 to 1.0.
        /// </summary>
        public float ManaFactor { get { return m_Mana / m_MaxMana; } }
        /// <summary>
        /// Gets or sets the hero's mana regeneration rate.
        /// This rate is mana/second.
        /// </summary>
        public float ManaRegenerationRate { get { return m_ManaRegenRate; } protected set { m_ManaRegenRate = value; } }
        /// <summary>
        /// Gets or sets the hero's attack counter, used to limit attacks/second.
        /// </summary>
        public float AttackCounter { get { return m_AttackCounter; } protected set { m_AttackCounter = value; } }
        /// <summary>
        /// Gets or sets whether the hero is strafing, when the hero is strafing
        /// the character won't turn around.
        /// </summary>
        public bool IsStrafing { get { return m_bStrafing; } set { m_bStrafing = value; } }
        /// <summary>
        /// Gets whether the hero is silenced or not.
        /// Use SetSilence(ammount) to silence a hero.
        /// </summary>
        public bool IsSilenced { get { return m_Silenced > 0; } }
        /// <summary>
        /// Gets the time remaining in seconds of the silence.
        /// </summary>
        public float SilenceDuration { get { return m_Silenced; } }
        /// <summary>
        /// Gets or sets whether the hero is allowed to attack or not.
        /// </summary>
        public bool CanAttack { get { return m_bCanAttack; } set { m_bCanAttack = value; } }
        /// <summary>
        /// Gets or sets the playerID which decides who controls this hero.
        /// </summary>
        public int PlayerOwner { get { return m_PlayerID; } set { m_PlayerID = value; } }
        /// <summary>
        /// Used to synchronize the animations.
        /// </summary>
        public float AnimationSync { get { return m_AnimationSync; } set { m_AnimationSync = value; } }
        /// <summary>
        /// Gets or sets the hero color, different colors are used for
        /// different game modes.
        /// </summary>
        public int HeroColor { get { return m_HeroColor; } set { m_HeroColor = value; } }
        /// <summary>
        /// Gets or sets the weapon offset for the hero.
        /// </summary>
        public Vector2 WeaponOffset { get { return m_WeaponOffset; } protected set { m_WeaponOffset = value; } }

#endregion

#region Initialization

        public Hero()
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            Name = "Hero";

            if (s_InvincibleDuration == 0)
                s_InvincibleDuration = Configuration.GetValue("General_Invincible_Respawn");

            // All heroes on layer 2.
            Layer = 2;
            m_ManaBar = new UnitBar(Graphics.GetTexture("mana_bar"));
            //m_ManaBar.Offset = new Vector2(-30, 0);

            m_IsTutorial = (ScreenManager.GetInstance().GetScreen(ScreenManager.GetInstance().NumScreens - 1) is TutorialScreen);
        }

#endregion

#region Methods

        public void MaximizeEverything(bool changeMap)
        {
            Health = MaxHealth;
            m_Mana = m_MaxMana;
            

            foreach (Ability ability in Abilities)
                ability.Reset();
            if (changeMap)
            {
                HeroState = HeroStates.IDLE;
                foreach (Buff buff in Buffs)
                    buff.Remove();
            }
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            
            if (!m_IsTutorial)
                m_ManaBar.Update(delta);

	        if(!IsAlive) return;

	        // Update abilities.
            for(int i = 0; i < m_Abilities.Count; ++i)
	        {
                m_Abilities[i].Update(delta);
	        }

            bool gameWon = false;
            if (GameMode.Instance != null)
            {
                gameWon = GameMode.Instance.GameWon;
                if (gameWon)
                {
                    Animations.ChangeAnimation(VictoryAnimation * 4);
                    StopMoving();
                    Show();
                    return;
                }
            }

	
	        if (m_Silenced > 0){
		        m_Silenced -= delta;
		        if (m_Silenced < 0){
			        m_Silenced = 0;
		        }
	        }

	        if (m_HeroState == HeroStates.WALKING || m_HeroState == HeroStates.IDLE)
            {
		        // Get the zero vector.
		        Vector2 movement = Vector2.Zero;
                

                if (!IsStunned && !gameWon)
		        {
			        // Get the movement directions.
			        if(Input.GetPlayerState(m_PlayerID).IsPressed(InputCommand.Down))
			        {
				        movement.Y = 1;
			        } 
			        if (Input.GetPlayerState(m_PlayerID).IsPressed(InputCommand.Up))
			        {
				        movement.Y = -1;
			        }
			        if (Input.GetPlayerState(m_PlayerID).IsPressed(InputCommand.Right))
			        {
				        movement.X = 1;
			        } 
			        if (Input.GetPlayerState(m_PlayerID).IsPressed(InputCommand.Left))
			        {
				        movement.X = -1;
			        }
			        // Check if we are moving.
			        if (movement.X == 0 || movement.Y == 0) 
                    {
				        m_HeroState = HeroStates.IDLE;
			        } 
                    else 
                    {
				        // Normalize the direction.
                        this.m_HeroState = HeroStates.WALKING;
				        movement.Normalize();
			        }

			        if (m_bCanAttack)
                    {
				        if (Input.GetPlayerState(m_PlayerID).WasPressed(InputCommand.Attack))
				        {
                            this.m_HeroState = HeroStates.ATTACKING;
                            this.m_AttackCounter = 0.0f;
				        }

                        if(Input.GetPlayerState(m_PlayerID).WasPressed(InputCommand.RightShoulder)){
                            ShowMana(1);
                        }

                        if (Input.GetPlayerState(m_PlayerID).WasPressed(InputCommand.LeftShoulder))
                        {
                            ShowHp(1);
                        }
				
				        if (!IsSilenced)
				        {
					        if(Input.GetPlayerState(m_PlayerID).WasPressed(InputCommand.Special1)) 
                            {
						        if(m_Abilities.Count > 0) 
                                {
							        m_Abilities[0].Activate();
                                    if (GameMode.Instance != null)
                                        ShowMana(1);
						        }
					        }
					        if(Input.GetPlayerState(m_PlayerID).WasPressed(InputCommand.Special2)) 
                            {
						        if(m_Abilities.Count > 1) 
                                {
							        m_Abilities[1].Activate();
                                    if (GameMode.Instance != null)
                                        ShowMana(1);
						        }
					        }
					        if(Input.GetPlayerState(m_PlayerID).WasPressed(InputCommand.Special3)) 
                            {
						        if(m_Abilities.Count > 2) 
                                {
							        m_Abilities[2].Activate();
                                    if (GameMode.Instance != null)
                                        ShowMana(1);
						        }
					        }
				        }
			        }
		        }
		        // Set the movement.
		        if (Control)
			        this.Movement = movement;
	        }

	        if (Input.GetPlayerState(m_PlayerID).IsPressed(InputCommand.Special1)) 
		        m_bSpecialDown[0] = true;
            else 
		        m_bSpecialDown[0] = false;

	        if (Input.GetPlayerState(m_PlayerID).IsPressed(InputCommand.Special2)) 
		        m_bSpecialDown[1] = true;
            else 
		        m_bSpecialDown[1] = false;

	        if (Input.GetPlayerState(m_PlayerID).IsPressed(InputCommand.Special3)) 
		        m_bSpecialDown[2] = true;
            else 
		        m_bSpecialDown[2] = false;

	        if (m_Mana < m_MaxMana)
	        {
		        m_Mana += m_ManaRegenRate * delta;
	        } 
            else 
            {
		        m_Mana = m_MaxMana;
	        }
        }

        public override void Draw()
        {
            base.Draw();
            m_ManaBar.Draw(Position, m_Mana, m_MaxMana);
        }

        public void ShowMana(float time)
        {
            m_ManaBar.Show(time);
        }

        protected void AddAbility(Ability ability)
        {
            if (m_Abilities.Count >= 3)
                throw new Exception("No more than three abilities allowed!");
            ability.Initialize();
            m_Abilities.Add(ability);
        }

        public void DecreaseMana(float amount)
        {
            m_Mana -= amount;
            if (m_Mana < 0)
                m_Mana = 0;
        }

        public bool GetSpecialDown(int num)
        {
            return m_bSpecialDown[num];
        }

        public override void Revive()
        {
            base.Revive();

            SetInvincibility(s_InvincibleDuration);
	
	        // Reset the abilities.
            for(int i = 0; i < m_Abilities.Count; ++i)
	        {
		        m_Abilities[i].Reset();
	        }

	        m_Mana = m_MaxMana;
	        m_Silenced = 0;
	        m_AttackCounter = 0;
	        m_bCanAttack = true;
	        m_bStrafing = false;

            base.ChangeDirection(Directions.Down);
	        Animations.ChangeAnimation((int)HeroStates.IDLE*4 + (int)Directions.Down);
            m_HeroState = HeroStates.IDLE;
        }

        public override void ApplyDamage(float damage, Unit source)
        {
            if (!IsInvincible) Input.GetPlayerState(m_PlayerID).SetRumble(m_RumbleTakeDamage);
            base.ApplyDamage(damage, source);
        }

        public override void Die()
        {
            base.Die();

            m_bCanAttack = true;
            m_bStrafing = false;
            m_Silenced = 0.0f;
            m_HeroState = GameObjects.HeroStates.IDLE;
            m_AttackCounter = 0.0f;
        }

        public void SetSilence(float duration)
        {
            if(duration > m_Silenced)
                m_Silenced = duration;
        }

#endregion

    }
}
