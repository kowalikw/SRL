using System.Windows;
using SRL.Models.Enum;
using Microsoft.Win32;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using SRL.Models.Model;
using System.IO;
using Point = SRL.Models.Model.Point;

namespace SRL.Main
{
    /// <summary>
    /// Interaction logic for MapEditorWindow.xaml
    /// </summary>
    public partial class MapEditorWindow : Window
    {
        public MapEditorWindow()
        {
            InitializeComponent();
        }

        private void btnDraw_Checked(object sender, RoutedEventArgs e)
        {
            MapEditorControl.Mode = MapEditorMode.DrawPolygon;
        }

        private void btnDraw_Unchecked(object sender, RoutedEventArgs e)
        {
            MapEditorControl.Mode = MapEditorMode.Idle;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
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

        }

        private void MapEditorControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (MapEditorControl.Mode == MapEditorMode.DrawPolygon)
                MapEditorControl.ActualPolygon.Vertices.Add(new Point(e.GetPosition(MapEditorControl).X, e.GetPosition(MapEditorControl).Y));
        }

        private void MapEditorControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            MapEditorControl.CursorPosition = new Point(e.GetPosition(MapEditorControl).X, e.GetPosition(MapEditorControl).Y);
        }
    }
}
