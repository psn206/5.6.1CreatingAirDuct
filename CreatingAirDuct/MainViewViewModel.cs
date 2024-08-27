using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreatingAirDuct
{
    internal class MainViewViewModel
    {
        MethodRevitApp methodRevitApp;
        public DelegateCommand SaveCommand { get; }
        public List<DuctType> TypeDuct { get; set; } = new List<DuctType>();
        public DuctType SelectedTypeDuct { get; set; }
        public List<MEPSystemType> TypeSystem { get; set; } = new List<MEPSystemType>();
        public MEPSystemType SelectedTypeSystem { get; set; }
        List<XYZ> Points { get; } = new List<XYZ>();
        public List<Level> Levels { get; set; } = new List<Level>();
        public Level SelectedLevel { get; set; }
        public string Offset { get; set; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            SaveCommand = new DelegateCommand(OnSaveCommand);
            methodRevitApp = new MethodRevitApp(commandData);
            TypeSystem = methodRevitApp.GetSystemTypes();
            TypeDuct = methodRevitApp.GetDuctTypes();
            Levels = methodRevitApp.GetListLevel();
            Points = methodRevitApp.GetPoints();
        }

        private void OnSaveCommand()
        {
            methodRevitApp.CreateDuct(Points, SelectedLevel, SelectedTypeSystem, SelectedTypeDuct, Offset);
            RaiseCloseRecuest();
        }

        public event EventHandler CloseRecuest;

        public void RaiseCloseRecuest()
        {
            CloseRecuest?.Invoke(this, EventArgs.Empty);
        }

    }
}
