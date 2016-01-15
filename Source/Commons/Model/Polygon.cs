﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Serialization;

namespace SRL.Commons.Model
{
    [XmlRoot(ElementName = "polygon")]
    public struct Polygon : IEquatable<Polygon>
    {
        public List<Point> Vertices { get; }

        public Polygon(IEnumerable<Point> vertices)
        {
            Vertices = new List<Point>(vertices);
        }

        public bool Equals(Polygon other)
        {
            if (Vertices.Count == other.Vertices.Count)
            {
                int i;
                for (i = 0; i < Vertices.Count; i++)
                {
                    if (Vertices[i] == other.Vertices[0])
                        break;
                }
                if (i < Vertices.Count)
                {
                    for (int j = 0; j < Vertices.Count; j++)
                        if (Vertices[(i + j) % Vertices.Count] != other.Vertices[j])
                            return false;
                    return true;
                }
            }
            return false;
        }
    }
}
