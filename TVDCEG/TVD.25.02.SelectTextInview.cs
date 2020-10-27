#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using TVDCEG.Ultis;
using System.Collections;
using TVDCEG.LBR;
#endregion

namespace TVDCEG
{
    public class SelectTextInview
    {
        private static SelectTextInview _instance;
        private SelectTextInview()
        {

        }
        public static SelectTextInview Instance => _instance ?? (_instance = new SelectTextInview());
        public List<View> GetAllViews(Document doc)
        {
            List<View> listview = new List<View>();
            var views = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(Autodesk.Revit.DB.View)).ToElements().Cast<View>().ToList();
            foreach (var view in views)
            {
                if (view.ViewType == ViewType.FloorPlan ||
                    view.ViewType == ViewType.CeilingPlan ||
                    view.ViewType == ViewType.Elevation ||
                    view.ViewType == ViewType.ThreeD ||
                    view.ViewType == ViewType.Schedule ||
                    view.ViewType == ViewType.DraftingView ||
                    view.ViewType == ViewType.Legend ||
                    view.ViewType == ViewType.Section ||
                    view.ViewType == ViewType.EngineeringPlan ||
                    view.ViewType == ViewType.Detail)
                {
                    if (view.IsTemplate) continue;
                    if (view.ViewType == ViewType.Schedule && view.Name.Contains("Revision Schedule"))
                    {
                        continue;
                    }
                    listview.Add(view);
                }
            }
            return listview;
        }
        public List<TextNote> GetTextNode(Document doc, string textselect)
        {
            //var col2 = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_TextNotes).OfClass(typeof(TextNote)).Cast<TextNote>().ToList();
            var col = (from TextNote x in new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_TextNotes).OfClass(typeof(TextNote)) where x.Text.Contains(textselect) select x).Cast<TextNote>().ToList();
            return col;
        }
        public void CopyText(Document doc,List<ElementId> ids,View source, List<View> Taggets)
        {
            ProgressbarWPF progressbarWPF = new ProgressbarWPF(Taggets.Count, "Copy Text");
            progressbarWPF.Show();
            foreach (var Tagget in Taggets)
            {
                if(progressbarWPF.iscontinue==false)
                {
                    break;
                }
                using (Transaction t = new Transaction(doc, "Copy Text"))
                {
                    t.Start();
                    FailureHandlingOptions options = t.GetFailureHandlingOptions();
                    MyPreProcessor ignoreProcess = new MyPreProcessor();
                    options.SetClearAfterRollback(true);
                    options.SetFailuresPreprocessor(ignoreProcess);
                    t.SetFailureHandlingOptions(options);
                    try
                    {
                        ElementTransformUtils.CopyElements(source, ids, Tagget, Transform.Identity, new CopyPasteOptions());
                    }
                    catch
                    {

                    }
                    t.Commit();
                }
            }
            progressbarWPF.Close();
        }
    }

}
