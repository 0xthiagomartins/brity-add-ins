using BrityWorks.AddIn.Teste.Properties;
using RPAGO.AddIn;
using RPAGO.Common.Library;
using RPAGO.Common.Data;
using System.IO;
using System.Windows;
using System.Collections.Generic;


namespace BrityWorks.AddIn.Teste.Activities
{
    internal class NormalActivitySample : IActivityItem
    {
        public static readonly PropKey OutputPropKey = new PropKey("Group1", "Prop1");
        public static readonly PropKey Num1PropKey = new PropKey("Group1", "Prop2");
        public static readonly PropKey Num2PropKey = new PropKey("Group2", "Prop3");
        public static readonly PropKey LocationPropKey = new PropKey("Group2", "Prop4");
        public static readonly PropKey TogglePropKey = new PropKey("Group2", "Prop5");
        public static readonly PropKey SelectPropKey = new PropKey("Group2", "Prop6");
        public static readonly PropKey FilePropKey = new PropKey("Files", "File");
        public static readonly PropKey TextFilePropKey = new PropKey("Files", "TextFile");

        public string DisplayName => "Do something";

        public System.Drawing.Bitmap Icon => Resources.teste_activity;

        public LibraryHeadlessType Mode => LibraryHeadlessType.Both;

        public PropKey DisplayTextProperty => OutputPropKey;

        public PropKey OutputProperty => OutputPropKey;

         
        public List<Property> OnCreateProperties()
        {
            var properties = new List<Property>()
            {
                new Property(this, OutputPropKey, "RESULT").SetRequired(),
                new Property(this, Num1PropKey, 1),
                new Property(this, Num2PropKey, 1.0),
                new Property(this, LocationPropKey, new Point(0, 0)),
                new Property(this, TogglePropKey, true),
                new Property(this, SelectPropKey, "item1").SetDropDownList("item1;item2;item3"),
                new Property(this, FilePropKey, "''").SetControlType(PropertyControls.PropertyItemPathView),
                new Property(this, TextFilePropKey, "''").SetControlType(PathControlType.Text),
            };
            return properties;
        }

        public void OnLoad(PropertySet properties)
        {
            return;
        }

        public object OnRun(IDictionary<string, object> properties)
        {
            return properties[LocationPropKey] + " : "
                + ((int)properties[Num1PropKey] + (double)properties[Num2PropKey])
                + "\n" + properties[FilePropKey]
                + "\n" + File.ReadAllText((string)properties[TextFilePropKey]);
        }
    }
}
