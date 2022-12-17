using System.Collections.Generic;
using CMon.Entities;

namespace CMon.Models
{
    public class Device : DbDevice
    {
        public IList<DbInput> Inputs { get; set; }
    }
}