using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.View.Source.Classes.SettingGen;

namespace MapGen.View.Source.Interfaces
{
    public interface ITestCase
    {
        int Id { get; set; }
        VSettingGen SettingGen { get; set; }
        long SourceScale { get; set; }
        long DistScale { get; set; }
        long Time { get; set; }
        
        bool IsRunningProgressBar { get; set; }
        bool IsSuccess { get; set; }
    }
}
