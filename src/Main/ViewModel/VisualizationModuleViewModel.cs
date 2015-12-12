using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
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



        public VisualizationModuleViewModel()
        {

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

                currentAngle = frames.Peek().Rotation;
                currentPosition = frames.Peek().Position;
                if (Math.Abs(dx) < 0.5)
                {
                    for (double y = 1; y <= dy; y++)
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
                    for (double x = 1; x <= dx; x++)
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
                    double xStep = dx > 0 ? 1 : -1;
                    double yStep = dy > 0 ? 1 : -1;

                    // Based on Bresenham's line algorithm.
                    if (Math.Abs(dx) > Math.Abs(dy))
                    {
                        double error = Math.Abs(dx/2) - Math.Abs(dy);
                        for (double x = xStep, y = 0; x <= dx; x += xStep)
                        {
                            if (error < 0)
                            {
                                y += yStep;
                                error += Math.Abs(dx);
                            }
                            else
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
                        for (double y = yStep, x = 0; y <= dx; y += yStep)
                        {
                            if (error < 0)
                            {
                                x += xStep;
                                error += Math.Abs(dy);
                            }
                            else
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
            _frames = (Frame[])_frames.Reverse();
        }
    }
}
