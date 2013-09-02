using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HeroBattleArena.Game
{
    public enum InputCommand
    {
        Up          = 0,
        Down        = 1,
        Right       = 2,
        Left        = 3,
        Attack      = 4,
        Special1    = 5,
        Special2    = 6,
        Special3    = 7,
        MenuBack    = 8,
        MenuSelect  = 9,
        LeftTrigger = 10,
        RightTrigger= 11,
        LeftShoulder =12,
        RightShoulder=13,

        LAST        = 14,
    }

    public class Input
    {
        private const int   _NUM_PLAYERS = 4;
        private const float _AXIS_THRESHOLD = 0.4f;

        private static Input[] s_Inputs;

        private static KeyboardState s_CurrentKeyboardState = new KeyboardState();
        private static KeyboardState s_LastKeyboardState = new KeyboardState();


        private int m_ID = 0;
        private float m_Rumble = 0;
        private static float s_RumbleDecreaseRate = 0.01f;

        private Keys[] m_Keys = new Keys[(int)InputCommand.LAST];

        private GamePadState m_currentGamePadState = new GamePadState();
        private GamePadState m_lastGamePadState = new GamePadState();

        public Vector2 ThumbStickLeftPos()
        {
            return new Vector2(m_currentGamePadState.ThumbSticks.Left.X, m_currentGamePadState.ThumbSticks.Left.Y*-1); 
        }

        public Vector2 ThumbStickRightPos()
        {
            return new Vector2(m_currentGamePadState.ThumbSticks.Right.X, m_currentGamePadState.ThumbSticks.Right.Y*-1);
        }

        private static int s_LastSelectID = 0;
        /// <summary>
        /// Gets the player who triggered the last AnyWasPressed or AnyIsPressed
        /// that was checked.
        /// </summary>
        public static int LastSelectID { get { return s_LastSelectID;  } }

        /// <summary>
        /// Set the rumble of a controller to a value between 1 and 0. 
        /// </summary>
        public void SetRumble(float rumble) {
            if (rumble > 1) rumble = 1; 
            if (rumble < 0) rumble = 0; 
            m_Rumble = rumble; 
        }

        public static void SetAllRumble(float rumble)
        {
            for (int i = 0; i < _NUM_PLAYERS; ++i)
                s_Inputs[i].SetRumble(rumble);
        }


        public Input()
        {
        }

        public bool IsPressed(InputCommand command)
        {
            // Check keyboard
            if (s_CurrentKeyboardState.IsKeyDown(m_Keys[(int)command]))
                return true;

            // Check game pad buttons
            switch (command)
            {
                case InputCommand.Down:
                    if (m_currentGamePadState.ThumbSticks.Left.Y < -_AXIS_THRESHOLD)
                        return true;
                    break;
                case InputCommand.Up:
                    if (m_currentGamePadState.ThumbSticks.Left.Y > _AXIS_THRESHOLD)
                        return true;
                    break;
                case InputCommand.Right:
                    if (m_currentGamePadState.ThumbSticks.Left.X > _AXIS_THRESHOLD)
                        return true;
                    break;
                case InputCommand.Left:
                    if (m_currentGamePadState.ThumbSticks.Left.X < -_AXIS_THRESHOLD)
                        return true;
                    break;
                case InputCommand.Attack:
                    if (m_currentGamePadState.Buttons.A == ButtonState.Pressed)
                        return true;
                    break;
                case InputCommand.Special1:
                    if (m_currentGamePadState.Buttons.X == ButtonState.Pressed)
                        return true;
                    break;
                case InputCommand.Special2:
                    if (m_currentGamePadState.Buttons.Y == ButtonState.Pressed)
                        return true;
                    break;
                case InputCommand.Special3:
                    if (m_currentGamePadState.Buttons.B == ButtonState.Pressed)
                        return true;
                    break;
                case InputCommand.MenuSelect:
                    if (m_currentGamePadState.Buttons.Start == ButtonState.Pressed)
                        return true;
                    break;
                case InputCommand.MenuBack:
                    if (m_currentGamePadState.Buttons.Back == ButtonState.Pressed)
                        return true;
                    break;

                case InputCommand.LeftTrigger:
                    if (m_currentGamePadState.Triggers.Left > 0)
                        return true;
                    break;
                case InputCommand.RightTrigger:
                    if (m_currentGamePadState.Triggers.Right > 0)
                        return true;
                    break;
                case InputCommand.LeftShoulder:
                    if (m_currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
                        return true;
                    break;
                case InputCommand.RightShoulder:
                    if (m_currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed)
                        return true;
                    break;
            }

            return false;
        }

        public bool WasPressed(InputCommand command)
        {
            // Check keyboard
            if (s_CurrentKeyboardState.IsKeyDown(m_Keys[(int)command]) &&
                s_LastKeyboardState.IsKeyUp(m_Keys[(int)command]))
                return true;

            // Check game pad buttons
            switch (command)
            {
                case InputCommand.Down:
                    if (m_currentGamePadState.ThumbSticks.Left.Y < -_AXIS_THRESHOLD &&
                        m_lastGamePadState.ThumbSticks.Left.Y >= -_AXIS_THRESHOLD)
                        return true;
                    break;
                case InputCommand.Up:
                    if (m_currentGamePadState.ThumbSticks.Left.Y > _AXIS_THRESHOLD &&
                        m_lastGamePadState.ThumbSticks.Left.Y <= _AXIS_THRESHOLD)
                        return true;
                    break;
                case InputCommand.Right:
                    if (m_currentGamePadState.ThumbSticks.Left.X > _AXIS_THRESHOLD &&
                        m_lastGamePadState.ThumbSticks.Left.X <= _AXIS_THRESHOLD)
                        return true;
                    break;
                case InputCommand.Left:
                    if (m_currentGamePadState.ThumbSticks.Left.X < -_AXIS_THRESHOLD &&
                        m_lastGamePadState.ThumbSticks.Left.X >= -_AXIS_THRESHOLD)
                        return true;
                    break;
                case InputCommand.Attack:
                    if (m_currentGamePadState.Buttons.A == ButtonState.Pressed &&
                        m_lastGamePadState.Buttons.A == ButtonState.Released)
                        return true;
                    break;
                case InputCommand.Special1:
                    if (m_currentGamePadState.Buttons.B == ButtonState.Pressed &&
                        m_lastGamePadState.Buttons.B == ButtonState.Released)
                        return true;
                    break;
                case InputCommand.Special2:
                    if (m_currentGamePadState.Buttons.X == ButtonState.Pressed &&
                        m_lastGamePadState.Buttons.X == ButtonState.Released)
                        return true;
                    break;
                case InputCommand.Special3:
                    if (m_currentGamePadState.Buttons.Y == ButtonState.Pressed &&
                        m_lastGamePadState.Buttons.Y == ButtonState.Released)
                        return true;
                    break;
                case InputCommand.MenuSelect:
                    if (m_currentGamePadState.Buttons.Start == ButtonState.Pressed &&
                        m_lastGamePadState.Buttons.Start == ButtonState.Released)
                        return true;
                    break;
                case InputCommand.MenuBack:
                    if (m_currentGamePadState.Buttons.Back == ButtonState.Pressed &&
                        m_lastGamePadState.Buttons.Back == ButtonState.Released)
                        return true;
                    break;
                case InputCommand.LeftTrigger:
                    if (m_currentGamePadState.Triggers.Left > 0 &&
                        m_lastGamePadState.Triggers.Left <= 0)
                        return true;
                    break;
                case InputCommand.RightTrigger:
                    if (m_currentGamePadState.Triggers.Right > 0 &&
                        m_lastGamePadState.Triggers.Right <= 0)
                        return true;
                    break;
                case InputCommand.LeftShoulder:
                    if (m_currentGamePadState.Buttons.LeftShoulder == ButtonState.Pressed &&
                        m_lastGamePadState.Buttons.LeftShoulder == ButtonState.Released)
                        return true;
                    break;
                case InputCommand.RightShoulder:
                    if (m_currentGamePadState.Buttons.RightShoulder == ButtonState.Pressed &&
                        m_lastGamePadState.Buttons.RightShoulder == ButtonState.Released)
                        return true;
                    break;
            }

            return false;
        }

        private void InternalUpdate()
        {
            m_lastGamePadState = m_currentGamePadState;
            m_currentGamePadState = GamePad.GetState((PlayerIndex)m_ID);

            if (m_Rumble >= 0)
            {
                m_Rumble -= s_RumbleDecreaseRate;
                m_Rumble = MathHelper.Clamp(m_Rumble, 0, 1);
                GamePad.SetVibration((PlayerIndex)m_ID, m_Rumble, m_Rumble); 
            }
        
        }

        public void OrkarInteMedAttFixaXML()
        {
            switch(m_ID)
            {
                case 0:
                    m_Keys[(int)InputCommand.Up] = Keys.W;
                    m_Keys[(int)InputCommand.Down] = Keys.S;
                    m_Keys[(int)InputCommand.Left] = Keys.A;
                    m_Keys[(int)InputCommand.Right] = Keys.D;
                    m_Keys[(int)InputCommand.Attack] = Keys.V;
                    m_Keys[(int)InputCommand.Special1] = Keys.F;
                    m_Keys[(int)InputCommand.Special2] = Keys.G;
                    m_Keys[(int)InputCommand.Special3] = Keys.H;
                    m_Keys[(int)InputCommand.MenuBack] = Keys.Escape;
                    m_Keys[(int)InputCommand.MenuSelect] = Keys.Space;
                    m_Keys[(int)InputCommand.LeftTrigger] = Keys.End;
                    m_Keys[(int)InputCommand.RightTrigger] = Keys.End;
                    m_Keys[(int)InputCommand.LeftShoulder] = Keys.X;
                    m_Keys[(int)InputCommand.RightShoulder] = Keys.C;
                    break;
                case 1:
                    m_Keys[(int)InputCommand.Up] = Keys.Up;
                    m_Keys[(int)InputCommand.Down] = Keys.Down;
                    m_Keys[(int)InputCommand.Left] = Keys.Left;
                    m_Keys[(int)InputCommand.Right] = Keys.Right;
                    m_Keys[(int)InputCommand.Attack] = Keys.RightControl;
                    m_Keys[(int)InputCommand.Special2] = Keys.OemComma;
                    m_Keys[(int)InputCommand.Special3] = Keys.OemPeriod;
                    m_Keys[(int)InputCommand.Special1] = Keys.M;
                    m_Keys[(int)InputCommand.MenuBack] = Keys.Back;
                    m_Keys[(int)InputCommand.MenuSelect] = Keys.Enter;
                    m_Keys[(int)InputCommand.LeftTrigger] = Keys.End;
                    m_Keys[(int)InputCommand.RightTrigger] = Keys.End;
                    m_Keys[(int)InputCommand.LeftShoulder] = Keys.PageDown;
                    m_Keys[(int)InputCommand.RightShoulder] = Keys.PageUp;
                    break;
                case 2:
                    m_Keys[(int)InputCommand.Up] = Keys.I;
                    m_Keys[(int)InputCommand.Down] = Keys.K;
                    m_Keys[(int)InputCommand.Left] = Keys.J;
                    m_Keys[(int)InputCommand.Right] = Keys.L;
                    m_Keys[(int)InputCommand.Attack] = Keys.R;
                    m_Keys[(int)InputCommand.Special1] = Keys.T;
                    m_Keys[(int)InputCommand.Special2] = Keys.Y;
                    m_Keys[(int)InputCommand.Special3] = Keys.U;
                    m_Keys[(int)InputCommand.MenuSelect] = Keys.F1;
                    m_Keys[(int)InputCommand.MenuBack] = Keys.F2;
                    m_Keys[(int)InputCommand.LeftTrigger] = Keys.End;
                    m_Keys[(int)InputCommand.RightTrigger] = Keys.End;
                    m_Keys[(int)InputCommand.LeftShoulder] = Keys.End;
                    m_Keys[(int)InputCommand.RightShoulder] = Keys.End;
                    break;
                case 3:
                    m_Keys[(int)InputCommand.Up] = Keys.NumPad8;
                    m_Keys[(int)InputCommand.Down] = Keys.NumPad2;
                    m_Keys[(int)InputCommand.Left] = Keys.NumPad4;
                    m_Keys[(int)InputCommand.Right] = Keys.NumPad6;
                    m_Keys[(int)InputCommand.Attack] = Keys.OemPlus;
                    m_Keys[(int)InputCommand.Special1] = Keys.Divide;
                    m_Keys[(int)InputCommand.Special2] = Keys.Multiply;
                    m_Keys[(int)InputCommand.Special3] = Keys.OemMinus;
                    m_Keys[(int)InputCommand.MenuSelect] = Keys.F3;
                    m_Keys[(int)InputCommand.MenuBack] = Keys.F4;
                    m_Keys[(int)InputCommand.LeftTrigger] = Keys.End;
                    m_Keys[(int)InputCommand.RightTrigger] = Keys.End;
                    m_Keys[(int)InputCommand.LeftShoulder] = Keys.End;
                    m_Keys[(int)InputCommand.RightShoulder] = Keys.End;
                    break;
            }
        }

        /// <summary>
        /// Initializes the Input class for use.
        /// </summary>
        public static void Initialize()
        {
            s_Inputs = new Input[_NUM_PLAYERS];
            for (int i = 0; i < _NUM_PLAYERS; ++i)
            {
                s_Inputs[i] = new Input();
                s_Inputs[i].m_ID = i;
                s_Inputs[i].OrkarInteMedAttFixaXML();
            }
        }

        /// <summary>
        /// Read input.
        /// </summary>
        public static void Update()
        {
            s_LastKeyboardState = s_CurrentKeyboardState;
            s_CurrentKeyboardState = Keyboard.GetState();

            for (int i = 0; i < _NUM_PLAYERS; ++i)
                s_Inputs[i].InternalUpdate();
        }

        public static Input GetPlayerState(int playerID)
        {
            return s_Inputs[playerID];
        }

        public static bool AnyIsPressed(InputCommand command)
        {
            for (int i = 0; i < _NUM_PLAYERS; ++i)
            {
                if (s_Inputs[i].IsPressed(command))
                {
                    s_LastSelectID = i;
                    return true;
                }
            }
            return false;
        }

        public static bool AnyWasPressed(InputCommand command)
        {
            for (int i = 0; i < _NUM_PLAYERS; ++i)
            {
                if (s_Inputs[i].WasPressed(command))
                {
                    s_LastSelectID = i;
                    return true;
                }
            }
            return false;
        }

        public static bool AnyWasPressed(Keys key)
        {
            if (s_CurrentKeyboardState.IsKeyDown(key) && s_LastKeyboardState.IsKeyUp(key))
                return true;
            return false;
        }
    }
}
