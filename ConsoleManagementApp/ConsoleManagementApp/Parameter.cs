using System.Text;

namespace ControllerManagementApp
{
    public class Parameter
    {
        public int Id { get; set; }

        public double Value { get; set; }

        public double MinValue { get; set; }

        public double MaxValue { get; set; }

        public bool IsConstant { get; set; }

        public bool IsInt { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Id: {Id}");
            sb.AppendLine($"Value: {Value}");
            sb.AppendLine($"Minimal Value: {MinValue}");
            sb.AppendLine($"Maximal Value: {MaxValue}");

            if (IsConstant)
            {
                sb.AppendLine("The parameter can't be changed");
            }
            else 
            {
                sb.AppendLine("The parameter can be changed");
            }

            return sb.ToString();
        }
    }
}
