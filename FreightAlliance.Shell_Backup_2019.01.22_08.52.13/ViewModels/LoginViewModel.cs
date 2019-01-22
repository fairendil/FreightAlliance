using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Resources;
using FreightAlliance.Base.Models;
using Caliburn.Micro;
using FreightAlliance.Common.Attributes;
using FreightAlliance.Shell.Properties;

namespace FreightAlliance.Shell.ViewModels
{
    [Export(typeof(ILogin))]
    class LoginViewModel : Screen, ILogin
    {
        private string user;
        private string password;
        //[ImportingConstructor]
        public LoginViewModel()//[Import("DataProvider")] IBaseProvider provider)
        {
            //var dataProvider = (DataProvider) provider;
            //var users = dataProvider.Users.ToIList();
            this.Users = new BindableCollection<User>()
            {
                new User() {Role = RoleEnum.Manager, Vessel = "Manager", Name = "Vlad", Password = "1"},
                new User() {Role = RoleEnum.Capitan, Vessel = "Argo", Name = "Alex", Password = "2"},
                new User() {Role = RoleEnum.Capitan, Vessel = "Toronto", Name = "Redon", Password = "3"},
                new User() {Role = RoleEnum.Manager, Vessel = "Manager", Name = "Pavel", Password = "Pavel"},
                new User() {Role = RoleEnum.Capitan, Vessel = "ANDREY OSIPOV", Name = "captain", Password = "0"},
                new User() {Role = RoleEnum.Capitan, Vessel = "Horizon", Name = "Tan", Password = "4"}
            };

            this.Positions = new BindableCollection<string>
            {
                Resources.Master,
                Resources.Chief_engineer,
                Resources._2_engineer,
                Resources._3_engineer,
                Resources._4_engineer,
                Resources.Chief_officer,
                Resources._2Mate,
                Resources._3Mate,
                Resources.Cook,
                Resources.Superintendant
            };
            this.password = string.Empty;
            //foreach (var u in users)
            //{
            //    this.Users.Add((User)u);
            //}
            this.SelectedUser = this.Users.FirstOrDefault();
            this.SelectedPosition = this.Positions.FirstOrDefault();
        }

        public void Select()
        {
            if (this.Users.Any(u => u.Name == this.Login))
            {
                this.SelectedUser = this.Users.FirstOrDefault(u => u.Name == this.user);
                if (!(this.SelectedUser.Password == this.password))
                {
                    
                    this.Error = Resources.PasswordIsIncorrectText;
                    this.OnPropertyChanged(new PropertyChangedEventArgs("Error"));
                    return;
                }
                this.SelectedUser.Position = this.SelectedPosition;
                this.Confirmed = true;
                ((IDeactivate)this).Deactivate(true);
            }
            else
            {
                this.Error = Resources.UserDoesNotExistText;
                this.OnPropertyChanged(new PropertyChangedEventArgs("Error"));
                return;
            }
            
            
        }
         
        public void Cancel()
        {
            this.Confirmed = false;
            this.TryClose(false);
        }
        public bool CanSelect(string name)
        {
            return !string.IsNullOrWhiteSpace(name);
        }

        public string Login
        {
            get
            {
                return this.user;
            }
            set
            {
                if (this.user == value)
                {
                    return;
                }
                this.user = value;
                this.Error = "";
                this.OnPropertyChanged(new PropertyChangedEventArgs("Error"));
            }
        }

        public string Error { get; set; }

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                if (this.password == value)
                {
                    return;
                }

                this.password = value;
                this.Error = "";
                this.OnPropertyChanged(new PropertyChangedEventArgs("Error"));
            }
        }
        public BindableCollection<User> Users { get; set; }

        public BindableCollection<string> Positions { get; set; }


        public string SelectedPosition { get; set; }

        public User SelectedUser { get; set; }

        public  bool Confirmed { get; set; }
        
    }
}
