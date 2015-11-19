using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Win32;
using SRL.Models;
using SRL.Models.Enum;
using SRL.Models.Model;
using Point = SRL.Models.Model.Point;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for MapEditorView.xaml
    /// </summary>
    public partial class MapEditorView : Window
    {
        public MapEditorView()
        {
            InitializeComponent();
        }

        private void btnDraw_Checked(object sender, RoutedEventArgs e)
        {
            MapEditorControl.Mode = MapEditorMode.DrawPolygon;
        }

        private void btnDraw_Unchecked(object sender, RoutedEventArgs e)
        {
            MapEditorControl.Mode = MapEditorMode.DrawDone;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            btnDraw.IsChecked = false;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML file (*.xml)|*.xml";
            if (saveFileDialog.ShowDialog() == true)
            {
                var serializer = new XmlSerializer(typeof(Map));
                var output = new XDocument();

                using (XmlWriter writer = output.CreateWriter())
                    serializer.Serialize(writer, MapEditorControl.Map);

                File.WriteAllText(saveFileDialog.FileName, output.ToString());
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MapEditorControl.CurrentPolygon.VertexCount > Polygon.MinVerticesCount && GeometryHelper.GetDistance(MapEditorControl.CurrentPolygon.Vertices[0],
                new Point(e.GetPosition(MapEditorControl).X, e.GetPosition(MapEditorControl).Y)) <= Polygon.StartPointRadius && MapEditorControl.CurrentPolygonState != DrawPolygonState.Incorrect)
            {
                btnDraw.IsChecked = false;
            }
        }

        private void MapEditorControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MapEditorControl.Mode == MapEditorMode.DrawPolygon && MapEditorControl.CurrentPolygonState != DrawPolygonState.Incorrect)
                MapEditorControl.CurrentPolygon.Vertices.Add(new Point(e.GetPosition(MapEditorControl).X, e.GetPosition(MapEditorControl).Y));
        }

        private void MapEditorControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MapEditorControl.CursorPosition = new Point(e.GetPosition(MapEditorControl).X, e.GetPosition(MapEditorControl).Y);
        }

        private void resetMap_Click(object sender, RoutedEventArgs e)
        {
            btnDraw.IsChecked = false;
            MapEditorControl.Map.Obstacles.Clear();
            MapEditorControl.CurrentPolygon.Vertices.Clear();
            MapEditorControl.Mode = MapEditorMode.Idle;
        }
    }
}
