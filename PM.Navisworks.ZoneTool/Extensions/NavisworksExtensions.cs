using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.ComApi;
using Autodesk.Navisworks.Api.Interop.ComApi;
using PM.Navisworks.ZoneTool.Models;
using System.Collections.Generic;

namespace PM.Navisworks.ZoneTool.Extensions
{
    public static class NavisworksExtensions
    {
        public static double ConvertUnitsToMeters(double dim)
        {
            // Get current active document.
            Document doc = Application.ActiveDocument;

            // Get units of the document
            Units units = doc.Units;

            // Return converted value to meters
            switch (units)
            {
                case (Units.Centimeters): return dim * 0.01d;
                case (Units.Feet): return dim * 0.3048d;
                case (Units.Inches): return dim * 0.0254d;
                case (Units.Kilometers): return dim * 1000d;
                case (Units.Meters): return dim * 1f;
                case (Units.Microinches): return dim * 0.0000000254d;
                case (Units.Micrometers): return dim * 0.000001d;
                case (Units.Miles): return dim * 1609.43d;
                case (Units.Millimeters): return dim * 0.001d;
                case (Units.Mils): return dim * 0.0000254d;
                case (Units.Yards): return dim * 0.9144d;
                default: return dim * 1;
            }
        }

        public static void AddZoneToElements(this ModelItemCollection elements, ModelItemCollection zones, string category, string property)
        {
            var zonesList = new List<NavisZone>();

            var cDocument = ComApiBridge.State;

            var catDisplayName = "PMG";
            var catName = catDisplayName + "_InternalName";

            foreach (ModelItem zone in zones)
            {
                var zoneValue = zone.GetZoneParameter(category, property);
                var newZone = new NavisZone()
                {
                    Element = zone,
                    Code = zoneValue

                };
                zonesList.Add(newZone);
            }

            foreach (var ele in elements)
            {
                var eleCenter = ele.BoundingBox().Center;
                foreach (var zone in zonesList)
                {
                    if (zone.Element.BoundingBox().Contains(eleCenter))
                    {

                        // convert ModelItem to COM Path
                        InwOaPath cItem = (InwOaPath)ComApiBridge.ToInwOaPath(ele);

                        // declare Category (PropertyDataCollection)
                        InwOaPropertyVec newCat = (InwOaPropertyVec)cDocument.ObjectFactory(nwEObjectType.eObjectType_nwOaPropertyVec, null, null);

                        // get item's PropertyCategoryCollection
                        InwGUIPropertyNode2 cItemCats = (InwGUIPropertyNode2)cDocument.GetGUIPropertyNode(cItem, true);

                        // set index
                        int index = 0;

                        // create a new Property (PropertyData), set PropertyName, set PropertyDisplayName, set PropertyValue and addd Porperty yo Category
                        InwOaProperty newProp = (InwOaProperty)cDocument.ObjectFactory(nwEObjectType.eObjectType_nwOaProperty, null, null);
                        newProp.name = "Zone" + "_InternalName";
                        newProp.UserName = "Zone";
                        newProp.value = zone.Code;
                        newCat.Properties().Add(newProp);
                        cItemCats.SetUserDefined(index, catDisplayName, catName, newCat);

                    }
                }
            }
        }

        public static string GetZoneParameter(this ModelItem item, string category, string property)
        {
            var zoneParameter = item.PropertyCategories.FindPropertyByDisplayName(category, property);
            if (zoneParameter == null) return null;
            var zoneParameterValue = zoneParameter.Value.ToString().Replace($"{zoneParameter.Value.DataType.ToString()}:", "");
            return zoneParameterValue;
        }
    }
}