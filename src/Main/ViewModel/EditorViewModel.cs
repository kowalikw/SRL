using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Win32;
using SRL.Main.Annotations;
using SRL.Model.Xml;
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
        public ICommand AddVertexCommand { get; }

        public abstract T CurrentModel { get; protected set; }
        public abstract bool IsCurrentModelValid { get; protected set; } // Don't forget to call OnPropertyChanged!
        protected abstract string SaveFileExtension { get; }

        protected EditorViewModel()
        {
            ResetCommand = new RelayCommand(o => Reset());

            SaveModelCommand = new RelayCommand(o =>
            {
                //TODO serialize encoding (UTF-8)

                var dialog = new SaveFileDialog();
                dialog.Filter = String.Format("{0} files (*.{1})|*.{1}",
                    SaveFileExtension.ToUpper(),
                    SaveFileExtension.ToLower());

                if (dialog.ShowDialog() == true)
                {
                    var output = Marshaller<T>.Marshall(CurrentModel);

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

            AddVertexCommand = new RelayCommand(o =>
            {
                var point = (Point) o;
                AddVertex(point);
            },
            c =>
            {
                var point = (Point)c;
                return CanAddVertex(point);
            });
        }

        protected abstract void Reset();
        protected abstract void LoadModel(T model);
        protected abstract bool CanAddVertex(Point point);
        protected abstract void AddVertex(Point point);


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
