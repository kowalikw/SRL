using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SRL.Models.Enum;
using Point = SRL.Models.Model.Point;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Serialization;
using SRL.Models.Model;
using System.Xml.Linq;

namespace SRL.Main
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class VehicleEditorWindow : Window
    {
        public VehicleEditorWindow()
        {
            InitializeComponent();

            btnSetAxis.IsEnabled = false;
            btnSave.IsEnabled = false;
        }

        private void VehicleEditorControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var cursorPosition = e.GetPosition(VehicleEditorControl);
            if (VehicleEditorControl.Mode == VehicleEditorMode.DrawPolygon && !VehicleEditorControl.IsSegmentIntersection)
                VehicleEditorControl.Vehicle.Shape.Vertices.Add(new Point(cursorPosition.X, (cursorPosition.Y)));
            else if (VehicleEditorControl.Mode == VehicleEditorMode.SetAxis)
            {
                if (VehicleEditorControl.OriginStart == null)
                {
                    VehicleEditorControl.OriginStart = new Point(cursorPosition.X, cursorPosition.Y);
                    VehicleEditorControl.Vehicle.Origin = VehicleEditorControl.OriginStart;
                }
                else if (VehicleEditorControl.OriginEnd == null)
                {
                    VehicleEditorControl.OriginEnd = new Point(cursorPosition.X, cursorPosition.Y);
                    btnSetAxis.IsChecked = false;
                    btnSetAxis.IsEnabled = false;
                    btnSave.IsEnabled = true;
                }
            }
        }

        private void VehicleEditorControl_MouseMove(object sender, MouseEventArgs e)
        {
            VehicleEditorControl.CursorPosition = new Point(e.GetPosition(VehicleEditorControl).X, e.GetPosition(VehicleEditorControl).Y);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML file (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                var serializer = new XmlSerializer(typeof(Vehicle));
                var output = new XDocument();

                using (XmlWriter writer = output.CreateWriter())
                    serializer.Serialize(writer, VehicleEditorControl.Vehicle);

                File.WriteAllText(saveFileDialog.FileName, output.ToString());
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDraw_Checked(object sender, RoutedEventArgs e)
        {
            VehicleEditorControl.Mode = VehicleEditorMode.DrawPolygon;
        }

        private void btnDraw_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!VehicleEditorControl.IsSegmentIntersectionUnchecked)
            {
                VehicleEditorControl.Mode = VehicleEditorMode.DrawDone;
                btnDraw.IsEnabled = false;
                btnSetAxis.IsEnabled = true;
            }
            else
                VehicleEditorControl.Mode = VehicleEditorMode.Empty;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var cursorPosition = e.GetPosition(VehicleEditorControl);
            if (VehicleEditorControl.Vehicle.Shape.VertexCount >= 3 && GeometryHelper.DistanceBetweenPoints(VehicleEditorControl.Vehicle.Shape.Vertices[0], new Point(cursorPosition.X, cursorPosition.Y)) <= 8 && !VehicleEditorControl.IsSegmentIntersection)
            {
                btnDraw.IsEnabled = false;
                btnSetAxis.IsEnabled = true;
            }
        }

        private void bntReset_Click(object sender, RoutedEventArgs e)
        {
            btnDraw.IsChecked = false;
            btnDraw.IsEnabled = true;
            btnSetAxis.IsChecked = false;
            btnSetAxis.IsEnabled = false;
            btnSave.IsEnabled = false;

            VehicleEditorControl.Vehicle.Shape.Vertices.Clear();
            VehicleEditorControl.Vehicle.Origin = null;
            VehicleEditorControl.OriginStart = null;
            VehicleEditorControl.OriginEnd = null;
            VehicleEditorControl.IsAngleSet = false;
            VehicleEditorControl.Mode = VehicleEditorMode.Empty; 
        }

        private void btnSetAxis_Checked(object sender, RoutedEventArgs e)
        {
            VehicleEditorControl.Mode = VehicleEditorMode.SetAxis;
        }

        private void btnSetAxis_Unchecked(object sender, RoutedEventArgs e)
        {
            /*VehicleEditorControl.Vehicle.Origin = null;
            VehicleEditorControl.OriginStart = null;
            VehicleEditorControl.OriginEnd = null;
            VehicleEditorControl.IsAngleSet = false;
            VehicleEditorControl.Mode = VehicleEditorMode.DrawDone;*/
        }
    }
}

