using System.Collections.Generic;
using System.Windows.Input;
using SRL.Main.Utilities;
using SRL.Model;
using SRL.Model.Model;
using Point = SRL.Model.Model.Point;

namespace SRL.Main.ViewModel
{
    internal sealed class MapEditorViewModel : EditorViewModel<Map>
    {
        private readonly double _mapWidth = 512; //TODO perhaps let user set it? also, I don't like to have something dependant on WPF control size
        private readonly double _mapHeight = 512; //TODO [same as above]


        public ICommand CloseCurrentPolygonCommand { get; }

        /// <summary>
        /// Currently drawn (valid) map. Doesn't include the polygon that is being constructed (if such exists).
        /// </summary>
        public override Map CurrentModel { get; protected set; }
        /// <summary>
        /// Currently constructed polygon. Empty list if no polygon is being placed.
        /// </summary>
        public List<Point> CurrentPolygon { get; private set; }
        /// <summary>
        /// Checks if all polygons (those already placed and the one being drawn) make up a valid map. Virtually, it says whether a new polygon is being constructed by the user.
        /// </summary>
        public override bool IsCurrentModelValid // Model is both map AND currently drawn polygon.
        {
            get
            {
                return _isCurrentModelValid;
            }
            protected set
            {
                if (_isCurrentModelValid != value)
                {
                    _isCurrentModelValid = value;
                    OnPropertyChanged();
                    ((RelayCommand)SaveModelCommand).OnCanExecuteChanged();
                }

            }
        }
        protected override string SaveFileExtension => "vmd";

        
        private bool _isCurrentModelValid; 


        public MapEditorViewModel()
        {
            Reset();

            CloseCurrentPolygonCommand = new RelayCommand(o =>
            {
                CurrentModel.Obstacles.Add(new Polygon(CurrentPolygon));
                CurrentPolygon = new List<Point>();
                IsCurrentModelValid = true;
            },
            c => CurrentPolygon.Count >= 3);
        }

        protected override void Reset()
        {
            CurrentModel = new Map(_mapWidth, _mapHeight);
            CurrentPolygon = new List<Point>();

            IsCurrentModelValid = true;
        }
        
        protected override void LoadModel(Map model)
        {
            Reset();
            CurrentModel = model;
        }

        protected override bool CanAddVertex(Point newPoint)
        {
            int vCount = CurrentPolygon.Count;

            if (vCount <= 2)
                return true;
            
            for (int i = 0; i < vCount - 2; i++)
            {
                if (GeometryHelper.DoSegmentsIntersect(
                    CurrentPolygon[i], CurrentPolygon[i + 1],
                    CurrentPolygon[vCount - 1], newPoint))
                {
                    return false;
                }
            }
            return true;
        }

        protected override void AddVertex(Point newPoint)
        {
            CurrentPolygon.Add(newPoint);
            IsCurrentModelValid = false;
        }
    }
}
