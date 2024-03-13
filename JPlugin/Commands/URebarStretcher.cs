using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace Jplugin
{
    [Transaction(TransactionMode.Manual)]
    public class URebarStretcher : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document document = uIDocument.Document;

            Result operationResult = RebarSelector.GetRebars(uIDocument, out List<Element> selectedURebars, RebarDetailShape.U);

            if (operationResult == Result.Cancelled)
            {
                return Result.Cancelled;
            }

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Удлинить хвосты П-стержня");

                foreach (Element element in selectedURebars)
                {
                    operationResult = RebarLengthCorrector.StretchURebar(element, out string errorMessage);

                    if (operationResult == Result.Failed)
                    {
                        message = errorMessage;
                        return Result.Failed;
                    }
                }
                transaction.Commit();
            }

            return Result.Succeeded;
        }
    }
}
