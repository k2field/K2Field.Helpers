using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2Field.Helpers.Core
{

    public enum CoreDataFieldType
    {
        Process,
        Activity
    }

    public class CoreDataField
    {
        public string Name { get; set; }
        public CoreDataFieldType Type { get; set; }
        public object Value { get; set; }
        public string Check { get; set; }
    }
}
