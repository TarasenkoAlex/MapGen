using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MapGen.View.Source.Classes.SettingGen;

namespace MapGen.View.Source.Classes
{
    public class VTestResult
    {
        public int IdTestCase { get; set; }
        public long Time { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class VTestCase
    {
        public VSettingGen SettingGen { get; set; }
        public long Scale { get; set; }
        
    }
}
