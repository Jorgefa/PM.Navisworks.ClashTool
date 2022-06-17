﻿using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.ComApi;
using Autodesk.Navisworks.Api.Interop.ComApi;
using PM.Navisworks.ZoneTool.Models;
using PM.Navisworks.ZoneTool.Utilities.ProgressBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Application = Autodesk.Navisworks.Api.Application;

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

        //Main

        public static void CreateZoneSelectionSets(this Document doc, ModelItemCollection elements, ModelItemCollection zones, Configuration config)
        {
            var selSets = doc.SelectionSets;

            var folderName = config.FolderName;

            var selSetDoc = doc.SelectionSets.RootItem;

            var zoneCollections = doc.GetZoneBoxesAndElementsInside(elements, zones);

            try
            {
                //Merge same zones

                var zoneCollectionsMerged = new Dictionary<string, ModelItemCollection>();

                foreach (var zoneCollection in zoneCollections)
                {
                    var zoneCode = zoneCollection.Key.GetZoneParameter(config.ZoneCategory, config.ZoneProperty);

                    if (!zoneCollectionsMerged.Keys.Contains(zoneCode))
                    {
                        zoneCollectionsMerged.Add(zoneCode, zoneCollection.Value);
                    }

                    else
                    {
                        zoneCollectionsMerged[zoneCode].AddRange(zoneCollection.Value);
                    }
                }

                //Check if folder exists or create it if not.

                FolderItem folder;

                var matchFolders = selSetDoc.FindItemsByDisplayName(folderName);

                if (matchFolders.Count == 0)
                {
                    folder = new FolderItem() { DisplayName = folderName };
                    selSets.AddCopy(folder);
                    folder = (FolderItem)selSetDoc.FindItemsByDisplayName(folderName)[0];
                }
                else if (matchFolders.Count == 1)
                {
                    folder = (FolderItem)matchFolders[0];
                }
                else
                {
                    MessageBox.Show("There is already more than one folder with that name. Please select a new name or use a name of a unique existing folder that you want to update.");
                    return;
                }

                //Loop through each zone

                foreach (var zoneCollection in zoneCollectionsMerged)
                {
                    SavedItem item;

                    var setName = zoneCollection.Key;

                    var elementsGroup = zoneCollection.Value;

                    var matchItems = folder.FindItemsByDisplayName(setName);

                    if (matchItems.Count == 1 && (FolderItem)matchItems[0].Parent == folder)
                    {
                        item = matchItems[0];
                        if (elementsGroup.Count == 0 && config.OnlyNotEmpty)
                        {
                            //Check if it is possible remove previous sets. Not through item.Parent.Children.Remove(item);
                            break;
                        }
                        doc.UpdateSelectionSet(elementsGroup, (SelectionSet)item);
                    }

                    else
                    {
                        if (elementsGroup.Count == 0 && config.OnlyNotEmpty)
                        {
                            break;
                        }
                        doc.CreateSelectionSetInFolder(elementsGroup, setName, folder);
                    }
                }

                folder.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
        }

        public static void CreateZoneSelectionSetsAndViews(this Document doc, ModelItemCollection elements, ModelItemCollection zones, Configuration config)
        {
            var selSets = doc.SelectionSets;

            var viewPoints = doc.SavedViewpoints;

            var folderName = config.FolderName;

            var selSetDoc = doc.SelectionSets.RootItem;

            var viewPointDoc = doc.SavedViewpoints.RootItem;

            var cDoc = ComApiBridge.State;

            try
            {
                //Create sets

                doc.CreateZoneSelectionSets(elements, zones, config);

                FolderItem setsFolder = (FolderItem)selSetDoc.FindItemsByDisplayName(folderName)[0];

                var sets = setsFolder.Children;

                //Check if views folder exists or create it if not.

                FolderItem folder;

                var matchFolders = viewPointDoc.FindItemsByDisplayName(folderName);

                if (matchFolders.Count == 0)
                {
                    folder = new FolderItem() { DisplayName = folderName };
                    viewPoints.AddCopy(folder);
                    folder = (FolderItem)viewPointDoc.FindItemsByDisplayName(folderName)[0];
                }
                else if (matchFolders.Count == 1)
                {
                    folder = (FolderItem)matchFolders[0];
                }
                else
                {
                    MessageBox.Show("There is already more than one folder with that name. Please select a new name or use a name of a unique existing folder that you want to update.");
                    return;
                }

                var current = 0;
                var total = sets.Count;

                ProgressUtilDefined.Start();

                foreach (SelectionSet set in sets)
                {
                    ProgressUtilDefined.Update($"{set.DisplayName}", current, total);

                    var elementGroup = set.GetSelectedItems();

                    var viewName = set.DisplayName;

                    doc.IsolateSelection(elementGroup);

                    doc.SaveCurrentViewPoint(viewName);

                    current++;
                }

                ProgressUtilDefined.Finish();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
        }

        public static void AddZoneToElements(this ModelItemCollection elements, ModelItemCollection zones, Configuration config)
        {
            var document = Application.ActiveDocument;
            var cDocument = ComApiBridge.State;

            var catDisplayName = "PMG";
            var catName = catDisplayName + "_InternalName";

            var propDisplayName = "Zone";
            var propName = propDisplayName + "_InternalName";

            if (elements == null)
            {
                return;
            }

            foreach (var zone in zones)
            {
                var zoneValue = zone.GetZoneParameter(config.ZoneCategory, config.ZoneProperty);
                if (zoneValue == null)
                {
                    MessageBox.Show("Zone parameter was not found in any zone.");
                    return;
                }
            }

            // filter elements if don't want to update previous values

            if (!config.UpdatePrevValues)
            {
                var search = new Search();
                search.Selection.CopyFrom(elements);
                search.PruneBelowMatch = true;
                search.Locations = SearchLocations.Self;

                var condition = SearchCondition.HasPropertyByDisplayName(catDisplayName, propDisplayName).Negate();

                search.SearchConditions.Add(condition);

                elements = search.FindAll(document, true);
            }

            var current = 0;
            var total = elements.Count;

            ProgressUtilDefined.Start();

            foreach (var ele in elements)
            {
                ProgressUtilDefined.Update($"{ele.ClassName} - {ele.ClassDisplayName}", current, total);

                string zoneValue = "Zone not found";
                var eBB = ele.BoundingBox();
                if (eBB == null)
                {
                    continue;
                }
                var eBBCP = eBB.Center;
                if (eBBCP == null)
                {
                    continue;
                }
                foreach (var zone in zones)
                {
                    if (zone.BoundingBox().Contains(eBBCP))
                    {
                        // get zone code
                        zoneValue = zone.GetZoneParameter(config.ZoneCategory, config.ZoneProperty);
                        break;
                    }
                }
                ele.AddDataToItem(cDocument, catDisplayName, propDisplayName, zoneValue);
                current++;
            }

            ProgressUtilDefined.Finish();

            document.CurrentSelection.Clear();
            document.CurrentSelection.AddRange(elements);

            MessageBox.Show(elements.Count.ToString() + " elements has been updated.");
        }

        //Others

        public static void UpdateSelectionSet(this Document doc, ModelItemCollection elements, SelectionSet selectionSet)
        {
            //Set variables
            var item = selectionSet;
            var parent = item.Parent;
            var index = parent.Children.IndexOf(item);

            //Copy item
            var copyItem = item.CreateCopy() as SelectionSet;

            //Get DocumentSelectionsSets
            var docSelSets = doc.SelectionSets;

            ////change name
            //copyItem.DisplayName += "_Updated";

            //change selection
            copyItem.ExplicitModelItems.CopyFrom(elements);

            docSelSets.ReplaceWithCopy(parent, index, copyItem);
        }

        public static void CreateSelectionSetInFolder(this Document doc, ModelItemCollection elements, string setName, FolderItem folder)
        {
            var selSets = doc.SelectionSets;

            var finalSetName = setName;

            var set = new SelectionSet(elements) { DisplayName = finalSetName };

            selSets.AddCopy(set);

            var oldParent = selSets.Value;

            var oldIndex = selSets.Value.IndexOfDisplayName(finalSetName);

            var newSet = oldParent[oldIndex];

            selSets.Move(newSet.Parent, oldIndex, folder, 0);
        }

        public static List<SavedItem> FindItemsByDisplayName(this SavedItem item, string displayName)
        {
            var matchItems = new List<SavedItem>();

            if (item.DisplayName == displayName)
            {
                matchItems.Add(item);
            }

            //See if this SavedItem is a GroupItem
            if (item.IsGroup)
            {
                //iterate the children and output
                foreach (SavedItem childItem in ((GroupItem)item).Children)
                {
                    matchItems.AddRange(childItem.FindItemsByDisplayName(displayName));
                }
            }

            return matchItems;
        }

        public static string GetZoneParameter(this ModelItem item, string category, string property)
        {
            var zoneParameter = item.PropertyCategories.FindPropertyByDisplayName(category, property);
            if (zoneParameter == null) return null;
            var zoneParameterValue = zoneParameter.Value.ToString().Replace($"{zoneParameter.Value.DataType.ToString()}:", "");
            return zoneParameterValue;
        }

        public static Dictionary<ModelItem, ModelItemCollection> GetZoneBoxesAndElementsInside(this Document doc, ModelItemCollection elements, ModelItemCollection zones)
        {
            var zoneCollections = new Dictionary<ModelItem, ModelItemCollection>();

            try
            {
                foreach (var zone in zones)
                {
                    zoneCollections.Add(zone, new ModelItemCollection());
                }

                foreach (var ele in elements)
                {
                    var eBB = ele.BoundingBox();
                    if (eBB == null)
                    {
                        continue;
                    }
                    var eBBCP = eBB.Center;
                    if (eBBCP == null)
                    {
                        continue;
                    }
                    foreach (var zoneCollection in zoneCollections)
                    {
                        if (zoneCollection.Key.BoundingBox().Contains(eBBCP))
                        {
                            zoneCollection.Value.Add(ele);
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }

            return zoneCollections;
        }

        public static ModelItemCollection InvertElementCollection(this ModelItemCollection elements)
        {
            //Create hidden collection
            ModelItemCollection hidden = new ModelItemCollection();

            //create a store for the visible items
            ModelItemCollection visible = new ModelItemCollection();

            //Add all the items that are visible to the visible collection
            foreach (ModelItem item in elements)
            {
                if (item.AncestorsAndSelf != null)
                {
                    visible.AddRange(item.AncestorsAndSelf);
                }
                if (item.Descendants != null)
                {
                    visible.AddRange(item.Descendants);
                }
            }

            //mark as invisible all the siblings of the visible items
            foreach (ModelItem toShow in visible)
            {
                if (toShow.Parent != null)
                {
                    hidden.AddRange(toShow.Parent.Children);
                }
            }

            //remove the visible items from the collection
            foreach (ModelItem toShow in visible)
            {
                hidden.Remove(toShow);
            }

            //hide the remaining items
            Application.ActiveDocument.Models.SetHidden(hidden, true);

            return (hidden);
        }

        private static void IsolateSelection(this Document doc, ModelItemCollection elements)
        {
            var allElements = doc.Models.CreateCollectionFromRootItems().SelectMany(x => x.DescendantsAndSelf);

            try
            {
                var curSel = doc.CurrentSelection;

                curSel.Clear();

                doc.Models.SetHidden(allElements, false);

                curSel.AddRange(elements);

                doc.State.InvertSelection();

                doc.Models.SetHidden(curSel.SelectedItems, true);

                doc.ActiveView.LookFromFrontRightTop();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.StackTrace);
            }
        }

        private static void SaveCurrentViewPoint(this Document doc, string name)
        {
            var state = ComApiBridge.State;
            var cv = state.CurrentView.Copy();

            var vp = state.ObjectFactory(nwEObjectType.eObjectType_nwOpView);
            var view = vp as InwOpView;

            view.ApplyHideAttribs = true;
            view.ApplyMaterialAttribs = true;

            view.name = name;
            view.anonview = (InwOpAnonView)cv;

            state.SavedViews().Add(vp);
        }

        public static void AddDataToItem(this ModelItem modelItem, InwOpState10 cDocument, string catDisplayName, string propDisplayName, string value)
        {
            var catName = catDisplayName + "_InternalName";
            var propName = propDisplayName + "_InternalName";

            // convert ModelItem to COM Path
            InwOaPath cItem = (InwOaPath)ComApiBridge.ToInwOaPath(modelItem);

            // declare Category (PropertyDataCollection)
            InwOaPropertyVec newCat = (InwOaPropertyVec)cDocument.ObjectFactory(nwEObjectType.eObjectType_nwOaPropertyVec, null, null);

            // get item's PropertyCategoryCollection
            InwGUIPropertyNode2 cItemCats = (InwGUIPropertyNode2)cDocument.GetGUIPropertyNode(cItem, true);

            // set index
            int index = 0;

            // check if the element already has the category
            var itemCategory = modelItem.PropertyCategories.FindCategoryByDisplayName(catDisplayName);

            // create a new category and new properties if category doesn't exist
            if (Equals(itemCategory, null))
            {
                // create a new Property (PropertyData), set PropertyName, set PropertyDisplayName, set PropertyValue and addd Porperty yo Category
                InwOaProperty newProp = (InwOaProperty)cDocument.ObjectFactory(nwEObjectType.eObjectType_nwOaProperty, null, null);
                newProp.name = propName;
                newProp.UserName = propDisplayName;
                newProp.value = value;
                newCat.Properties().Add(newProp);

                // add CategoryData to item's CategoryDataCollection
                cItemCats.SetUserDefined(index, catDisplayName, catName, newCat);
            }

            // loop trough existing categories if exists
            else
            {
                index = 1;

                // category looping using COM
                foreach (InwGUIAttribute2 atrib in cItemCats.GUIAttributes())
                {
                    // checks if is user defined
                    if (!atrib.UserDefined)
                    {
                        continue;
                    }

                    // checks if is same name. If not, increase index and continue
                    if (!Equals(atrib.ClassUserName, catDisplayName))
                    {
                        index += 1;
                        continue;
                    }

                    // create a new Property (PropertyData), set PropertyName, set PropertyDisplayName, set PropertyValue and addd Porperty yo Category
                    InwOaProperty newProp = (InwOaProperty)cDocument.ObjectFactory(nwEObjectType.eObjectType_nwOaProperty, null, null);
                    newProp.name = propName;
                    newProp.UserName = propDisplayName;
                    newProp.value = value;
                    newCat.Properties().Add(newProp);

                    // if category is user defined and same name, keep existing parameters in new category (newCat)
                    if (atrib.Properties().Count > 1)
                    {
                        foreach (InwOaProperty prop in atrib.Properties())
                        {
                            // check if there is already a zone parameter
                            if (prop.UserName == propDisplayName)
                            {
                                continue;
                            }
                            // create a new Property (PropertyData) for the rest of the previous data
                            InwOaProperty prevProp = (InwOaProperty)cDocument.ObjectFactory(nwEObjectType.eObjectType_nwOaProperty, null, null);
                            prevProp.name = prop.name;
                            prevProp.UserName = prop.UserName;
                            prevProp.value = prop.value;
                            newCat.Properties().Add(prevProp);
                        }
                    }

                    break;
                }

                // add CategoryData to item's CategoryDataCollection
                cItemCats.SetUserDefined(index, catDisplayName, catName, newCat);
            }
        }
    }
}