<!-- default file list -->
*Files to look at*:

* [DinamicDictionary.cs](./CS/SilverlightApplication1/DinamicDictionary.cs) (VB: [DinamicDictionary.vb](./VB/SilverlightApplication1/DinamicDictionary.vb))
* [GridColumnBindingHelper.cs](./CS/SilverlightApplication1/GridColumnBindingHelper.cs) (VB: [GridColumnBindingHelper.vb](./VB/SilverlightApplication1/GridColumnBindingHelper.vb))
* [MainPage.xaml](./CS/SilverlightApplication1/MainPage.xaml) (VB: [MainPage.xaml](./VB/SilverlightApplication1/MainPage.xaml))
* [MainPage.xaml.cs](./CS/SilverlightApplication1/MainPage.xaml.cs) (VB: [MainPage.xaml.vb](./VB/SilverlightApplication1/MainPage.xaml.vb))
* [ViewModel.cs](./CS/SilverlightApplication1/ViewModel.cs) (VB: [ViewModel.vb](./VB/SilverlightApplication1/ViewModel.vb))
<!-- default file list end -->
# How to bind grid columns to complex object properties


<p><strong>Updated</strong><br><br><em>Starting with version 11.1.3, you can use the built-in Binding/DisplayMemberBinding property to bind to nested properties and/or indexers:</em></p>


```xaml
<em><dxg:GridColumn Binding="{Binding Properties[0].Value, Mode=TwoWay}" Header="Text (Properties[0].Value)"/></em>
```


<p><em>Use the approach demonstrated in this example only if you are working with DynamicObject descendants, bindings to which are not supported in Silverlight.</em><br><br><br>This example demonstrates how to display and edit properties of complex objects in DXGrid. The GridBindingHelper helper class is used to bind grid columns to complex properties of this kind. Here are instructions on how to use it:</p>
<p>1) Use the GridBindingHelper.ItemsSource attached property to assign a datasource instead of the built-in DataSource property;<br> 2) Use the GridBindingHelper.ComplexFieldName attached property to specify the field name;<br> 3) Set the column's UnboundType property value.</p>

<br/>


