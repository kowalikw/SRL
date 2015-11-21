using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Win32;
using SRL.Main.ViewModel;
using SRL.Model;
using SRL.Model.Enum;
using SRL.Model.Model;
using Point = SRL.Model.Model.Point;

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
            DataContext = new MapEditorViewModel();
        }


    }
}
