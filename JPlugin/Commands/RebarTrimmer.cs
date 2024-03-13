using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Jplugin
{
    [Transaction(TransactionMode.Manual)]
    public class RebarTrimmer : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document document = uIDocument.Document;

            Result operationResult = RebarSelector.GetRebars(uIDocument, out List<Element> selectedRebars);

            if (operationResult == Result.Cancelled)
            {
                return Result.Cancelled;
            }

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Уменьшить длину стержня");

                foreach (Element element in selectedRebars)
                {
                    operationResult = RebarLengthCorrector.Trim(element, out string errorMessage);

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
