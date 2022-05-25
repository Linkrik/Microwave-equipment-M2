using Microwave_equipment_M2.Infrastructure.Commands;
using Microwave_equipment_M2.Models;
using Microwave_equipment_M2.Services;
using Microwave_equipment_M2.Themes;
using Microwave_equipment_M2.ViewModels.Base;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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

        #region Themes
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
        #endregion Themes

        #region Module

        public M2MicrowaveCom Module { get; set; }

        public ObservableCollection<string>ComPortsNames{get;set;}
        public string SelectedComPortName
        {
            get => Module.NameComPort;
            set
            {
                Module.NameComPort = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabledComPortsName { get => !Module.IsConnected; }

        public bool Connected
        {
            get => Module.IsConnected;
            set
            {
                if (Module.IsConnected)
                {
                    SHDN12V = false;
                    Module.Disconnect();
                    Status = "не подключен";
                }
                else
                {
                    Status = Module.Connect()? "подключен": "не подключен";
                }


                //Фактически устанавливает параметры, при подключеном модуле
                if (Module.IsConnected)
                {
                    //переключаю канал на сквозной
                    ThroughChannel = true;
                }



                OnPropertyChanged();
                OnPropertyChanged("IsEnabledComPortsName");
            }
        }


        #region Status : string - Статус подключения

        /// <summary>Статус подключения</summary>
        private string _status = "не подключен";

        /// <summary>Статус подключения</summary>
        public string Status
        {
            get => _status;
            set => Set(ref _status, value);
        }

        #endregion



        #endregion Module

        #region Channels

        private Сhannels channel = Сhannels.Through;


        private Сhannels valueChannel
        {
            get { return channel; }
            set
            {
                if (channel == value)
                    return;

                if (value == Сhannels.UM40 || value == Сhannels.Amplifying) 
                {
                    if (!SHDN7971High)
                    {
                        Enabled460 = false;
                        EnabledBorder7971 = true;
                    }
                }
                else
                {
                    Dacs[2].Value = -2;
                    SHDN7971High = false;
                    SHDN7971Low = false;
                    EnabledBorder7971 = false;

                    if (SHDN7972High)
                    {
                        Enabled460 = true;
                    }
                }

                channel = value;
                ActivateСhannel(channel);
                OnPropertyChanged("ThroughChannel");
                OnPropertyChanged("LowFrequencyChannel");
                OnPropertyChanged("AmplifyingChannel");
                OnPropertyChanged("UM40Channel");
                OnPropertyChanged("LockingChannel");
            }
        }
       
        public bool ThroughChannel
        {
            get => valueChannel == Сhannels.Through;
            set => valueChannel = value ? Сhannels.Through : valueChannel;
        }

        public bool LowFrequencyChannel
        {
            get => valueChannel == Сhannels.LowFrequency;
            set => valueChannel = value ? Сhannels.LowFrequency : valueChannel;
        }

        public bool AmplifyingChannel
        {
            get => valueChannel == Сhannels.Amplifying;
            set => valueChannel = value ? Сhannels.Amplifying : valueChannel;
        }

        public bool UM40Channel
        {
            get => valueChannel == Сhannels.UM40;
            set => valueChannel = value ? Сhannels.UM40 : valueChannel;
        }

        public bool LockingChannel
        {
            get => valueChannel == Сhannels.Locking;
            set => valueChannel = value ? Сhannels.Locking : valueChannel;
        }

        public void ActivateСhannel(Сhannels сhannelName)
        {
            Module.SetRfChnl(0, (short)сhannelName);
        }


        private bool enabledBorder7971 = false;
        public bool EnabledBorder7971
        {
            get => enabledBorder7971;
            set => Set(ref enabledBorder7971, value);
        }

        private bool enabled460 = false;
        public bool Enabled460
        {
            get => enabled460;
            set 
            {
                if (!value)
                {
                    SHDN460 = false;
                }

                Set(ref enabled460, value);
            }
        }
        #endregion Channels

        #region SHDNs

        private bool shdn12v = false;
        public bool SHDN12V
        {
            get => shdn12v;
            set
            {
                //При отключении питания +12, должен отключить все остальные SHDN
                if (!value)
                {
                    //Включаю все аттенюаторы
                    foreach (var att in Attenuators)
                    {
                        att.Value = 31.5m;
                    }

                    //Включаю все DAC в minValue кроме 0
                    for (int i = 0; i < Dacs.Count; i++)
                    {
                        Dacs[i].Value = i == 1 ? Dacs[i].MaxValue : Dacs[i].MinValue;
                    }

                    //переключить все свитчи SETSW в состояние «-2,0В» (0-сост)
                    Module.SetSwitchAll(0);

                    SHDN460 = false;
                    SHDN7972Low = false;
                    SHDN5597 = false;
                    SHDN5920 = false;
                }

                ControlPower(Power.Power12V, value);

                if (value)
                {
                    //Включаю все аттенюаторы
                    foreach (var att in Attenuators)
                    {
                        att.Value = 31.5m;
                    }

                    //переключить все свитчи SETSW в состояние «-2,0В» (0-сост)
                    Module.SetSwitchAll(0);

                    //Включаю все DAC в -2В
                    for (int i = 0; i < Dacs.Count; i++)
                    {
                        Dacs[i].Value = i==1? Dacs[i].MaxValue : Dacs[i].MinValue;
                    }

                    //переключить все свитчи SETSW в состояние канала DACs (1-сост)
                    Module.SetSwitchAll(1);
                }


                Set(ref shdn12v, value);
            }
          
        }

        private bool shdn460 = false;
        public bool SHDN460
        {
            get => shdn460;
            set
            {
                ControlPower(Power.Power460, value);
                Set(ref shdn460, value);
            }
        }

        private bool shdn5597 = false;
        public bool SHDN5597
        {
            get => shdn5597;
            set
            {
                ControlPower(Power.Power5597, value);
                Set(ref shdn5597, value);
            }

        }

        private bool shdn5920 = false;
        public bool SHDN5920
        {
            get => shdn5920;
            set
            {
                ControlPower(Power.Power5920, value);
                Set(ref shdn5920, value);
            }

        }

        private bool shdn7971low = false;
        public bool SHDN7971Low
        {
            get => shdn7971low;
            set
            {
                if (!value)
                {
                    SHDN7971High = false;
                }

                ControlPower(Power.Power797_1Low, value);
                Set(ref shdn7971low, value);
            }
        }

        private bool shdn7971high = false;
        public bool SHDN7971High
        {
            get => shdn7971high;
            set
            {
                if (!value)
                {
                    Dacs[2].Value = -2;
                }
                

                if (valueChannel == Сhannels.UM40 || valueChannel == Сhannels.Amplifying)
                {
                    Enabled460 = value;
                }


                ControlPower(Power.Power797_1High, value);
                Set(ref shdn7971high, value);
            }
            
        }

        private bool shdn7972low = false;
        public bool SHDN7972Low
        {
            get => shdn7972low;
            set
            {
                if (!value)
                {
                    SHDN7972High = false;
                }

                ControlPower(Power.Power797_2Low, value);
                Set(ref shdn7972low, value);
            }
        }

        private bool shdn7972high = false;
        public bool SHDN7972High
        {
            get => shdn7972high;
            set
            {
                if (!(valueChannel == Сhannels.UM40 || valueChannel == Сhannels.Amplifying))
                {
                    Enabled460 = value;
                }

                if (!value)
                {
                    Dacs[3].Value = -2;
                    SHDN7971High = false;
                    SHDN7971Low = false;
                }


                ControlPower(Power.Power797_2High,value);
                Set(ref shdn7972high, value);
            }
        }


        public void ActivatePower(Power powerName)
        {
            Module.SetPwr((uint)powerName, (uint)Power.All);
        }

        public void DeactivatePower(Power powerName)
        {
            Module.SetPwr((uint)powerName, 0);
        }

        private void ControlPower(Power powerName, bool state)
        {
            if (state) ActivatePower(powerName);
            else DeactivatePower(powerName);
        }

        #endregion SHDNs

        #region Temperatures
        private DispatcherTimer timer;

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!Module.IsConnected)
            {
                timer.Stop();
            }
            else
            {
                try
                {
                    TemperatureShdn5597 = GetDatTemp(ThermalSensors.TermSHDN5597);
                    Temperature7971 = GetDatTemp(ThermalSensors.TermSHDN797_1);
                    Temperature7972 = GetDatTemp(ThermalSensors.TermSHDN797_2);
                }
                catch (Exception)
                {
                    timer.Stop();
                    //MessageBox.Show($"Прибор не отвечает\nПЕРЕПОДКЛЮЧИТЕСЬ!\n\n{ex}", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        } //событие запуска таймера


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

        public string GetDatTemp(ThermalSensors thermalSensors)
        {
            double data = Module.GetTemp((short)thermalSensors);

            if (data == -273.15)
            {
                return null;
            }
            return data.ToString("0.##");
        }


        #endregion Temperatures


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


        #region Update List Com Ports Command
        public ICommand UpdateListComPortsCommand { get; }

        private bool CanUpdateListComPortsCommandExecute(object p) => true;
        private void OnUpdateListComPortsCommandExecuted(object p)
        {
            ComPortsNames.Clear(); //new ObservableCollection<string>();
            foreach (var Name in Module.SearchComPorts())
            {
                ComPortsNames.Add(Name);
            }
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

            UpdateListComPortsCommand = new LambdaCommand(OnUpdateListComPortsCommandExecuted, CanUpdateListComPortsCommandExecute);

            #endregion

            #region Search Com Port
            Module = new M2MicrowaveCom();
            ComPortsNames = new ObservableCollection<string>();
            foreach (var Name in Module.SearchComPorts())
            {
                ComPortsNames.Add(Name);
            }
            #endregion Search Com Port

            Module.PropertyChanged += OnModelPropertyChanged;

            #region Temperature timer
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);     //добавляем событие таймера при запуске
            timer.Interval = new TimeSpan(0, 0, 0, 3); //событие будет срабатывать через каждые 3 сек. 
            #endregion Temperature timer

            #region ADC and ATT
            Dacs = new ObservableCollection<Dac>
            {
                new Dac(-4.9959m,-0.3m,0.00122m,-5,false,9.76m, Models.Dacs.Dac1_Att3,Module),
                new Dac(-4.9959m,-0.3m,0.00122m,-5,false,9.76m, Models.Dacs.Dac2_Att3,Module),
                new Dac(-2,-0.3m,0.00061m,-2,true,9.76m, Models.Dacs.Dac3_797_1,Module),
                new Dac(-2,-0.3m,0.00061m,-2,true,9.76m, Models.Dacs.DAC4_797_2,Module),
            };

            Attenuators = new ObservableCollection<Attenuator>
            {
                new Attenuator(0, 31.5m, 0, 0.5m, Models.Attenuators.Att1, Module),
                new Attenuator(0, 31.5m, 0, 0.5m, Models.Attenuators.Att2, Module )
            };
            #endregion ADC and ATT
        }

        #region Обработка событий
        private void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Connected");
            Status = Module.IsConnected ? "подключен" : "не подключен";
            
            if (Module.IsConnected)
            {
                timer.Start();
            }
            else
            {
                timer.Stop();
            }
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            SHDN12V = false;
            Module.Disconnect();
        }
        #endregion

        public class Dac : ViewModel
        {
            public Dac(decimal minimumValue,decimal maximumValue,
                       decimal minimumValueStep, decimal newValue,
                       bool useRamp,
                       decimal newStep, Dacs dacName , M2MicrowaveCom m2MicrowaveCom)
            {
                module = m2MicrowaveCom;
                MinValue = minimumValue;
                MaxValue = maximumValue;
                MinValueStep = minimumValueStep;
                Value = newValue;
                rump = useRamp;
                range = Math.Abs(maximumValue - minimumValue);
                Step = newStep;
                dacNamber = dacName;
            }

            private Dac() { }
            private Dacs dacNamber;
            private M2MicrowaveCom module;
            private decimal range;
            private bool rump = false;
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
                    newValue = decimal.Parse(newValue.ToString("#.00000"));

                    if (rump)
                    {
                        SetValueDacRamp(dacNamber, newValue);
                    }
                    else
                    {
                        SetValueDac(dacNamber, newValue);
                    }

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

            private void SetValueDacRamp(Dacs dacName, decimal valueVolt)
            {
                uint dacValue = (uint)(Math.Abs(valueVolt) / MinValueStep);
                short stepRamp  = Convert.ToInt16(decimal.Round(0.01m/MinValueStep));
                module.SetDacRamp((short)dacName, dacValue, stepRamp);
            }

            private void SetValueDac(Dacs dacName, decimal valueVolt)
            {
                uint dacValue = (uint)(Math.Abs(valueVolt) / MinValueStep);
                module.SetDac((short)dacName, dacValue);
            }
        }

        public class Attenuator : ViewModel
        {
            private decimal step;
            private uint value;
            decimal minValue;
            decimal maxValue;
            private bool[] pins = new bool[6];

            private Attenuators attNamber;
            private M2MicrowaveCom module;

            public Attenuator(decimal minimumValue, decimal maximumValue,
                              decimal newValue, decimal newStep,
                              Attenuators attName, M2MicrowaveCom m2MicrowaveCom)
            {
                module = m2MicrowaveCom;
                minValue = minimumValue;
                maxValue = maximumValue;
                Value = newValue;
                step = newStep;
                attNamber = attName;
            }

            private void SetValueAtt(Attenuators attName, uint valueUint)
            {
                module.SetAtt(0, (short)attName, (short)valueUint);
            }

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
                    SetValueAtt(attNamber,this.value);

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
