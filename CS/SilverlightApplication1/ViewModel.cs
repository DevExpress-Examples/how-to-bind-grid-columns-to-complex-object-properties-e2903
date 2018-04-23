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
using System.Collections.Generic;
using DevExpress.Xpf.Core.Commands;
using System.ComponentModel;
using System.Dynamic;

namespace SilverlightApplication1 {
    public class ViewModel {
        public IList<RowModel> Rows { get; private set; }
        public ViewModel() {
            Rows = new List<RowModel>();
            for(int i = 0; i < 50; i++) {
                RowModel row = new RowModel(3);
                row.Properties[0].Value = "row" + i;
                row.Properties[1].Value = i;
                row.Properties[2].Value = i % 2 == 0;
                row.Dictionary["Text"] = new PropertyValue() { Value = "row" + i };
                row.Dictionary["Number"] = new PropertyValue() { Value = i };
                row.Dictionary["Bool"] = new PropertyValue() { Value = i % 2 == 0 };
                row.DynamicObject.Text = "row" + i;
                row.DynamicObject.Number = i;
                row.DynamicObject.Bool = i % 2 == 0 ;

                Rows.Add(row);
            }
            ChangedSelectedRowCommand = new DelegateCommand<object>(o => ChangeSelectedRow());
        }
        public RowModel SelectedRow { get; set; }
        public ICommand ChangedSelectedRowCommand { get; private set; }
        void ChangeSelectedRow() {
            if(SelectedRow != null) {
                SelectedRow.SetValue(1, (int)SelectedRow.Properties[1].Value + 1);
            }
        }
    }
    public class RowModel : INotifyPropertyChanged {
        public dynamic DynamicObject { get; private set; }
        public List<PropertyValue> Properties { get; private set; }
        public Dictionary<string, PropertyValue> Dictionary { get; private set; }
        public RowModel(int propertyCount) {
            Properties = new List<PropertyValue>();
            Dictionary = new Dictionary<string, PropertyValue>();
            DynamicObject = new DynamicDictionary();
            for(int i = 0; i < propertyCount; i++) {
                Properties.Add(new PropertyValue());
            }
        }
        public void SetValue(int propertyIndex, object value) {
            Properties[propertyIndex].Value = value;
            if(PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(null));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class PropertyValue {
        public object Value { get; set; }
    }
}
