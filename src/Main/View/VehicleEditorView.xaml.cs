using System.Windows;
using SRL.Main.ViewModel;
using SRL.Model.Model;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class VehicleEditorView : Window
    {
        public VehicleEditorView(object model = null)
        {
            InitializeComponent();
            DataContext = new VehicleEditorViewModel(model);
        }
    }
}

