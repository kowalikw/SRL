using System.Reflection;
using System.Resources;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;
using SRL.Commons.Model.Base;
using SRL.Main.Messages;
using SRL.Main.View.Localization;
using SRL.Commons.Localization;
using SRL.Main.ViewModel.Services;

namespace SRL.Main.ViewModel.Base
{
    /// <summary>
    /// Base class for view-models of the model editors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EditorViewModel<T> : ViewModel
        where T : SvgSerializable
    {
        /// <summary>
        /// Opens up a dialog and saved the edited model to SVG format.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(() =>
                    {
                        var rm = new ResourceManager(typeof(Dialogs).FullName, Assembly.GetExecutingAssembly());
                        string modelName = rm.GetString(typeof(T).Name);

                        ServiceLocator.Current.GetInstance<IDialogService>().ShowSaveFileDialog(
                            string.Format(rm.GetString("modelFilter"), modelName),
                            (r, f) => { if (r) SvgSerializable.Serialize(GetEditedModel(), f); });
                    },
                    () => IsEditedModelValid);
                }
                return _saveCommand;
            }
        }
        /// <summary>
        /// Opens up a dialog and loads model from the specified file.
        /// </summary>
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

        /// <summary>
        /// Resets the edited model.
        /// </summary>
        public abstract RelayCommand ResetCommand { get; }

        private RelayCommand _saveCommand;
        private RelayCommand _loadCommand;

        /// <summary>
        /// Returns a value that indicates whether the edited model is valid.
        /// </summary>
        protected abstract bool IsEditedModelValid { get; }

        protected EditorViewModel()
        {
            MessengerInstance.Register<SetModelMessage<T>>(this, HandleSetModelMsg);
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
        /// <typeparam name="TR">Model type.</typeparam>
        /// <returns>Deserialized model instance.</returns>
        protected static TR LoadModelViaDialog<TR>()
            where TR : SvgSerializable
        {
            string modelName = Models.ResourceManager.GetString(typeof(TR).Name);
            string filter = Dialogs.ResourceManager.GetString("modelFilter");

            bool invalidFile = false;
            TR output = null;

            ServiceLocator.Current.GetInstance<IDialogService>().ShowOpenFileDialog(
                string.Format(filter, modelName),
                (result, filename) =>
                {
                    if (!result)
                        return;

                    if (SvgSerializable.CanDeserialize<TR>(filename))
                        output = SvgSerializable.Deserialize<TR>(filename);
                    else
                        invalidFile = true;
                });

            if (invalidFile)
            {
                ServiceLocator.Current.GetInstance<IDialogService>().ShowMessageDialog(
                    Dialogs.ResourceManager.GetString("modelNotFoundTitle"),
                    string.Format(Dialogs.ResourceManager.GetString("modelNotFoundMsg"), modelName),
                    null);
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
