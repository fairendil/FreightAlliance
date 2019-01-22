namespace FreightAlliance.Common.Common
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using FreightAlliance.Common.Annotations;

    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            
        }
    }
}