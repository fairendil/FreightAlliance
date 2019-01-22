using System.Text;

namespace FreightAlliance.Base.Models
{
    using System;

    public class Code 
    {
        public Code()
        {
            //this.Name = string.Empty;
        }

        public Code(int number, string name, int type)
        {
            this.Number = number;
            this.Name = name;
            this.CodeTypeId = type;
        }

        public int CodeId { get; set; }
        public string Name { get; set; }

        public int Number { get; set; }

        public int CodeTypeId { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.Number, this.Name);
        }
    }
}
