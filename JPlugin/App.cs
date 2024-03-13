using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Jplugin
{
    public class App : IExternalApplication
    {        
        public Result OnStartup(UIControlledApplication application)
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string imgDirectoryPath = Path.GetDirectoryName(assemblyPath) + @"\img\";
            string tabName = "J-plugin";

            application.CreateRibbonTab(tabName);

            #region Reinforcement panel creation

            RibbonPanel reinforcementPanel = application.CreateRibbonPanel(tabName, "Армирование");

            PushButtonData stretchButtonData = new PushButtonData(nameof(RebarStretcher), "Удлинить\nстержень", assemblyPath, typeof(RebarStretcher).FullName)
            {
                LargeImage = new BitmapImage(new Uri(imgDirectoryPath + "stretch.png")),
                ToolTip = "Увеличивает длину арматурного стержня до ближайшей стандартной"
            };

            PushButtonData trimButtonData = new PushButtonData(nameof(RebarTrimmer), "Укоротить\nстержень", assemblyPath, typeof(RebarTrimmer).FullName)
            {
                LargeImage = new BitmapImage(new Uri(imgDirectoryPath + "trim.png")),
                ToolTip = "Уменьшает длину арматурного стержня до ближайшей стандартной"
            };

            PushButtonData uStretchButtonData = new PushButtonData(nameof(URebarStretcher), "Удлинить\nП-стержень", assemblyPath, typeof(URebarStretcher).FullName)
            {
                LargeImage = new BitmapImage(new Uri(imgDirectoryPath + "ustretch.png")),
                ToolTip = "Увеличивает длину арматурного П-стержня до ближайшей стандартной с сохранением равенства хвостов"
            };

            PushButtonData uTrimButtonData = new PushButtonData(nameof(URebarTrimmer), "Укоротить\nП-стержень", assemblyPath, typeof(URebarTrimmer).FullName)
            {
                LargeImage = new BitmapImage(new Uri(imgDirectoryPath + "utrim.png")),
                ToolTip = "Уменьшает длину арматурного П-стержня до ближайшей стандартной с сохранением равенства хвостов"
            };

            reinforcementPanel.AddItem(stretchButtonData);
            reinforcementPanel.AddItem(trimButtonData);
            reinforcementPanel.AddItem(uStretchButtonData);
            reinforcementPanel.AddItem(uTrimButtonData);

            #endregion

            #region Annotations panel creation

            RibbonPanel annotationsPanel = application.CreateRibbonPanel(tabName, "Аннотации");

            PushButtonData sketchPlaneButtonData = new PushButtonData(nameof(SketchPlaneCreator), "Плоскость\nперпендикулярная\nвзгляду", assemblyPath, typeof(SketchPlaneCreator).FullName)
            {
                LargeImage = new BitmapImage(new Uri(imgDirectoryPath + "plane.png")),
                ToolTip = "Создает плоскость перпендикулярную взгляду на 3D-виде и делает ее рабочей"
            };

            PushButtonData coverInsertButtonData = new PushButtonData(nameof(CoverInserter), "ЗСБ", assemblyPath, typeof(CoverInserter).FullName)
            {
                LargeImage = new BitmapImage(new Uri(imgDirectoryPath + "cover.png")),
                ToolTip = "Защитный слой бетона"
            };

            annotationsPanel.AddItem(sketchPlaneButtonData);
            annotationsPanel.AddItem(coverInsertButtonData);

            #endregion

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }
}
