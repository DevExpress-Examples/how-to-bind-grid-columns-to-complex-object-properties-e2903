Imports Microsoft.VisualBasic
Imports System
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports System.Collections.Generic
Imports DevExpress.Xpf.Core.Commands
Imports System.ComponentModel
Imports System.Dynamic

Namespace SilverlightApplication1
	Public Class ViewModel
		Private privateRows As IList(Of RowModel)
		Public Property Rows() As IList(Of RowModel)
			Get
				Return privateRows
			End Get
			Private Set(ByVal value As IList(Of RowModel))
				privateRows = value
			End Set
		End Property
		Public Sub New()
			Rows = New List(Of RowModel)()
			For i As Integer = 0 To 49
				Dim row As New RowModel(3)
				row.Properties(0).Value = "row" & i
				row.Properties(1).Value = i
				row.Properties(2).Value = i Mod 2 = 0
				row.Dictionary("Text") = New PropertyValue() With {.Value = "row" & i}
				row.Dictionary("Number") = New PropertyValue() With {.Value = i}
				row.Dictionary("Bool") = New PropertyValue() With {.Value = i Mod 2 = 0}
				row.DynamicObject.Text = "row" & i
				row.DynamicObject.Number = i
				row.DynamicObject.Bool = i Mod 2 = 0

				Rows.Add(row)
			Next i
            ChangedSelectedRowCommand = New DelegateCommand(Of Object)(AddressOf ChangeSelectedRow)
		End Sub
		Private privateSelectedRow As RowModel
		Public Property SelectedRow() As RowModel
			Get
				Return privateSelectedRow
			End Get
			Set(ByVal value As RowModel)
				privateSelectedRow = value
			End Set
		End Property
		Private privateChangedSelectedRowCommand As ICommand
		Public Property ChangedSelectedRowCommand() As ICommand
			Get
				Return privateChangedSelectedRowCommand
			End Get
			Private Set(ByVal value As ICommand)
				privateChangedSelectedRowCommand = value
			End Set
		End Property
		Private Sub ChangeSelectedRow()
			If SelectedRow IsNot Nothing Then
				SelectedRow.SetValue(1, CInt(Fix(SelectedRow.Properties(1).Value)) + 1)
			End If
		End Sub
	End Class
	Public Class RowModel
		Implements INotifyPropertyChanged
        Private privateDynamicObject As Object
        Public Property DynamicObject() As Object
            Get
                Return privateDynamicObject
            End Get
            Private Set(ByVal value As Object)
                privateDynamicObject = value
            End Set
        End Property
		Private privateProperties As List(Of PropertyValue)
		Public Property Properties() As List(Of PropertyValue)
			Get
				Return privateProperties
			End Get
			Private Set(ByVal value As List(Of PropertyValue))
				privateProperties = value
			End Set
		End Property
		Private privateDictionary As Dictionary(Of String, PropertyValue)
		Public Property Dictionary() As Dictionary(Of String, PropertyValue)
			Get
				Return privateDictionary
			End Get
			Private Set(ByVal value As Dictionary(Of String, PropertyValue))
				privateDictionary = value
			End Set
		End Property
		Public Sub New(ByVal propertyCount As Integer)
			Properties = New List(Of PropertyValue)()
			Dictionary = New Dictionary(Of String, PropertyValue)()
			DynamicObject = New DynamicDictionary()
			For i As Integer = 0 To propertyCount - 1
				Properties.Add(New PropertyValue())
			Next i
		End Sub
		Public Sub SetValue(ByVal propertyIndex As Integer, ByVal value As Object)
			Properties(propertyIndex).Value = value
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(Nothing))
		End Sub
		Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
	End Class
	Public Class PropertyValue
		Private privateValue As Object
		Public Property Value() As Object
			Get
				Return privateValue
			End Get
			Set(ByVal value As Object)
				privateValue = value
			End Set
		End Property
	End Class
End Namespace
