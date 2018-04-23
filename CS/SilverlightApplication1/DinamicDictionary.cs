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
using System.Dynamic;
using System.Collections.Generic;

namespace SilverlightApplication1 {
    public class DynamicDictionary : DynamicObject {
        Dictionary<string, object> dictionary
            = new Dictionary<string, object>();

        public int Count {
            get {
                return dictionary.Count;
            }
        }
        public override bool TryGetMember(
            GetMemberBinder binder, out object result) {
            string name = binder.Name.ToLower();
            return dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(
            SetMemberBinder binder, object value) {
            dictionary[binder.Name.ToLower()] = value;
            return true;
        }
    }
}
