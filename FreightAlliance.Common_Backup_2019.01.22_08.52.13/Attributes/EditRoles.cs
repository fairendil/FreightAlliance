


namespace FreightAlliance.Common.Attributes
{

    using System.Collections.Generic;
    using System;
    public class EditRoles : Attribute
    {
        public EditRoles(params RoleEnum[] userRoles)
        {
            this.UserRoles = userRoles;
        }

        public RoleEnum[] UserRoles { get; set; }

    }
}