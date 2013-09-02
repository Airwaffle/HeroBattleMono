using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace HeroBattleArena.Game
{
	public class AnimationCycleParameters
	{
		public int StartX, StartY;
		public int NumFrames;
	}

	public class AnimationManagerParameters
	{
		public List<AnimationCycleParameters> Cycles = new List<AnimationCycleParameters>();

		public int FrameRate = 24;
		public int FrameWidth, FrameHeight;
		public int FramesX, FramesY;
	}

    public class AnimationManager
    {
#region Fields
        private uint m_FramesX          = 0;
        private uint m_FramesY          = 0;
        private uint m_CurrentAnimation = 0;

        private List<Animation> m_Animations = new List<Animation>();
        private Rectangle m_Rectangle        = new Rectangle();

        private float m_AnimationSpeed          = 1.0f/8.0f;
        private float m_StandardAnimationSpeed  = 1.0f/8.0f;

        private float m_Time    = 0.0f;
        private bool m_bPlaying = true;
#endregion

#region Properties
        public Rectangle Rectangle
        {
            get { return m_Rectangle; }
        }

        public float AnimationSpeed
        {
            get { return m_AnimationSpeed; }
            set { m_AnimationSpeed = 1.0f/value; }
        }

        public float StandardAnimationSpeed
        {
            get { return m_StandardAnimationSpeed; }
            set { m_StandardAnimationSpeed = 1.0f/value; m_AnimationSpeed = 1.0f/value; }
        }

        public bool IsPlaying
        {
            get { return m_bPlaying; }
        }
#endregion

        public AnimationManager()
        {
        }

        public AnimationManager(int frameWidth, int frameHeight, uint framesX, uint framesY)
        {
            m_Rectangle.Width = frameWidth;
            m_Rectangle.Height = frameHeight;
            m_FramesX = framesX;
            m_FramesY = framesY;
        }

#region Methods
        public void Update(float delta)
        {
            if (m_bPlaying && m_Animations.Count > 0)
            {
                m_Time += delta;
                while (m_Time >= m_AnimationSpeed)
                {
                    m_Time -= m_AnimationSpeed;
                    m_Animations[(int)m_CurrentAnimation].NextFrame(m_FramesX, m_FramesY);

                    if (m_Animations[(int)m_CurrentAnimation].bChangeToParent)
                    {
                        ReturnToIdle();
                    }
                }
                m_Rectangle.X = (int)m_Animations[(int)m_CurrentAnimation].CurrentX*m_Rectangle.Width;
                m_Rectangle.Y = (int)m_Animations[(int)m_CurrentAnimation].CurrentY*m_Rectangle.Height;
            }
        }

        public void ChangeAnimation(int animation)
        {
            if (animation < m_Animations.Count)
            {
                if (m_CurrentAnimation != animation)
                {
                    m_Time = 0;
                    m_Animations[(int)m_CurrentAnimation].Reset();
                    m_CurrentAnimation = (uint)animation;
                    if (m_AnimationSpeed != m_StandardAnimationSpeed)
                    {
                        m_AnimationSpeed = m_StandardAnimationSpeed;
                    }
                }
            }
        }

        public void SetAnimationDuration(float seconds)
        {
            m_AnimationSpeed = seconds/m_Animations[(int)m_CurrentAnimation].NumFrames;
        }

        public void ReturnToIdle()
        {
            ChangeAnimation((int)m_Animations[(int)m_CurrentAnimation].IdleDirection);
        }

        public void AddAnimation(Animation animation)
        {
            m_Animations.Add(animation);
        }

        public uint GetCurrentAnimation()
        {
            return m_CurrentAnimation;
        }

        public void Pause()
        {
            m_bPlaying = false;
        }

        public void Resume()
        {
            m_bPlaying = false;
        }

#endregion
    }
}