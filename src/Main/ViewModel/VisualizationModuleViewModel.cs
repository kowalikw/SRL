using System;
using System.Collections.Generic;
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
            double radiansPerPart = 1 / partsPerRadian;
            
            Stack<Frame> frames = new Stack<Frame>();
            frames.Push(new Frame
            {
                Position = Startpoint,
                Rotation = InitialRotation
            });

            for (int o = 0; o < orders.Count; o++)
            {
                // rotation
                double relativeRotation;

                if (o == 0)
                {
                    relativeRotation = orders[o].Rotation - InitialRotation;
                    if (orders[o].Rotation < 0)
                        relativeRotation += Math.PI * 2;
                }
                else
                {
                    relativeRotation = orders[o].Rotation - orders[o - 1].Rotation;
                    if (orders[o].Rotation < 0)
                        relativeRotation += Math.PI * 2;
                    if (orders[o - 1].Rotation < 0)
                        relativeRotation += Math.PI*2;
                }


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
            }









        }
    }
}
