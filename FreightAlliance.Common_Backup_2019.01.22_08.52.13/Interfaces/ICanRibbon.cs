namespace FreightAlliance.Common.Interfaces
{
    using System.Collections.Generic;

    using FreightAlliance.Common.Helpers;

    public interface ICanRibbon
    {
        List<RibbonTab> Tabs { get; }

    }
}
