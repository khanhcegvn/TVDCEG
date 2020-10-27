using Autodesk.Revit.DB;
using System.Globalization;

namespace TVDCEG.LBR
{
    public static class ParamterUtils
    {
        public static string GetProperty(this Element elem, BuiltInParameter paraEnum)
        {
            return GetParameterValue(elem.get_Parameter(paraEnum));
        }

        private static string GetParameterValue(Parameter param)
        {
            if (param == null)
                return string.Empty;
            string str;
            switch (param.StorageType)
            {
                case StorageType.Integer:
                    str = param.AsInteger().ToString();
                    break;
                case StorageType.Double:
                    str = param.AsDouble().ToString(CultureInfo.InvariantCulture);
                    break;
                case StorageType.String:
                    str = param.AsString();
                    break;
                case StorageType.ElementId:
                    str = param.AsElementId().ToString();
                    break;
                default:
                    str = param.AsString();
                    break;
            }
            return str ?? string.Empty;
        }
        public static bool HaveParameterInTypeorRebar(this FamilyInstance familyInstance,Document doc,string pa)
        {
            ElementId elementId = familyInstance.GetTypeId();
            Element eletype = doc.GetElement(elementId);
            var val = eletype.LookupParameter("CONTROL_MARK");
            var val2 = eletype.LookupParameter("SORTING_ORDER");
            if (val!=null)
            {
                return true;
            }
            else if(val2!=null)
            {
                bool flag = val2.AsInteger() == 405;
                if(flag)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static string GetParameterValueString(this Parameter param)
        {
            string empty = string.Empty;
            string str;
            switch ((int)param.StorageType)
            {
                case 0:
                    str = param.AsValueString().ToString();
                    break;
                case 1:
                    str = param.AsInteger().ToString();
                    break;
                case 2:
                    str = param.AsDouble().ToString(CultureInfo.InvariantCulture);
                    break;
                case 3:
                    str = param.AsString();
                    break;
                case 4:
                    str = param.AsElementId().ToString();
                    break;
                default:
                    str = param.AsValueString();
                    break;
            }
            return str ?? string.Empty;
        }

        public static void SetParameterValue(this Parameter param, string input)
        {
            if (param.IsReadOnly)
                return;
            switch ((int)param.StorageType)
            {
                case 0:
                    param.SetValueString(input);
                    break;
                case 1:
                    bool result1 = false;
                    if (bool.TryParse(input, out result1))
                    {
                        param.Set(result1 ? 1 : 0);
                        break;
                    }
                    param.SetValueString(input);
                    break;
                case 2:
                    param.SetValueString(input);
                    break;
                case 3:
                    param.Set(input);
                    break;
                case 4:
                    int result2;
                    if (int.TryParse(input, out result2))
                    {
                        ElementId elementId = new ElementId(result2);
                        param.Set(elementId);
                    }
                    if (!input.Equals("(none)"))
                        break;
                    param.Set(ElementId.InvalidElementId);
                    break;
            }
        }
    }
}
