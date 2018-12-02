using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Airlock.Util;

namespace Airlock.Client
{
    public class ClientInputs
    {
        public KeyboardState KeyState;
        private KeyboardState LastKeys;

        public Vector2 Cursor { get { return MouseState.Position.ToVector2(); } }
        public MouseButtons MouseButtonState;
        public MouseButtons LastMouseButtonState;
        public MouseState MouseState;

        [Flags]
        public enum MouseButtons
        {
            None    = 0,
            Left    = 1,
            Right   = 2, 
            Middle  = 4
        }

        public ClientInputs()
        {
            KeyState = Keyboard.GetState();
            LastKeys = KeyState;
            MouseState = Mouse.GetState();
            MouseButtonState = GetMouseButtonsStates(MouseState);
        }

        private MouseButtons GetMouseButtonsStates(MouseState state)
        {
            return (MouseButtons)(((int)state.LeftButton   << 0)
                                + ((int)state.RightButton  << 1)
                                + ((int)state.MiddleButton << 2));
        }

        public void Update()
        {
            LastKeys = KeyState;
            KeyState = Keyboard.GetState();
            MouseState = Mouse.GetState();
            LastMouseButtonState = MouseButtonState;
            MouseButtonState = GetMouseButtonsStates(MouseState);
            
        }

        public bool KeyPressed(Keys key)
        {
            return KeyState.IsKeyDown(key) && !LastKeys.IsKeyDown(key);
        }

        public bool KeyHeld(Keys key)
        {
            return KeyState.IsKeyDown(key);
        }

        public bool KeyReleased(Keys key)
        {
            return !KeyState.IsKeyDown(key) && LastKeys.IsKeyDown(key);
        }

        public bool MouseButtonPressed(MouseButtons button)
        {
            return (MouseButtonState & (~LastMouseButtonState) & button) != 0;
        }

        public bool MouseButtonReleased(MouseButtons button)
        {
            return (~MouseButtonState & (LastMouseButtonState) & button) != 0;
        }

        public bool MouseButtonHeld(MouseButtons button)
        {
            return (MouseButtonState & button) != 0;
        }

        public Vector2 GetWASDVector(bool normalise = true)
        {
            Vector2 v = new Vector2(
                (KeyState.IsKeyDown(Keys.A) ? -1 : 0) + (KeyState.IsKeyDown(Keys.D) ? 1 : 0),
                (KeyState.IsKeyDown(Keys.W) ? -1 : 0) + (KeyState.IsKeyDown(Keys.S) ? 1 : 0)
                );
            if (normalise && v.X != 0 && v.Y != 0)
            {
                v.X *= Fmath.InvRootTwo;
                v.Y *= Fmath.InvRootTwo;
            }
            return v;
        }
    }
}
