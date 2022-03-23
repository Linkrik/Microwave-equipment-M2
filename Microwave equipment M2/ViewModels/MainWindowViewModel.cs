using Microwave_equipment_M2.Infrastructure.Commands;
using Microwave_equipment_M2.Services;
using Microwave_equipment_M2.Themes;
using Microwave_equipment_M2.ViewModels.Base;

using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
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
        }
        #endregion

        private bool themeDark = false;

        public bool ThemeDark
        {
            get => themeDark;
            set
            {
                Set(ref themeDark, value);
                if (themeDark == true)
                {
                    ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
                }
                else
                {
                    ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
                }
            }
        }

        #region Channels

        enum СhannelsEnum
        {
            Through,        //Сквозной
            LowFrequency,   //Низкочастотный 
            Amplifying,     //Уселительный
            UM40,           //УМ-40
            Locking         //Запирание
        }

        private СhannelsEnum channel = СhannelsEnum.Through;

        private void OnPropertyChangedAllСhannels()
        {
            OnPropertyChanged("ThroughChannel");
            OnPropertyChanged("LowFrequencyChannel");
            OnPropertyChanged("AmplifyingChannel");
            OnPropertyChanged("UM40Channel");
            OnPropertyChanged("LockingChannel");
        }

        /// <summary> window title </summary>
        public bool ThroughChannel
        {
            get => channel == СhannelsEnum.Through;
            set
            {
                channel = СhannelsEnum.Through;
                OnPropertyChangedAllСhannels();
            }
        }

        public bool LowFrequencyChannel
        {
            get => channel == СhannelsEnum.LowFrequency;
            set
            {
                channel = СhannelsEnum.LowFrequency;
                OnPropertyChangedAllСhannels();
            }
        }

        public bool AmplifyingChannel
        {
            get => channel == СhannelsEnum.Amplifying;
            set
            {
                channel = СhannelsEnum.Amplifying;
                OnPropertyChangedAllСhannels();
            }
        }

        public bool UM40Channel
        {
            get => channel == СhannelsEnum.UM40;
            set
            {
                channel = СhannelsEnum.UM40;
                OnPropertyChangedAllСhannels();
            }
        }

        public bool LockingChannel
        {
            get => channel == СhannelsEnum.Locking;
            set
            {
                channel = СhannelsEnum.Locking;
                OnPropertyChangedAllСhannels();
            }
        }

        #endregion Channels

        #region SHDNs

        private bool shdn12v = false;
        public bool SHDN12V
        {
            get => shdn12v;
            set => Set(ref shdn12v, value);
        }

        private bool shdn460 = false;
        public bool SHDN460
        {
            get => shdn460;
            set => Set(ref shdn460, value);
        }

        private bool shdn5597 = false;
        public bool SHDN5597
        {
            get => shdn5597;
            set => Set(ref shdn5597, value);
        }

        private bool shdn7971low = false;
        public bool SHDN7971Low
        {
            get => shdn7971low;
            set => Set(ref shdn7971low, value);
        }

        private bool shdn7971high = false;
        public bool SHDN7971High
        {
            get => shdn7971high;
            set => Set(ref shdn7971high, value);
        }

        private bool shdn7972low = false;
        public bool SHDN7972Low
        {
            get => shdn7972low;
            set => Set(ref shdn7972low, value);
        }

        private bool shdn7972high = false;
        public bool SHDN7972High
        {
            get => shdn7972high;
            set => Set(ref shdn7972high, value);
        }

        #endregion SHDNs

        #region Temperatures

        private string temperatureShdn5597;

        /// <summary> window title </summary>
        public string TemperatureShdn5597
        {
            get => temperatureShdn5597;
            set => Set(ref temperatureShdn5597, value);
        }

        private string temperature7971;

        /// <summary> window title </summary>
        public string Temperature7971
        {
            get => temperature7971;
            set => Set(ref temperature7971, value);
        }

        private string temperature7972;

        /// <summary> window title </summary>
        public string Temperature7972
        {
            get => temperature7972;
            set => Set(ref temperature7972, value);
        }

        #endregion Temperatures

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

        #region Socket
        #region IP address

        public IPEndPoint IPPoint { get => new IPEndPoint(ipAddress, port); }


        private IPAddress ipAddress = IPAddress.Parse("192.168.0.1");
        public string IpAddress
        {
            get => Convert.ToString(ipAddress);
            set
            {
                IPAddress newValue;
                if (!IPAddress.TryParse(value, out newValue))
                {
                    newValue = ipAddress;
                }
                Set(ref ipAddress, newValue);
            }
        }

        #endregion IP address

        #region Port

        private int port = 5025;
        public string Port
        {
            get => Convert.ToString(port);
            set
            {
                int newValue;
                if (!int.TryParse(value, out newValue))
                {
                    newValue = port;
                }
                Set(ref port, newValue);
            }
        }

        #endregion Port
        #endregion Socket

        public ObservableCollection<Dac> Dacs { get; set; }
        public ObservableCollection<Attenuator> Attenuators { get; set; }


        #region Команды


        #region NumericUpDownDac

        public ICommand UpButtonDacCommand { get; }
        private bool CanUpButtonDacCommandExecute(object p) => true;
        private void OnUpButtonDacCommandExecuted(object p)
        {
            Dac dac = p as Dac;
            dac.Value += dac.Step/1000;
        }

        public ICommand DownButtonDacCommand { get; }
        private bool CanDownButtonDacCommandExecute(object p) => true;
        private void OnDownButtonDacCommandExecuted(object p)
        {
            Dac dac = p as Dac;
            dac.Value -= dac.Step / 1000;
        }

        #endregion

        #region NumericUpDownAtt
        public ICommand UpButtonAttCommand { get; }
        private bool CanUpButtonAttCommandExecute(object p) => true;
        private void OnUpButtonAttCommandExecuted(object p)
        {
            Attenuator att = p as Attenuator;
            att.Value += att.Step;
        }

        public ICommand DownButtonAttCommand { get; }
        private bool CanDownButtonAttCommandExecute(object p) => true;
        private void OnDownButtonAttCommandExecuted(object p)
        {
            Attenuator att = p as Attenuator;
            att.Value -= att.Step;
        }
        #endregion

        #region EnterValueCommand

        public ICommand EnterValueCommand { get; }
        private bool CanEnterValueCommandCommandExecute(object p) => true;
        private void OnEnterValueCommandExecuted(object p)
        {
            TextBox txtBx = p as TextBox;
            txtBx.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        #endregion EnterValueCommand

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

            UpButtonDacCommand = new LambdaCommand(OnUpButtonDacCommandExecuted, CanUpButtonDacCommandExecute);
            DownButtonDacCommand = new LambdaCommand(OnDownButtonDacCommandExecuted, CanDownButtonDacCommandExecute);

            UpButtonAttCommand = new LambdaCommand(OnUpButtonAttCommandExecuted, CanUpButtonAttCommandExecute);
            DownButtonAttCommand = new LambdaCommand(OnDownButtonAttCommandExecuted, CanDownButtonAttCommandExecute);



            EnterValueCommand = new LambdaCommand(OnEnterValueCommandExecuted, CanEnterValueCommandCommandExecute);
            CloseApplicationCommand = new LambdaCommand(OnCloseApplicationCommandExecuted, CanCloseApplicationCommandExecute);

            #endregion

            Dacs = new ObservableCollection<Dac>
            {
                new Dac(-2,-0.3m,0.00122m,-0.3m,9.76m),
                new Dac(-2,-0.3m,0.00122m,-0.3m,9.76m),
                new Dac(-2,-0.3m,0.00122m,-0.3m,9.76m),
                new Dac(-2,-0.3m,0.00122m,-0.3m,9.76m)
            };

            Attenuators = new ObservableCollection<Attenuator>
            {
                new Attenuator(0, 31.5m, 0, 0.5m),
                new Attenuator(0, 31.5m, 0, 0.5m)
            };

        }



        public class Dac : ViewModel
        {
            public Dac(decimal minimumValue,decimal maximumValue,decimal minimumValueStep, decimal newValue, decimal newStep)
            {
                MinValue = minimumValue;
                MaxValue = maximumValue;
                MinValueStep = minimumValueStep;
                Value = newValue;
                range = Math.Abs(maximumValue - minimumValue);
                Step = newStep;
            }

            private Dac() { }

            private decimal range;
            private decimal value;
            public decimal Value
            {
                get => value;
                set
                {
                    decimal newValue;
                    newValue = Math.Max(MinValue, Math.Min(MaxValue, value));
                    if (MinValueStep != 0 && newValue != 0)
                    {
                        newValue = decimal.Round(newValue / MinValueStep) * MinValueStep;
                    }
                    newValue = decimal.Parse(newValue.ToString("G29"));
                    Set(ref this.value, newValue);
                }
            }


            private decimal minValue;
            public decimal MinValue
            {
                get => minValue;
                set => Set(ref minValue, value);
            }


            private decimal maxValue;
            public decimal MaxValue
            {
                get => maxValue;
                set => Set(ref maxValue, value);
            }

            private decimal minValueStep;
            public decimal MinValueStep
            {
                get => minValueStep;
                set => Set(ref minValueStep, value);
            }

            private decimal step;
            public decimal Step
            {
                get => step;
                set
                {
                    decimal result = value / 1000;
                    if (result < MinValueStep)
                    {
                        result = MinValueStep;
                    }

                    if (result > range)
                    {
                        result = range;
                    }

                    if (result!=0 && MinValueStep != 0)
                    {
                        result = decimal.Round(result / MinValueStep) * MinValueStep;
                    }
                    else
                    {
                        result = 1;
                    }
                    result *= 1000;
                    result = decimal.Parse(result.ToString("G29"));
                    Set(ref step, result);
                }
            }
        }

        public class Attenuator : ViewModel
        {
            private decimal step;
            private uint value;
            decimal minValue;
            decimal maxValue;
            private bool[] pins = new bool[6];

            private bool activityFlagValue;
            private bool activityFlagPins;

            public Attenuator(decimal minimumValue, decimal maximumValue, decimal newValue, decimal newStep)
            {
                minValue = minimumValue;
                maxValue = maximumValue;
                Value = newValue;
                step = newStep;
            }

            private Attenuator() { }

            public decimal Step { get => step; }
            public bool[] Pins { get => pins; }

            public decimal Value
            {
                get => value*0.5m;
                set
                {
                    decimal newValue;
                    newValue = Math.Max(minValue, Math.Min(maxValue, value));

                    Set(ref this.value, (uint)(newValue * 2));

                    for (int i = 0; i < 6; i++)
                    {
                        OnPropertyChanged($"Pin{i}");
                    }
                }

            }


            public bool Pin5
            {
                get => BitOperator.BitSetted(value, 5);
                set
                {
                    uint valueAtt = value ?
                        (BitOperator.SubstituteOnes(this.value, 5, 1)) :
                        (BitOperator.SubstituteZero(this.value, 5, 1));
                    Value = ((decimal)valueAtt)/2;
                    OnPropertyChanged();
                }
            }

            public bool Pin4
            {
                get => BitOperator.BitSetted(value, 4);
                set
                {
                    uint valueAtt = value ?
                        (BitOperator.SubstituteOnes(this.value, 4, 1)) :
                        (BitOperator.SubstituteZero(this.value, 4, 1));
                    Value = ((decimal)valueAtt) / 2;
                    OnPropertyChanged();
                }
            }

            public bool Pin3
            {
                get => BitOperator.BitSetted(value, 3);
                set
                {
                    uint valueAtt = value ?
                        (BitOperator.SubstituteOnes(this.value, 3, 1)) :
                        (BitOperator.SubstituteZero(this.value, 3, 1));
                    Value = ((decimal)valueAtt) / 2;
                    OnPropertyChanged();
                }
            }

            public bool Pin2
            {
                get => BitOperator.BitSetted(value, 2);
                set
                {
                    uint valueAtt = value ?
                        (BitOperator.SubstituteOnes(this.value, 2, 1)) :
                        (BitOperator.SubstituteZero(this.value, 2, 1));
                    Value = ((decimal)valueAtt) / 2;
                    OnPropertyChanged();
                }
            }

            public bool Pin1
            {
                get => BitOperator.BitSetted(value, 1);
                set
                {
                    uint valueAtt = value ?
                        (BitOperator.SubstituteOnes(this.value, 1, 1)) :
                        (BitOperator.SubstituteZero(this.value, 1, 1));
                    Value = ((decimal)valueAtt) / 2;
                    OnPropertyChanged();
                }
            }

            public bool Pin0
            {
                get => BitOperator.BitSetted(value, 0);
                set
                {
                    uint valueAtt = value ?
                        (BitOperator.SubstituteOnes(this.value, 0, 1)) :
                        (BitOperator.SubstituteZero(this.value, 0, 1));
                    Value = ((decimal)valueAtt) / 2;
                    OnPropertyChanged();
                }
            }
        }
    }
}
