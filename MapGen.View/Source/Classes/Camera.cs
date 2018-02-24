using System;
using SharpGL;

namespace MapGen.View.Source.Classes
{
    public struct Vector3D
    {
        public float X;
        public float Y;
        public float Z;
    }

    public class Camera
    {

        #region Region private fields.
        
        private Vector3D _mPos;
        
        private Vector3D _mView;
        
        private Vector3D _mUp;

        /// <summary>
        /// Вектор для стрейфа (движения влево и вправо) камеры.
        /// </summary>
        private Vector3D _mStrafeLeftRigth;

        /// <summary>
        /// Вектор для стрейфа (движения вверх и вниз) камеры.
        /// </summary>
        private Vector3D _mStrafeUpDown;

        /// <summary>
        /// 
        /// </summary>
        private Vector3D _mStrafeForwardBackward;

        #endregion


        #region Region properties.

        /// <summary>
        /// Вектор позиции камеры.
        /// </summary>
        public Vector3D Pos => _mPos;

        /// <summary>
        /// Направление, куда смотрит камера.
        /// </summary>
        public Vector3D View => _mView;

        /// <summary>
        /// Вектор верхнего направления.
        /// </summary>
        public Vector3D Up => _mUp;

        #endregion


        #region Region constructor.

        /// <summary>
        /// Создание камеры в трехмерном пространстве.
        /// </summary>
        /// <param name="pos_x"></param>
        /// <param name="pos_y"></param>
        /// <param name="pos_z"></param>
        /// <param name="view_x"></param>
        /// <param name="view_y"></param>
        /// <param name="view_z"></param>
        /// <param name="up_x"></param>
        /// <param name="up_y"></param>
        /// <param name="up_z"></param>
        public Camera(float pos_x, float pos_y, float pos_z, float view_x, float view_y, float view_z, float up_x, float up_y, float up_z)
        {
            // Позиция камеры.
            _mPos.X = pos_x;
            _mPos.Y = pos_y;
            _mPos.Z = pos_z;
            // Куда смотрит, т.е. взгляд.
            _mView.X = view_x;
            _mView.Y = view_y;
            _mView.Z = view_z;
            // Вертикальный вектор камеры.
            _mUp.X = up_x;
            _mUp.Y = up_y;
            _mUp.Z = up_z;
        }

        #endregion


        #region Region methods moving.

        /// <summary>
        /// Установить позицию, взгляд и вертикальный вектор камеры.
        /// </summary>
        /// <param name="pos_x"></param>
        /// <param name="pos_y"></param>
        /// <param name="pos_z"></param>
        /// <param name="view_x"></param>
        /// <param name="view_y"></param>
        /// <param name="view_z"></param>
        /// <param name="up_x"></param>
        /// <param name="up_y"></param>
        /// <param name="up_z"></param>ы
        public void SetPositionCamera(float pos_x, float pos_y, float pos_z, float view_x, float view_y, float view_z, float up_x, float up_y, float up_z)
        {
            // Позиция камеры.
            _mPos.X = pos_x;
            _mPos.Y = pos_y;
            _mPos.Z = pos_z;
            // Куда смотрит, т.е. взгляд.
            _mView.X = view_x;
            _mView.Y = view_y;
            _mView.Z = view_z;
            // Вертикальный вектор камеры.
            _mUp.X = up_x;
            _mUp.Y = up_y;
            _mUp.Z = up_z;
        }

        /// <summary>
        /// Движение камеры влево-вправо.
        /// </summary>
        /// <param name="speed"></param>
        public void MoveLeftRight(float speed)
        {
            // добавим вектор стрейфа к позиции
            _mPos.X += /*_mStrafeLeftRigth.X **/ speed;

            // Добавим теперь к взгляду
            _mView.X += speed;
        }

        /// <summary>
        /// Движение камеры вверх-вниз.
        /// </summary>
        /// <param name="speed"></param>
        public void MoveUpDown(float speed)
        {
            // добавим вектор стрейфа к позиции
            _mPos.Y += /*_mStrafeUpDown.Y **/ speed;

            // Добавим теперь к взгляду
            _mView.Y += speed;
        }

        /// <summary>
        /// Движение камеры вперед-назад.
        /// </summary>
        /// <param name="speed"></param>
        public void MoveForwardBackward(float speed)
        {
            //Vector3D vVector; // Получаем вектор взгляда
            //vVector.X = _mView.X - _mPos.X;
            //vVector.Y = _mView.Y - _mPos.Y;
            //vVector.Z = _mView.Z - _mPos.Z;

            //vVector = Normalize(vVector);

            //_mPos.X += vVector.X * speed;
            //_mPos.Z += vVector.Z * speed;

            //_mPos.Y += vVector.Y * speed;
            //_mView.Y += vVector.Y * speed;

            //_mView.X += vVector.X * speed;
            //_mView.Z += vVector.Z * speed;

            if (_mPos.Z + speed >= 0.0f)
            {
                _mPos.Z += speed;
                _mView.Z = _mPos.Z - 1.0f;
            }
        }
        

        public void Update()
        {
            Vector3D vCross = Cross(_mView, _mPos, _mUp);

            // Нормализуем вектор стрейфа
            _mStrafeLeftRigth = Normalize(vCross);
        }

        // Нужен метод, который будет обновлять взгляд и позицию камеры: 
        public void Look(ref OpenGL gl)
        {
            gl.LookAt(_mPos.X, _mPos.Y, _mPos.Z,
                      _mView.X, _mView.Y, _mView.Z,
                      _mUp.X, _mUp.Y, _mUp.Z);
        }

        #endregion


        #region Region special private methods.

        /// <summary>
        /// Метод, который будет нам возвращать перпендикулярный вектор от трех переданных векторов. 
        /// Два любых вектора образуют плоскость, от которой мы и ищем перпендикуляр. 
        /// Это все нам понадобится для того, чтобы мы смогли реализовать Стрейф.
        /// </summary>
        /// <param name="vV1"></param>
        /// <param name="vV2"></param>
        /// <param name="vVector2"></param>
        /// <returns></returns>
        private Vector3D Cross(Vector3D vV1, Vector3D vV2, Vector3D vVector2)
        {
            Vector3D vNormal;

            Vector3D vVector1;
            vVector1.X = vV1.X - vV2.X;
            vVector1.Y = vV1.Y - vV2.Y;
            vVector1.Z = vV1.Z - vV2.Z;

            // Если у нас есть 2 вектора (вектор взгляда и вертикальный вектор), 
            // У нас есть плоскость, от которой мы можем вычислить угол в 90 градусов.
            // Рассчет cross'a прост, но его сложно запомнить с первого раза. 
            // Значение X для вектора = (V1.y * V2.z) - (V1.z * V2.y)
            vNormal.X = ((vVector1.Y * vVector2.Z) - (vVector1.Z * vVector2.Y));

            // Значение Y = (V1.z * V2.x) - (V1.x * V2.z)
            vNormal.Y = ((vVector1.Z * vVector2.X) - (vVector1.X * vVector2.Z));

            // Значение Z = (V1.x * V2.y) - (V1.y * V2.x)
            vNormal.Z = ((vVector1.X * vVector2.Y) - (vVector1.Y * vVector2.X));

            // вернем результат.
            return vNormal;
        }

        /// <summary>
        /// Возвращает длину вектора.
        /// </summary>
        /// <param name="vNormal"></param>
        /// <returns></returns>
        private float Magnitude(Vector3D vNormal)
        {
            // Это даст нам величину нашей нормали, т.е. длину вектора.
            // Мы используем эту информацию для нормализации вектора. 
            // Вот формула: magnitude = sqrt(V.x^2 + V.y^2 + V.z^2) где V - вектор.
            return (float)Math.Sqrt((vNormal.X * vNormal.X) + (vNormal.Y * vNormal.Y) + (vNormal.Z * vNormal.Z));
        }

        /// <summary>
        /// Нормализация вектора.
        /// </summary>
        /// <param name="vVector"></param>
        /// <returns></returns>
        private Vector3D Normalize(Vector3D vVector)
        {
            float magnitude = Magnitude(vVector);

            vVector.X = vVector.X / magnitude;
            vVector.Y = vVector.Y / magnitude;
            vVector.Z = vVector.Z / magnitude;

            return vVector;
        }

        #endregion
        
    }
}
