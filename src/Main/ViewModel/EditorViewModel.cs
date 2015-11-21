using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Win32;
using SRL.Main.Annotations;
using Point = SRL.Model.Model.Point;

namespace SRL.Main.ViewModel
{
    internal abstract class EditorViewModel<T> : INotifyPropertyChanged 
        where T : IXmlSerializable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand ResetCommand { get; }
        public ICommand SaveModelCommand { get; }
        public ICommand LoadModelCommand { get; }
        public ICommand PlacePointCommand { get; }

        public abstract T CurrentModel { get; protected set; }
        public abstract bool IsCurrentModelValid { get; protected set; } // Don't forget to call OnPropertyChanged!
        protected abstract string SaveFileExtension { get; }

        protected EditorViewModel()
        {
            ResetCommand = new RelayCommand(o => Reset());

            SaveModelCommand = new RelayCommand(o =>
            {
                var dialog = new SaveFileDialog();
                dialog.Filter = String.Format("{0} files (*.{1})|*.{1}",
                    SaveFileExtension.ToUpper(),
                    SaveFileExtension.ToLower());

                if (dialog.ShowDialog() == true)
                {
                    var serializer = new XmlSerializer(typeof(T));
                    var output = new XDocument();

                    using (XmlWriter writer = output.CreateWriter())
                        serializer.Serialize(writer, CurrentModel);

                    File.WriteAllText(dialog.FileName, output.ToString());
                }
            },
            predicate => IsCurrentModelValid);

            LoadModelCommand = new RelayCommand(o =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = String.Format("{0} files (*.{1})|*.{1}",
                    SaveFileExtension.ToUpper(),
                    SaveFileExtension.ToLower());

                if (dialog.ShowDialog() == true)
                {
                    var serializer = new XmlSerializer(typeof(T));

                    //TODO deserialize and call LoadModel
                }
            });

            PlacePointCommand = new RelayCommand(o =>
            {
                var point = (Point) o;
                PlacePoint(point);
            },
            c =>
            {
                var point = (Point)c;
                return CanPlacePoint(point);
            });
        }

        protected abstract void Reset();
        protected abstract void LoadModel(T model);
        protected abstract bool CanPlacePoint(Point point);
        protected abstract void PlacePoint(Point point);


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
