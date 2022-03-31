
using Microwave_equipment_M2.Models.Base;

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microwave_equipment_M2.Models
{
    class M2MicrowaveCom: Model
    {
        private SerialPort comPort;
        private bool isConnected;
        private string portName;

        private uint dac12CodeInit = (uint)(2 / 0.00122m); // код для DAC1 и DAC2 = 2 Вольта 
        private uint dac34CodeInit = (uint)(2 / 0.00061m); // код для DAC3 и DAC4 = 2 Вольта

        public M2MicrowaveCom()
        {
            power = Power.None;
            comPort = null;
            isConnected = false;
        }

        public bool IsConnected
        {
            get => isConnected;
            private set
            {
                Set(ref isConnected, value);
            }
        }

        public string NameComPort
        {
            get => portName;
            set
            {
                if (!IsConnected)
                {
                    portName = value;
                }
            }
        }


        public string[] SearchComPorts()
        {
            return SerialPort.GetPortNames();
        }


        public bool Connect()
        {
            if (portName != null)
            {
                if (comPort == null)
                {
                    comPort = new SerialPort(portName, 115200, Parity.None, 8, StopBits.One);
                    comPort.WriteTimeout = 3000;
                    comPort.ReadTimeout = 3000;
                }

                try
                {
                    comPort.Open();
                    IsConnected = true;//TestConnection();
                    SetStartParameters();
                }
                catch
                {
                    Disconnect();
                }
                return IsConnected;
            }
            else return IsConnected = false;
        }


        public void Disconnect()
        {
            if (comPort != null && comPort.IsOpen)
            {
                SetEndParameters();
                comPort.Close();
            }
            IsConnected = false;
        }


        private bool TestConnection()
        {
            int ID = 67305985;
            byte[] sendBytes = new byte[] { 1, 0 };
            byte[] receiveBytes = new byte[4];

            try
            {
                comPort.Write(sendBytes, 0, sendBytes.Length);
                System.Threading.Thread.Sleep(1);

                int counter = 0;
                while (comPort.BytesToRead < 4 && counter < 100)
                {
                    System.Threading.Thread.Sleep(1);
                    counter++;
                }

                if (comPort.BytesToRead >= 4)
                {
                    comPort.Read(receiveBytes, 0, receiveBytes.Length);
                }

            }
            catch (Exception)
            {
                return false;
            }


            int val = BitConverter.ToInt32(receiveBytes, 0);


            if (!(val == ID))
            {
                return false;
            }

            return true;
        }

        private void SetStartParameters()
        {
            //Сброс всех SHDN
            SetPwr((uint)Power.Power460, 0);        //Отключаем 460
            SetPwr((uint)Power.Power797_1High, 0);  //Отключаем «Высокий» 797_1 
            SetPwr((uint)Power.Power797_2High, 0);  //Отключаем «Высокий» 797_2
            SetPwr((uint)Power.Power797_1Low, 0);   //Отключаем «Низкий» 797_1 
            SetPwr((uint)Power.Power797_2Low, 0);   //Отключаем «Низкий» 797_2
            SetPwr((uint)Power.Power5597, 0);       //Отключаем 5597 //Сброс всех SHDN

            //Устанавливаем значения на DACs
            SetSwitch(0, 0);    // SETSW в состояние «-2,0В» (0-сост)
            SetDac((short)Dacs.Dac1_Att3, dac12CodeInit); //Устанавливаем значение ЦАП1 -2,0В
            SetDac((short)Dacs.Dac2_Att3, dac12CodeInit); //Устанавливаем значение ЦАП2 -2,0В
            SetDac((short)Dacs.Dac3_797_1, dac34CodeInit); //Устанавливаем значение ЦАП3 -2,0В
            SetDac((short)Dacs.DAC4_797_2, dac34CodeInit); //Устанавливаем значение ЦАП4 -2,0В
            SetSwitch(0, 1);    // SETSW в состояние «-2,0В» (0-сост)

            //Устанавливаем значения на Att и Chan
            SetAtt(0, (short)Attenuators.Att1, 0);    //Устанавливаем значение Аттенюатора 1 в 0
            SetAtt(0, (short)Attenuators.Att2, 0);    //Устанавливаем значение Аттенюатора 2 в 0
            SetRfChnl(0, (short)Сhannels.Through);    //Устанавливаем канал "Сквозной"
        }

        private void SetEndParameters()
        {
            SetDacRamp((short)Dacs.Dac3_797_1, dac34CodeInit, Convert.ToInt16(decimal.Round(0.01m / 0.00061m))); //Устанавливаем значение ЦАП3 -2,0В 
            SetDacRamp((short)Dacs.DAC4_797_2, dac34CodeInit, Convert.ToInt16(decimal.Round(0.01m / 0.00061m))); //Устанавливаем значение ЦАП4 -2,0В

            SetPwr((uint)Power.Power460, 0); //Отключаем 460
            SetPwr((uint)Power.Power797_1High, 0); //Отключаем «Высокий» 797_1 
            SetPwr((uint)Power.Power797_2High, 0); //Отключаем «Высокий» 797_2
            SetPwr((uint)Power.Power797_1Low, 0); //Отключаем «Низкий» 797_1 
            SetPwr((uint)Power.Power797_2Low, 0); //Отключаем «Низкий» 797_2
            SetPwr((uint)Power.Power5597, 0); //Отключаем 5597
            SetPwr((uint)Power.Power12V, 0); //Отключаем 12
            SetSwitch(0, 0);    // SETSW в состояние «-2,0В» (0-сост)
        }

        private void SetSwitch(short numberSwitch, short numberChanal)
        {
            if (IsConnected)
            {
                short idСomand = 0x21;
                byte[] cmd = new byte[6];
                BitConverter.GetBytes(idСomand).CopyTo(cmd, 0);
                BitConverter.GetBytes(numberSwitch).CopyTo(cmd, 2);
                BitConverter.GetBytes(numberChanal).CopyTo(cmd, 4);

                try
                {
                    comPort.Write(cmd, 0, cmd.Length);
                }
                catch (Exception)
                {
                    IsConnected = false;
                }
            }
        }


        public void SetAtt(short flags, short numberAtt, short valueAtt)
        {
            if (IsConnected)
            {
                short idСomand = 0x13;
                byte[] cmd = new byte[8];
                BitConverter.GetBytes(idСomand).CopyTo(cmd, 0);
                BitConverter.GetBytes(flags).CopyTo(cmd, 2);
                BitConverter.GetBytes(numberAtt).CopyTo(cmd, 4);
                BitConverter.GetBytes(valueAtt).CopyTo(cmd, 6);

                try
                {
                    comPort.Write(cmd, 0, cmd.Length);
                }
                catch (Exception)
                {
                    IsConnected = false;
                }
            }
        }

        #region DAC
        public void SetDac(short numberDAC, uint valueDAC)
        {
            if (IsConnected)
            {
                short idСomand = 0x1C;
                byte[] cmd = new byte[8];
                BitConverter.GetBytes(idСomand).CopyTo(cmd, 0);     // SETATT    sendBytes[0] = 0x13;
                BitConverter.GetBytes(numberDAC).CopyTo(cmd, 2);
                BitConverter.GetBytes(valueDAC).CopyTo(cmd, 4);

                try
                {
                    comPort.Write(cmd, 0, cmd.Length);
                }
                catch (Exception)
                {
                    IsConnected = false;
                }
            }
        }

        public void SetDacRamp(short numberDAC, uint valueDAC, short stepIncrement)
        {
            if (IsConnected)
            {
                short idСomand = 0x1C;
                byte[] cmd = new byte[10];
                BitConverter.GetBytes(idСomand).CopyTo(cmd, 0);     // SETATT    sendBytes[0] = 0x13;
                BitConverter.GetBytes(numberDAC).CopyTo(cmd, 2);
                BitConverter.GetBytes(valueDAC).CopyTo(cmd, 4);
                BitConverter.GetBytes(stepIncrement).CopyTo(cmd, 8);

                try
                {
                    comPort.Write(cmd, 0, cmd.Length);
                }
                catch (Exception)
                {
                    IsConnected = false;
                }
            }
        }
        #endregion DAC


        public void SetRfChnl(short flags, short numberChanal)
        {
            if (IsConnected)
            {
                short idСomand = 0x1F;
                byte[] cmd = new byte[6];
                BitConverter.GetBytes(idСomand).CopyTo(cmd, 0);
                BitConverter.GetBytes(flags).CopyTo(cmd, 2);
                BitConverter.GetBytes(numberChanal).CopyTo(cmd, 4);

                try
                {
                    comPort.Write(cmd, 0, cmd.Length);
                }
                catch (Exception)
                {
                    IsConnected = false;
                }
            }
        }


        public void SetPwr(uint numberPwr, uint activePwr)
        {
            if (IsConnected)
            {
                short idСomand = 0x0011;
                byte[] cmd = new byte[10];
                BitConverter.GetBytes(idСomand).CopyTo(cmd, 0);
                BitConverter.GetBytes(numberPwr).CopyTo(cmd, 2);
                BitConverter.GetBytes(activePwr).CopyTo(cmd, 6);

                try
                {
                    comPort.Write(cmd, 0, cmd.Length);
                }
                catch (Exception)
                {
                    IsConnected = false;
                }
            }
        }


        public double GetTemp(short tempNumber)
        {
            // Send
            short idСomand = 0x10;
            byte[] cmd = new byte[4];
            BitConverter.GetBytes(idСomand).CopyTo(cmd, 0);
            BitConverter.GetBytes(tempNumber).CopyTo(cmd, 2);

            // Receive
            byte[] bufReceve = new byte[2];

            try
            {
                if (IsConnected)
                {
                    comPort.Write(cmd, 0, cmd.Length);
                    System.Threading.Thread.Sleep(1);

                    int counter = 0;
                    while (comPort.BytesToRead < 2 && counter < 100)
                    {
                        System.Threading.Thread.Sleep(1);
                        counter++;
                    }

                    if (comPort.BytesToRead >= 2)
                    {
                        comPort.Read(bufReceve, 0, bufReceve.Length);
                    }
                }
            }
            catch (Exception)
            {
                IsConnected = false;
            }

            short val = BitConverter.ToInt16(bufReceve, 0);
            return ((double)(val - 27315) / 100);
        }

    }

    #region Enums
    enum Dacs
    {
        Dac1_Att3,
        Dac2_Att3,
        Dac3_797_1,
        DAC4_797_2
    }

    enum Attenuators
    {
        Att1,
        Att2
    }

    enum Сhannels
    {
        Through,        //Сквозной
        LowFrequency,   //Низкочастотный 
        Amplifying,     //Уселительный
        UM40,           //УМ-40
        Locking         //Запирание
    }

    enum ThermalSensors
    {
        TermSHDN5597 = 1,
        TermSHDN797_1,
        TermSHDN797_2
    }

    [Flags]
    enum Power
    {
        None = 0,
        Power12V = 1,
        Power460 = 2,
        Power5597 = 4,
        Power797_1Low = 8,
        Power797_1High = 16,
        Power797_2Low = 32,
        Power797_2High = 64,
        Power5920 = 128,
        All = Power12V| Power460| Power5597| Power797_1Low| Power797_1High| Power797_2Low| Power797_2High| Power5920
    }
    #endregion Enums


}
