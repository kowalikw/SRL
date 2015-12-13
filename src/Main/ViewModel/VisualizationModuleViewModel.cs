using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using SRL.Main.Utilities;
using SRL.Model;
using SRL.Model.Model;

namespace SRL.Main.ViewModel
{
    internal class VisualizationModuleViewModel
    {
        public class Frame
        {
            public Point Position { get; set; }
            public double Rotation { get; set; }
        }

        public ICommand CalculatePathCommand { get; }

        public ICommand LoadSimulationCommand { get; }
        public ICommand SaveSimulationCommand { get; }
        public ICommand LoadVehicleCommand { get; }
        public ICommand LoadMapCommand { get; }

        public ICommand SetInitialRotation { get; }
        public ICommand SetStartpoint { get; }
        public ICommand SetEndpoint { get; }


        public Frame CurrentFrame { get; private set; }
        private Frame[] _frames;


        public Map Map { get; private set; }
        public Vehicle Vehicle { get; private set; }
        public double InitialRotation { get; private set; }
        public Point Startpoint { get; private set; }
        public Point Endpoint { get; private set; }


        private IAlgorithm _algorithm = new MockAlgorithm();


        public VisualizationModuleViewModel()
        {
            CalculatePathCommand = new RelayCommand(o =>
            {
                var orders = _algorithm.GetPath(Map, Vehicle, Startpoint, Endpoint, InitialRotation, 360); //TODO set angle denisty somewhere else

                if (orders != null)
                    _frames = DivideIntoFrames(orders);
                else
                {
                    _frames = null;
                    //TODO ??
                }
            },
            c =>
            {
                //TODO map, vehicle, startpoint, endpoint, initialrotation are set
                return false;
            });


            LoadMapCommand = new RelayCommand(o =>
            {
                //TODO 
            });
            LoadVehicleCommand = new RelayCommand(o =>
            {
                //TODO
            });
            LoadSimulationCommand = new RelayCommand(o =>
            {
                //TODO
            });
            SaveSimulationCommand = new RelayCommand(o =>
            {
                //TODO
            },
            c =>
            {
                //TODO only if path exists and is calculated
                return false;
            });


            SetInitialRotation = new RelayCommand(o =>
            {
                InitialRotation = (double) o;
            }, c =>
            {
                if (Map == null || Vehicle == null)
                    return false;

                //TODO the vehicle has to fit in without colliding with any obstacles
                return false;
            });
            SetStartpoint = new RelayCommand(o =>
            {
                ResetSimulation();
                Startpoint = (Point) o;
            }, c =>
            {
                return true;
            });
            SetEndpoint = new RelayCommand(o =>
            {
                ResetSimulation();
                Endpoint = (Point) o;
            }, c =>
            {
                return true;
            });
        }

        private void ResetSimulation()
        {
            _frames = null;
            CurrentFrame = null;
        }

        private Frame[] DivideIntoFrames(List<Order> orders)
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
            var fArray = frames.ToArray();
            Array.Reverse(fArray);
            return fArray;
        }
    }
}
