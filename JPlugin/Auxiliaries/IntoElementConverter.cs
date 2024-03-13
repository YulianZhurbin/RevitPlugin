using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Jplugin
{
    public static class IntoElementConverter
    {
        /// <summary>
        /// </summary>
        /// <param name="document"></param>
        /// <param name="listOfReferences"></param>
        /// <returns>Список с элементами, полученный из списка ссылок на эти элементы</returns>
        public static List<Element> GetListOfElements(Document document, IList<Reference> listOfReferences)
        {
            List<Element> listOfElements = new List<Element>();

            foreach (Reference reference in listOfReferences)
            {
                listOfElements.Add(document.GetElement(reference));
            }

            return listOfElements;
        }
    }
}
