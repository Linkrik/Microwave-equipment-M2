using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Microwave_equipment_M2.Models
{
    class M2Microwave
    {
        private Socket socket;
        private bool isConnected;
        private IPEndPoint ipPoint;
        //private ASCIIEncoding converter = new ASCIIEncoding();

        public bool Connect(string address, string port)
        {
            Disconnect();
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);
                //SetStartParameters();
                isConnected = true;
            }
            catch
            {
                Disconnect();
                isConnected = false;
            }
            return isConnected;
        }

        public void Disconnect()
        {
            if (socket != null && socket.Connected)
            {
                socket.Disconnect(false);
            }
            isConnected = false;
        }


        //private void SetStartParameters()
        //{
        //    //SetCommand(":DISP ON");
        //    SetCommand(":DISPlay:REMote ON");
        //    SetCommand(":POWer:MODE FIXed");
        //    SetCommand(":FREQuency:MODE FIXed");
        //    SetCommand(":UNIT:POWer DBM");
        //    SetFrequency(10000000000);
        //    SetPower(14);
        //}

        //private void SetCommand(string comStr)
        //{
        //    socket.Send(converter.GetBytes(comStr + "\n"));
        //}

        private void SetCommand(byte[] comBt)
        {
            socket.Send(comBt);
        }

        public void SetAtt(short flags, short numberAtt, short valueAtt)
        {
            short id_comand = 0x13;
            byte[] cmd = new byte[8];
            BitConverter.GetBytes(id_comand).CopyTo(cmd, 0);     // SETATT    sendBytes[0] = 0x13;
            BitConverter.GetBytes(flags).CopyTo(cmd, 2);
            BitConverter.GetBytes(numberAtt).CopyTo(cmd, 4);
            BitConverter.GetBytes(valueAtt).CopyTo(cmd, 6);
            
            try
            {
                socket.Send(cmd);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetPwr(uint numberPwr, uint activePwr)
        {
            short id_comand = 0x11;
            byte[] cmd = new byte[10];
            BitConverter.GetBytes(id_comand).CopyTo(cmd, 0);     // SETPWR       
            BitConverter.GetBytes(numberPwr).CopyTo(cmd, 2);
            BitConverter.GetBytes(activePwr).CopyTo(cmd, 6);

            try
            {
                socket.Send(cmd);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetDac(short numberDAC, uint valueDAC)
        {
            short id_comand = 0x1C;
            byte[] cmd = new byte[8];
            BitConverter.GetBytes(id_comand).CopyTo(cmd, 0);     // SETATT    sendBytes[0] = 0x13;
            BitConverter.GetBytes(numberDAC).CopyTo(cmd, 2);
            BitConverter.GetBytes(valueDAC).CopyTo(cmd, 4);
            
            try
            {
                socket.Send(cmd);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SetRfChnl(short flags, short numberChanal)
        {
            short id_comand = 0x1F;
            byte[] cmd = new byte[6];
            BitConverter.GetBytes(id_comand).CopyTo(cmd, 0);    
            BitConverter.GetBytes(flags).CopyTo(cmd, 2);
            BitConverter.GetBytes(numberChanal).CopyTo(cmd, 4);

            try
            {
                socket.Send(cmd);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public double GetTemp(short numberTemp)
        {
            // Send
            short id_comand = 0x10;
            byte[] cmd = new byte[4];
            BitConverter.GetBytes(id_comand).CopyTo(cmd, 0);
            BitConverter.GetBytes(numberTemp).CopyTo(cmd, 2);

            // Receive
            byte[] bufReceve = new byte[2];
            int counterBytesReceve = 0; // количество полученных байт

            try
            {
                socket.Send(cmd);

                do
                {
                    counterBytesReceve = socket.Receive(bufReceve, bufReceve.Length, 0);
                }
                while (counterBytesReceve > 0);
            }
            catch (Exception)
            {

                throw;
            }

            short val = BitConverter.ToInt16(bufReceve, 0);
            return ((double)(val - 27315) / 100);
        }
    }
}
