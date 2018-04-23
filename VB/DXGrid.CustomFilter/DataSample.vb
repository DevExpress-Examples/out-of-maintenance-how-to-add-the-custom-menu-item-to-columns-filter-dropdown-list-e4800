Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks

Namespace DXGrid.CustomFilter
	Friend Class DataSample
		Private privateId As Integer
		Public Property Id() As Integer
			Get
				Return privateId
			End Get
			Set(ByVal value As Integer)
				privateId = value
			End Set
		End Property
		Private privateName As String
		Public Property Name() As String
			Get
				Return privateName
			End Get
			Set(ByVal value As String)
				privateName = value
			End Set
		End Property
		Private privateStartDate As DateTime
		Public Property StartDate() As DateTime
			Get
				Return privateStartDate
			End Get
			Set(ByVal value As DateTime)
				privateStartDate = value
			End Set
		End Property
		Private privateFinishDate As DateTime
		Public Property FinishDate() As DateTime
			Get
				Return privateFinishDate
			End Get
			Set(ByVal value As DateTime)
				privateFinishDate = value
			End Set
		End Property
		Private privateIsCompleted As Boolean
		Public Property IsCompleted() As Boolean
			Get
				Return privateIsCompleted
			End Get
			Set(ByVal value As Boolean)
				privateIsCompleted = value
			End Set
		End Property

		Public Shared Function GetTasks(ByVal count As Integer) As List(Of DataSample)
			Dim rnd As New Random()
			Dim data As New List(Of DataSample)()
			For i As Integer = 0 To count - 1
				Dim ts As New DataSample()
				ts.Id = i
				ts.Name = "Name-" & i
				ts.StartDate = DateTime.Now
				ts.FinishDate = DateTime.Now.AddDays(CDbl(i))
				ts.IsCompleted = If(rnd.Next(1, 3) = 1, True, False)
				data.Add(ts)
			Next i
			Return data
		End Function
	End Class
End Namespace
