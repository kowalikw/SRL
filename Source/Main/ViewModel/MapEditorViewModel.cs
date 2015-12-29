using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;

namespace SRL.Main.ViewModel
{
    public class MapEditorViewModel : EditorViewModel<Map>
    {
        public RelayCommand<Point> AddVertexCommand
        {
            get
            {
                if (_addVertexCommand == null)
                {
                    _addVertexCommand = new RelayCommand<Point>(vertex =>
                    {
                        UnfinishedPolygon.Add(vertex);
                    }, vertex =>
                    {
                        if (UnfinishedPolygon.Count <= 2)
                            return true;

                        for (int i = 0; i < UnfinishedPolygon.Count - 2; i++)
                        {
                            if (GeometryHelper.DoSegmentsIntersect(
                                UnfinishedPolygon[i], UnfinishedPolygon[i + 1],
                                UnfinishedPolygon.GetLast(), vertex))
                            {
                                return false;
                            }
                        }
                        return true;
                    });
                }
                return _addVertexCommand;
            }
        }
        public RelayCommand FinishPolygonCommand
        {
            get
            {
                if (_finishPolygonCommand == null)
                {
                    _finishPolygonCommand = new RelayCommand(() =>
                    {
                        FinishedPolygons.Add(new Polygon(UnfinishedPolygon));
                        UnfinishedPolygon.Clear();
                    }, () =>
                    {
                        return UnfinishedPolygon.Count >= 3;
                    });
                }
                return _finishPolygonCommand;
            }
        }
        public override RelayCommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(() =>
                    {
                        UnfinishedPolygon.Clear();
                        FinishedPolygons.Clear();
                    }, () =>
                    {
                        return FinishedPolygons.Count > 0 || UnfinishedPolygon.Count > 0;
                    });
                }
                return _resetCommand;
            }
        }
        public RelayCommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                {
                    _backCommand = new RelayCommand(() =>
                    {
                        if (UnfinishedPolygon.Count == 0)
                        {
                            UnfinishedPolygon.AddRange(FinishedPolygons.GetLast().Vertices);
                            FinishedPolygons.RemoveLast();
                        }
                        else
                            UnfinishedPolygon.RemoveLast();
                    }, () =>
                    {
                        return FinishedPolygons.Count > 0 || UnfinishedPolygon.Count > 0;
                    });
                }
                return _backCommand;
            }
        }
        
        private RelayCommand<Point> _addVertexCommand;
        private RelayCommand _finishPolygonCommand;
        private RelayCommand _resetCommand;
        private RelayCommand _backCommand;


        public ObservableCollectionEx<Polygon> FinishedPolygons { get; }
        public ObservableCollectionEx<Point> UnfinishedPolygon { get; }


        protected override bool IsModelValid => UnfinishedPolygon.Count == 0;

        public MapEditorViewModel()
        {
            FinishedPolygons = new ObservableCollectionEx<Polygon>();
            UnfinishedPolygon = new ObservableCollectionEx<Point>();
        }

        protected override Map GetModel()
        {
            if (!IsModelValid)
                return null;

            Map map = new Map();
            map.Obstacles.AddRange(FinishedPolygons);
            return map;
        }

        protected override void SetModel(Map model)
        {
            UnfinishedPolygon.Clear();
            FinishedPolygons.ReplaceRange(model.Obstacles);
        }
    }
}
