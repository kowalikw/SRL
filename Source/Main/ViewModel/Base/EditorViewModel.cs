﻿using System;
using System.Reflection;
using System.Resources;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using SRL.Commons.Model.Base;
using SRL.Main.Messages;
using SRL.Main.View.Localization;
using SRL.Commons.Localization;
using SRL.Commons.Utilities;
using SRL.Main.ViewModel.Services;

namespace SRL.Main.ViewModel.Base
{
    public abstract class EditorViewModel<T> : ViewModel
        where T : SvgSerializable
    {
        protected readonly string _modelName;

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

                        SimpleIoc.Default.GetInstance<IDialogService>().ShowSaveFileDialog(
                            string.Format(rm.GetString("modelFilter"), modelName),
                            (r, f) => { if (r) SvgSerializable.Serialize(GetEditedModel(), f); });
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
                        ResetCommand.Execute(null);

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

            _modelName = Models.ResourceManager.GetString(typeof (T).Name);
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
            string modelName = Models.ResourceManager.GetString(typeof(R).Name);
            string filter = Dialogs.ResourceManager.GetString("modelFilter");

            bool invalidFile = false;
            R output = null;

            SimpleIoc.Default.GetInstance<IDialogService>().ShowOpenFileDialog(
                string.Format(filter, modelName),
                (result, filename) =>
                {
                    if (!result)
                        return;

                    if (SvgSerializable.CanDeserialize<R>(filename))
                        output = SvgSerializable.Deserialize<R>(filename);
                    else
                        invalidFile = true;
                });

            if (invalidFile)
            {
                SimpleIoc.Default.GetInstance<IDialogService>().ShowMessageDialog(
                    Dialogs.ResourceManager.GetString("modelNotFoundTitle"),
                    Dialogs.ResourceManager.GetString("modelNotFoundMsg"),
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
