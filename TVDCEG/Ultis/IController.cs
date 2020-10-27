using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace TVDCEG.LBR
{
    internal interface IController
    {
        UIApplication UiApp
        {
            get;
            set;
        }

        UIDocument UiDoc
        {
            get;
            set;
        }

        Document Doc
        {
            get;
            set;
        }

        View ActiveView
        {
            get;
            set;
        }
        Selection RSelection
        {
            get;
            set;
        }

        void LoadSettings();

        void SaveSettings();

        void Execute();

        void ReadData();

        void SaveData();

        void FilterData(string filter);

        void CheckedToSelected(int i);
    }
}
