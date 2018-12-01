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
        KeyboardState LastKeys;
        
        public ClientInputs()
        {
            KeyState = Keyboard.GetState();
            LastKeys = KeyState;
        }

        public void Update()
        {
            LastKeys = KeyState;
            KeyState = Keyboard.GetState();
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
