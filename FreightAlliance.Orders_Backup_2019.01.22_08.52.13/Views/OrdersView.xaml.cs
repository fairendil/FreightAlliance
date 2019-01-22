using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using FreightAlliance.Base.Models;
using FreightAlliance.Common.Attributes;
using FreightAlliance.Common.Enums;
using FreightAlliance.Orders.Converters;
using FreightAlliance.Orders.Selectors;
using Telerik.Windows.Controls.GridView;

namespace FreightAlliance.Orders.Views
{
    using System.Windows;
    using System.Windows.Data;

    using FreightAlliance.Orders.ViewModels;

    using Telerik.Windows.Controls;

    using CommonResources = Common.Properties.Resources;

    public partial class OrdersView
    {

        public OrdersView()
        {
            this.InitializeComponent();
        }

        private void RadDataForm_AutoGeneratingField(object sender, Telerik.Windows.Controls.Data.DataForm.AutoGeneratingFieldEventArgs e)
        {
            
            var frameworkElement = sender as FrameworkElement;
            var orderViewModel = frameworkElement?.DataContext as OrderViewModel;
            if (orderViewModel != null)
            {
                
                if (e.PropertyName == "Code")
                {

                    if (orderViewModel != null)
                    {
                        e.DataField = new DataFormComboBoxField()
                        {
                            Name = "CodeCombobox",
                            ItemsSource = orderViewModel.Codes,
                            SelectedValuePath = "Number",
                            DataMemberBinding = new Binding("Code")
                            {
                                Mode = BindingMode.TwoWay
                            },
                            Label = Properties.Resources.CodeText,
                            IsComboboxEditable = true

                        };
                        e.DataField.KeyUp += OnTextInput;

                    }
                }
                if (e.PropertyName == "Invoice")
                {


                    if (orderViewModel != null)
                    {
                        e.DataField = new DataFormComboBoxField()
                        {
                            ItemsSource = orderViewModel.Invoices,
                            FontSize = 15,
                            DataMemberBinding = new Binding("Invoice") {Mode = BindingMode.TwoWay},
                            Label = Properties.Resources.InvoiceText,
                            IsEnabled = orderViewModel.User.Role == RoleEnum.Manager

                        };
                    }
                }
                if (e.PropertyName == "Supplier")
                {


                    if (orderViewModel != null)
                    {
                        e.DataField = new DataFormComboBoxField()
                        {
                            ItemsSource = orderViewModel.Suppliers,
                            FontSize = 15,
                            DataMemberBinding = new Binding("Supplier") {Mode = BindingMode.TwoWay},
                            Label = Properties.Resources.SupplierColumnText,
                            IsEnabled = orderViewModel.User.Role == RoleEnum.Manager
                        };
                    }

                }
                if (e.PropertyName == "PesonPost")
                {

                    if (orderViewModel != null)
                    {
                        e.DataField = new DataFormComboBoxField()
                        {
                            ItemsSource = orderViewModel.Positions,
                            FontSize = 15,
                            DataMemberBinding = new Binding("PesonPost") { Mode = BindingMode.TwoWay },
                            Label = Properties.Resources.PersonPostText
                        };
                    }

                }
                if (e.PropertyName == "StoragePlaceColumnText")
                {


                    if (orderViewModel != null)
                    {
                        Binding bind = new Binding();
                        bind.Path = new PropertyPath("Received");
                        bind.Source = orderViewModel;

                        BindingOperations.SetBinding(e.DataField, TextBlock.IsEnabledProperty, bind);

                    }

                }
                if (e.PropertyName == "Vessel")
                {
                    e.DataField.IsEnabled = orderViewModel != null && orderViewModel.User.Role == RoleEnum.Manager;
                }
                if (e.PropertyName == "Status")
                {
                    e.DataField.Visibility = Visibility.Collapsed;
                }
                if (e.PropertyName == "Type")
                {
                    e.DataField.Visibility = Visibility.Collapsed;
                }
            }
            var binding = e.DataField.DataMemberBinding;
            if (binding != null)
            {
                e.DataField.DataMemberBinding.ValidatesOnDataErrors = true;
                e.DataField.DataMemberBinding.NotifyOnValidationError = true;
            }
        }

        private void OnTextInput(object sender, KeyEventArgs keyEventArgs)
        {
            var converter = new KeyConverter();
            if(!char.IsLetterOrDigit(converter.ConvertToString(keyEventArgs.Key).FirstOrDefault()) && keyEventArgs.Key != Key.Delete && keyEventArgs.Key != Key.Back) return;
            var frameworkElement = sender as FrameworkElement;
            var orderViewModel = frameworkElement?.DataContext as OrderViewModel;
            var combobox = sender as DataFormComboBoxField;
           
            var value = (combobox.Content as RadComboBox).Text;
            if (value == null) return;
            combobox.ItemsSource =
                orderViewModel.Codes.Where(code => code.Name.ToLower().Contains(value) || code.Number.ToString().Contains(value));

            (combobox.Content as RadComboBox).IsDropDownOpen = true;
        }


        private void RadDataForm1_EditEnded(object sender, Telerik.Windows.Controls.Data.DataForm.EditEndedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var orderViewModel = frameworkElement?.DataContext as OrderViewModel;
            if (orderViewModel != null)
            {
                orderViewModel.SaveChanges();
            }
        }

        private void RadGridView_CellEditEnded(object sender, GridViewRowEditEndedEventArgs gridViewRowEditEndedEventArgs)
        {
            var frameworkElement = sender as FrameworkElement;
            var orderViewModel = frameworkElement?.DataContext as OrderViewModel;
            if (orderViewModel is SparePartsOrderViewModel)
            {
                var spareParts = orderViewModel as SparePartsOrderViewModel;
                if (spareParts.SelectedOrderPosition.Received &&
                    string.IsNullOrEmpty(spareParts.SelectedOrderPosition.StoragePlace))
                {
                    System.Windows.MessageBox.Show(Properties.Resources.StoragePlaceNotSpecifiedText);
                    //var grid = sender as RadGridView;
                    //grid.CurrentColumn = grid.Columns[5];
                    return;
                }
                

            }

            if (orderViewModel != null)
            {
                orderViewModel.SaveChanges();
            }
        }

        private void OrderFiles_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var orderViewModel = frameworkElement?.DataContext as OrderViewModel;
            if (orderViewModel != null)
            {
                orderViewModel.OpenFile();
            }
        }

        private void GridViewDataControl_OnAutoGeneratingColumn(object sender, GridViewAutoGeneratingColumnEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var orderViewModel = frameworkElement?.DataContext as OrderViewModel;
            if (orderViewModel != null)
            {
                e.Column.IsReadOnly = !orderViewModel.IsEditable;
                if (e.Column.Header != null && (e.Column.Header.Equals(Properties.Resources.ReceivedColumnText)))
                {
                    e.Column.IsVisible = (orderViewModel.Status == StatusEnum.Confirmed ||
                                         orderViewModel.Status == StatusEnum.Received ||
                                         orderViewModel.Status == StatusEnum.Capitalized);
                    e.Column.IsReadOnly = orderViewModel.User.Role == RoleEnum.Manager;
                    e.Column.Width = new GridViewLength(1, GridViewLengthUnitType.Star);
                }
                if (e.Column.Header != null && e.Column.Header.Equals(Properties.Resources.StoragePlaceColumnText))
                {
                    var grid = sender as RadGridView;
                    var newColumn = new GridViewComboBoxColumn();
                    newColumn.ItemsSource = orderViewModel.StoragePlaces;
                    newColumn.SelectedValueMemberPath = "Place";
                    newColumn.DisplayMemberPath = "Place";
                    newColumn.Header = Properties.Resources.StoragePlaceColumnText;
                    newColumn.DataMemberBinding = new Binding("StoragePlace")
                    {
                        Mode = BindingMode.TwoWay
                    };
                    newColumn.IsComboBoxEditable = true;
                    newColumn.IsVisible = orderViewModel.Status == StatusEnum.Confirmed ||
                                          orderViewModel.Status == StatusEnum.Received ||
                                          orderViewModel.Status == StatusEnum.Capitalized;
                    newColumn.IsReadOnly = orderViewModel.User.Role == RoleEnum.Manager;
                    e.Column.Width = new GridViewLength(1, GridViewLengthUnitType.Star);
                    e.Column = newColumn;



                }
                if (e.Column.Header != null && (e.Column.Header.Equals(Properties.Resources.PriceText)
                                                || e.Column.Header.Equals(Properties.Resources.AmountText)
                                                || e.Column.Header.Equals(Properties.Resources.CurencyText)))
                {
                    if (orderViewModel != null)
                    {
                        e.Column.IsVisible = orderViewModel.Status != StatusEnum.ReadyToBeSent &&
                                             orderViewModel.Status != StatusEnum.New &&
                                             orderViewModel.User.Role == RoleEnum.Manager;
                        e.Column.Width = new GridViewLength(1, GridViewLengthUnitType.Star);
                    }
                }
                else
                {
                    e.Column.Width = new GridViewLength(1, GridViewLengthUnitType.Star);
                }
            }
        }

        private void PositionTab_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            

        }


        private void FrameworkElement_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var orderViewModel = frameworkElement?.DataContext as OrderViewModel;
            if (orderViewModel != null)
            {

                foreach (var column in (sender as RadGridView).Columns)
                {
                    column.Width = new GridViewLength(1, GridViewLengthUnitType.Star);
                    column.IsReadOnly = !orderViewModel.IsEditable;
                    if (column.Header != null && (column.Header.Equals(Properties.Resources.ReceivedColumnText)))
                    {
                        column.IsVisible = orderViewModel.Status == StatusEnum.Confirmed ||
                                             orderViewModel.Status == StatusEnum.Received;
                        column.IsReadOnly = false;
                    }
                    if (column.Header != null && column.Header.Equals(Properties.Resources.StoragePlaceColumnText))
                    {
                        column.IsVisible = orderViewModel.Status == StatusEnum.Confirmed ||
                                             orderViewModel.Status == StatusEnum.Received;
                        column.IsReadOnly = false;
                    }
                    if (column.Header != null && column.Header.Equals(Properties.Resources.DescriptionText))
                    {
                        column.Width = GridViewLength.Auto;
                    }
                    if (column.Header != null && column.Header.Equals(Properties.Resources.PartNumberTypeText))
                    {
                        column.Width = 100;
                    }
                    if (column.Header != null && (column.Header.Equals(Properties.Resources.PriceText)
                                                    || column.Header.Equals(Properties.Resources.AmountText)
                                                    || column.Header.Equals(Properties.Resources.CurencyText)))
                    {
                        if (orderViewModel != null)
                        {
                            column.IsVisible = orderViewModel.Status != StatusEnum.ReadyToBeSent &&
                                                 orderViewModel.Status != StatusEnum.New &&
                                                 orderViewModel.User.Role == RoleEnum.Manager;
                        }
                    }
                }
            }

        }

        private void GridViewDataControl_OnRowActivated(object sender, RowEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var orderViewModel = frameworkElement?.DataContext as OrderViewModel;
           
                if (orderViewModel.User.Role == RoleEnum.Capitan && (
                    orderViewModel.Status == StatusEnum.Received || orderViewModel.Status == StatusEnum.Capitalized))
                {
                    orderViewModel.UpdateStoragePlaces();
                }
            
        }
    }
}