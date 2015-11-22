using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SRL.Model.Model;

namespace SRL.Main.ViewModel
{
    internal class TracingViewModel
    {
        public int AreaThreshold { get; set; }
        public int ColorThreshold { get; set; }

        public BitmapSource BitmapToTrace { get; set; }
        public ObservableCollection<Polygon> TracedPolygons { get; private set; }



        public TracingViewModel()
        {
            AreaThreshold = 50;
            ColorThreshold = 50;

            TracedPolygons = new ObservableCollection<Polygon>();

        }
    }
}
