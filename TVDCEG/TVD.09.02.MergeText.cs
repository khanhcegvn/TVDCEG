using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;

namespace TVDCEG
{
    public class MergeText
    {
        private static MergeText _instance;
        private MergeText() { }
        public static MergeText Instance => _instance ?? (_instance = new MergeText());
        public UIDocument uidoc;
        public List<TextNote> GetText(Document doc, List<Reference> references)
        {
            List<TextNote> list = new List<TextNote>();
            foreach (var item in references)
            {
                TextNote textNote = doc.GetElement(item) as TextNote;
                list.Add(textNote);
            }
            return list;
        }
        public void CreateTextNote(Document doc, List<Reference> references, Selection sel)
        {
            var listtextnote = GetText(doc, references);
            var converttext = (from x in listtextnote select x.Text).ToList();
            var text = string.Join("", converttext.ToArray());
            ElementId elementId = listtextnote.First().TextNoteType.Id;
            XYZ point = sel.PickPoint();
            TextNoteOptions options = new TextNoteOptions();
            options.TypeId = elementId;
            options.HorizontalAlignment = HorizontalTextAlignment.Left;
            double noteWidth = 0.2;
            double minWidth = TextNote.GetMinimumAllowedWidth(doc, elementId);
            double maxWidth = TextNote.GetMaximumAllowedWidth(doc, elementId);
            if (noteWidth < minWidth)
            {
                noteWidth = minWidth;
            }
            else if (noteWidth > maxWidth)
            {
                noteWidth = maxWidth;
            }
            using (Transaction tran = new Transaction(doc, "Create Text"))
            {
                tran.Start();
                TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, point, noteWidth, text, options);
                tran.Commit();
            }
        }
    }
}
