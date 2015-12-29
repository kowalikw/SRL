using System.IO;
using System.Xml;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using SRL.Commons;
using SRL.Commons.Model.Base;
using SRL.Main.Messages;

namespace SRL.Main.ViewModel
{
    public abstract class EditorViewModel<T> : ViewModelBase
        where T : SvgSerializable
    {
        protected const string DialogFilter = "Scalable Vector Graphics (*.svg)|*.svg";

        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(() =>
                    {
                        var msg = new SaveFileDialogMessage();
                        msg.Filter = DialogFilter;
                        msg.FilenameCallback = filename =>
                        {
                            if (filename == null)
                                return;

                            SaveModelToFile(GetModel(), filename);
                        };
                        Messenger.Default.Send(msg);
                    },
                    () => IsModelValid);
                }
                return _saveCommand;
            }
        }
        public RelayCommand LoadCommand
        {
            get
            {
                if (_loadCommand == null)
                {
                    _loadCommand = new RelayCommand(() =>
                    {
                        var model = LoadModel<T>();
                        if (model != null)
                            SetModel(model);
                    });
                }
                return _loadCommand;
            }
        }

        public abstract RelayCommand ResetCommand { get; }

        private RelayCommand _saveCommand;
        private RelayCommand _loadCommand;


        protected abstract bool IsModelValid { get; }
        /// <summary>
        /// Returns created model.
        /// </summary>
        /// <returns>Model or null depending on whether IsModelValid property returns true.</returns>
        protected abstract T GetModel();
        /// <summary>
        /// Sets model in the editor.
        /// </summary>
        /// <param name="model">Model to set; can be incomplete or even erroneous.</param>
        /// <returns>True if it's possible to set <paramref name="model"/>; false otherwise.</returns>
        protected abstract void SetModel(T model);


        private static void SaveModelToFile<R>(R model, string filename)
        {
            var serializer = new XmlSerializer(typeof(R));

            using (var stream = File.Create(filename))
                serializer.Serialize(stream, model);
        }

        private static R LoadModelFromFile<R>(string filename)
                        where R : SvgSerializable
        {
            var serializer = new XmlSerializer(typeof(R));

            using (var reader = XmlReader.Create(filename))
            {
                if (serializer.CanDeserialize(reader))
                    return (R)serializer.Deserialize(reader);
            }
            return null;
        }

        /// <summary>
        /// Loads SvgSerializable model via dialog shown to the user.
        /// </summary>
        /// <typeparam name="R">Model type.</typeparam>
        /// <returns>Deserialized model instance.</returns>
        protected static R LoadModel<R>()
            where R : SvgSerializable
        {
            R output = null;

            var msg = new OpenFileDialogMessage();
            msg.Filter = DialogFilter;
            msg.FilenameCallback = filename =>
            {
                if (filename == null)
                    return;

                output = LoadModelFromFile<R>(filename);
            };
            Messenger.Default.Send(msg);

            if (output == null)
            {
                var errorMsg = new ErrorDialogMessage();
                errorMsg.ErrorDescription = $"Selected doesn't contain {nameof(R)} object."; //TODO Put error description in resources. Make polish version too.
                Messenger.Default.Send(errorMsg);
            }

            return output;
        }
    }
}
