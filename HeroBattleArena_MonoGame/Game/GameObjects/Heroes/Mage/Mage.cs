using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class Mage : Hero
    {
        private const int ElectricCelerity = 6;
        private const int WhirlWind = 7;

        private float m_projectileSpeed;
        private float m_whirlWindAnimationTime;


        private enum SpecialState
        {
            Begin,
            Channel,
            End,
        }
        private SpecialState[] m_SpecialState = new SpecialState[4];

        public Mage()
        {
            for (int i = 0; i < 4; ++i)
                m_SpecialState[i] = SpecialState.Begin;
        }

        public override void Initialize()
        {
            base.Initialize();

            Name = "Mage";
            CharacterID = 3;
            Scale = 2;

            Vector2 position = Position;
            // Set feet bounding box.
            MovementBB.MinX = position.X - 15;
            MovementBB.MaxX = position.X + 15;
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

            DrawOffset = new Vector2(-100, -135);

            /////////////////////////////////////////////////////////////////////////////////
            //OBS: kan flytta till hero's initialize() om alla spritesheet följer samma mall!
            AnimationManager animations = new AnimationManager(100, 100, 16, 20);

            //IDLE
            animations.AddAnimation(new Animation(4, 0, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 0, 4, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 0, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 0, 12, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //BLINK
            animations.AddAnimation(new Animation(8, 1, 0, true, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(8, 1, 8, true, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(8, 2, 0, true, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(8, 2, 8, true, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //WALK
            animations.AddAnimation(new Animation(4, 0, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 0, 4, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 0, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 0, 12, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //ATTACK
            animations.AddAnimation(new Animation(6, 4, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(6, 4, 6, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(6, 4, 12, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(6, 5, 2, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //DYING
            animations.AddAnimation(new Animation(16, 3, 0, true, (int)HeroStates.DEAD * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(16, 3, 0, true, (int)HeroStates.DEAD * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(16, 3, 0, true, (int)HeroStates.DEAD * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(16, 3, 0, true, (int)HeroStates.DEAD * 4 + (int)Directions.Right));

            //DEAD
            animations.AddAnimation(new Animation(1, 3, 15, false, (int)HeroStates.DEAD * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 3, 15, false, (int)HeroStates.DEAD * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 3, 15, false, (int)HeroStates.DEAD * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 3, 15, false, (int)HeroStates.DEAD * 4 + (int)Directions.Right));

            // ELECTIC CELERITY
            animations.AddAnimation(new Animation(13, 5, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(13, 6, 5, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(13, 7, 2, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(13, 7, 15, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            // WHIRLWIND
            animations.AddAnimation(new Animation(7, 9, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(7, 9, 7, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(7, 10, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(7, 10, 7, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //VICTORY
            animations.AddAnimation(new Animation(32, 11, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(32, 11, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(32, 11, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(32, 11, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            VictoryAnimation = 8;
            VictoryExclamation = "ex_nano_wins";

            Animations = animations;

            DefaultMoveSpeed = Configuration.GetValue("Mage_Walk_Speed");
            DefaultAttackDamage = Configuration.GetValue("Mage_Attack_Damage");
            DefaultAttackSpeed = Configuration.GetValue("Mage_Attack_Speed");
            DefaultArmor = Configuration.GetValue("Mage_Armor");
            MaxMana = Configuration.GetValue("Mage_Max_Mana");
            Mana = Configuration.GetValue("Mage_Max_Mana");
            Health = Configuration.GetValue("Mage_Max_Health");
            MaxHealth = Configuration.GetValue("Mage_Max_Health");
            ManaRegenerationRate = Configuration.GetValue("Mage_Mana_Regen_Rate");
	        m_projectileSpeed = Configuration.GetValue("Mage_Projectile_Speed");
	        m_whirlWindAnimationTime = Configuration.GetValue("Whirl_Wind_Animation_Time");


            WeaponOffset = new Vector2(0, -44);

            // Outdated.
            /*
	        if (HeroColor == 0){
		        Texture = Graphics.GetTexture("mage_sprite");
	        } else if (HeroColor == 1){
		        Texture = Graphics.GetTexture("mage_sprite_v2");
	        } else if (HeroColor == 2){
		        Texture = Graphics.GetTexture("mage_sprite_v3");
	        } else {
		        Texture = Graphics.GetTexture("mage_sprite_v4");
	        }
            */

            if (HeroColor == 0)
                Texture = Graphics.LoadTexture("Characters/Nano/Sprite_Sheet_Mage");
            else if (HeroColor == 1)
                Texture = Graphics.LoadTexture("Characters/Nano/Sprite_Sheet_Mage_Green");
            else if (HeroColor == 2)
                Texture = Graphics.LoadTexture("Characters/Nano/Sprite_Sheet_Mage_Purple");
            else
                Texture = Graphics.LoadTexture("Characters/Nano/Sprite_Sheet_Mage_Gul");

            Portrait = Graphics.GetTexture("nano_portrait");
            WinnerPortrait = Graphics.GetTexture("score_icon_nano");

            ChangeAnimation(2);

            AddAbility(new WhirlWind(this));
            AddAbility(new ElectricCelerity(this));
	        AddAbility(new MicroBotAbility(this));
	
	        Shadow = false; 
         }
    public override void Update(float delta)
    {
	    base.Update(delta);
        if (GameMode.Instance != null)
            if (GameMode.Instance.GameWon) return;

        AnimationSync += delta;

	    if (IsAlive)
        {
		    // Uses the movement vector recivied from the hero input check 
		    // to set the right walk animation.
		    if (HeroState == HeroStates.WALKING || HeroState == HeroStates.IDLE){
    			
                Vector2 movement = Movement;	
    	
			    if (movement.Y > 0){
				    ChangeAnimation((int)HeroStates.WALKING* 4 + (int)Directions.Down);
				    ChangeDirection(Directions.Down);
			    } 
			    else if (movement.Y < 0)
			    {
				    ChangeAnimation((int)HeroStates.WALKING* 4 + (int)Directions.Up);
				    ChangeDirection(Directions.Up);
			    } 
			    else if (movement.X > 0){
				    ChangeAnimation((int)HeroStates.WALKING* 4 + (int)Directions.Right);
				    ChangeDirection(Directions.Right);
			    } 
			    else if (movement.X < 0)
			    {
				    ChangeAnimation((int)HeroStates.WALKING* 4 + (int)Directions.Left);
				    ChangeDirection(Directions.Left);
			    } 
			    else 
			    {
				    // In case the hero isn't moving, the Idle animation will play.
				    if (Animations.GetCurrentAnimation() >= 8){
					    Animations.ReturnToIdle();
				    }
    				
			    }
		    } else if (HeroState == HeroStates.ATTACKING)
		    {
			    // Activates the attack animation and calls the attack function. 
			    if (AttackCounter == 0)
                {
				    Animations.ChangeAnimation((int)HeroStates.ATTACKING* 4 + (int)AnimationDirection);
				    Animations.SetAnimationDuration(AttackSpeed);
				    Attack();
			    }
    			

			    // Plays the attack animation til the counter reaches the time the 
			    // animation has played one time, then resets the heros state to Idle.
			    AttackCounter += delta;

			    if (AttackCounter >= AttackSpeed)
			    {
				    AttackCounter = 0;
				    HeroState = HeroStates.IDLE;
				    Animations.ReturnToIdle();
			    }
		    }
		    //ANIMATIONS RELATED TO ELECTRIC CELERITY 
		    else if (HeroState == HeroStates.SPECIAL1){
    			
			    // Activates the animation in the current direction the player is facing. 
			    // Must be done otherwise the animation wouldn't activate until a 
			    // move-button is pressed.
			    if ((int)AnimationDirection == 0){
				    Animations.ChangeAnimation(ElectricCelerity* 4 + (int)Directions.Up);
				    ChangeDirection(Directions.Up);
			    } else if ((int)AnimationDirection == 1){
				    Animations.ChangeAnimation(ElectricCelerity* 4 + (int)Directions.Left);
				    ChangeDirection(Directions.Left);
			    } else if ((int)AnimationDirection == 2){
				    Animations.ChangeAnimation(ElectricCelerity* 4 + (int)Directions.Down);
				    ChangeDirection(Directions.Down);
			    } else if ((int)AnimationDirection == 3){
				    Animations.ChangeAnimation(ElectricCelerity* 4 + (int)Directions.Right);
				    ChangeDirection(Directions.Right);
			    }

			    // Immediately changes the state to the next. To be able to recive 
			    // and react to input.
			    HeroState = HeroStates.SPECIAL2;

		    } else if (HeroState == HeroStates.SPECIAL2){

			    Vector2 movement = new Vector2();

			    // Reads input and changes the movement vector and animation accordingly.
			    if(Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Down))
			    {
				    movement.Y = 1;
				    Animations.ChangeAnimation(ElectricCelerity* 4 + (int)Directions.Down);
				    ChangeDirection(Directions.Down);
			    } 
			    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Up))
			    {
				    movement.Y = -1;
				    Animations.ChangeAnimation(ElectricCelerity* 4 + (int)Directions.Up);
				    ChangeDirection(Directions.Up);
			    }
			    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Right))
			    {
				    movement.X = 1;
				    Animations.ChangeAnimation(ElectricCelerity* 4 + (int)Directions.Right);
				    ChangeDirection(Directions.Right);
			    } 
			    if (Input.GetPlayerState(PlayerOwner).IsPressed(InputCommand.Left))
			    {
				    movement.X = -1;
				    Animations.ChangeAnimation(ElectricCelerity* 4 + (int)Directions.Left);
				    ChangeDirection(Directions.Left);
			    }
			    Movement = movement;

                if (Input.GetPlayerState(PlayerOwner).WasPressed(InputCommand.Special1))
                {
                    HeroState = HeroStates.IDLE;
                    for (int i = 0; i < Buffs.Count; i++)
                    {
                        if (Buffs[i] is ElectricCelerityBuff)
                        {
                            Buffs[i].Remove();
                        }
                    }
                }
		    } 
    		
		    // ANIMATIONS RELATED TO WHIRLWIND
		    else if (HeroState == HeroStates.SPECIAL3){

			    // Activates the animation. 
			    Animations.ChangeAnimation(WhirlWind* 4 + (int)AnimationDirection);
			    Animations.SetAnimationDuration(m_whirlWindAnimationTime);
			    HeroState = HeroStates.SPECIAL4;
			    StopMoving();
		    } else if (HeroState == HeroStates.SPECIAL4){

			    // Does nothing til the animation should be finished, then resets its state
			    // to Idle. 
			    AttackCounter += delta;
			    if (AttackCounter >= m_whirlWindAnimationTime)
			    {
				    AttackCounter = 0;
				    HeroState = HeroStates.IDLE;
			    }
		    }
    		
		    // Make the mage "blink" (in this case "shine") every fourth second. This 
		    // only occur if the hero is Idle.
		    if (HeroState == HeroStates.IDLE && AnimationSync >= 4.0f){
			    Animations.ChangeAnimation((int)HeroStates.BLINK* 4 + (int)AnimationDirection);
		    }
	    }

	     if (AnimationSync >= 1.0f)
             AnimationSync -= 1.0f;
        }

       // public override void onCollide(AABB other)
        //{
	        
        //}

        public override void Attack()
        {
	        //Creates a projectile. 
			SoundCenter.Instance.Play(SoundNames.MageAttack);

	        base.Attack();

	        MageProjectile proj = new MageProjectile(this, Direction);
	        proj.Position = Position + WeaponOffset; 
	        proj.Damage = AttackDamage;
	        proj.Speed = m_projectileSpeed;

	        EntityManager.Spawn(proj);
	        StopMoving();
        }

        public override void Die()
        {
	        base.Die();
	        Animations.ChangeAnimation((int)HeroStates.DYING* 4 + (int)Directions.Up);
	        Animations.AnimationSpeed = 12;

			SoundCenter.Instance.Play(SoundNames.MageDeath);
        }
    }
}
    
