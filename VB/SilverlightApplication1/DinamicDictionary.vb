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
Imports System.Dynamic
Imports System.Collections.Generic

Namespace SilverlightApplication1
	Public Class DynamicDictionary
		Inherits DynamicObject
		Private dictionary As New Dictionary(Of String, Object)()

		Public ReadOnly Property Count() As Integer
			Get
				Return dictionary.Count
			End Get
		End Property
		Public Overrides Function TryGetMember(ByVal binder As GetMemberBinder, <System.Runtime.InteropServices.Out()> ByRef result As Object) As Boolean
			Dim name As String = binder.Name.ToLower()
			Return dictionary.TryGetValue(name, result)
		End Function

		Public Overrides Function TrySetMember(ByVal binder As SetMemberBinder, ByVal value As Object) As Boolean
			dictionary(binder.Name.ToLower()) = value
			Return True
		End Function
	End Class
End Namespace
