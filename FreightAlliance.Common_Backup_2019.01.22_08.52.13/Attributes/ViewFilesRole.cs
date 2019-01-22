using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreightAlliance.Common.Attributes
{

        public class ViewFilesRole : Attribute
        {
            public ViewFilesRole(params RoleEnum[] userRoles)
            {
                this.UserRoles = userRoles;
            }

            public RoleEnum[] UserRoles { get; set; }

        }

}
