using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace HeroBattleArena.Game
{
    public abstract class Screen
    {
        public static ContentManager Content;
#region Fields
        private float m_Tick = 0;
        private int m_TickIndex = 0;

        private bool m_bRemoved             = false;
        private bool m_bDrawInBackground    = false;
        private bool m_bUpdateInBackground  = false;
        private bool m_bExitOnEscape        = true;
#endregion

#region Properties
        /// <summary>
        /// Gets or sets whether the screen should draw
        /// itself if it's not the top screen.
        /// Default is false.
        /// </summary>
        public bool DrawInBackground { get { return m_bDrawInBackground; } protected set { m_bDrawInBackground = value; } }
        /// <summary>
        /// Gets or sets whether the screen should update
        /// itself if it's not the top screen.
        /// Default is false.
        /// </summary>
        public bool UpdateInBackground { get { return m_bUpdateInBackground; } protected set { m_bUpdateInBackground = value; } }
        /// <summary>
        /// Gets or sets whether the screen should be exited
        /// if any user clicks back.
        /// Default is true.
        /// </summary>
        public bool ExitOnEscape { get { return m_bExitOnEscape; } protected set { m_bExitOnEscape = value; } }
        /// <summary>
        /// Gets whether the screen should be removed from
        /// the ScreenManager.
        /// </summary>
        public bool IsRemoved { get { return m_bRemoved; } }
#endregion

#region Initialization
        public Screen() { }
        public virtual void Initialize() { }
#endregion

#region Methods

        public virtual void Update(float delta) { }
        public virtual void Draw() { }

        /// <summary>
        /// Called when the screen becomes active, that is
        /// when the screen is becomes the top screen.
        /// </summary>
        public virtual void OnBecomeActive() { ScreenBounce.Reset(); }

        public virtual void Exit()
        {
            m_bRemoved = true;
        }

        public int GetEntryChanged(int currentSelection, int maxSelection, float delta)
        {
	        if(Input.AnyIsPressed(InputCommand.Down))
	        {
		        if(m_TickIndex < 0) {
			        m_TickIndex = 0;
			        m_Tick = 0;
		        }
		        if(m_TickIndex <= 2) {
			        if(m_Tick >= 0)
				        m_Tick -= delta;
			        if(m_Tick <= 0) {
				        if(m_TickIndex < 2)
					        m_TickIndex++;
				        if(m_TickIndex == 1)
					        m_Tick = 0.5f;
				        else
					        m_Tick = 0.15f;

						SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);
				        
				        currentSelection++;
				        if(currentSelection >= maxSelection)
				        {

					        currentSelection = 0;
				        }
			        }
		        }

	        } 
	        else if(Input.AnyIsPressed(InputCommand.Up))
	        {
		        if(m_TickIndex > 0) {
			        m_TickIndex = 0;
			        m_Tick = 0;
		        }
		        if(m_TickIndex >= -2) {
			        if(m_Tick >= 0)
				        m_Tick -= delta;
			        if(m_Tick <= 0) {
				        if(m_TickIndex > -2)
					        m_TickIndex--;
				        if(m_TickIndex == -1)
					        m_Tick = 0.5f;
				        else
					        m_Tick = 0.15f;

						SoundCenter.Instance.Play(SoundNames.InterfaceClickMove);

				        currentSelection--;
				        if(currentSelection < 0)
				        {
					        currentSelection = maxSelection - 1;
				        }
			        }
		        }
	        }
	        else {
		        m_Tick = 0;
		        m_TickIndex = 0;
	        }

	        return currentSelection;
        }

        public bool GetEntrySelected()
        {
	        if(Input.AnyWasPressed(InputCommand.MenuSelect))
	        {
				SoundCenter.Instance.Play(SoundNames.InterfaceClickConfirm);
		        return true;
	        }
	        return false;
        }

#endregion
    }
}
