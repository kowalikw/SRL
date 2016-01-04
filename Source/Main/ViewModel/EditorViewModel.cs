using System.IO;
using System.Xml;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using SRL.Commons;
using SRL.Commons.Model.Base;
using SRL.Main.Messages;
using System.Xml.Linq;
using SRL.Commons.Model;

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

                            SvgSerializable.Serialize(GetEditedModel(), filename);
                        };
                        Messenger.Default.Send(msg);
                    },
                    () => IsEditedModelValid);
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
                        var model = LoadModelViaDialog<T>();
                        if (model != null)
                            SetEditedModel(model);
                    });
                }
                return _loadCommand;
            }
        }

        public abstract RelayCommand ResetCommand { get; }

        private RelayCommand _saveCommand;
        private RelayCommand _loadCommand;


        protected abstract bool IsEditedModelValid { get; }

        protected EditorViewModel()
        {
            Messenger.Default.Register<SetModelMessage<T>>(this, HandleSetModelMsg);
        } 

        /// <summary>
        /// Returns created model.
        /// </summary>
        /// <returns>Model or null depending on whether <see cref="IsEditedModelValid"/> property returns true.</returns>
        public abstract T GetEditedModel();

        /// <summary>
        /// Sets model in the editor.
        /// </summary>
        /// <param name="model">Model to set; can be incomplete or even erroneous.</param>
        /// <returns>True if it's possible to set <paramref name="model"/>; false otherwise.</returns>
        public abstract void SetEditedModel(T model);

        /// <summary>
        /// Loads <see cref="SvgSerializable"/> model via dialog shown to the user.
        /// </summary>
        /// <typeparam name="R">Model type.</typeparam>
        /// <returns>Deserialized model instance.</returns>
        protected static R LoadModelViaDialog<R>()
            where R : SvgSerializable
        {
            R output = null;

            var msg = new OpenFileDialogMessage();
            msg.Filter = DialogFilter;
            msg.FilenameCallback = filename =>
            {
                if (filename == null)
                    return;

                output = SvgSerializable.Deserialize<R>(filename);
            };
            Messenger.Default.Send(msg);

            if (output == null)
            {
                var errorMsg = new ErrorDialogMessage();
                errorMsg.ErrorDescription = $"Selected doesn't contain {typeof(R)} object."; //TODO Put error description in resources. Make polish version too.
                Messenger.Default.Send(errorMsg);
            }

            return output;
        }

        private void HandleSetModelMsg(SetModelMessage<T> msg)
        {
            if (msg.Model != null)
                SetEditedModel(msg.Model);
        }
    }
}
