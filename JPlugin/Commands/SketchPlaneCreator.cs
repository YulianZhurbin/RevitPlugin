using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Jplugin
{
    [Transaction(TransactionMode.Manual)]
    public class SketchPlaneCreator : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uIDocument = commandData.Application.ActiveUIDocument;

            Document document = uIDocument.Document;

            View activeView = document.ActiveView;

            bool isActiveViewView3D = activeView is View3D;

            if(!isActiveViewView3D)
            {
                message = "Данная команда работает только с 3D-видами";
                return Result.Failed;
            }

            using (Transaction transaction = new Transaction(document))
            {
                transaction.Start("Назначение рабочей плоскости перпендикулярной взгляду камеры");

                Plane geometricPlane = Plane.CreateByNormalAndOrigin(activeView.ViewDirection, XYZ.Zero);

                SketchPlane sketchPlane = SketchPlane.Create(document, geometricPlane);
                sketchPlane.Name = "Sketch plane orthogonal to the current user's view";

                activeView.SketchPlane = sketchPlane;

                transaction.Commit();
            }
                return Result.Succeeded;
        }
    }
}
