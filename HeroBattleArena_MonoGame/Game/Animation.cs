using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroBattleArena.Game
{
    public class Animation
    {
#region Fields
	    public uint CurrentFrame    = 0;
        public uint CurrentX        = 0;
        public uint CurrentY        = 0;
        public uint NumFrames      = 0;
        public uint StartX     = 0;
        public uint StartY     = 0;
        public bool bOneTime         = false;
        public uint  IdleDirection   = 0; // kanske kan ta bort hela den här grejen, men finns möjligtvis lägen då den fortfarande kan vara användbar, så låter den vara kvar ett tag till.

        public bool bChangeToParent = false; // What's this?
#endregion

#region Initialization
        public Animation(uint frames, uint startY, uint startX, bool once, uint idle)
        {
            this.NumFrames = frames;
            this.StartX = startX;
            this.StartY = startY;
            this.CurrentX = startX;
            this.CurrentY = startY;
            this.bOneTime = once;
            this.IdleDirection = idle;
        }
#endregion

#region Methods
        /// <summary>
        /// Get the next frame.
        /// </summary>
        /// <param name="framesX">Maximum number of frames in X</param>
        /// <param name="framesY">Maximum number of frames in Y</param>
        public void NextFrame(uint framesX, uint framesY)
        {
            if (CurrentFrame < NumFrames - 1)
            {
                ++CurrentFrame;
                if (CurrentX < framesX - 1)
                {
                    ++CurrentX;
                }
                else
                {
                    CurrentX = 0;
                    if (CurrentY < framesY - 1)
                    {
                        ++CurrentY;
                    }
                    else
                    {
                        CurrentY = 0;
                    }
                }
            }
            else
            {
                if (bOneTime)
                {
                    bChangeToParent = true;
                }

                CurrentFrame = 0;
                CurrentX = StartX;
                CurrentY = StartY;

            }
        }

        public void Reset()
        {
            CurrentFrame = 0;
            CurrentX = StartX;
            CurrentY = StartY;
            bChangeToParent = false;
        }
#endregion
    }
}
