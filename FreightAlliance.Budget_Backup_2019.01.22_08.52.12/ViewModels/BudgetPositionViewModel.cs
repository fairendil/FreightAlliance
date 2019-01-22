using System.ComponentModel.DataAnnotations;
using FreightAlliance.Base.Models;

namespace FreightAlliance.Budget.ViewModels
{
    using FreightAlliance.Budget.Properties;
    internal class BudgetPositionViewModel
    {
        public Code code;

        private CodeType codeType;

        public BudgetPositionViewModel(Code code, CodeType codeType)
        {
            this.code = code;
            this.codeType = codeType;
        }

        [Display(Name = "TypeText", Order = 0, ResourceType = typeof(Resources),AutoGenerateFilter = true)]
        public string CodeType
        {
            get { return this.codeType.Type.ToString() + " " +this.codeType.Name; }

        }

        [Display(Name = "DescriptionText", Order = 0, ResourceType = typeof (Resources))]
        public string Description
        {
            get { return this.code.Number + " " + this.code.Name; }
        }
        [Display(Name = "PlanText", Order = 0, ResourceType = typeof(Resources))]
        public float Plan { get; set; }
        [Display(Name = "FactText", Order = 0, ResourceType = typeof(Resources))]
        public float Fact { get; set; }

        [Display(Name = "CommentsText", Order = 0, ResourceType = typeof(Resources))]
        public string Comments { get; set; }

    }
}