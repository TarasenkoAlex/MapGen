using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapGen.Model.Interpolation.Setting
{
    public interface ISettingInterpolation
    {
        double Step { get; set; }
    }
}
