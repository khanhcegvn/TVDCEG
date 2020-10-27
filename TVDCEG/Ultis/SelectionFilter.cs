using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;

namespace TVDCEG.LBR
{
    public class ViewPortSelectionFilter : ISelectionFilter
    {
        public static readonly ViewPortSelectionFilter Instance = new ViewPortSelectionFilter();

        public bool AllowElement(Element elem)
        {
            return elem != null && elem.Category.Id == new ElementId(BuiltInCategory.OST_Viewports);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class SelectionVoid : ISelectionFilter
    {
        public static readonly SelectionVoid Instance = new SelectionVoid();

        public bool AllowElement(Element elem)
        {
            return elem != null && elem.Category.Id == new ElementId(BuiltInCategory.OST_GenericModel);
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class FilterDoubletee : ISelectionFilter
    {
        public static readonly FilterDoubletee Instance = new FilterDoubletee();

        public bool AllowElement(Element elem)
        {
            FamilyInstance familyInstance = elem as FamilyInstance;
            string Name = familyInstance.Symbol.FamilyName;
            if (Name.Contains("DOUBLE_TEE") || Name.Contains("SINGLE_TEE")) return true;
            else return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class TextSelectionFilter : ISelectionFilter
    {
        public static readonly TextSelectionFilter Instance = new TextSelectionFilter();

        public bool AllowElement(Element element)
        {
            return element is ModelText || element is TextNote;
        }

        public bool AllowReference(Reference reference, XYZ point)
        {
            return false;
        }
    }
    public class RebarSelectionFilter : ISelectionFilter
    {
        public static readonly RebarSelectionFilter Instance = new RebarSelectionFilter();

        public bool AllowElement(Element element)
        {
            return element is Autodesk.Revit.DB.Structure.Rebar;
        }

        public bool AllowReference(Reference reference, XYZ point)
        {
            return false;
        }
    }
    public class CegRebarFilter : ISelectionFilter
    {
        //private Type filteredType;

        //private Category filteredCategory;
        //private double Sorting_order;
        //public CegRebarFilter(double Sorting_order)
        //{
        //    this.filteredType = null;
        //    this.filteredCategory = null;
        //}

        public bool AllowElement(Element elem)
        {
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_SpecialityEquipment)
            {
                Parameter pa = elem.LookupParameter("CONTROL_MARK");
                if(pa!=null)
                {
                    if(!string.IsNullOrEmpty(pa.AsString()))
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
            else return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
    public class GridSelectionFilter : ISelectionFilter
    {
        public static readonly RebarSelectionFilter Instance = new RebarSelectionFilter();

        public bool AllowElement(Element element)
        {
            return element is Grid;
        }

        public bool AllowReference(Reference reference, XYZ point)
        {
            return false;
        }
    }

    public class TextNoteSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Name == "Text Notes")
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }

    public class FamilyInstanceSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element is FamilyInstance)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
    public class TeeSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Name.Contains("DOUBLE_TEE"))
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
    public class FramingSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Name.Contains("Structural Framing"))
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
    public class InstanceFilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element is FamilyInstance)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
    public class SelectionFilter : ISelectionFilter
    {
        public Type FilteredType
        {
            get
            {
                return this.filteredType;
            }
            set
            {
                this.filteredType = value;
            }
        }

        public Category FilteredCategory
        {
            get
            {
                return this.filteredCategory;
            }
            set
            {
                this.filteredCategory = value;
            }
        }

        public SelectionFilter(Type type)
        {
            this.filteredType = type;
            this.filteredCategory = null;
        }

        public SelectionFilter(Category category)
        {
            this.filteredType = null;
            this.filteredCategory = category;
        }

        public SelectionFilter(Type type, Category category)
        {
            this.filteredType = type;
            this.filteredCategory = category;
        }

        public bool AllowElement(Element elem)
        {
            bool flag = this.filteredType == null && this.FilteredCategory == null;
            bool result;
            if (flag)
            {
                result = true;
            }
            else
            {
                bool flag2 = this.filteredType == null;
                if (flag2)
                {
                    result = (elem.Category.Name == this.filteredCategory.Name);
                }
                else
                {
                    result = (this.filteredCategory == null && elem.GetType() == this.filteredType);
                }
            }
            return result;
        }

        public bool AllowReference(Reference refer, XYZ pos)
        {
            return true;
        }

        private Type filteredType;

        private Category filteredCategory;
    }
    public class SelectCustomfilter : ISelectionFilter
    {
        private Type filteredType;

        private Category filteredCategory;
        private String filtername;
        public Type FilteredType
        {
            get
            {
                return this.filteredType;
            }
            set
            {
                this.filteredType = value;
            }
        }
        public string Name
        {
            get
            {
                return this.filtername;
            }
            set
            {
                this.filtername = value;
            }
        }
        public Category FilteredCategory
        {
            get
            {
                return this.filteredCategory;
            }
            set
            {
                this.filteredCategory = value;
            }
        }
        public SelectCustomfilter(Type type)
        {
            this.filteredType = type;
            this.filteredCategory = null;
        }

        public SelectCustomfilter(Category category)
        {
            this.filteredType = null;
            this.filteredCategory = category;
        }
        public SelectCustomfilter(string Name)
        {
            this.filteredType = null;
            this.filteredCategory = null;
            this.filtername = Name;
        }
        public SelectCustomfilter(Type type, Category category)
        {
            this.filteredType = type;
            this.filteredCategory = category;
        }
        public SelectCustomfilter(string _name, Category category)
        {
            this.filtername = _name;
            this.filteredCategory = category;
        }
        public bool AllowElement(Element elem)
        {
            bool flag = this.filteredType == null && this.FilteredCategory == null && this.filtername == null;
            bool result;
            if (flag)
            {
                result = true;
            }
            else
            {
                bool flag2 = this.filteredType == null;
                if (flag2)
                {
                    if(this.filteredCategory==null)
                    {
                        result = (elem.Name.Contains(this.filtername));
                    }
                    else
                    {
                        result = (elem.Category.Name.Contains(this.filteredCategory.Name));
                    }
                }
                else
                {
                    result = (this.filteredCategory == null && elem.GetType() == this.filteredType);
                }

            }
            return result;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
