using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HeroBattleArena.Game.GameObjects
{
    public class Aztec : Hero
    {
        // The Jawelin throw offsets calculated from the Aztec's position.
	    // There is four, one for each direction. For ultimate control!
        Vector2[] m_spearOffsets = {new Vector2(-20,0), new Vector2(-40,-15), new Vector2(20,0), new Vector2(40,-15)};
	    
        float m_slowSpearAnimationTime = Configuration.GetValue("Slow_Down_Spear_Animation_Time");
	    float m_spearCreationDelay = Configuration.GetValue("Slow_Down_Spear_Animation_Create_Spear");
        
        public Aztec()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            Name = "Aztec";
            CharacterID = 2;
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

            DrawOffset = new Vector2(-75, -90);

            //Loads all the animations
            ///////////////////////////////////////////////////////////////////////////
	        AnimationManager animations = new AnimationManager(75, 75, 12, 11);

            //IDLE
            animations.AddAnimation(new Animation(1, 0, 1, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 0, 5, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 0, 9, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 1, 1, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //BLINK
            animations.AddAnimation(new Animation(1, 0, 1, true, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 1, 4, true, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 1, 5, true, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 1, 6, true, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //WALK
            animations.AddAnimation(new Animation(4, 0, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 0, 4, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 0, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 1, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //ATTACK
            animations.AddAnimation(new Animation(9, 1, 7, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(9, 2, 4, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(9, 3, 1, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(9, 3, 10, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //DYING
            animations.AddAnimation(new Animation(4, 4, 7, true, (int)HeroStates.DEAD * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 4, 7, true, (int)HeroStates.DEAD * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 4, 7, true, (int)HeroStates.DEAD * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 4, 7, true, (int)HeroStates.DEAD * 4 + (int)Directions.Right));

            //DEAD
            animations.AddAnimation(new Animation(1, 4, 10, false, (int)HeroStates.DEAD * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 4, 10, false, (int)HeroStates.DEAD * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 4, 10, false, (int)HeroStates.DEAD * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 4, 10, false, (int)HeroStates.DEAD * 4 + (int)Directions.Right));

            // FLURRY FIRST
            animations.AddAnimation(new Animation(3, 5, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(3, 6, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(3, 7, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(3, 8, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            // FLURRY ÖVERGÅNG
            animations.AddAnimation(new Animation(5, 5, 3, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(5, 6, 3, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(5, 7, 3, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(5, 8, 3, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            // FLURRY LAST
            animations.AddAnimation(new Animation(4, 5, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 6, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 7, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 8, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            // THROW SPEAR
            animations.AddAnimation(new Animation(4, 9, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 9, 4, false, (int)HeroStates.IDLE * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 9, 8, false, (int)HeroStates.IDLE * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 10, 0, false, (int)HeroStates.IDLE * 4 + (int)Directions.Right));

            //VICTORY
            animations.AddAnimation(new Animation(9, 11, 0, true, (int)HeroStates.DEAD * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(9, 11, 0, true, (int)HeroStates.DEAD * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(9, 11, 0, true, (int)HeroStates.DEAD * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(9, 11, 0, true, (int)HeroStates.DEAD * 4 + (int)Directions.Right));

            VictoryAnimation = 10;
            VictoryExclamation = "ex_quertzali_wins";

            Animations = animations;
	        /////////////////////////////////////////////////////////////////////////////////

            // Sets default values.
	        DefaultMoveSpeed = Configuration.GetValue("Aztec_Walk_Speed");
	        DefaultAttackDamage = Configuration.GetValue("Aztec_Attack_Damage");
	        DefaultAttackSpeed = Configuration.GetValue("Aztec_Attack_Speed");
	        DefaultArmor = Configuration.GetValue("Aztec_Armor");
	        MaxMana = Configuration.GetValue("Aztec_Max_Mana");
	        Mana = Configuration.GetValue("Aztec_Max_Mana");
	        Health = Configuration.GetValue("Aztec_Max_Health");
	        MaxHealth = Configuration.GetValue("Aztec_Max_Health");
	        ManaRegenerationRate = Configuration.GetValue("Aztec_Mana_Regen_Rate");

            /*
	        if (HeroColor == 0)
                Texture = Graphics.GetTexture("aztec_sprite");
	        else if (HeroColor == 1)
                Texture = Graphics.GetTexture("aztec_sprite_v2");
	        else if (HeroColor == 2)
                Texture = Graphics.GetTexture("aztec_sprite_v3");
	        else
                Texture = Graphics.GetTexture("aztec_sprite_v4");
            */
            if (HeroColor == 0)
                Texture = Graphics.LoadTexture("Characters/Quentzali/Sprite_Sheet_Aztec");
            else if (HeroColor == 1)
                Texture = Graphics.LoadTexture("Characters/Quentzali/sprite_sheet_aztec_green");
            else if (HeroColor == 2)
                Texture = Graphics.LoadTexture("Characters/Quentzali/sprite_sheet_aztec_blue");
            else
                Texture = Graphics.LoadTexture("Characters/Quentzali/sprite_sheet_aztec_yellow");


            Portrait = Graphics.GetTexture("aztek_portrait");
            WinnerPortrait = Graphics.GetTexture("score_icon_aztec");

            ChangeAnimation(2);

            AddAbility(new GhostShield(this));
            AddAbility(new JawelinThrow(this));
            AddAbility(new Flurry(this));
            
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
                 if (HeroState == HeroStates.WALKING || HeroState == HeroStates.IDLE)
                 {
                     Vector2 movement = Movement;

                     if (movement.Y > 0)
                     {
                         ChangeAnimation((int)HeroStates.WALKING * 4 + (int)Directions.Down);
                         ChangeDirection(Directions.Down);
                     }
                     else if (movement.Y < 0)
                     {
                         ChangeAnimation((int)HeroStates.WALKING * 4 + (int)Directions.Up);
                         ChangeDirection(Directions.Up);
                     }
                     else if (movement.X > 0)
                     {
                         ChangeAnimation((int)HeroStates.WALKING * 4 + (int)Directions.Right);
                         ChangeDirection(Directions.Right);
                     }
                     else if (movement.X < 0)
                     {
                         ChangeAnimation((int)HeroStates.WALKING * 4 + (int)Directions.Left);
                         ChangeDirection(Directions.Left);
                     }
                     else
                     {
                         // In case the hero isn't moving, the Idle animation will play.
                         if (Animations.GetCurrentAnimation() >= 8)
                             Animations.ReturnToIdle();
                     }
                 }
                 else if (HeroState == HeroStates.ATTACKING)
                 {
                     if (AttackCounter == 0)
                     {
                         // Activates the attack animation and calls the attack function. 
                         Animations.ChangeAnimation((int)HeroStates.ATTACKING * 4 + (int)AnimationDirection);
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
                 // ANIMATIONS RELATED TO FLURRY
                 else if (HeroState == HeroStates.SPECIAL1)
                 {
                     Animations.ChangeAnimation(6 * 4 + (int)AnimationDirection);
                     Animations.SetAnimationDuration(0.25f);
                     HeroState = HeroStates.SPECIAL2;
                     StopMoving();
                 }
                 else if (HeroState == HeroStates.SPECIAL2)
                 {
                     AttackCounter += delta;
                     if (AttackCounter >= 0.5)
                     {
                         AttackCounter = 0;
                         Animations.ChangeAnimation(7 * 4 + (int)AnimationDirection);
                         Animations.SetAnimationDuration(0.5f);
                         HeroState = HeroStates.SPECIAL3;
                     }
                 }
                 else if (HeroState == HeroStates.SPECIAL3)
                 {
                     AttackCounter += delta;
                     if (AttackCounter >= 0.5)
                     {
                         AttackCounter = 0;
                         HeroState = HeroStates.SPECIAL4;
                         Animations.ChangeAnimation(8 * 4 + (int)AnimationDirection);
                         Animations.SetAnimationDuration(0.5f);
                     }
                 }

                 else if (HeroState == HeroStates.SPECIAL4)
                 {
                     
                 }
                 // ANIMATIONS RELATED TO JAWELIN THROW
                 else if (HeroState == HeroStates.SPECIAL5)
                 {
                     Animations.ChangeAnimation(9 * 4 + (int)AnimationDirection);
                     Animations.SetAnimationDuration(m_slowSpearAnimationTime);
                     HeroState = HeroStates.SPECIAL6;
                     StopMoving();
                 }
                 else if (HeroState == HeroStates.SPECIAL6)
                 {
                     AttackCounter += delta;

			        if (AttackCounter >= m_spearCreationDelay && AttackCounter <= m_spearCreationDelay + 1)
                    {
                        AttackCounter += 1;
				        JawelinProjectile spear = new JawelinProjectile(this, Direction);
				        spear.Position = Position + m_spearOffsets[(int)AnimationDirection]; 
                        EntityManager.Spawn(spear);	
			        }
			        if (AttackCounter >= m_slowSpearAnimationTime + 1)
			        {
				        AttackCounter = 0;
				        HeroState = HeroStates.IDLE;
			        }
                 }

                 if (HeroState == HeroStates.IDLE && AnimationSync >= 1.0f && !IsStunned)
                     Animations.ChangeAnimation((int)HeroStates.BLINK * 4 + (int)AnimationDirection);
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
			SoundCenter.Instance.Play(SoundNames.AztecAttack);
            AztecSpear spear = new AztecSpear(this, Direction);        	
            spear.Position = Position + Direction * 50.0f; 
            spear.Life = AttackSpeed;
            spear.Damage = AttackDamage;
            EntityManager.Spawn(spear);

             StopMoving();
         }

         public override void Die()
         {
			 SoundCenter.Instance.Play(SoundNames.AztecDeath);
             base.Die();
             Animations.ChangeAnimation((int)HeroStates.DYING * 4 + (int)Directions.Down);
         }
    }
}
