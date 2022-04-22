
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

        public M2MicrowaveCom()
        {
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
            SetPwr((uint)Power.Power5597, 0);       //Отключаем 5597 
            SetPwr((uint)Power.Power5920, 0);       //Отключаем 5920 
        }


        private bool Send(byte[] cmd)
        {
            bool status = true;
            try
            {
                comPort.Write(cmd, 0, cmd.Length);
            }
            catch (Exception)
            {
                status = false;
            }

            System.Threading.Thread.Sleep(1);
            return status;
        }
        private bool Receive(byte[] bufReceve)
        {
            bool status = true;
            try
            {
                int counter = 0;
                while (comPort.BytesToRead < bufReceve.Length && counter < 100)
                {
                    System.Threading.Thread.Sleep(1);
                    counter++;
                }

                if (comPort.BytesToRead >= bufReceve.Length)
                {
                    comPort.Read(bufReceve, 0, bufReceve.Length);
                }
            }
            catch (Exception)
            {
                status = false;
            }

            System.Threading.Thread.Sleep(1);
            return status;
        }

        #region Set Switch
        public void SetSwitch(short numberSwitch, short numberChanal)
        {
            if (IsConnected)
            {
                short idСomand = 0x21;
                byte[] cmd = new byte[6];
                BitConverter.GetBytes(idСomand).CopyTo(cmd, 0);
                BitConverter.GetBytes(numberSwitch).CopyTo(cmd, 2);
                BitConverter.GetBytes(numberChanal).CopyTo(cmd, 4);

                if (!Send(cmd)) IsConnected = false;
            }
        }

        public void SetSwitchAll(short numberChanal)
        {
            SetSwitch((short)Switchs.SW_1_S1Att, numberChanal);
            SetSwitch((short)Switchs.SW_2_S2Att, numberChanal);
            SetSwitch((short)Switchs.SW_3_S1Dac, numberChanal);
            SetSwitch((short)Switchs.SW_4_S2Dac, numberChanal);
        }
        #endregion Set Switch



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

                if (!Send(cmd)) IsConnected = false;
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

                if (!Send(cmd)) IsConnected = false;
            }
        }

        public void SetDacRamp(short numberDAC, uint valueDAC, short stepIncrement)
        {
            if (IsConnected)
            {
                short idСomand = 0x1E;
                byte[] cmd = new byte[10];
                BitConverter.GetBytes(idСomand).CopyTo(cmd, 0);     // SETATT    sendBytes[0] = 0x13;
                BitConverter.GetBytes(numberDAC).CopyTo(cmd, 2);
                BitConverter.GetBytes(valueDAC).CopyTo(cmd, 4);
                BitConverter.GetBytes(stepIncrement).CopyTo(cmd, 8);

                if (!Send(cmd)) IsConnected = false;
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

                if (!Send(cmd)) IsConnected = false;
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

                if (!Send(cmd)) IsConnected = false;
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

            if (!Send(cmd)) IsConnected = false;
            if (!Receive(bufReceve)) IsConnected = false;
         
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

    enum Switchs
    {
        SW_1_S1Att,
        SW_2_S2Att,
        SW_3_S1Dac,
        SW_4_S2Dac
    }

    [Flags]
    enum Power
    {
        None = 0,
        Power797_1High = 1,
        Power797_2High = 2,
        Power797_1Low = 4,
        Power797_2Low = 8,
        Power460 = 16,
        Power12V = 32,
        Power5597 = 64,
        Power5920 = 128,
        All = Power12V | Power460 | Power5597 | Power797_1Low | Power797_1High | Power797_2Low | Power797_2High | Power5920
    }
    #endregion Enums


}
