using System.Windows;
using SRL.Main.ViewModel;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class VehicleEditorView : Window
    {
        public VehicleEditorView()
        {
            InitializeComponent();
            DataContext = new VehicleEditorViewModel();
        }
    }
}

