using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerManagement.Service
{
    public class Parameter
    {
        public int Id { get; set; }

        public double Value { get; set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public bool IsConstant { get; set; }

        public bool IsInt { get; set; }
    }
}
