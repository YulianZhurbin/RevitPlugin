using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;

namespace Jplugin
{
    public class RebarSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            bool isElementRebar = elem is Rebar;
            return isElementRebar;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return true;
        }
    }
}
