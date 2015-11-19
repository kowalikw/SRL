using System;
using System.IO;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace SRL.Main.ViewModel
{
    internal abstract class EditorViewModel<T> where T : IXmlSerializable
    {
        public ICommand ResetModelCommand { get; }
        public ICommand SaveModelCommand { get; }
        public ICommand LoadModelCommand { get; }

        protected abstract string SaveFileExtension { get; }
        protected abstract T ModelToSave { get; }
        protected abstract bool IsModelValid { get; }

        protected EditorViewModel()
        {
            ResetModelCommand = new RelayCommand(o => ResetModel());

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
                        serializer.Serialize(writer, ModelToSave);

                    File.WriteAllText(dialog.FileName, output.ToString());
                }
            },
            predicate => IsModelValid);

            LoadModelCommand = new RelayCommand(o =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = String.Format("{0} files (*.{1})|*.{1}",
                    SaveFileExtension.ToUpper(),
                    SaveFileExtension.ToLower());

                if (dialog.ShowDialog() == true)
                {
                    var serializer = new XmlSerializer(typeof(T));

                    //TODO deserialize and call OnModelLoad
                }
            });

        }

        protected abstract void ResetModel();
        protected abstract void LoadModel(T model);
    }
}
