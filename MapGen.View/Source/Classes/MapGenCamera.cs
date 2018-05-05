using System;
using System.Windows.Media.Media3D;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Cameras;

namespace MapGen.View.Source.Classes
{
    public class MapGenCamera : LookAtCamera
    {
        #region Region methods moving.
        
        /// <summary>
        /// Движение камеры влево-вправо.
        /// </summary>
        /// <param name="speed"></param>
        public void MoveLeftRight(float speed)
        {
            Target = new Vertex(Target.X + speed, Target.Y, Target.Z);
            Position = new Vertex(Position.X + speed, Position.Y, Position.Z);
        }

        /// <summary>
        /// Движение камеры вверх-вниз.
        /// </summary>
        /// <param name="speed">Шаг смещения.</param>
        public void MoveUpDown(float speed)
        {
            Target = new Vertex(Target.X, Target.Y - speed, Target.Z);
            Position = new Vertex(Position.X, Position.Y - speed, Position.Z);
        }

        /// <summary>
        /// Движение камеры вперед-назад.
        /// </summary>
        /// <param name="speed">Шаг смещения.</param>
        public void MoveForwardBackward(float speed)
        {
            if (Position.Z - speed > 0.0f)
            {
                Target = new Vertex(Target.X, Target.Y, Target.Z - speed);
                Position = new Vertex(Position.X, Position.Y, Position.Z - speed);
            }
        }

        #endregion

        /// <summary>
        /// Обновляем взгляд камеры.
        /// </summary>
        /// <param name="gl"></param>
        public void Look(OpenGL gl)
        {
            TransformProjectionMatrix(gl);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
