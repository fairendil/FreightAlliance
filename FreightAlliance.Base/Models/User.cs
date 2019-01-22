using FreightAlliance.Common.Attributes;
using FreightAlliance.Common.Interfaces;

namespace FreightAlliance.Base.Models
{
    public class User : IUser
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        
        public string Vessel { get; set; }

        public RoleEnum Role { get; set; }

        public string Password { get; set; }

        public string Position { get; set; }
    }

}