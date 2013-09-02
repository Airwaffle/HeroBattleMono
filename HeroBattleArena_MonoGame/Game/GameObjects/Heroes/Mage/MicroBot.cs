using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class MicroBot: Entity
    {

         float m_angle;
         float m_life;
         bool m_isDisappearing;
         bool m_bLeftSide;
         Hero m_owner;
         static float PI			= 3.14159265f;
         static float PI_OVER_2 = PI / 2.0f;
         static float PI_2 = PI * 2.0f;
         static float PI_OVER_4 = PI / 4.0f;

        public MicroBot(Hero owner, bool left)
        {
            m_bLeftSide = left;
            m_owner = owner;
        }
        static float wrapAngle(float radians) 
        { 
            while (radians < -PI) 
            { 
                radians += PI_2; 
            } 
            while (radians > PI) 
            { 
                radians -= PI_2; 
            } 
                 return radians; 
    } 

        public override void Initialize()
        {
	        m_angle = (float)Math.Atan2(m_owner.Direction.X, m_owner.Direction.Y);
	        m_life = Configuration.GetValue("ability_microbot_duration");
	        AnimationManager animations = new AnimationManager(70,70,29,2);
	        animations.AddAnimation(new Animation(13,0,0,true,1));
	        animations.AddAnimation(new Animation(24,0,13,false,0));
	        animations.AddAnimation(new Animation(8,0,0,false,1));
	        animations.AnimationSpeed = 30;
	        Animations = animations;
    
	        IsSolid = false;

	        Vector2 position = Position;
	        DrawOffset = new Vector2(position.X - 39, position.Y - 35);

	        Texture = Graphics.GetTexture("microbot_ball");
}

        /*int MicroBot.Layer() 
        {
	        return 2;
        }*/

        public void Attack()
        {
	        float damage = Configuration.GetValue("ability_microbot_damage");
	        float speed  = Configuration.GetValue("ability_microbot_speed");

	        MageProjectile proj = new MageProjectile(m_owner, m_owner.Direction);
	        proj.Damage = (m_owner.AttackDamage);
	        proj.Speed = speed;
	        proj.Position = Position; 
	        EntityManager.Spawn(proj);

	//SoundCenter::playSound(SoundCenter::SOUND_mageAttack);
        }   

        public override void Update(float delta)
        {
	         m_life -= delta;
             float speed = PI_2 * Configuration.GetValue("ability_microbot_rotationspeed");
	         float distance = Configuration.GetValue("ability_microbot_distance");
	         float microbotLifeTime = Configuration.GetValue("ability_microbot_disappear_time");

	         base.Update(delta);

	    if (m_life - microbotLifeTime <= 0 && !m_isDisappearing)
        {
		     Animations.ChangeAnimation(2);
		     Animations.AnimationSpeed/*(AnimationDuration)*/ =  Configuration.GetValue("ability_microbot_disappear_time");
		     m_isDisappearing = true;
	    }

	         Vector2 position = m_owner.Position;

	        float desiredAngle = (float)Math.Atan2(m_owner.Direction.Y, m_owner.Direction.X);
	        desiredAngle += PI;

	        float difference = wrapAngle(desiredAngle - m_angle);
	        difference = (difference < -speed ? -speed : (difference > speed ? speed : difference));
	        m_angle += difference * speed * delta;

	        float tempAngle = 0;
	        if(m_bLeftSide)
		        tempAngle = m_angle - PI_OVER_4;
	        else
		        tempAngle = m_angle + PI_OVER_4;

	            Vector2 offset = new Vector2((float)Math.Cos(tempAngle)*distance, (float)Math.Sin(tempAngle)*distance) + new Vector2(0,-30);

	            Position = (position + offset);	
}


    }
}
