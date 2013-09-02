using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class Arthur : Hero
    {
        private const int LASER = 7;
        private const int SHIELDED = 9;

        private enum SpecialState
        {
            Begin,
            Channel,
            End,
        }
        private SpecialState[] m_SpecialState = new SpecialState[4];

        public Arthur()
        {
            for (int i = 0; i < 4; ++i)
                m_SpecialState[i] = SpecialState.Begin;
        }

        public override void Initialize()
        {
            base.Initialize();

            Name = "Arthur";
            CharacterID = 0;
			Scale = 2;

	        Vector2 position = Position;
	        // Set feet bounding box.
	        MovementBB.MinX = position.X - 20;
	        MovementBB.MaxX = position.X + 20;
	        MovementBB.MinY = position.Y - 10;
	        MovementBB.MaxY = position.Y + 10;

	        // Create collision bounding box.
	        AABB boundingBox = new AABB();
	        boundingBox.Owner = this;
	        boundingBox.LayerMask = AABBLayers.LayerHeroBody;
	        boundingBox.CollisionMask = AABBLayers.CollisionHeroBody;
	        boundingBox.MinX = position.X - 20;
	        boundingBox.MaxX = position.X + 20;
	        boundingBox.MinY = position.Y - 40;
	        boundingBox.MaxY = position.Y + 10;
	        AddAABB(boundingBox);

            DrawOffset = new Vector2(-100, -120);

	        /////////////////////////////////////////////////////////////////////////////////
	        AnimationManager animations = new AnimationManager(100, 100, 12, 23);

	        //IDLE
	        animations.AddAnimation(new Animation(1,0,0,false, (int)HeroStates.IDLE*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(1,0,4,false, (int)HeroStates.IDLE*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(1,0,8,false, (int)HeroStates.IDLE*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(1,1,0,false, (int)HeroStates.IDLE*4 + (int)Directions.Right)); 

	        //BLINK
	        animations.AddAnimation(new Animation(1,0,0,false, (int)HeroStates.IDLE*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(1,0,4,false, (int)HeroStates.IDLE*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(1,0,8,false, (int)HeroStates.IDLE*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(1,1,0,false, (int)HeroStates.IDLE*4 + (int)Directions.Right));

	        //WALK
	        animations.AddAnimation(new Animation(4,0,0,false, (int)HeroStates.IDLE*4 + (int)Directions.Up));
	        animations.AddAnimation(new Animation(4,0,4,false, (int)HeroStates.IDLE*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(4,0,8,false, (int)HeroStates.IDLE*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(4,1,0,false, (int)HeroStates.IDLE*4 + (int)Directions.Right)); 

	        //ATTACK
	        animations.AddAnimation(new Animation(5,1,4,false, (int)HeroStates.IDLE*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(5,2,0,false, (int)HeroStates.IDLE*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(5,2,5,false, (int)HeroStates.IDLE*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(5,3,0,false, (int)HeroStates.IDLE*4 + (int)Directions.Right));

	        //DYING
	        animations.AddAnimation(new Animation(4,3,5,true, (int)HeroStates.DEAD*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(4,3,5,true, (int)HeroStates.DEAD*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(4,3,5,true, (int)HeroStates.DEAD*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(4,3,5,true, (int)HeroStates.DEAD*4 + (int)Directions.Right));

	        //DEAD
	        animations.AddAnimation(new Animation(1,3,8,false, (int)HeroStates.DEAD*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(1,3,8,false, (int)HeroStates.DEAD*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(1,3,8,false, (int)HeroStates.DEAD*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(1,3,8,false, (int)HeroStates.DEAD*4 + (int)Directions.Right));

	        //LASERBEGIN
	        animations.AddAnimation(new Animation(9,4,0,true, LASER*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(9,5,0,true, LASER*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(9,6,0,true, LASER*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(9,7,0,true, LASER*4 + (int)Directions.Right));

	        //LASER
	        animations.AddAnimation(new Animation(2,4,7,false, (int)HeroStates.IDLE*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(2,5,7,false, (int)HeroStates.IDLE*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(2,6,7,false, (int)HeroStates.IDLE*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(2,7,7,false, (int)HeroStates.IDLE*4 + (int)Directions.Right));

	        //SHIELDING
	        animations.AddAnimation(new Animation(12,8,0,true, SHIELDED*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(12,9,0,true, SHIELDED*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(12,10,0,true, SHIELDED*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(12,11,0,true, SHIELDED*4 + (int)Directions.Right));

	        //SHIELDED
	        animations.AddAnimation(new Animation(3,8,9,false, (int)HeroStates.IDLE*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(3,9,9,false, (int)HeroStates.IDLE*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(3,10,9,false, (int)HeroStates.IDLE*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(3,11,9,false, (int)HeroStates.IDLE*4 + (int)Directions.Right));

	        //TELEPORTING
	        animations.AddAnimation(new Animation(10,12,0,true, (int)HeroStates.IDLE*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(10,12,0,true, (int)HeroStates.IDLE*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(10,12,0,true, (int)HeroStates.IDLE*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(10,12,0,true, (int)HeroStates.IDLE*4 + (int)Directions.Right));

	        //RETURNING
	        animations.AddAnimation(new Animation(6,13,0,false, (int)HeroStates.IDLE*4 + (int)Directions.Up)); 
	        animations.AddAnimation(new Animation(6,13,6,false, (int)HeroStates.IDLE*4 + (int)Directions.Left)); 
	        animations.AddAnimation(new Animation(6,14,0,false, (int)HeroStates.IDLE*4 + (int)Directions.Down)); 
	        animations.AddAnimation(new Animation(6,14,6,false, (int)HeroStates.IDLE*4 + (int)Directions.Right));

            //VICTORY
            animations.AddAnimation(new Animation(13, 15, 0, true, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(13, 15, 0, true, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(13, 15, 0, true, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(13, 15, 0, true, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            Animations = animations;
            VictoryAnimation = 12;
            VictoryExclamation = "ex_arthur_wins";
	        /////////////////////////////////////////////////////////////////////////////////

	        DefaultMoveSpeed = Configuration.GetValue("Arthur_Walk_Speed");
	        DefaultAttackDamage = Configuration.GetValue("Arthur_Attack_Damage");
	        DefaultAttackSpeed = Configuration.GetValue("Arthur_Attack_Speed");
	        DefaultArmor = Configuration.GetValue("Arthur_Armor");
	        MaxMana = Configuration.GetValue("Arthur_Max_Mana");
	        Mana = Configuration.GetValue("Arthur_Max_Mana");
	        Health = Configuration.GetValue("Arthur_Max_Health");
	        MaxHealth = Configuration.GetValue("Arthur_Max_Health");
	        ManaRegenerationRate = Configuration.GetValue("Arthur_Mana_Regen_Rate");

            /*
	        if (HeroColor == 0)
		        Texture = Graphics.GetTexture("arthur_sprite");
	        else if (HeroColor == 1)
		        Texture = Graphics.GetTexture("arthur_sprite_v2");
	        else if (HeroColor == 2)
		        Texture = Graphics.GetTexture("arthur_sprite_v3");
	        else
		        Texture = Graphics.GetTexture("arthur_sprite_v4");
            */

            if (HeroColor == 0)
                Texture = Graphics.LoadTexture("Characters/Arthur/Sprite_Sheet_Arthur");
	        else if (HeroColor == 1)
                Texture = Graphics.LoadTexture("Characters/Arthur/Sprite_Sheet_Arthur_Green");
	        else if (HeroColor == 2)
                Texture = Graphics.LoadTexture("Characters/Arthur/Sprite_Sheet_Arthur_Red");
	        else
                Texture = Graphics.LoadTexture("Characters/Arthur/Sprite_Sheet_Arthur_Brown");

            Portrait = Graphics.GetTexture("arthur_portrait");
            WinnerPortrait = Graphics.GetTexture("score_icon_arthur");

            ChangeAnimation(2);

            AddAbility(new CounterTeleport(this));
	        AddAbility(new SilenceShine(this));
	        AddAbility(new LaserBlade(this));
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if (GameMode.Instance != null)
                if (GameMode.Instance.GameWon) return;

            AnimationSync += delta;

	        if (IsAlive)
            {
		        // WALK
		        if (HeroState == HeroStates.WALKING || HeroState == HeroStates.IDLE)
                {
                    Vector2 movement = Movement;

			        if (movement.Y > 0)
                    {
                        ChangeAnimation((int)HeroStates.WALKING*4 + (int)Directions.Down);
                        ChangeDirection(Directions.Down);
			        } 
			        else if (movement.Y < 0)
			        {
                        ChangeAnimation((int)HeroStates.WALKING*4 + (int)Directions.Up);
                        ChangeDirection(Directions.Up);
			        } 
			        else if (movement.X > 0)
                    {
                        ChangeAnimation((int)HeroStates.WALKING*4 + (int)Directions.Right);
                        ChangeDirection(Directions.Right);
			        } 
			        else if (movement.X < 0)
			        {
                        ChangeAnimation((int)HeroStates.WALKING*4 + (int)Directions.Left);
                        ChangeDirection(Directions.Left);
			        } 
			        else 
			        {
				        if (Animations.GetCurrentAnimation() >= 8)
					        Animations.ReturnToIdle();
			        }
		        } 
                else if (HeroState == HeroStates.ATTACKING)
		        {
			        if (AttackCounter == 0)
                    {
				        Animations.ChangeAnimation((int)HeroStates.ATTACKING* 4 + (int)AnimationDirection);
				        Animations.SetAnimationDuration(AttackSpeed);
				        Attack();
			        }
			        AttackCounter += delta;

			        if (AttackCounter >= AttackSpeed)
			        {
				        AttackCounter = 0;
				        HeroState = HeroStates.IDLE;
				        Animations.ReturnToIdle();
			        }
		        } 
		        else if (HeroState == HeroStates.SPECIAL1)
		        {
			        Animations.ChangeAnimation(8* 4 + (int)AnimationDirection);
			        Animations.SetAnimationDuration(1.0f);
			        HeroState = HeroStates.SPECIAL2;
			        StopMoving();
		        } 
                else if (HeroState == HeroStates.SPECIAL2)
		        {
			        AttackCounter += delta;
			        if (AttackCounter >= 1.0)
			        {
				        AttackCounter = 0;
				        HeroState = HeroStates.SPECIAL3;
				        Animations.ChangeAnimation(9* 4 + (int)AnimationDirection);
			        }
		        } 
                else if (HeroState == HeroStates.SPECIAL3)
		        {
			        //setAttackCounter(getAttackCounter() + delta);
			        //if (getAttackCounter() >= Configuration::getValue("Arthur_Counter_Buff_Duration")){
			        //	setAttackCounter(0.0f);
			        //	setHeroState(IDLE);
			        //	removeStunned();
			        //	removeInvincible();
			        //	std::vector<Buff*> buffs = getBuffs();
			        //	setDirection(DOWN);
			        //	getAnimations()->changeAnimation(IDLE* 4 + DOWN);
			        //	/*for (int i = 0; buffs.size(); i++){
			        //		if (dynamic_cast<CounterTeleportBuff*>(buffs[i])){
			        //			buffs[i]->deactivate;
			        //		}
			        //	}*/
			        //}
		        }
                else if (HeroState == HeroStates.SPECIAL4)
                {
			        Animations.ChangeAnimation(10* 4 + (int)AnimationDirection);
			        Animations.SetAnimationDuration(Configuration.GetValue("Arthur_Counter_Disappear_Animation_Duration"));
			        HeroState = HeroStates.SPECIAL5;
		        } 
                else if (HeroState == HeroStates.SPECIAL5)
                {
			        AttackCounter += delta;
			        if (AttackCounter >= Configuration.GetValue("Arthur_Counter_Disappear_Animation_Duration"))
			        {
				        AttackCounter = 0.0f;
				        HeroState = HeroStates.SPECIAL11;
				        Hide();
				        IsSolid = false;
				        List<Buff> buffs = Buffs;
				        for (int i = 0; i < buffs.Count; ++i)
                        {
					        if (buffs[i] is StickyGrenadeBuff)
						        buffs[i].Remove();
				        }
			        }
		        } 
                else if (HeroState == HeroStates.SPECIAL6)
                {
			        Animations.ChangeAnimation(11* 4 + (int)AnimationDirection);
			        Animations.SetAnimationDuration(Configuration.GetValue("Arthur_Counter_Appear_Animation_Duration"));
			        HeroState = HeroStates.SPECIAL7;
			        IsSolid = true;
		        } 
                else if (HeroState == HeroStates.SPECIAL7)
                {
			        AttackCounter += delta;
			        if (AttackCounter >= Configuration.GetValue("Arthur_Counter_Appear_Animation_Duration"))
			        {
				        AttackCounter = 0.0f;
				        HeroState = HeroStates.IDLE;
                        RemoveInvincibility();
			        }
		        } 
                else if (HeroState == HeroStates.SPECIAL8)
                {
			        Animations.ChangeAnimation(6* 4 + (int)AnimationDirection);
			        Animations.SetAnimationDuration(0.5f);
			        HeroState = HeroStates.SPECIAL9;
		        } 
                else if (HeroState == HeroStates.SPECIAL9)
                {
                    AttackCounter += delta;
			        if (AttackCounter >= 0.5f)
			        {
				        AttackCounter = 0.0f;
				        HeroState = HeroStates.SPECIAL10;
			        }
		        } 
                else if (HeroState == HeroStates.SPECIAL10)
                {
			        Animations.ChangeAnimation(7* 4 + (int)AnimationDirection);
		        }
		
		        if (HeroState == HeroStates.IDLE && AnimationSync >= 1.0f && !IsStunned)
			        Animations.ChangeAnimation((int)HeroStates.BLINK* 4 + (int)AnimationDirection);
	        } 
            else 
            {
		        Animations.Update(delta);
	        }

	        if (AnimationSync >= 1.0f)
                AnimationSync -= 1.0f;
        }

		public override void Attack()
		{
			ArthurSword sword = new ArthurSword(this, Direction);

			SoundCenter.Instance.Play(SoundNames.ArthurAttack);
	
			sword.Position = Position + Direction * 25; 
			sword.Life = AttackSpeed;
			sword.Damage = AttackDamage;
			EntityManager.Spawn(sword);

			StopMoving();
		}

        public override void Die()
        {
			SoundCenter.Instance.Play(SoundNames.ArthurDeath);

            base.Die();
            Animations.ChangeAnimation((int)HeroStates.DYING*4 + (int)Directions.Down);
        }
    }
}
