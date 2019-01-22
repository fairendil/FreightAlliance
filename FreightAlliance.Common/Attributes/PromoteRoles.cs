using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreightAlliance.Common.Attributes
{
    public class PromoteRoles : Attribute
    {

        public PromoteRoles(params RoleEnum[] userRoles)
        {
            this.UserRoles = userRoles;
            
        }

        public RoleEnum[] UserRoles { get; set; }
    }
}
