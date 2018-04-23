using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DevExpress.Xpf.Grid;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Dynamic;

namespace SilverlightApplication1 {
    public class GridBindingHelper {


        class ComplextPath {
            class PathPartSimple {
                class GetMemberValueBinder : GetMemberBinder {
                    public GetMemberValueBinder(string name)
                        : base(name, false) {
                    }
                    public override DynamicMetaObject FallbackGetMember(DynamicMetaObject self, DynamicMetaObject onBindingError) {
                        return onBindingError;
                    }
                }
                class SetMemberValueBinder : SetMemberBinder {
                    public SetMemberValueBinder(string propertyName)
                        : base(propertyName, false) {
                    }
                    public override DynamicMetaObject FallbackSetMember(DynamicMetaObject target, DynamicMetaObject value, DynamicMetaObject errorSuggestion) {
                        return errorSuggestion;
                    }
                }
                readonly string propertyName;
                public PathPartSimple(string propertyName) {
                    this.propertyName = propertyName;
                }

                internal virtual object CalcValue(object row) {
                    if(row is DynamicObject) {
                        GetMemberValueBinder binder = new GetMemberValueBinder(propertyName);
                        object result;
                        if(((DynamicObject)row).TryGetMember(binder, out result))
                            return result;
                        return null;
                    }
                    PropertyInfo propertyInfo = GetPropertyInfo(row);
                    if(propertyInfo == null)
                        return null;
                    return propertyInfo.GetValue(row, null);
                }

                internal virtual void SetValue(object row, object value) {
                    if(row is DynamicObject) {
                        SetMemberValueBinder binder = new SetMemberValueBinder(propertyName);
                        ((DynamicObject)row).TrySetMember(binder, value);
                        return;
                    }
                    PropertyInfo propertyInfo = GetPropertyInfo(row);
                    if(propertyInfo == null)
                        return;
                    propertyInfo.SetValue(row, value, null);
                }
                PropertyInfo GetPropertyInfo(object row) {
                    return row.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                }
            }
            class PathPartList : PathPartSimple {
                readonly int index;
                public PathPartList(string propertyName, int index)
                    : base(propertyName) {
                    this.index = index;
                }
                internal override object CalcValue(object row) {
                    IList list = GetList(row);
                    if(list == null)
                        return null;
                    return list[index];
                }
                internal override void SetValue(object row, object value) {
                    IList list = GetList(row);
                    if(list == null)
                        return;
                    list[index] = value;
                }
                IList GetList(object row) {
                    return base.CalcValue(row) as IList;
                }
            }
            class PathPartDictionary : PathPartSimple {
                readonly string index;
                public PathPartDictionary(string propertyName, string index)
                    : base(propertyName) {
                    this.index = index;
                }
                internal override object CalcValue(object row) {
                    IDictionary dict = GetDictionary(row);
                    if(dict == null)
                        return null;
                    return dict[index];
                }
                internal override void SetValue(object row, object value) {
                    IDictionary dict = GetDictionary(row);
                    if(dict == null)
                        return;
                    dict[index] = value;
                }
                IDictionary GetDictionary(object row) {
                    return base.CalcValue(row) as IDictionary;
                }
            }
            List<PathPartSimple> pathParts = new List<PathPartSimple>();
            public ComplextPath(string complexPath) {
                string[] paths = complexPath.Split('.');
                for(int i = 0; i < paths.Length; i++) {
                    string path = paths[i];
                    int braceIndex = path.IndexOf("[");
                    string index = null;
                    if(braceIndex >= 0) {
                        index = path.Substring(braceIndex + 1, path.Length - braceIndex - 2);
                        path = path.Substring(0, braceIndex);
                    }
                    PathPartSimple pathPart = null;
                    if(!string.IsNullOrEmpty(index) && Char.IsDigit(index[0])) {
                        pathPart = new PathPartList(path, int.Parse(index));
                    } else if(!string.IsNullOrEmpty(index)) {
                        pathPart = new PathPartDictionary(path, index);
                    } else {
                        pathPart = new PathPartSimple(path);
                    }
                    pathParts.Add(pathPart);
                }
            }
            public object CalcValue(object row) {
                return CalcValueCore(row, false);
            }

            public void SetValue(object row, object value) {
                object lastValue = CalcValueCore(row, true);
                if(lastValue == null)
                    return;
                pathParts[pathParts.Count - 1].SetValue(lastValue, value);
            }
            public object CalcValueCore(object row, bool skipLastPath) {
                for(int i = 0; i < pathParts.Count - (skipLastPath ? 1 : 0); i++) {
                    row = pathParts[i].CalcValue(row);
                    if(row == null)
                        break;
                }
                return row;
            }
        }


        static ComplextPath GetComplexPath(GridColumn obj) {
            return (ComplextPath)obj.GetValue(ComplexPathProperty);
        }

        static void SetComplexPath(GridColumn obj, ComplextPath value) {
            obj.SetValue(ComplexPathProperty, value);
        }

        public static readonly DependencyProperty ComplexPathProperty =
            DependencyProperty.RegisterAttached("ComplexPath", typeof(ComplextPath), typeof(ComplextPath), new PropertyMetadata(null));



        public static string GetComplexFieldName(GridColumn obj) {
            return (string)obj.GetValue(ComplexFieldNameProperty);
        }

        public static void SetComplexFieldName(GridColumn obj, string value) {
            obj.SetValue(ComplexFieldNameProperty, value);
        }

        public static readonly DependencyProperty ComplexFieldNameProperty =
            DependencyProperty.RegisterAttached("ComplexFieldName", typeof(string), typeof(GridBindingHelper), new PropertyMetadata(null, OnComplexFieldNameChanged));

        static void OnComplexFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            GridColumn column = (GridColumn)d;
            if(!string.IsNullOrEmpty((string)e.OldValue))
                column.ClearValue(ComplexPathProperty);
            column.FieldName = (string)e.NewValue;
            if(!string.IsNullOrEmpty(column.FieldName))
                SetComplexPath(column, new ComplextPath(column.FieldName));
        }

        public static IEnumerable GetItemsSource(GridControl obj) {
            return (IEnumerable)obj.GetValue(ItemsSourceProperty);
        }

        public static void SetItemsSource(GridControl obj, IEnumerable value) {
            obj.SetValue(ItemsSourceProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached("ItemsSource", typeof(IEnumerable), typeof(GridBindingHelper), new PropertyMetadata(null, OnItemsSourceChanged));

        static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            GridControl grid = (GridControl)d;
            if(e.OldValue != null)
                grid.CustomUnboundColumnData -= new GridColumnDataEventHandler(grid_CustomUnboundColumnData);
            grid.DataSource = e.NewValue;
            if(e.NewValue != null)
                grid.CustomUnboundColumnData += new GridColumnDataEventHandler(grid_CustomUnboundColumnData);
        }

        static void grid_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e) {
            if(string.IsNullOrEmpty(GetComplexFieldName(e.Column)))
                return;
            ComplextPath complexPath = GetComplexPath(e.Column);
            GridControl grid = (GridControl)sender;
            if(e.IsGetData)
                e.Value = complexPath.CalcValue(grid.GetRowByListIndex(e.ListSourceRowIndex));
            if(e.IsSetData)
                complexPath.SetValue(grid.GetRowByListIndex(e.ListSourceRowIndex), e.Value);
        }
    }
}
