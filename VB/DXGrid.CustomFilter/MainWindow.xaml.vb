Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Windows
Imports DevExpress.Xpf.Editors
Imports DevExpress.Xpf.Editors.Native
Imports DevExpress.Xpf.Editors.Popups
Imports DevExpress.Xpf.Grid

Namespace DXGrid.CustomFilter
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>
	Partial Public Class MainWindow
		Inherits Window
		Private commandItem As CustomComboBoxItem
		Private focusedColumn As GridColumn
		Private combo As ComboBoxEdit
		Public Sub New()
			InitializeComponent()
			Me.dxGrid.ItemsSource = DataSample.GetTasks(10000)
		End Sub

		Private Sub view_ShowFilterPopup(ByVal sender As Object, ByVal e As FilterPopupEventArgs)
			combo = e.ComboBoxEdit
			Dim list As List(Of Object) = TryCast(combo.ItemsSource, List(Of Object))
			Dim customItem As New CustomComboBoxItem() With {.DisplayValue = "(Custom)", .EditValue = New CustomComboBoxItem()}
			Dim data As New List(Of Object)()
			Dim index As Integer = 0
			For i As Integer = 0 To list.Count - 1
				data.Add(list(i))
				If TypeOf list(i) Is CustomComboBoxItem Then
					If (TryCast(list(i), CustomComboBoxItem)).DisplayValue.ToString() = "(Non blanks)" Then
						index = i
						Exit For
					End If
				End If
			Next i
			list.RemoveRange(0, index + 1)
			data.Add(customItem)
			data.AddRange(list)
			combo.ItemsSource = data
			AddHandler e.ComboBoxEdit.PopupOpened, AddressOf ComboBoxEdit_PopupOpened
			focusedColumn = CType(e.Column, GridColumn)
			e.Handled = True
		End Sub

		Private Sub OnPopupClosed(ByVal sender As Object, ByVal e As RoutedEventArgs)
			e.Handled = True
			If commandItem IsNot Nothing Then
				commandItem = Nothing
				Dim wndFilter As New FilterWindow()
				wndFilter.Owner = Me
				wndFilter.GridColumn = focusedColumn
				If CBool(wndFilter.ShowDialog()) Then
					Me.dxGrid.FilterString = wndFilter.FilterString
				End If
			End If
		End Sub

		Private Sub ComboBoxEdit_PopupOpened(ByVal sender As Object, ByVal e As RoutedEventArgs)
			Dim edit As ComboBoxEdit = TryCast(sender, ComboBoxEdit)
			Dim plb As PopupListBox = (CType(LookUpEditHelper.GetVisualClient(edit).InnerEditor, PopupListBox))
			AddHandler plb.SelectionChanged, AddressOf plb_SelectionChanged
			e.Handled = True
		End Sub

		Private Sub plb_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs)
			Dim plb As PopupListBox = TryCast(sender, PopupListBox)
			Me.commandItem = Nothing
			e.Handled = True
			Dim customItem As CustomComboBoxItem = TryCast(plb.SelectedItem, CustomComboBoxItem)
			If customItem Is Nothing Then
				Return
			End If
			If customItem.DisplayValue = "(Custom)" Then
				Me.commandItem = customItem
			End If
		End Sub
	End Class


End Namespace
