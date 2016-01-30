using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;
using SRL.Main.ViewModel.Base;

namespace SRL.Main.ViewModel
{
    /// <summary>
    /// View-model class that contains non-UI logic for the map editor.
    /// </summary>
    public class MapEditorViewModel : EditorViewModel<Map>
    {
        /// <summary>
        /// Adds new vertex to the map which may be part of an existing <see cref="Polygon"/> or start of a new one.
        /// </summary>
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
                        if (UnfinishedPolygon.Count > 0 && UnfinishedPolygon.GetLast() == vertex)
                            return false;

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
        /// <summary>
        /// Creates <see cref="Polygon"/> from placed vertices.
        /// </summary>
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
        /// <summary>
        /// Deletes the last-placed vertex.
        /// </summary>
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

        /// <summary>
        /// Collection of all finished and valid polygons in the editor.
        /// </summary>
        public ObservableCollectionEx<Polygon> FinishedPolygons { get; }
        /// <summary>
        /// Collection of vertices of the currently edited and not-yet-closed <see cref="Polygon"/>.
        /// </summary>
        public ObservableCollectionEx<Point> UnfinishedPolygon { get; }


        protected override bool IsEditedModelValid => UnfinishedPolygon.Count == 0;

        public MapEditorViewModel()
        {
            FinishedPolygons = new ObservableCollectionEx<Polygon>();
            UnfinishedPolygon = new ObservableCollectionEx<Point>();
        }

        public override Map GetEditedModel()
        {
            if (!IsEditedModelValid)
                return null;

            Map map = new Map();
            map.Obstacles.AddRange(FinishedPolygons);
            return map;
        }

        public override void SetEditedModel(Map model)
        {
            var obstacles = model.Obstacles;
            RemoveEnclosedPolygons(obstacles);

            UnfinishedPolygon.Clear();
            FinishedPolygons.ReplaceRange(obstacles);
        }

        /// <summary>
        /// Removes polygons wholly enclosed by another polygons.
        /// </summary>
        /// <param name="polygons">List of polygons that might contain enclosed enclosed shapes.</param>
        private void RemoveEnclosedPolygons(List<Polygon> polygons)
        {
            bool[] enclosed = new bool[polygons.Count];

            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = i + 1; j < polygons.Count; j++)
                {
                    if (enclosed[j])
                        continue;

                    enclosed[j] = GeometryHelper.IsEnclosed(polygons[j], polygons[i]);
                }
            }

            var nonEnclosedPolygons = new List<Polygon>(polygons.Where((polygon, i) => !enclosed[i]));
            polygons.Clear();
            polygons.AddRange(nonEnclosedPolygons);
        }
    }
}
