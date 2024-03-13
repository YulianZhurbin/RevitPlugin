using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jplugin
{
    public static class RebarSelector
    {
        private static readonly string[] acceptableRebarShapes =
        {
            "Прямой стержень",
            "Cтержень Z",
            "Стержень Г",
            "Стержень П",
            "Стержень П с разными концами"
        };

        /// <summary>
        /// </summary>
        /// <param name="uIDocument"></param>
        /// <param name="rebars"></param>
        /// <param name="onlyURebars"></param>
        /// <returns>Список элементов с допустимыми формами</returns>
        public static Result GetRebars(UIDocument uIDocument, out List<Element> rebars, RebarDetailShape shape = RebarDetailShape.All)
        {
            rebars = new List<Element> ();
            Selection selection = uIDocument.Selection;
            Document document = uIDocument.Document;

            ICollection<ElementId> selectedElementIds = selection.GetElementIds();

            if (selectedElementIds.Count == 0)
            {
                try
                {
                    IList<Reference> selectedReferences = selection.PickObjects(ObjectType.Element, new RebarSelectionFilter(), "Выберете арматурные стержни");
                    rebars = IntoElementConverter.GetListOfElements(document, selectedReferences);
                }
                catch (OperationCanceledException)
                {
                    return Result.Cancelled;
                }
            }
            else
            {
                FilteredElementCollector collector = new FilteredElementCollector(document, selectedElementIds);
                rebars = collector.OfClass(typeof(Rebar)).ToList();
            }

            if(shape == RebarDetailShape.All)
            {
                rebars = SelectRebarsWithAcceptableShapes(rebars);
            }
            else
            {     
                rebars = SelectRebarsWithAcceptableUShape(rebars);
            }

            return Result.Succeeded;
        }

        private static List<Element> SelectRebarsWithAcceptableShapes(List<Element> rebars)
        {
            List<Element> selectedRebars = new List<Element>();

            foreach (Element rebar in rebars)
            {
                string rebarShape = rebar.LookupParameter("Форма").AsValueString();

                if (acceptableRebarShapes.Contains(rebarShape))
                    selectedRebars.Add(rebar);
            }

            if (selectedRebars.Count < rebars.Count)
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach(var form in acceptableRebarShapes)
                {
                    stringBuilder.AppendLine(form.ToString());
                }

                TaskDialog.Show("Недопустимые формы арматурных стержней", $"Команда работает только со следующими формами арматурных деталей: \t{stringBuilder}");
            }                  

            return selectedRebars;
        }

        private static List<Element> SelectRebarsWithAcceptableUShape(List<Element> rebars)
        {
            List<Element> selectedURebars = new List<Element>();

            foreach(Element rebar in rebars)
            {
                string rebarShape = rebar.LookupParameter("Форма").AsValueString();

                if (rebarShape == acceptableRebarShapes[3])
                    selectedURebars.Add(rebar);
            }

            if(selectedURebars.Count < rebars.Count)
            {
                TaskDialog.Show("Недопустимые формы арматурных стержней", $"Команда работает только со формой арматурных деталей {acceptableRebarShapes[3]}");
            }

            return selectedURebars;
        }
    }
}
