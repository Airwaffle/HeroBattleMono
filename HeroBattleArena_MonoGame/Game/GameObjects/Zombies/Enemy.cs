using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace HeroBattleArena.Game.GameObjects
{
    public class Enemy : Unit
    {

        public static Random RANDOM = new Random();
        // The time it takes for the enemy to dissappear after his death
        private float m_decayTime = 10;
        private float m_noticeRange = Configuration.GetValue("Zombie_Follow_Range");
        private float m_attackRange = 0;
        private bool m_isMoving = true;
        // The currently active behavior.
        Behavior m_currentBehavior;
        // Used when switching between behaviors.
        Behavior m_nextBehavior;

        // List over heroes the zombie wont aggro
        private List<Hero> m_wontAggro = new List<Hero>();
        private Unit m_owner = null;

        private float m_chaseMulti = Configuration.GetValue("Zombie_Chase_Speed_Multiplication");

        public void Decay(float time) { m_decayTime -= time; }
        public float DecayTime { set { m_decayTime = value; } }

        public float ChaseMulti { get { return m_chaseMulti; } }
        public float NoticeRange { get { return m_noticeRange; } set { m_noticeRange = value; } }
        public float AttackRange { get { return m_attackRange; } set { m_attackRange = value; } }
        public Boolean IsMoving { get { return m_isMoving; } set { m_isMoving = value; } }
        public Unit Owner { get { return m_owner; } set { m_owner = value; } }
        public bool CheckAggro(Hero hero1)
        {
            foreach (Hero hero2 in m_wontAggro)
            {
                if (hero1 == hero2)
                    return false;
            }
            return true;
        }

        public void AddWontAggro(Hero hero){
            m_wontAggro.Add(hero);
        }

        public override void Update(float delta)
        {
            base.Update(delta);
            if (!IsAlive) {
		        Decay(delta);
		        if (m_decayTime < 0){
			        if (Convert.ToInt32(m_decayTime * 1) % 2 == 0){
				        Show();
			        } else {
				        Hide();
			        }
			        if (m_decayTime <= -1.50){
				        Remove();
			        }
        			
		        }
	        } else if (!IsStunned){
		        if( m_currentBehavior != null) {
			        m_currentBehavior.Update(delta);
		        }
		        // Check if we want to change behavior.
		        if( m_nextBehavior != null) {
			        m_currentBehavior = m_nextBehavior;
			        m_nextBehavior = null;
		        }
	        }
	        // Don't move if stunned or dead.
	        if(!IsAlive || IsStunned)
		        Movement = new Vector2();

        }

        public void ChangeBehavior(Behavior nextBehavior)
        {
            // Check if there is already a behavior, if it is -
            // it needs to be added the next frame, mainly to
            // avoid deletion of the current behavior is that
            // one wants to change behavior.
            if (m_currentBehavior != null)
            {
                // If this was called twice before the update
                // use the latest added behavior.

#warning the following lines arn't needed anymore right?
//if (m_nextBehavior != null)
//{
//    delete m_nextBehavior;
//}

                // Set the next behavior so that the behavior
                // can be changed next update.
                m_nextBehavior = nextBehavior;
            }
            else
            {
                // There was no behavior already,
                // use this one directly.
                m_currentBehavior = nextBehavior;
                m_nextBehavior = null;
            }
        }
        public void GetPath(Vector2 destination){
            Vector2 move = destination - Position;
	        move.Normalize();
	        Movement = move;
        }
        public bool IsInsideScreen()
        {
	        if (Position.X > 0 + AIManager.SegmentWidth && 
		        Position.X < 1024 - AIManager.SegmentWidth &&
		        Position.Y > 0 + AIManager.SegmentHeight &&
		        Position.Y < 768 - AIManager.SegmentHeight){
		        return true;
	        }
	        return false;
        }
    }
}
