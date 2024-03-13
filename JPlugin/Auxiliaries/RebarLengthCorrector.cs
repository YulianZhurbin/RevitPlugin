using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Jplugin
{
    /// <summary>
    /// Изменяет длину арматурного стержня до ближайшей стандартной
    /// </summary>
    public static class RebarLengthCorrector
    {
        private const int PRECISION = 5;
        private static double correctingNumber = 0;
        private static readonly double[] acceptableLengths =
            { 780, 900, 970, 1170, 1300, 1460, 1670, 1950, 2340, 2920, 3900, 4680, 5850, 7020, 7800, 8780, 9360, 9750, 10030, 11700 };

        /// <summary>
        /// Увеличивает длину арматурного стержня до ближайшей стандартной
        /// </summary>
        /// <param name="rebar"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Результат операции</returns>
        public static Result Stretch(Element rebar, out string errorMessage)
        {
            errorMessage = string.Empty;
            double rebarLength = UnitUtils.ConvertFromInternalUnits(rebar.LookupParameter("Длина стержня").AsDouble(), UnitTypeId.Millimeters);

            if (rebar.LookupParameter("Форма").AsValueString() != "Прямой стержень")
                correctingNumber = PRECISION;

            for (int i = 0; i < acceptableLengths.Length; i++)
            {
                if (rebarLength + correctingNumber < acceptableLengths[i])
                {
                    double lengthDifference = acceptableLengths[i] - rebarLength;
                    double valueOfA = UnitUtils.ConvertFromInternalUnits(rebar.LookupParameter("A").AsDouble(), UnitTypeId.Millimeters);
                    int newValueOfA = (int)(valueOfA + lengthDifference);

                    newValueOfA -= newValueOfA % PRECISION;

                    rebar.LookupParameter("A").Set(UnitUtils.ConvertToInternalUnits(newValueOfA, UnitTypeId.Millimeters));

                    return Result.Succeeded;
                }
            }

            errorMessage = string.Format("Общая длина детали (id={0}) превышает длину стандартного стержня {1} мм",
                                         rebar.Id, acceptableLengths[acceptableLengths.Length - 1]);
            return Result.Failed;
        }

        /// <summary>
        /// Уменьшает длину арматурного стержня до ближайшей стандартной
        /// </summary>
        /// <param name="rebar"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Результат операции</returns>
        public static Result Trim(Element rebar, out string errorMessage)
        {
            errorMessage = string.Empty;
            double rebarLength = UnitUtils.ConvertFromInternalUnits(rebar.LookupParameter("Длина стержня").AsDouble(), UnitTypeId.Millimeters);

            if (rebarLength < acceptableLengths[0])
            {
                errorMessage = string.Format("Общая длина детали (id={0}) меньше длины минимального стандартного стержня {1} мм",
                              rebar.Id, acceptableLengths[0]);
                return Result.Failed;
            }

            double lengthDifference = 0;

            for (int i = 1; i < acceptableLengths.Length; i++)
            {
                if (rebarLength <= acceptableLengths[i])
                {
                    lengthDifference = rebarLength - acceptableLengths[i - 1];
                    break;
                }
            }

            if (lengthDifference == 0)
            {
                lengthDifference = rebarLength - acceptableLengths[acceptableLengths.Length - 1];
            }

            double valueOfA = UnitUtils.ConvertFromInternalUnits(rebar.LookupParameter("A").AsDouble(), UnitTypeId.Millimeters);
            int newValueOfA = (int)(valueOfA - lengthDifference);

            newValueOfA -= newValueOfA % PRECISION;
            rebar.LookupParameter("A").Set(UnitUtils.ConvertToInternalUnits(newValueOfA, UnitTypeId.Millimeters));

            return Result.Succeeded;
        }

        /// <summary>
        /// Увеличивает длину арматурного П-стержня до ближайшей стандартной с сохранением равенства длин хвостов
        /// </summary>
        /// <param name="rebar"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static Result StretchURebar(Element rebar, out string errorMessage)
        {
            errorMessage = string.Empty;
            double uRebarLength = UnitUtils.ConvertFromInternalUnits(rebar.LookupParameter("Длина стержня").AsDouble(), UnitTypeId.Millimeters);

            correctingNumber = PRECISION * 2;

            for (int i = 0; i < acceptableLengths.Length; i++)
            {
                if (uRebarLength + correctingNumber <= acceptableLengths[i])
                {
                    double lengthDifference = acceptableLengths[i] - uRebarLength;
                    double valueOfA = UnitUtils.ConvertFromInternalUnits(rebar.LookupParameter("A").AsDouble(), UnitTypeId.Millimeters);
                    double valueOfC = UnitUtils.ConvertFromInternalUnits(rebar.LookupParameter("C").AsDouble(), UnitTypeId.Millimeters);

                    double newValueOfAAndC = (valueOfA + valueOfC + lengthDifference) / 2;
                    int roundedDownNewValueOfAAndC = (int)newValueOfAAndC;
                    roundedDownNewValueOfAAndC -= roundedDownNewValueOfAAndC % PRECISION;

                    rebar.LookupParameter("A").Set(UnitUtils.ConvertToInternalUnits(roundedDownNewValueOfAAndC, UnitTypeId.Millimeters));
                    rebar.LookupParameter("C").Set(UnitUtils.ConvertToInternalUnits(roundedDownNewValueOfAAndC, UnitTypeId.Millimeters));

                    return Result.Succeeded;
                }
            }

            errorMessage = string.Format("Общая длина детали (id={0}) превышает длину стандартного стержня {1} мм",
                                         rebar.Id, acceptableLengths[acceptableLengths.Length - 1]);
            return Result.Failed;
        }

        /// <summary>
        /// Уменьшает длину арматурного П-стержня до ближайшей стандартной с сохранением равенства длин хвостов
        /// </summary>
        /// <param name="rebar"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static Result TrimURebar(Element rebar, out string errorMessage)
        {
            errorMessage = string.Empty;
            double uRebarLength = UnitUtils.ConvertFromInternalUnits(rebar.LookupParameter("Длина стержня").AsDouble(), UnitTypeId.Millimeters);

            if (uRebarLength < acceptableLengths[0])
            {
                errorMessage = string.Format("Общая длина детали (id={0}) меньше длины минимального стандартного стержня {1} мм",
                              rebar.Id, acceptableLengths[0]);
                return Result.Failed;
            }

            double lengthDifference = 0;

            for (int i = 1; i < acceptableLengths.Length; i++)
            {
                if (uRebarLength <= acceptableLengths[i])
                {
                    lengthDifference = uRebarLength - acceptableLengths[i - 1];
                    break;
                }
            }

            if (lengthDifference == 0)
            {
                lengthDifference = uRebarLength - acceptableLengths[acceptableLengths.Length - 1];
            }

            double valueOfA = UnitUtils.ConvertFromInternalUnits(rebar.LookupParameter("A").AsDouble(), UnitTypeId.Millimeters);
            double valueOfC = UnitUtils.ConvertFromInternalUnits(rebar.LookupParameter("C").AsDouble(), UnitTypeId.Millimeters);

            double newValueOfAAndC = (valueOfA + valueOfC - lengthDifference) / 2;
            int roundedDownNewValueOfAAndC = (int)newValueOfAAndC;
            roundedDownNewValueOfAAndC -= roundedDownNewValueOfAAndC % PRECISION;

            rebar.LookupParameter("A").Set(UnitUtils.ConvertToInternalUnits(roundedDownNewValueOfAAndC, UnitTypeId.Millimeters));
            rebar.LookupParameter("C").Set(UnitUtils.ConvertToInternalUnits(roundedDownNewValueOfAAndC, UnitTypeId.Millimeters));

            return Result.Succeeded;
        }
    }
}
