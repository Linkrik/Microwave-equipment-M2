using Microwave_equipment_M2.Models.Base;

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microwave_equipment_M2.Models
{
    class M2MicrowaveCom : Model
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
            private set => Set(ref isConnected, value);
        }
        public string NameComPort
        {
            get => portName;
            set
            {
                if (!IsConnected)
                {
                    Set(ref portName, value);
                }
            }
        }


        public string[] SearchComPorts()
        {
            return SerialPort.GetPortNames();
        }


        public bool Connect()
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
            }
            catch
            {
                Disconnect();
            }
            return IsConnected;
        }


        public void Disconnect()
        {
            if (comPort != null)
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
                throw new Exception();
            }


            int val = BitConverter.ToInt32(receiveBytes, 0);


            if (!(val == ID))
            {
                return false;
            }

            return true;
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
                    //throw new Exception();
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
                    //throw new Exception();
                }
            }
        }


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
                    //throw new Exception();
                }
            }
        }


        public void SetRfChnl(short flags, short numberChanal)
        {
            if (true)
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
                    //throw new Exception();
                }
            }
        }


        public double GetTemp(short numberTemp)
        {
            // Send
            short idСomand = 0x10;
            byte[] cmd = new byte[4];
            BitConverter.GetBytes(idСomand).CopyTo(cmd, 0);
            BitConverter.GetBytes(numberTemp).CopyTo(cmd, 2);

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
                throw;
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
        Dac3_7971,
        DAC4_7972
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
        SHDN5597,
        SHDN7971,
        SHDN7972
    }
    #endregion Enums


}
