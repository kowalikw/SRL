using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using SRL.Model.Model;
using SrlPoint = SRL.Model.Model.Point;
using WinPoint = System.Windows.Point;

namespace SRL.Main.Utilities
{
    public class PolygonToPointCollection : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var vertices = ((Polygon) value).Vertices;
            var pointCollection = new PointCollection(vertices.Select(
                srlPoint => (WinPoint)srlPoint));
            
            return pointCollection;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            var pointCollection = (PointCollection) value;
            var polygon = new Polygon((ICollection<SrlPoint>)pointCollection.Select(
                winPoint => (SrlPoint)winPoint));
            
            return polygon;
        }
    }
}
