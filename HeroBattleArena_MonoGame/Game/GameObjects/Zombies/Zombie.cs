using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class Zombie : Enemy
    {
        public enum ZombieStates
        {
            WALKING,
            IDLE,
            ATTACKING,
            FROZEN,
            UNBURROW,
            CONFUSED
        }
        private float m_unburrowSpeed = 3.0f;
        private ZombieStates m_state = ZombieStates.WALKING;
        private Boss m_bossOwner = null;

        public ZombieStates State { get { return m_state; } set { m_state = value; } }
        public float UnburrowSpeed { get { return m_unburrowSpeed; } set { m_unburrowSpeed = value; } }
        public Boss BossOwner { get { return m_bossOwner; } }

        public Zombie(Boss bossOwner)
        {
            m_bossOwner = bossOwner;
        }

        public void CreateOutside()
        {
            Vector2 position = Position;
            Random rnd = new Random();
            int side = rnd.Next(0, 4);
            switch (side)
            {
                case 0:
                    position = new Vector2((float)(rnd.Next(0, 1024)), -50);
                    break;
                case 1:
                    position = new Vector2((float)(rnd.Next(0, 1024)), (float)768 + 50);
                    break;
                case 2:
                    position = new Vector2(-50, (float)(rnd.Next(0, 768)));
                    break;
                case 3:
                    position = new Vector2((float)1024 + 50, (float)(rnd.Next(0, 768)));
                    break;
            }
            Position = position;
            ChangeBehavior(new ZombieOutsideScreen(this));
        }

        public void CreateInside()
        {
            Vector2 position = Position;
            Random rnd = new Random();
            AABB feet = new AABB();

            feet.LayerMask = AABBLayers.LayerHeroFeet;
            feet.CollisionMask = AABBLayers.CollisionHeroFeet;

            List<AABB> boxes;
            do
            {
                position.X = rnd.Next(50, 1024 - 50);
                position.Y = rnd.Next(50, 768 - 80);

                feet.MinX = position.X - 20 - 20 + 20 - 1;
                feet.MaxX = position.X + 20 - 20 + 20 + 1;
                feet.MinY = position.Y - 10 - 20 + 26 - 1;
                feet.MaxY = position.Y + 10 - 20 + 26 + 1;

                boxes = Collision.GetCollidingBoxes(feet);
                if (boxes.Count > 0)
                {
                    position.X = 9; 
                }

            } while (boxes.Count > 0);

            Position = position;
            ChangeBehavior(new ZombieEmergingFromGround(this));
        }

        public void CreateAtPosition(Vector2 pos)
        {
            Position = pos;
            ChangeBehavior(new ZombieEmergingFromGround(this));
        }

        public void setStats(int nr, float hp, float move, float dmg, float attspd, float ubspd)
        {

            switch (nr)
            {
                case 0:
                    Texture = Graphics.LoadTexture("Characters/Zombie/Sprite_Sheet_Zombie");
                    break;
                case 1:
                    Texture = Graphics.LoadTexture("Characters/Zombie/Sprite_Sheet_Zombie_2");
                    break;
                case 2:
                    Texture = Graphics.LoadTexture("Characters/Zombie/Sprite_Sheet_Zombie_3");
                    break;
                case 3:
                    Texture = Graphics.LoadTexture("Characters/Zombie/Sprite_Sheet_Zombie_4");
                    break;
                case 4:
                    Texture = Graphics.LoadTexture("Characters/Zombie/Sprite_Sheet_Zombie_Minion");
                    break;
            }

            Name = "Zombie"+nr;

            // Outdated.
            /*
            if (nr == 0)
            {
                Texture = Graphics.GetTexture("zombie_sprite");
                
            } 
            else if (nr == 1)
            {
                Texture = Graphics.GetTexture("zombie_sprite2");
                Name = "Zombie2";
            }
            else if (nr == 2)
            { 
                Texture = Graphics.GetTexture("zombie_sprite3");
                Name = "Zombie3";
            }
            else if (nr == 3)
            {
                Texture = Graphics.GetTexture("zombie_sprite4");
                Name = "Zombie4";
            }
            else if (nr == 4)
            {
                Texture = Graphics.GetTexture("zombie_sprite_minion");
                Name = "Zombie_Minion";
            }
            */

            MaxHealth = hp;
            DefaultMoveSpeed = move;
            DefaultAttackDamage = dmg;
            DefaultAttackSpeed = attspd;
            m_unburrowSpeed = ubspd;
            AttackRange = Configuration.GetValue("Zombie_Attack_Range");
            Health = MaxHealth;
        }

       public override void Initialize()
       {
            base.Initialize();
            Scale = 2;
            Layer = 2.01f;

            Vector2 position = Position;

            // Set feet bounding box.
            AABB movementBB = MovementBB;
            movementBB.MinX = position.X - 20 - 20 + 20;
            movementBB.MaxX = position.X + 20 - 20 + 20;
            movementBB.MinY = position.Y - 10 - 20 + 26;
            movementBB.MaxY = position.Y + 10 - 20 + 26;

            // Create collision bounding box.
            AABB boundingBox = new AABB();
            boundingBox.Owner = this;
            boundingBox.LayerMask = AABBLayers.LayerHeroBody;
            boundingBox.CollisionMask = AABBLayers.CollisionHeroBody;
            boundingBox.MinX = position.X - 20 - 20 + 20;
            boundingBox.MaxX = position.X + 20 - 20 + 20;
            boundingBox.MinY = position.Y - 40 - 20 + 26;
            boundingBox.MaxY = position.Y + 10 - 20 + 26;
            AddAABB(boundingBox);

            DrawOffset = new Vector2(position.X - 80, position.Y - 94);

            /////////////////////////////////////////////////////////////////////////////////
            //OBS: kan flytta till hero's initialize() om alla spritesheet följer samma mall!
            AnimationManager animations = new AnimationManager(80, 80, 13, 11);

            //IDLE
            animations.AddAnimation(new Animation(1, 0, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 1, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 2, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 3, 0, false, (int)Directions.Right));

            //WALK
            animations.AddAnimation(new Animation(12, 0, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(13, 1, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(12, 2, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(13, 3, 0, false, (int)Directions.Right));

            //ATTACK 
            animations.AddAnimation(new Animation(5, 7, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(5, 7, 5, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(5, 8, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(5, 8, 5, false, (int)Directions.Right));

            //DYING
            animations.AddAnimation(new Animation(3, 6, 2, true, 4 * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(3, 6, 2, true, 4 * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(3, 6, 2, true, 4 * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(3, 6, 2, true, 4 * 4 + (int)Directions.Right));

            //DEAD
            animations.AddAnimation(new Animation(1, 6, 5, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 6, 5, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 6, 5, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 6, 5, false, (int)Directions.Right));

            //DYING2
            animations.AddAnimation(new Animation(4, 6, 6, true, 6 * 4 + (int)Directions.Up));
            animations.AddAnimation(new Animation(4, 6, 6, true, 6 * 4 + (int)Directions.Left));
            animations.AddAnimation(new Animation(4, 6, 6, true, 6 * 4 + (int)Directions.Down));
            animations.AddAnimation(new Animation(4, 6, 6, true, 6 * 4 + (int)Directions.Right));

            //DEAD2
            animations.AddAnimation(new Animation(1, 6, 10, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(1, 6, 10, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(1, 6, 10, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(1, 6, 10, false, (int)Directions.Right));

            //UNBURROW
            animations.AddAnimation(new Animation(20, 9, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(20, 9, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(20, 9, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(20, 9, 0, false, (int)Directions.Right));
        	
           // PUKING 
            animations.AddAnimation(new Animation(6, 4, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(7, 4, 6, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(8, 5, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(7, 5, 8, false, (int)Directions.Right));

            //UNBURROW (minion)
            animations.AddAnimation(new Animation(9, 9, 0, false, (int)Directions.Up));
            animations.AddAnimation(new Animation(9, 9, 0, false, (int)Directions.Left));
            animations.AddAnimation(new Animation(9, 9, 0, false, (int)Directions.Down));
            animations.AddAnimation(new Animation(9, 9, 0, false, (int)Directions.Right));

            Animations = animations;
            ///////////////////////////////////////////////////////////////////////////////

            DefaultArmor = 0;
            DecayTime = Configuration.GetValue("Zombie_Decay_Time");
            SetInvincibility(Configuration.GetValue("Zombie_Invincible_Spawn"));
        }
        public override void Update(float delta)
        {
            base.Update(delta);

            if (m_state == ZombieStates.WALKING || m_state == ZombieStates.IDLE)
            {
                
	            if (IsAlive) 
                {
		            if(!IsStunned) 
                    {

                        Vector2 movement = Movement;
                        if (Math.Abs(movement.Y) > Math.Abs(movement.X))
                        {
                            if (movement.Y > 0)
                            {
                                ChangeAnimation(1 * 4 + (int)Directions.Down);
                                ChangeDirection(Directions.Down);

                            }
                            else if (movement.Y < 0)
                            {
                                ChangeAnimation(1 * 4 + (int)Directions.Up);
                                ChangeDirection(Directions.Up);
                            }
                        }
                        else
                        {
                            if (movement.X > 0)
                            {
                                ChangeAnimation(1 * 4 + (int)Directions.Right);
                                ChangeDirection(Directions.Right);

                            }
                            else if (movement.X < 0)
                            {
                                ChangeAnimation(1 * 4 + (int)Directions.Left);
                                ChangeDirection(Directions.Left);

                            }
                        }

		            } // !isStunned()
		            else {
                        ChangeAnimation((int)AnimationDirection); // IDLE
		            }
	            } // !isAlive()
            }
        }

        public override void Die()
        {
  
            SoundCenter.Instance.Play(SoundNames.Zombie);
            Random rnd = new Random();
            int randomNumber = rnd.Next(0,2);
	        if (randomNumber == 0){
                ChangeAnimation(3 * 4 + (int)Directions.Down);
	        } else if (randomNumber == 1){
                ChangeAnimation(5 * 4 + (int)Directions.Down);
	        }
	        Shadow = false;
            base.Die();

            if (Owner != null)
            {
                (Owner as ZombieHero).MinionDied();
            }
        }

        public override void OnCollide(AABB other)
        {
            base.OnCollide(other);
        }

        public override void  Remove()
        {
            if (Owner != null)
            {
                 Die();
                (Owner as ZombieHero).MinionDied();
            }
 	         base.Remove();
        }
        
    }
}
