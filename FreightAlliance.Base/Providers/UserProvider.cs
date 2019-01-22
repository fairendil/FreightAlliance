namespace FreightAlliance.Base.Providers
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;

    using FreightAlliance.Base.Models;
    using FreightAlliance.Common.Extensions;
    using FreightAlliance.Common.Interfaces;

    [Export(nameof(UserProvider), typeof(IDataProvider<Code>))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class UserProvider : IDataProvider<Code>
    {
        private readonly ConcurrentDictionary<int, Code> codes = new ConcurrentDictionary<int, Code>();

        public IEnumerable<Code> GetAll()
        {
            if (!this.codes.Any())
            {
                this.codes.AddOrUpdate(7110, new Code(7110, "Victualling"));
                this.codes.AddOrUpdate(7115, new Code(7115, "MastersRepresentation"));
                this.codes.AddOrUpdate(7120, new Code(7120, "Slopchest"));
                this.codes.AddOrUpdate(7125, new Code(7125, "FreshWaterForCrewNeeds"));
                this.codes.AddOrUpdate(7130, new Code(7130, "MedicalAttendanceMedicines"));
                this.codes.AddOrUpdate(7320, new Code(7320, "Lubricants"));
                this.codes.AddOrUpdate(7330, new Code(7330, "Chemicals"));
                this.codes.AddOrUpdate(7331, new Code(7331, "WeldingGases"));
                this.codes.AddOrUpdate(7335, new Code(7335, "Refrigerants"));
                this.codes.AddOrUpdate(7340, new Code(7340, "DeckStores"));
                this.codes.AddOrUpdate(7350, new Code(7350, "SeastockPaints"));
                this.codes.AddOrUpdate(7360, new Code(7360, "ChartsAndPublications"));
                this.codes.AddOrUpdate(7370, new Code(7370, "StewardStores"));
                this.codes.AddOrUpdate(7380, new Code(7380, "TechnicalSupply"));
                this.codes.AddOrUpdate(7390, new Code(7390, "OtherStores"));
                this.codes.AddOrUpdate(7395, new Code(7395, "ForwardingExpensesStores"));
                this.codes.AddOrUpdate(7410, new Code(7410, "MEAE"));
                this.codes.AddOrUpdate(7420, new Code(7420, "AE"));
                this.codes.AddOrUpdate(7430, new Code(7430, "Machinery"));
                this.codes.AddOrUpdate(7440, new Code(7440, "DeckEquipment"));
                this.codes.AddOrUpdate(7450, new Code(7450, "ElEquipmentAndAutomation"));
                this.codes.AddOrUpdate(7460, new Code(7460, "RadioAndNavigationEquipment"));
                this.codes.AddOrUpdate(7470, new Code(7470, "SpecialEquipment"));
                this.codes.AddOrUpdate(7490, new Code(7490, "OtherSpares"));
                this.codes.AddOrUpdate(7495, new Code(7495, "ForwardingExpensesSpares"));
                this.codes.AddOrUpdate(7415, new Code(7415, "ME"));
                this.codes.AddOrUpdate(7425, new Code(7425, "AE_rep"));
                this.codes.AddOrUpdate(7435, new Code(7435, "Machinery_rep"));
                this.codes.AddOrUpdate(7445, new Code(7445, "DeckEquipment_rep"));
                this.codes.AddOrUpdate(7455, new Code(7455, "ElEquipmentAndAutomation_rep"));
                this.codes.AddOrUpdate(7465, new Code(7465, "RadioAndNavigationEquipment_rep"));
                this.codes.AddOrUpdate(7475, new Code(7475, "SpecialEquipment_rep"));
                this.codes.AddOrUpdate(7485, new Code(7485, "SteelAndPipes"));
            }
            return this.codes.Select(pair => pair.Value);
        }

        public IEnumerable<Code> Get(int count)
        {
            throw new System.NotImplementedException();
        }

        public Code GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public void Add(Code item)
        {
            throw new System.NotImplementedException();
        }

        public void AddRange(IEnumerable<Code> items)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(Code item)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveById(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
