using System;
using System.Collections.Generic;
using System.Windows;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Grid;

namespace DXGrid.CustomFilter {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        CustomComboBoxItem commandItem;
        GridColumn focusedColumn;
        ComboBoxEdit combo;
        public MainWindow() {
            InitializeComponent();
            this.dxGrid.ItemsSource = DataSample.GetTasks(10000);
        }

        private void view_ShowFilterPopup(object sender, FilterPopupEventArgs e) {
            combo = e.ComboBoxEdit;
            List<object> list = combo.ItemsSource as List<object>;
            CustomComboBoxItem customItem = new CustomComboBoxItem() { DisplayValue = "(Custom)", EditValue = new CustomComboBoxItem() };
            List<object> data = new List<object>();
            int index = 0;
            for (int i = 0; i < list.Count; i++)
            {
                data.Add(list[i]);
                if (list[i] is CustomComboBoxItem)
                {
                    if ((list[i] as CustomComboBoxItem).DisplayValue.ToString() == "(Non blanks)")
                    {
                        index = i;
                        break;
                    }
                }
            }
            list.RemoveRange(0, index + 1);
            data.Add(customItem);
            data.AddRange(list);
            combo.ItemsSource = data;
            e.ComboBoxEdit.PopupOpened += ComboBoxEdit_PopupOpened;
            focusedColumn = (GridColumn)e.Column;
            e.Handled = true;
        }

        private void OnPopupClosed(object sender, RoutedEventArgs e) {            
            e.Handled = true;
            if(commandItem != null) {
                commandItem = null;
                FilterWindow wndFilter = new FilterWindow();
                wndFilter.Owner = this;
                wndFilter.GridColumn = focusedColumn;
                if((bool)wndFilter.ShowDialog()) {
                    this.dxGrid.FilterString = wndFilter.FilterString;
                }
            }
        }

        void ComboBoxEdit_PopupOpened(object sender, RoutedEventArgs e) {            
            ComboBoxEdit edit = sender as ComboBoxEdit;
            PopupListBox plb = ((PopupListBox)LookUpEditHelper.GetVisualClient(edit).InnerEditor);
            plb.SelectionChanged += plb_SelectionChanged;
            e.Handled = true;
        }

        void plb_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {            
            PopupListBox plb = sender as PopupListBox;
            this.commandItem = null;
            e.Handled = true;
            CustomComboBoxItem customItem = plb.SelectedItem as CustomComboBoxItem;
            if(customItem == null) {
                return;
            }
            if(customItem.DisplayValue == "(Custom)") {
                this.commandItem = customItem;
            }
        }
    }


}
