using System.Windows;
using SRL.Main.ViewModel;

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
