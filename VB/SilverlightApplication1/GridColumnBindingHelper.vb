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
Imports DevExpress.Xpf.Grid
Imports System.Collections
Imports System.Reflection
Imports System.Collections.Generic
Imports System.Dynamic

Namespace SilverlightApplication1
	Public Class GridBindingHelper


		Private Class ComplextPath
			Private Class PathPartSimple
				Private Class GetMemberValueBinder
					Inherits GetMemberBinder
					Public Sub New(ByVal name As String)
						MyBase.New(name, False)
					End Sub
					Public Overrides Function FallbackGetMember(ByVal self As DynamicMetaObject, ByVal onBindingError As DynamicMetaObject) As DynamicMetaObject
						Return onBindingError
					End Function
				End Class
				Private Class SetMemberValueBinder
					Inherits SetMemberBinder
					Public Sub New(ByVal propertyName As String)
						MyBase.New(propertyName, False)
					End Sub
					Public Overrides Function FallbackSetMember(ByVal target As DynamicMetaObject, ByVal value As DynamicMetaObject, ByVal errorSuggestion As DynamicMetaObject) As DynamicMetaObject
						Return errorSuggestion
					End Function
				End Class
				Private ReadOnly propertyName As String
				Public Sub New(ByVal propertyName As String)
					Me.propertyName = propertyName
				End Sub

				Friend Overridable Function CalcValue(ByVal row As Object) As Object
					If TypeOf row Is DynamicObject Then
						Dim binder As New GetMemberValueBinder(propertyName)
						Dim result As Object
						If (CType(row, DynamicObject)).TryGetMember(binder, result) Then
							Return result
						End If
						Return Nothing
					End If
					Dim propertyInfo As PropertyInfo = GetPropertyInfo(row)
					If propertyInfo Is Nothing Then
						Return Nothing
					End If
					Return propertyInfo.GetValue(row, Nothing)
				End Function

				Friend Overridable Sub SetValue(ByVal row As Object, ByVal value As Object)
					If TypeOf row Is DynamicObject Then
						Dim binder As New SetMemberValueBinder(propertyName)
						CType(row, DynamicObject).TrySetMember(binder, value)
						Return
					End If
					Dim propertyInfo As PropertyInfo = GetPropertyInfo(row)
					If propertyInfo Is Nothing Then
						Return
					End If
					propertyInfo.SetValue(row, value, Nothing)
				End Sub
				Private Function GetPropertyInfo(ByVal row As Object) As PropertyInfo
					Return row.GetType().GetProperty(propertyName, BindingFlags.Public Or BindingFlags.Instance)
				End Function
			End Class
			Private Class PathPartList
				Inherits PathPartSimple
				Private ReadOnly index As Integer
				Public Sub New(ByVal propertyName As String, ByVal index As Integer)
					MyBase.New(propertyName)
					Me.index = index
				End Sub
				Friend Overrides Function CalcValue(ByVal row As Object) As Object
					Dim list As IList = GetList(row)
					If list Is Nothing Then
						Return Nothing
					End If
					Return list(index)
				End Function
				Friend Overrides Sub SetValue(ByVal row As Object, ByVal value As Object)
					Dim list As IList = GetList(row)
					If list Is Nothing Then
						Return
					End If
					list(index) = value
				End Sub
				Private Function GetList(ByVal row As Object) As IList
					Return TryCast(MyBase.CalcValue(row), IList)
				End Function
			End Class
			Private Class PathPartDictionary
				Inherits PathPartSimple
				Private ReadOnly index As String
				Public Sub New(ByVal propertyName As String, ByVal index As String)
					MyBase.New(propertyName)
					Me.index = index
				End Sub
				Friend Overrides Function CalcValue(ByVal row As Object) As Object
					Dim dict As IDictionary = GetDictionary(row)
					If dict Is Nothing Then
						Return Nothing
					End If
					Return dict(index)
				End Function
				Friend Overrides Sub SetValue(ByVal row As Object, ByVal value As Object)
					Dim dict As IDictionary = GetDictionary(row)
					If dict Is Nothing Then
						Return
					End If
					dict(index) = value
				End Sub
				Private Function GetDictionary(ByVal row As Object) As IDictionary
					Return TryCast(MyBase.CalcValue(row), IDictionary)
				End Function
			End Class
			Private pathParts As New List(Of PathPartSimple)()
			Public Sub New(ByVal complexPath As String)
				Dim paths() As String = complexPath.Split("."c)
				For i As Integer = 0 To paths.Length - 1
					Dim path As String = paths(i)
					Dim braceIndex As Integer = path.IndexOf("[")
					Dim index As String = Nothing
					If braceIndex >= 0 Then
						index = path.Substring(braceIndex + 1, path.Length - braceIndex - 2)
						path = path.Substring(0, braceIndex)
					End If
					Dim pathPart As PathPartSimple = Nothing
					If (Not String.IsNullOrEmpty(index)) AndAlso Char.IsDigit(index.Chars(0)) Then
						pathPart = New PathPartList(path, Integer.Parse(index))
					ElseIf (Not String.IsNullOrEmpty(index)) Then
						pathPart = New PathPartDictionary(path, index)
					Else
						pathPart = New PathPartSimple(path)
					End If
					pathParts.Add(pathPart)
				Next i
			End Sub
			Public Function CalcValue(ByVal row As Object) As Object
				Return CalcValueCore(row, False)
			End Function

			Public Sub SetValue(ByVal row As Object, ByVal value As Object)
				Dim lastValue As Object = CalcValueCore(row, True)
				If lastValue Is Nothing Then
					Return
				End If
				pathParts(pathParts.Count - 1).SetValue(lastValue, value)
			End Sub
			Public Function CalcValueCore(ByVal row As Object, ByVal skipLastPath As Boolean) As Object
				For i As Integer = 0 To pathParts.Count - (If(skipLastPath, 1, 0)) - 1
					row = pathParts(i).CalcValue(row)
					If row Is Nothing Then
						Exit For
					End If
				Next i
				Return row
			End Function
		End Class


		Private Shared Function GetComplexPath(ByVal obj As GridColumn) As ComplextPath
			Return CType(obj.GetValue(ComplexPathProperty), ComplextPath)
		End Function

		Private Shared Sub SetComplexPath(ByVal obj As GridColumn, ByVal value As ComplextPath)
			obj.SetValue(ComplexPathProperty, value)
		End Sub

		Public Shared ReadOnly ComplexPathProperty As DependencyProperty = DependencyProperty.RegisterAttached("ComplexPath", GetType(ComplextPath), GetType(ComplextPath), New PropertyMetadata(Nothing))



		Public Shared Function GetComplexFieldName(ByVal obj As GridColumn) As String
			Return CStr(obj.GetValue(ComplexFieldNameProperty))
		End Function

		Public Shared Sub SetComplexFieldName(ByVal obj As GridColumn, ByVal value As String)
			obj.SetValue(ComplexFieldNameProperty, value)
		End Sub

        Public Shared ReadOnly ComplexFieldNameProperty As DependencyProperty = DependencyProperty.RegisterAttached("ComplexFieldName", GetType(String), GetType(GridBindingHelper), New PropertyMetadata(Nothing, AddressOf OnComplexFieldNameChanged))

		Private Shared Sub OnComplexFieldNameChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
			Dim column As GridColumn = CType(d, GridColumn)
			If (Not String.IsNullOrEmpty(CStr(e.OldValue))) Then
				column.ClearValue(ComplexPathProperty)
			End If
			column.FieldName = CStr(e.NewValue)
			If (Not String.IsNullOrEmpty(column.FieldName)) Then
				SetComplexPath(column, New ComplextPath(column.FieldName))
			End If
		End Sub

		Public Shared Function GetItemsSource(ByVal obj As GridControl) As IEnumerable
			Return CType(obj.GetValue(ItemsSourceProperty), IEnumerable)
		End Function

		Public Shared Sub SetItemsSource(ByVal obj As GridControl, ByVal value As IEnumerable)
			obj.SetValue(ItemsSourceProperty, value)
		End Sub

        Public Shared ReadOnly ItemsSourceProperty As DependencyProperty = DependencyProperty.RegisterAttached("ItemsSource", GetType(IEnumerable), GetType(GridBindingHelper), New PropertyMetadata(Nothing, AddressOf OnItemsSourceChanged))

		Private Shared Sub OnItemsSourceChanged(ByVal d As DependencyObject, ByVal e As DependencyPropertyChangedEventArgs)
			Dim grid As GridControl = CType(d, GridControl)
			If e.OldValue IsNot Nothing Then
				RemoveHandler grid.CustomUnboundColumnData, AddressOf grid_CustomUnboundColumnData
			End If
            grid.ItemsSource = e.NewValue
			If e.NewValue IsNot Nothing Then
				AddHandler grid.CustomUnboundColumnData, AddressOf grid_CustomUnboundColumnData
			End If
		End Sub

		Private Shared Sub grid_CustomUnboundColumnData(ByVal sender As Object, ByVal e As GridColumnDataEventArgs)
			If String.IsNullOrEmpty(GetComplexFieldName(e.Column)) Then
				Return
			End If
			Dim complexPath As ComplextPath = GetComplexPath(e.Column)
			Dim grid As GridControl = CType(sender, GridControl)
			If e.IsGetData Then
				e.Value = complexPath.CalcValue(grid.GetRowByListIndex(e.ListSourceRowIndex))
			End If
			If e.IsSetData Then
				complexPath.SetValue(grid.GetRowByListIndex(e.ListSourceRowIndex), e.Value)
			End If
		End Sub
	End Class
End Namespace
