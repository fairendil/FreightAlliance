

namespace FreightAlliance.Base.Models
{
    public class Vessel 
    {
        public int VesselId { get; set; }
        public string Name { get; set; }
        
        public override string ToString()
        {
            return this.Name;
        }
        
    }
}
