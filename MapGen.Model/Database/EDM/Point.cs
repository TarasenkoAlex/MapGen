//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MapGen.Model.Database.EDM
{
    using System;
    using System.Collections.Generic;
    
    public partial class Point
    {
        public long Idp { get; set; }
        public long X { get; set; }
        public long Y { get; set; }
        public double Depth { get; set; }
        public long Idm { get; set; }
    
        public virtual Map Map { get; set; }
    }
}
