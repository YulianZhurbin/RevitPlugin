using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Linq;

namespace Jplugin
{
    [Transaction(TransactionMode.Manual)]
    public class CoverInserter : IExternalCommand
    {
        private readonly string familyName = "Аннотации_ЗСБ";
        private readonly string familyFolder = @"C:\Julian\Families\";
        private readonly string familyExtension = ".rfa";

        private string FamilyPath
        {
            get
            {
                return familyFolder + familyName + familyExtension;
            }
        }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;
            Document document = uIDocument.Document;

            Family coverFamily = new FilteredElementCollector(document).OfClass(typeof(Family))
                                                                  .FirstOrDefault(e => e.Name == familyName) as Family;

            if (coverFamily == null)
            {
                if (!File.Exists(FamilyPath))
                {
                    message = string.Format("Семейства {0} не было найдено в папке {1}", familyName, familyFolder);
                    return Result.Failed;
                }

                using (Transaction transaction = new Transaction(document))
                {
                    transaction.Start("Загрузка семейства");
                    document.LoadFamily(FamilyPath, out Family family);
                    coverFamily = family;
                    transaction.Commit();
                }
            }

            ElementId elementId = coverFamily.GetFamilySymbolIds().FirstOrDefault();
            FamilySymbol familySymbol = document.GetElement(elementId) as FamilySymbol;

            uIDocument.PostRequestForElementTypePlacement(familySymbol);

            return Result.Succeeded;
        }
    }
}
