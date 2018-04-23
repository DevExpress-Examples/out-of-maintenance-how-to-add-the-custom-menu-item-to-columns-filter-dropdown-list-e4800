Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports System.Windows.Input
Imports DevExpress.Xpf.Editors
Imports DevExpress.Xpf.Grid
Imports System.Windows.Controls
Imports System.Windows.Media

Namespace DXGrid.CustomFilter
    ''' <summary>
    ''' Interaction logic for FilterWindow.xaml
    ''' </summary>
    Partial Public Class FilterWindow
        Inherits Window

        Private ReadOnly Property StringConditions() As String()
            Get
                Return stringConditions_Renamed
            End Get
        End Property
        Private ReadOnly Property NumericConditions() As String()
            Get
                Return intConditions
            End Get
        End Property
        Private stringConditions_Renamed() As String = {"Is Like", "Is Not Like", "Equals", "Does Not Equal", "Is Greater Than", "Is Greater Than Or Equal To", "Is Less Than", "Is Less Than Or Equal To", "Is Null", "Is Not Null"}
        Private intConditions() As String = {"Equals", "Does Not Equal", "Is Greater Than", "Is Greater Than Or Equal To", "Is Less Than", "Is Less Than Or Equal To", "Is Null", "Is Not Null"}

        Public Shared ReadOnly FilterStringProperty As DependencyProperty = DependencyProperty.Register("FilterString", GetType(String), GetType(FilterWindow), New PropertyMetadata(""))
        Public Property FilterString() As String
            Get
                Return CStr(GetValue(FilterStringProperty))
            End Get
            Set(ByVal value As String)
                SetValue(FilterStringProperty, value)
            End Set
        End Property

        Public Shared ReadOnly GridColumnProperty As DependencyProperty = DependencyProperty.Register("GridColumn", GetType(GridColumn), GetType(FilterWindow), New PropertyMetadata(Nothing, New PropertyChangedCallback(AddressOf OnGridColumnPropertyChanged)))
        Public Property GridColumn() As GridColumn
            Get
                Return CType(GetValue(GridColumnProperty), GridColumn)
            End Get
            Set(ByVal value As GridColumn)
                SetValue(GridColumnProperty, value)
            End Set
        End Property

        Private Shared Sub OnGridColumnPropertyChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
            Dim cts As CustomTemplateSelector = CustomTemplateSelector.Instance
            Dim gc As GridColumn = TryCast(e.NewValue, GridColumn)
            Dim t As Type = gc.FieldType
            Select Case t.Name
                Case "DateTime"
                    cts.MainTemplate = CType((TryCast(d, FilterWindow)).Resources("DateTimeData"), DataTemplate)
                    TryCast(d, FilterWindow).cmbOperationsH.ItemsSource = (TryCast(d, FilterWindow)).NumericConditions
                    TryCast(d, FilterWindow).cmbOperationsL.ItemsSource = (TryCast(d, FilterWindow)).NumericConditions
                    Exit Select
                Case "String"
                    cts.MainTemplate = CType((TryCast(d, FilterWindow)).Resources("RegularData"), DataTemplate)
                    TryCast(d, FilterWindow).cmbOperationsH.ItemsSource = (TryCast(d, FilterWindow)).StringConditions
                    TryCast(d, FilterWindow).cmbOperationsL.ItemsSource = (TryCast(d, FilterWindow)).StringConditions
                    Exit Select
                Case Else
                    cts.MainTemplate = CType((TryCast(d, FilterWindow)).Resources("RegularData"), DataTemplate)
                    TryCast(d, FilterWindow).cmbOperationsH.ItemsSource = (TryCast(d, FilterWindow)).NumericConditions
                    TryCast(d, FilterWindow).cmbOperationsL.ItemsSource = (TryCast(d, FilterWindow)).NumericConditions
            End Select
        End Sub

        Public Sub New()
            InitializeComponent()
            DataContext = Me

        End Sub

        Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.DialogResult = False
            Me.Close()
        End Sub

        Private Sub btnOk_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
            Me.CreateFilterString()
        End Sub

        Private Function SetFormat(ByVal input As String) As String
            Dim t As Type = GridColumn.FieldType
            If t.Name = "String" Then
                Return String.Format("'{0}'", input)
            End If
            Return input
        End Function

        Private Function BuildFilterExpression(ByVal exprValue As String, ByVal condition As String) As String
            Return String.Format("[{0}] {1} {2}", Me.GridColumn.FieldName, condition, exprValue)
        End Function

        Private Function GetComparisonOperator(ByVal choice As String) As String
            Dim result As String = ""
            Select Case choice
                Case "Is Like"
                    result = "LIKE"
                    Exit Select
                Case "Is Not Like"
                    result = "NOT LIKE"
                    Exit Select
                Case "Equals"
                    result = "="
                    Exit Select
                Case "Does Not Equal"
                    result = "<>"
                    Exit Select
                Case "Is Greater Than"
                    result = ">"
                    Exit Select
                Case "Is Greater Than Or Equal To"
                    result = ">="
                    Exit Select
                Case "Is Less Than"
                    result = "<"
                    Exit Select
                Case "Is Less Than Or Equal To"
                    result = "<="
                    Exit Select
                Case "Is Null"
                    result = "IS NULL"
                    Exit Select
                Case "Is Not Null"
                    result = "IS NOT NULL"
                    Exit Select
                Case Else
                    result = ""
            End Select
            If result = "" Then
                Throw New ArgumentException("choice error")
            End If
            Return result

        End Function

        Private Function GetOrAndExpression() As String
            Return If(CBool(rbAnd.IsChecked), "AND", "OR")
        End Function

        Private Sub presenter_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs)
            If e.Key = Key.Enter Then
                Me.CreateFilterString()
            End If

        End Sub

        Private Sub CreateFilterString()
            Dim h As DependencyObject = VisualTreeHelper.GetChild(presenterH, 0)
            Dim l As DependencyObject = VisualTreeHelper.GetChild(presenterL, 0)
            Dim filterExpressionL As String = ""
            Dim filter As String = ""
            If h.DependencyObjectType.Name = "TextEdit" Then
                Dim editorH As TextEdit = CType(h, TextEdit)
                Dim editorL As TextEdit = CType(l, TextEdit)
                If editorL.Text IsNot "" AndAlso editorL.Text IsNot "enter value" Then
                    Dim conditionL As String = GetComparisonOperator(cmbOperationsL.SelectedItem.ToString())
                    Dim valL As String = SetFormat(editorL.Text)
                    filterExpressionL = GetOrAndExpression() & " " & BuildFilterExpression(valL, conditionL)
                End If
                Dim conditionH As String = GetComparisonOperator(cmbOperationsH.SelectedItem.ToString())
                Dim valH As String = SetFormat(editorH.Text)
                filter = BuildFilterExpression(valH, conditionH) & " " & filterExpressionL
            End If
            If h.DependencyObjectType.Name = "DateEdit" Then
                Dim editorH As DateEdit = CType(h, DateEdit)
                Dim editorL As DateEdit = CType(l, DateEdit)
                If editorL.EditValue IsNot Nothing AndAlso editorL.Text IsNot "enter value" Then
                    Dim conditionL As String = GetComparisonOperator(cmbOperationsL.SelectedItem.ToString())
                    Dim valL As String = "#" & editorL.EditValue.ToString() & "#"
                    filterExpressionL = GetOrAndExpression() & " " & BuildFilterExpression(valL, conditionL)
                End If
                Dim conditionH As String = GetComparisonOperator(cmbOperationsH.SelectedItem.ToString())
                Dim valH As String = "#" & editorH.EditValue & "#"
                filter = BuildFilterExpression(valH, conditionH) & " " & filterExpressionL
            End If
            Me.DialogResult = True

            Me.FilterString = filter
        End Sub

    End Class

    Public Class CustomTemplateSelector
        Inherits DataTemplateSelector
        Private privateMainTemplate As DataTemplate
        Public Property MainTemplate() As DataTemplate
            Get
                Return privateMainTemplate
            End Get
            Set(ByVal value As DataTemplate)
                privateMainTemplate = value
            End Set
        End Property
        Private Shared privateInstance As CustomTemplateSelector
        Public Shared Property Instance() As CustomTemplateSelector
            Get
                Return privateInstance
            End Get
            Private Set(ByVal value As CustomTemplateSelector)
                privateInstance = value
            End Set
        End Property
        Public Sub New()
            Instance = Me
        End Sub
        Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As DependencyObject) As DataTemplate
            Return CType(MainTemplate, DataTemplate)
        End Function
    End Class
End Namespace
