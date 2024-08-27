using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CreatingAirDuct
{
    internal class MethodRevitApp
    {

        UIApplication uiApp;
        UIDocument uiDoc;
        Document doc;

        public UIApplication UiApp { get => uiApp; }
        public UIDocument UiDoc { get => uiDoc; }
        public Document Doc { get => doc; }

        public MethodRevitApp(ExternalCommandData commandData)
        {
            uiApp = commandData.Application;
            uiDoc = UiApp.ActiveUIDocument;
            doc = UiDoc.Document;
        }

        public List<MEPSystemType> GetSystemTypes()
        {
            List<MEPSystemType> systemTypes = new FilteredElementCollector(Doc)
                 .OfClass(typeof(MEPSystemType))
                 .Cast<MEPSystemType>()
                 .ToList();
            return systemTypes;
        }


        public List<DuctType> GetDuctTypes()
        {
            var ductTypes =
                new FilteredElementCollector(doc)
                    .OfClass(typeof(DuctType))
                    .Cast<DuctType>()
                    .ToList();
            return ductTypes;
        }

        public List<Level> GetListLevel()
        {
            List<Level> levels = new FilteredElementCollector(doc)
                                                .OfClass(typeof(Level))
                                                .Cast<Level>()
                                                .ToList();
            return levels;
        }

        public List<XYZ> GetPoints()
        {
            List<XYZ> points = new List<XYZ>();
            while (true)
            {
                XYZ pickedPoint = null;
                try
                {
                    pickedPoint = UiDoc.Selection.PickPoint(ObjectSnapTypes.Endpoints, "Выберете точку");
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
                {
                    break;
                }
                points.Add(pickedPoint);
            }
            return points;
        }

        public void CreateDuct(List<XYZ> points, Level selectedLevel, MEPSystemType selectedMEPSystemType, DuctType selectedTypeDuct, string offset)
        {
            if (points.Count == 0 || selectedLevel == null || selectedTypeDuct == null || selectedMEPSystemType == null) return;
            double setCentrLine = Convert.ToDouble(offset);

            using (var ts = new Transaction(Doc, "Create Duct"))
            {
                ts.Start();
                for (int i = 0; i < points.Count; i++)
                {
                    if (i == 0) continue;
                    XYZ startPoint = points[i - 1];
                    XYZ endPoint = points[i];
                    Duct duct = Duct.Create(Doc, selectedMEPSystemType.Id, selectedTypeDuct.Id, selectedLevel.Id, startPoint, endPoint);
                    Parameter parameter = duct.get_Parameter(BuiltInParameter.MEP_UPPER_CENTERLINE_ELEVATION);
                    parameter.Set(UnitUtils.ConvertToInternalUnits(setCentrLine, UnitTypeId.Millimeters));
                }

                ts.Commit();
            }

        }


    }
}
