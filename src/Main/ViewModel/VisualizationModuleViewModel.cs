using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SRL.Model;
using SRL.Model.Model;
using System.Windows.Threading;
using SRL.Main.Utilities;

namespace SRL.Main.ViewModel
{
    internal class VisualizationModuleViewModel
    {
        

        public ICommand CalculatePathCommand { get; }

        public ICommand LoadSimulationCommand { get; }
        public ICommand LoadVehicleCommand { get; }
        public ICommand LoadMapCommand { get; }

        public ICommand SetInitialRotation { get; }
        public ICommand SetStartpoint { get; }
        public ICommand SetEndpoint { get; }


        public Frame CurrentFrame { get; private set; }
        private Frame[] _frames;

        private DispatcherTimer _timer;
        private int _frameNumber;

        public Map Map { get; private set; }
        public Vehicle Vehicle { get; private set; }
        public double InitialRotation { get; private set; }
        public Point Startpoint { get; private set; }
        public Point Endpoint { get; private set; }

        public List<Order> orders { get; set; } // TODO: DELETE

        public VisualizationModuleViewModel()
        {
            Startpoint = new Point(50, 100);
            Endpoint = new Point(500, 500);

            Polygon obstacle1 = new Polygon(new List<Point>() { new Point(200, 200), new Point(240, 240), new Point (200, 240) });
            Polygon obstacle2 = new Polygon(new List<Point>() { new Point(420, 90), new Point(480, 120), new Point(470, 180), new Point(450, 150) });

            Map = new Map(512, 512, new List<Polygon>() { obstacle1, obstacle2 });

            Polygon vehicle = new Polygon(new List<Point>() { new Point(30,90), new Point(70, 90), new Point(80, 100), new Point(70, 110), new Point(30, 110) });
            Vehicle = new Vehicle(vehicle, new Point(50, 100), 0);

            MockAlgorithm mock = new MockAlgorithm();
            orders = mock.GetPath(Map, Vehicle, Startpoint, Endpoint, 0, 0);

            DivideIntoFrames(orders);
            CurrentFrame = _frames[0];

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            _frameNumber++;
            CurrentFrame = _frames[_frameNumber];

            if (_frameNumber == _frames.Length - 1)
                _timer.Stop();
        }

        private void DivideIntoFrames(List<Order> orders)
        {
            const int partsPerRadian = 128;
            double radiansPerPart = 1 / (double)partsPerRadian;

            Stack<Frame> frames = new Stack<Frame>();
            frames.Push(new Frame
            {
                Position = Startpoint,
                Rotation = InitialRotation
            });

            for (int o = 0; o < orders.Count; o++)
            {
                // Rotation frames.
                double relativeRotation;

                if (o == 0)
                    relativeRotation = orders[o].Rotation - InitialRotation;
                else
                    relativeRotation = orders[o].Rotation - orders[o - 1].Rotation;

                int partCount = (int)(Math.Abs(relativeRotation) * partsPerRadian);

                double currentAngle = frames.Peek().Rotation;
                Point currentPosition = frames.Peek().Position;
                double angleChange = relativeRotation > 0 ? radiansPerPart : -radiansPerPart;

                for (int p = 0; p < partCount - 1; p++)
                {
                    currentAngle += angleChange;

                    frames.Push(new Frame
                    {
                        Position = currentPosition,
                        Rotation = currentAngle < 0 ? Math.PI * 2 - currentAngle : currentAngle
                    });
                }

                frames.Push(new Frame
                {
                    Position = currentPosition,
                    Rotation = orders[o].Rotation
                });

                // Move frames.
                Point start = o == 0 ? Startpoint : orders[o - 1].Destination;
                Point end = orders[o].Destination;

                double dx = end.X - start.X;
                double dy = end.Y - start.Y;

                double xStep = dx > 0 ? 1 : -1;
                double yStep = dy > 0 ? 1 : -1;

                currentAngle = frames.Peek().Rotation;
                currentPosition = frames.Peek().Position;
                if (Math.Abs(dx) < 0.5)
                {
                    for (double y = yStep; Math.Abs(y) <= Math.Abs(dy); y += yStep)
                    {
                        frames.Push(new Frame
                        {
                            Position = new Point(currentPosition.X, currentPosition.Y + y),
                            Rotation = currentAngle,
                        });
                    }
                }
                else if (Math.Abs(dy) < 0.5)
                {
                    for (double x = xStep; Math.Abs(x) <= Math.Abs(dx); x += yStep)
                    {
                        frames.Push(new Frame
                        {
                            Position = new Point(currentPosition.X + x, currentPosition.Y),
                            Rotation = currentAngle,
                        });
                    }
                }
                else
                {
                    // Based on Bresenham's line algorithm.
                    if (Math.Abs(dx) > Math.Abs(dy))
                    {
                        double error = Math.Abs(dx / 2) - Math.Abs(dy);
                        for (double x = xStep, y = 0; Math.Abs(x) <= Math.Abs(dx); x += xStep)
                        {
                            if (error < 0)
                            {
                                y += yStep;
                                error += Math.Abs(dx);
                            }
                            error -= Math.Abs(dy);

                            frames.Push(new Frame
                            {
                                Position = new Point(currentPosition.X + x, currentPosition.Y + y),
                                Rotation = currentAngle,
                            });
                        }
                    }
                    else
                    {
                        double error = Math.Abs(dy / 2) - Math.Abs(dx);
                        for (double y = yStep, x = 0; Math.Abs(y) <= Math.Abs(dy); y += yStep)
                        {
                            if (error < 0)
                            {
                                x += xStep;
                                error += Math.Abs(dy);
                            }
                            error -= Math.Abs(dx);

                            frames.Push(new Frame
                            {
                                Position = new Point(currentPosition.X + x, currentPosition.Y + y),
                                Rotation = currentAngle,
                            });
                        }
                    }
                }

            }
            _frames = frames.ToArray();
            Array.Reverse(_frames);
        }
    }
}
