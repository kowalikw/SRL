using System.Windows;
using SRL.Main.ViewModel;
using SRL.Model.Model;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for MapEditorView.xaml
    /// </summary>
    public partial class MapEditorView : Window
    {
        public MapEditorView(object model = null)
        {
            InitializeComponent();
            DataContext = new MapEditorViewModel(model);
        }
    }
}
