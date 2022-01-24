using Microwave_equipment_M2.Infrastructure.Commands;
using Microwave_equipment_M2.ViewModels.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Microwave_equipment_M2.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        #region window title 
        private string _title = "Оснастка СВЧ для М2";

        /// <summary> window title </summary>
        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
            //set 
            //{
            //    //if (Equals(_title, value)) return;
            //    //_title = value;
            //    //OnPropertyChanged();

            //    Set(ref _title, value);
            //}
        }
        #endregion

        #region Status : string - Статус программы

        /// <summary>Статус подключения</summary>
        private string _status = "не подключен";

        /// <summary>Статус подключения</summary>
        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        #endregion

        #region Команды

        #region CloseApplicationCommand
        public ICommand CloseApplicationCommand { get; }

        private bool CanCloseApplicationCommandExecute(object p) => true;
        private void OnCloseApplicationCommandExecuted(object p)
        {
            Application.Current.Shutdown();
        }
        #endregion

        #endregion
        public MainWindowViewModel()
        {
            #region Команды

            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted,CanCloseApplicationCommandExecute);

            #endregion
        }
    }
}
