using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SRL.Model.Model;

namespace SRL.Main.ViewModel
{
    internal class TracingViewModel
    {
        public class Segment
        {
            public Point A { get; set; }
            public Point B { get; set; }
        }

        public int AreaThreshold { get; set; }
        public int ColorThreshold { get; set; }

        public BitmapSource BitmapToTrace { get; set; }


        //   public ObservableCollection<Segment> TracedSegments { get; set; } 

            public ObservableCollection<Polygon> TracedPolygons { get; set; } 

        //public ObservableCollection<TestPolygon> TracedSegments { get; set; }

        public int CanvasTop { get; set; }
        public int CanvasLeft { get; set; }

        public TracingViewModel()
        {
            AreaThreshold = 50;
            ColorThreshold = 50;




            /*
                        TracedSegments = new ObservableCollection<Segment>();
                        var l = new Segment();

                        l.A = new Point(10,20);
                        l.B = new Point(100,100);

                        TracedSegments.Add(l);
                        */

            TracedPolygons = new ObservableCollection<Polygon>();
            var p = new Polygon(new Point(1, 1), new Point(101, 1), new Point(101, 101), new Point(1, 101));
            TracedPolygons.Add(p);

            /*
            TracedSegments = new ObservableCollection<TestPolygon>();
            var pc = new PointCollection(new Point[] { new Point(1, 1), new Point(101, 1), new Point(101, 101), new Point(1, 101) });
            var tp = new TestPolygon();
            tp.Vertices = pc;
            TracedSegments.Add(tp);
            */
        }
    }
}
