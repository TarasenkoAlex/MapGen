using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MapGen.View.Source.Classes
{
    public class KeyboardController
    {
        #region Region methods of processing input keys from keyboard.

        public void KeyDown(OpenGL gl, MapGenCamera camera, Key key)
        {
            switch (key)
            {
                case Key.A: // Движение камеры влево.
                    {
                        camera.MoveLeftRight(-float.Parse(ResourcesView.MoveSpeed));
                        camera.Look(gl);
                        //_isDrawMap = true;
                        break;
                    }
                case Key.D: // Движение камеры вправо.
                    {
                        camera.MoveLeftRight(float.Parse(ResourcesView.MoveSpeed));
                        camera.Look(gl);
                        //_isDrawMap = true;
                        break;
                    }
                case Key.S: // Движение камеры вниз.
                    {
                        camera.MoveUpDown(-float.Parse(ResourcesView.MoveSpeed));
                        camera.Look(gl);
                        //_isDrawMap = true;
                        break;
                    }
                case Key.W: // Движение камеры вверх.
                    {
                        camera.MoveUpDown(float.Parse(ResourcesView.MoveSpeed));
                        camera.Look(gl);
                        //_isDrawMap = true;
                        break;
                    }
                case Key.Q: // Движение камеры вперед.
                    {
                        camera.MoveForwardBackward(-float.Parse(ResourcesView.MoveSpeed));
                        camera.Look(gl);
                        //_isDrawMap = true;
                        break;
                    }
                case Key.E: // Движение камеры назад.
                    {
                        camera.MoveForwardBackward(float.Parse(ResourcesView.MoveSpeed));
                        //camera.Look(gl);
                        //_isDrawMap = true;
                        break;
                    }
            }
        }

        #endregion
    }
}
