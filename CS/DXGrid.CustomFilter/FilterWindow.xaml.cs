using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using System.Windows.Controls;
using System.Windows.Media;

namespace DXGrid.CustomFilter {
    /// <summary>
    /// Interaction logic for FilterWindow.xaml
    /// </summary>
    public partial class FilterWindow : Window {
        #region
        string[] StringConditions { get { return stringConditions; } }
        string[] NumericConditions { get { return intConditions; } }
        string[] stringConditions = new string[] {
            "Is Like", 
            "Is Not Like",
            "Equals", 
            "Does Not Equal", 
            "Is Greater Than",
            "Is Greater Than Or Equal To",
            "Is Less Than",
            "Is Less Than Or Equal To",
            "Is Null", 
            "Is Not Null"            
        };
        string[] intConditions = new string[] {
            "Equals", 
            "Does Not Equal", 
            "Is Greater Than",
            "Is Greater Than Or Equal To",
            "Is Less Than",
            "Is Less Than Or Equal To",
            "Is Null", 
            "Is Not Null",            
        };
        #endregion
        public static readonly DependencyProperty FilterStringProperty = DependencyProperty.Register("FilterString", typeof(string), typeof(FilterWindow), new PropertyMetadata(""));
        public string FilterString {
            get { return (string)GetValue(FilterStringProperty); }
            set { SetValue(FilterStringProperty, value); }
        }

        public static readonly DependencyProperty GridColumnProperty = DependencyProperty.Register("GridColumn", typeof(GridColumn), typeof(FilterWindow), new PropertyMetadata(null, new PropertyChangedCallback(OnGridColumnPropertyChanged)));
        public GridColumn GridColumn {
            get { return (GridColumn)GetValue(GridColumnProperty); }
            set { SetValue(GridColumnProperty, value); }
        }

        static void OnGridColumnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            CustomTemplateSelector cts = CustomTemplateSelector.Instance;
            GridColumn gc = e.NewValue as GridColumn;
            Type t = gc.FieldType;
            switch(t.Name) {
                case "DateTime": {
                        cts.MainTemplate = (DataTemplate)(d as FilterWindow).Resources["DateTimeData"];
                        (d as FilterWindow).cmbOperationsH.ItemsSource = (d as FilterWindow).NumericConditions;
                        (d as FilterWindow).cmbOperationsL.ItemsSource = (d as FilterWindow).NumericConditions;
                        break;
                    }
                case "String": {
                        cts.MainTemplate = (DataTemplate)(d as FilterWindow).Resources["RegularData"];
                        (d as FilterWindow).cmbOperationsH.ItemsSource = (d as FilterWindow).StringConditions;
                        (d as FilterWindow).cmbOperationsL.ItemsSource = (d as FilterWindow).StringConditions;
                        break;
                    }
                default: {
                        cts.MainTemplate = (DataTemplate)(d as FilterWindow).Resources["RegularData"];
                        (d as FilterWindow).cmbOperationsH.ItemsSource = (d as FilterWindow).NumericConditions;
                        (d as FilterWindow).cmbOperationsL.ItemsSource = (d as FilterWindow).NumericConditions;
                    }
                    break;
            }
        }

        public FilterWindow() {
            InitializeComponent();
            DataContext = this;

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) {
            this.CreateFilterString();
        }

        string SetFormat(string input) {
            Type t = GridColumn.FieldType;
            if(t.Name == "String") {
                return string.Format(@"'{0}'", input);
            }
            return input;
        }

        string BuildFilterExpression(string exprValue, string condition) {
            return string.Format("[{0}] {1} {2}", this.GridColumn.FieldName, condition, exprValue);
        }

        string GetComparisonOperator(string choice) {
            string result = "";
            switch(choice) {
                case "Is Like": {
                        result = "LIKE";
                        break;
                    }
                case "Is Not Like": {
                        result = "NOT LIKE";
                        break;
                    }
                case "Equals": {
                        result = "=";
                        break;
                    }
                case "Does Not Equal": {
                        result = "<>";
                        break;
                    }
                case "Is Greater Than": {
                        result = ">";
                        break;
                    }
                case "Is Greater Than Or Equal To": {
                        result = ">=";
                        break;
                    }
                case "Is Less Than": {
                        result = "<";
                        break;
                    }
                case "Is Less Than Or Equal To": {
                        result = "<=";
                        break;
                    }
                case "Is Null": {
                        result = "IS NULL";
                        break;
                    }
                case "Is Not Null": {
                        result = "IS NOT NULL";
                        break;
                    }
                default: {
                        result = "";
                    }
                    break;
            }
            if(result == "") {
                throw new ArgumentException("choice error");
            }
            return result;

        }

        string GetOrAndExpression() {
            return (bool)rbAnd.IsChecked ? "AND" : "OR";
        }

        private void presenter_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if(e.Key == Key.Enter) {
                this.CreateFilterString();
            }

        }

        void CreateFilterString() {
            DependencyObject h = VisualTreeHelper.GetChild(presenterH, 0);
            DependencyObject l = VisualTreeHelper.GetChild(presenterL, 0);
            string filterExpressionL = "";
            string filter = "";
            if(h.DependencyObjectType.Name == "TextEdit") {
                TextEdit editorH = (TextEdit)h;
                TextEdit editorL = (TextEdit)l;
                if(editorL.Text != "" && editorL.Text != "enter value") {
                    string conditionL = GetComparisonOperator(cmbOperationsL.SelectedItem.ToString());
                    string valL = SetFormat(editorL.Text);
                    filterExpressionL = GetOrAndExpression() + " " + BuildFilterExpression(valL, conditionL);
                }
                string conditionH = GetComparisonOperator(cmbOperationsH.SelectedItem.ToString());
                string valH = SetFormat(editorH.Text);
                filter = BuildFilterExpression(valH, conditionH) + " " + filterExpressionL;
            }
            if(h.DependencyObjectType.Name == "DateEdit") {
                DateEdit editorH = (DateEdit)h;
                DateEdit editorL = (DateEdit)l;
                if(editorL.EditValue != null && editorL.Text != "enter value") {
                    string conditionL = GetComparisonOperator(cmbOperationsL.SelectedItem.ToString());
                    string valL = "#" + editorL.EditValue.ToString() + "#";
                    filterExpressionL = GetOrAndExpression() + " " + BuildFilterExpression(valL, conditionL);
                }
                string conditionH = GetComparisonOperator(cmbOperationsH.SelectedItem.ToString());
                string valH = "#" + editorH.EditValue + "#";
                filter = BuildFilterExpression(valH, conditionH) + " " + filterExpressionL;
            }
            this.DialogResult = true;

            this.FilterString = filter;
        }

    }

    public class CustomTemplateSelector : DataTemplateSelector {
        public DataTemplate MainTemplate { get; set; }
        public static CustomTemplateSelector Instance { get; private set; }
        public CustomTemplateSelector() {
            Instance = this;
        }
        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            return (DataTemplate)MainTemplate;
        }
    }
}
