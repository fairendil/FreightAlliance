

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using FreightAlliance.Shell.Properties;

namespace FreightAlliance.Shell.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;


    using Caliburn.Micro;

    using FreightAlliance.Common.Interfaces;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Controls.RibbonView;

    [Export(typeof(ISupportInitialize))]
    [Export(typeof(RibbonViewModel))]
    public class RibbonViewModel : ISupportInitialize
    {
        private readonly CompositionContainer container;

        [ImportingConstructor]
        public RibbonViewModel(CompositionContainer container)
        {
            this.container = container;
            this.Tabs = new BindingList<RadRibbonTab>();
        }

        public void BeginInit()
        {
            List<RadRibbonTab> ribbonTabs = new List<RadRibbonTab>();
            List<RadRibbonGroup> ribbonGroups = new List<RadRibbonGroup>();
            
            var temp = this.container.GetExports<IScreen>();
            foreach (var screen in this.container.GetExports<IScreen>())
            {
                var navigationItem = screen.Value as ICanRibbon;
                
                if (navigationItem != null && navigationItem.Tabs != null)
                {
                    foreach (var ribbonTab in navigationItem.Tabs)
                    {
                        if (ribbonTab.Title == null) continue;
                        RadRibbonTab tab = ribbonTabs.FirstOrDefault(t => (string)t.Header == ribbonTab.Title) ?? new RadRibbonTab();
                        tab.IsSelected = true;
                        tab.Header = ribbonTab.Title;
                        
                        
                        foreach (var ribbonGroup in ribbonTab.Groups)
                        {
                            RadRibbonGroup group = ribbonGroups.FirstOrDefault(g => (string)g.Header == ribbonGroup.Title) ?? new RadRibbonGroup();

                            group.Header = ribbonGroup.Title;
                            foreach (var ribbonButton in ribbonGroup.Buttons)
                            {
                                //var uriSource = new Uri(@"pack://application:,,,/ReferencedAssembly;/FreightAlliance.Shell/Resources/Images/plus.png");


                                var button = new RadRibbonButton()
                                                 {
                                                     Text = ribbonButton.Title,
                                                     Width = 80,
                                                     Height = 80,
                                                     Command = ribbonButton.Command,
                                                     SplitText = true,
                                                     Size = ButtonSize.Large
                                                 };

                                if (ribbonButton.ImgPath != null)
                                {
                                    var uri = new Uri(ribbonButton.ImgPath);
                                    var bitmap = new BitmapImage(uri);
                                    button.LargeImage = bitmap;
                                }
                                group.Items.Add(button);
                            }

                            if (!ribbonGroups.Contains(group))
                            {
                                tab.Items.Add(group);
                                ribbonGroups.Add(group);
                            }

                            if (!ribbonTabs.Contains(tab))
                            {
                                this.Tabs.Add(tab);
                                ribbonTabs.Add(tab);
                            }
                        }
                    }
                }
            }
            var radRibbonTab = new RadRibbonTab() { Header = Resources.TechServiceText };
            this.Tabs.Add(radRibbonTab);
            ribbonTabs.Add(radRibbonTab);

        }

        private void ChangeLanguage(object o)
        {
            if (string.IsNullOrEmpty(((string)FreightAlliance.Common.Properties.Settings.Default["Lang"])) || (string)FreightAlliance.Common.Properties.Settings.Default["Lang"] == "en-US")
            {
                FreightAlliance.Common.Properties.Settings.Default["Lang"] = "ru-RU";
            }
            else
            {
                FreightAlliance.Common.Properties.Settings.Default["Lang"] = "en-US";
            }
            FreightAlliance.Common.Properties.Settings.Default.Save();
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }


        public void EndInit()
        {
            //throw new System.NotImplementedException();
        }

        public DelegateCommand ChangeLang
        {
            get
            {
                return new DelegateCommand(ChangeLanguage);
            }
        }

        public BindingList<RadRibbonTab> Tabs { get; set; }

    }
}
