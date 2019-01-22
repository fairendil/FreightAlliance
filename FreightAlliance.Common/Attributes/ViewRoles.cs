using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreightAlliance.Common.Attributes
{

    public class ViewRoles : Attribute
    {
        public ViewRoles(params RoleEnum[] userRoles)
        {
            this.UserRoles = userRoles;
        }

        public RoleEnum[] UserRoles { get; set; }

    }

}
