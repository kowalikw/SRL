using System.Windows.Input;
using SRL.Model.Model;

namespace SRL.Main.ViewModel
{
    internal class MapEditorViewModel : EditorViewModel<Map>
    {
        protected override string SaveFileExtension => "vmd";
        protected override Map ModelToSave => _currentMap;
        protected override bool IsCurrentModelValid { get; set; }


        private Map _currentMap;


        protected override void Reset()
        {
            throw new System.NotImplementedException();
        }

        protected override void LoadModel(Map model)
        {
            throw new System.NotImplementedException();
        }

        protected override bool CanAddPoint(Point point)
        {
            throw new System.NotImplementedException();
        }

        protected override void AddPoint(Point point)
        {
            //TODO Add point, check if map is valid and update IsCurrentModelValid property. 

            throw new System.NotImplementedException();
        }
    }
}
