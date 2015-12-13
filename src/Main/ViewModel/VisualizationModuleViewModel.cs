﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using SRL.Main.Utilities;
using SRL.Model;
using SRL.Model.Model;
using System.Windows.Threading;
using System.IO;
using System.Xml;
using Microsoft.Win32;

namespace SRL.Main.ViewModel
{
    internal class VisualizationModuleViewModel
    {
        public ICommand CalculatePathCommand { get; }

        public ICommand LoadSimulationCommand { get; }
        public ICommand SaveSimulationCommand { get; }
        public ICommand LoadVehicleCommand { get; }
        public ICommand LoadMapCommand { get; }

        public ICommand SetInitialRotation { get; }
        public ICommand SetStartpoint { get; }
        public ICommand SetEndpoint { get; }

        public ICommand ResumeCommand { get; }
        public ICommand PauseCommand { get; }


        public int CurrentFrameIdx { get; set; }
        public Frame CurrentFrame => _frames?[CurrentFrameIdx];
        public int MaxFrameIdx => _frames?.Length - 1 ?? -1;
        private Frame[] _frames;

        private DispatcherTimer _timer;

        public Map Map { get; private set; }
        public Vehicle Vehicle { get; private set; }
        public double? InitialRotation { get; private set; }
        public Point Startpoint { get; private set; }
        public Point Endpoint { get; set; }

        public List<Order> orders { get; set; } // TODO: Temp to test.

        private IAlgorithm _algorithm = new MockAlgorithm();


        public VisualizationModuleViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            _timer.Tick += _timer_Tick;

            CalculatePathCommand = new RelayCommand(o =>
            {
                orders = _algorithm.GetPath(Map, Vehicle, Startpoint, Endpoint, InitialRotation.Value, 360); //TODO set angle denisty somewhere else

                if (orders != null)
                    _frames = DivideIntoFrames(orders);
                else
                {
                    //TODO ??
                }
            },
            c =>
            {
                if (Map == null || Vehicle == null)
                    return false;
                if (Startpoint == null || Endpoint == null)
                    return false;
                InitialRotation = 0;
                /*if (InitialRotation == null)
                    return false;*/
                return true;
            });


            LoadMapCommand = new RelayCommand(o =>
            {
                ResetSimulation();

                Map = new Map(512, 512);

                var dialog = new OpenFileDialog();
                dialog.Filter = String.Format("{0} files (*.{1})|*.{1}",
                "vmd".ToUpper(),
                "vmd".ToLower());

                if (dialog.ShowDialog() == true)
                {
                    var sr = new StreamReader(dialog.FileName);

                    var serializer = XmlReader.Create(new StringReader(sr.ReadToEnd()));

                    Map.ReadXml(serializer);
                }

                ((RelayCommand)CalculatePathCommand).OnCanExecuteChanged();
            });

            LoadVehicleCommand = new RelayCommand(o =>
            {
                ResetSimulation();

                Vehicle = new Vehicle();

                var dialog = new OpenFileDialog();
                dialog.Filter = String.Format("{0} files (*.{1})|*.{1}",
                "vvd".ToUpper(),
                "vvd".ToLower());

                if (dialog.ShowDialog() == true)
                {
                    var sr = new StreamReader(dialog.FileName);

                    var serializer = XmlReader.Create(new StringReader(sr.ReadToEnd()));

                    Vehicle.ReadXml(serializer);
                }

                Startpoint = Vehicle.OrientationOrigin;

                ((RelayCommand)CalculatePathCommand).OnCanExecuteChanged();
            });
            LoadSimulationCommand = new RelayCommand(o =>
            {
                ResetSimulation();
                //TODO
            });
            SaveSimulationCommand = new RelayCommand(o =>
            {
                //TODO
            },
            c =>
            {
                return _frames != null;
            });


            SetInitialRotation = new RelayCommand(o =>
            {
                InitialRotation = (double) o;
            });
            SetStartpoint = new RelayCommand(o =>
            {
                ResetSimulation();
                Startpoint = (Point) o;
            }, c =>
            {
                Point point = (Point) c;

                if (Map == null)
                    return false;

                //TODO check if point is inside an obstacle
                return true;
            });
            SetEndpoint = new RelayCommand(o =>
            {
                ResetSimulation();
                Endpoint = (Point) o;
            }, c =>
            {
                Point point = (Point)c;

                if (Map == null)
                    return false;

                //TODO check if point is inside an obstacle
                return true;
            });

            ResumeCommand = new RelayCommand(o =>
            {
                _timer.Start();
            });
            PauseCommand = new RelayCommand(o =>
            {
                _timer.Stop();
            });
        }

        private void ResetSimulation()
        {
            _frames = null;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            CurrentFrameIdx++;

            if (CurrentFrameIdx + 1 > MaxFrameIdx)
            {
                CurrentFrameIdx = 0;
                _timer.Stop();
            }
            else
                CurrentFrameIdx++;
        }

        private Frame[] DivideIntoFrames(List<Order> orders)
        {
            const int partsPerRadian = 128;
            double radiansPerPart = 1 / (double)partsPerRadian;

            Stack<Frame> frames = new Stack<Frame>();
            frames.Push(new Frame
            {
                Position = Startpoint,
                Rotation = InitialRotation.Value
            });

            for (int o = 0; o < orders.Count; o++)
            {
                // Rotation frames.
                double relativeRotation;

                if (o == 0)
                    relativeRotation = orders[o].Rotation - InitialRotation.Value;
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
                        Rotation = currentAngle < 0 ? Math.PI * 2 + currentAngle : currentAngle
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
                    for (double x = xStep; Math.Abs(x) <= Math.Abs(dx); x += xStep)
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
