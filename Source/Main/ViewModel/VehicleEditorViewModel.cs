using System.Windows;
using FirstFloor.ModernUI.Presentation;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using Microsoft.Win32;
using System;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace SRL.Main.ViewModel
{
    public class VehicleEditorViewModel : EditorViewModel<Vehicle>
    {
        public override RelayCommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(() =>
                    {
                        ShapeDone = false;
                        VehicleShape.Clear();
                        Pivot = null;
                        Direction = null;

                        // TODO: To tests only
                        /*System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
                        customCulture.NumberFormat.NumberDecimalSeparator = ".";

                        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

                        var dialog = new OpenFileDialog();
                        dialog.Filter = String.Format("svg files (*.svg)|*.svg");

                        if (dialog.ShowDialog() == true)
                        {
                            XDocument doc = XDocument.Load(dialog.FileName);
                            var serializer = new XmlSerializer(typeof(Vehicle));

                            Vehicle output;

                            using (XmlReader reader = doc.CreateReader())
                                output = (Vehicle)serializer.Deserialize(reader);

                            VehicleShape.AddRange(output.Shape.Vertices);
                        }*/



                        // TODO: To tests only
                        /*System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
                        customCulture.NumberFormat.NumberDecimalSeparator = ".";

                        System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

                        var dialog = new SaveFileDialog();
                        dialog.Filter = String.Format("SVG files (*.svg)|*.svg");

                        if (dialog.ShowDialog() == true)
                        {
                            var serializer = new XmlSerializer(typeof(Vehicle));
                            var output = new XDocument();

                            using (XmlWriter writer = output.CreateWriter())
                                serializer.Serialize(writer, GetModel());

                            File.WriteAllText(dialog.FileName, output.ToString());
                        }*/
                    }, () =>
                    {
                        return VehicleShape.Count > 0
                        || ShapeDone
                        || Pivot.HasValue
                        || Direction.HasValue;
                    });
                }
                return _resetCommand;
            }
        }
        public override RelayCommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                {
                    _backCommand = new RelayCommand(() =>
                    {
                        if (Direction.HasValue)
                            Direction = null;
                        else if (Pivot.HasValue)
                            Pivot = null;
                        else if (ShapeDone)
                            ShapeDone = false;
                        else
                            VehicleShape.RemoveLast();
                    }, () =>
                    {
                        return VehicleShape.Count > 0
                        || ShapeDone
                        || Pivot.HasValue
                        || Direction.HasValue;
                    });
                }
                return _backCommand;
            }
        }
        public RelayCommand<Point> AddShapeVertexCommand
        {
            get
            {
                if (_addShapeVertexCommand == null)
                {
                    _addShapeVertexCommand = new RelayCommand<Point>(vertex =>
                    {
                        VehicleShape.Add(vertex);
                        ShapeDone = false;
                    }, vertex =>
                    {
                        if (VehicleShape.Count <= 2)
                            return true;

                        for (int i = 0; i < VehicleShape.Count - 2; i++)
                        {
                            if (GeometryHelper.DoSegmentsIntersect(
                                VehicleShape[i], VehicleShape[i + 1],
                                VehicleShape.GetLast(), vertex))
                            {
                                return false;
                            }
                        }
                        return true;
                    });
                }
                return _addShapeVertexCommand;
            }
        }
        public RelayCommand FinishShapeCommand
        {
            get
            {
                if (_finishShapeCommand == null)
                {
                    _finishShapeCommand = new RelayCommand(
                        () => ShapeDone = true,
                        () => VehicleShape.Count >= 3);
                }
                return _finishShapeCommand;
            }
        }
        public RelayCommand<Point> SetPivotCommand
        {
            get
            {
                if (_setPivotCommand == null)
                {
                    _setPivotCommand = new RelayCommand<Point>(
                        point => Pivot = point,
                        point => GeometryHelper.IsInsidePolygon(point, new Polygon(VehicleShape)));
                }
                return _setPivotCommand;
            }
        }
        public RelayCommand<double> SetDirectionCommand
        {
            get
            {
                if (_setDirectionCommand == null)
                {
                    _setDirectionCommand = new RelayCommand<double>(
                        angle => Direction = angle,
                        angle => Pivot.HasValue);
                }
                return _setDirectionCommand;
            }
        }

        private RelayCommand _resetCommand;
        private RelayCommand _backCommand;
        private RelayCommand<Point> _addShapeVertexCommand;
        private RelayCommand _finishShapeCommand;
        private RelayCommand<Point> _setPivotCommand;
        private RelayCommand<double> _setDirectionCommand;


        public ObservableCollectionEx<Point> VehicleShape { get; }

        public bool ShapeDone
        {
            get { return _shapeDone; }
            set
            {
                if (value != _shapeDone)
                {
                    _shapeDone = value;
                    RaisePropertyChanged();
                }
            }
        }
        public Point? Pivot { get; private set; }
        public double? Direction { get; private set; }


        private bool _shapeDone;

        public override bool IsModelValid
        {
            get
            {
                return ShapeDone
                  && Pivot.HasValue
                  && Direction.HasValue;
            }
        }
        public bool AntialiasingEnabled { get; set; }


        public VehicleEditorViewModel()
        {
            VehicleShape = new ObservableCollectionEx<Point>();
        }

        public override Vehicle GetModel()
        {
            if (!IsModelValid)
                return null;

            Vehicle vehicle = new Vehicle();

            //TODO resize & rotate vehicle

            return vehicle;
        }


    }
}
