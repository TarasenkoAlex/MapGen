using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.Model.General;

namespace MapGen.Model.Interpolation.Setting
{
    public interface ISettingInterpolRbf : ISettingInterpolGeneral
    {
        /// <summary>
        /// Базисная функция.
        /// </summary>
        BasicFunctions BasicFunction { get; set; }

        /// <summary>
        /// Фактор сглаживания.
        /// </summary>
        double R { get; set; }
    }
}
