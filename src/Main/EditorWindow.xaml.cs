﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SRL.Models.Enum;
using Point = SRL.Models.Model.Point;

namespace SRL.Main
{
    /// <summary>
    /// Interaction logic for EditorWindow.xaml
    /// </summary>
    public partial class EditorWindow : Window
    {
        public EditorWindow()
        {
            InitializeComponent();
        }

        private void VehicleEditorControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var cursorPosition = e.GetPosition(VehicleEditorControl);
            if (VehicleEditorControl.Mode == VehicleEditorMode.DrawPolygon)
                VehicleEditorControl.Vertices.Add(new Point(cursorPosition.X, (cursorPosition.Y)));
            else if (VehicleEditorControl.Mode == VehicleEditorMode.SetAxis)
            {
                if (VehicleEditorControl.OriginStart == null)
                    VehicleEditorControl.OriginStart = new Point(cursorPosition.X, cursorPosition.Y);
                else if (VehicleEditorControl.OriginEnd == null)
                    VehicleEditorControl.OriginEnd = new Point(cursorPosition.X, cursorPosition.Y);
            }
        }

        private void VehicleEditorControl_MouseMove(object sender, MouseEventArgs e)
        {
            VehicleEditorControl.CursorPosition = new Point(e.GetPosition(VehicleEditorControl).X, e.GetPosition(VehicleEditorControl).Y);
        }

        private void btnSetAxis_Click(object sender, RoutedEventArgs e)
        {
            VehicleEditorControl.Mode = VehicleEditorMode.SetAxis;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnDraw_Checked(object sender, RoutedEventArgs e)
        {
            VehicleEditorControl.Mode = VehicleEditorMode.DrawPolygon;
        }

        private void btnDraw_Unchecked(object sender, RoutedEventArgs e)
        {
            VehicleEditorControl.Mode = VehicleEditorMode.DrawDone;
            btnDraw.IsEnabled = false;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var cursorPosition = e.GetPosition(VehicleEditorControl);
            if (VehicleEditorControl.Vertices.Count >= 3 && GeometryHelper.DistanceBetweenPoints(VehicleEditorControl.Vertices[0], new Point(cursorPosition.X, cursorPosition.Y)) <= 8)
                btnDraw.IsEnabled = false;
        }
    }
}
