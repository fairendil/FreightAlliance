using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreightAlliance.Common.Attributes;

namespace FreightAlliance.Common.Enums
{
    public enum StatusEnum
    {
        [PromoteRoles(RoleEnum.Capitan)]
        [EditRoles(RoleEnum.Capitan)]
        [ViewRoles(RoleEnum.Manager,RoleEnum.Capitan)]
        [ViewFilesRole()]
        New,
        [PromoteRoles(RoleEnum.Capitan)]
        [EditRoles(RoleEnum.Capitan)]
        [ViewRoles(RoleEnum.Manager, RoleEnum.Capitan)]
        [ViewFilesRole()]
        ReadyToBeSent,
        [PromoteRoles(RoleEnum.Manager)]
        [EditRoles(RoleEnum.Manager)]
        [ViewRoles(RoleEnum.Manager)]
        [ViewFilesRole()]
        SentToTheOffice,
        [PromoteRoles(RoleEnum.Manager, RoleEnum.Capitan)]
        [EditRoles(RoleEnum.Manager,RoleEnum.Capitan)]
        [ViewRoles(RoleEnum.Manager)]
        [ViewFilesRole()]
        ReceivedAtOffice,
        [PromoteRoles()]
        [EditRoles()]
        [ViewRoles(RoleEnum.Manager)]
        [ViewFilesRole(RoleEnum.Manager)]
        SentForQuotation,
        [PromoteRoles()]
        [EditRoles()]
        [ViewRoles(RoleEnum.Manager, RoleEnum.Capitan)]
        [ViewFilesRole(RoleEnum.Manager)]
        Confirmed,
        [PromoteRoles(RoleEnum.Capitan)]
        [EditRoles()]
        [ViewRoles(RoleEnum.Manager, RoleEnum.Capitan)]
        [ViewFilesRole(RoleEnum.Manager)]
        Received,
        [PromoteRoles()]
        [EditRoles()]
        [ViewRoles(RoleEnum.Manager, RoleEnum.Capitan)]
        [ViewFilesRole(RoleEnum.Manager)]
        Capitalized,
        [PromoteRoles()]
        [EditRoles()]
        [ViewRoles(RoleEnum.Manager, RoleEnum.Capitan)]
        Closed,
        [PromoteRoles()]
        [EditRoles()]
        [ViewRoles(RoleEnum.Manager, RoleEnum.Capitan)]
        Canceled
    }

}
