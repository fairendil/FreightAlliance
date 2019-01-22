namespace FreightAlliance.Common.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class ValidationBase : INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        private object @lock = new object();

        [Display(AutoGenerateField = false)]
        public bool IsValid
        {
            get
            {
                return this.HasErrors;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        [Display(AutoGenerateField = false)]
        public bool HasErrors
        {
            get
            {
                lock (this.@lock)
                {
                    return this.errors.Any(propErrors => propErrors.Value != null && propErrors.Value.Count > 0);
                }
            }
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                lock (this.@lock)
                {
                    if (this.errors.ContainsKey(propertyName) && (this.errors[propertyName] != null)
                        && this.errors[propertyName].Count > 0)
                    {
                        return this.errors[propertyName].ToList();
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                lock (this.@lock)
                {
                    return this.errors.SelectMany(err => err.Value.ToList());
                }
            }
        }

        public void OnErrorsChanged(string propertyName)
        {
            if (this.ErrorsChanged != null)
            {
                this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            lock (this.@lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                validationContext.MemberName = propertyName;
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateProperty(value, validationContext, validationResults);

                // clear previous errors from tested property
                if (propertyName != null && this.errors.ContainsKey(propertyName))
                {
                    this.errors.Remove(propertyName);
                }

                this.OnErrorsChanged(propertyName);
                this.HandleValidationResults(validationResults);
            }
        }

        public void Validate()
        {
            lock (this.@lock)
            {
                var validationContext = new ValidationContext(this, null, null);
                var validationResults = new List<ValidationResult>();
                Validator.TryValidateObject(this, validationContext, validationResults, true);

                // clear all previous errors
                var propNames = this.errors.Keys.ToList();
                this.errors.Clear();
                propNames.ForEach(this.OnErrorsChanged);
                this.HandleValidationResults(validationResults);
            }
        }

        private void HandleValidationResults(List<ValidationResult> validationResults)
        {
            // Group validation results by property names
            var resultsByPropNames = from res in validationResults
                                     from mname in res.MemberNames
                                     group res by mname
                                     into g select g;

            // add errors to dictionary and inform binding engine about errors
            foreach (var prop in resultsByPropNames)
            {
                var messages = prop.Select(r => r.ErrorMessage).ToList();
                this.errors.Add(prop.Key, messages);
                this.OnErrorsChanged(prop.Key);
            }
        }
    }
}