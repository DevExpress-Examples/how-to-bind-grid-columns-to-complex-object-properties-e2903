﻿Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports DevExpress.Xpf.Core

Namespace SilverlightApplication1
	Partial Public Class MainPage
		Inherits UserControl
		Public Sub New()
			InitializeComponent()
			DataContext = New ViewModel()
		End Sub

	End Class
End Namespace
