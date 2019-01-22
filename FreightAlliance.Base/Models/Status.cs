using System.Data.SqlTypes;

using FreightAlliance.Common.Enums;

namespace FreightAlliance.Base.Models
{
    public class Status
    {
        public int StatusId { get; set; }

        public StatusEnum StatusEnum { get; set; }

        public override string ToString()
        {
            return this.StatusEnum.ToString();
        }

        public void NextStatus()
        {
            this.StatusEnum = this.StatusEnum + 1;
        }
    }

}
