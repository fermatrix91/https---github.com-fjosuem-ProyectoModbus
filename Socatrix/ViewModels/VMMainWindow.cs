using Modbus.Device;
using MontesAcc.Ayuda;
using Socatrix.Ayuda;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Socatrix.ViewModels
{
    public class VMMainWindow : ViewModelBase
    {
        public RelayCommand AceptarCommand { get; set; }

        public ObservableCollection<ParidadInfo> listaParidades;
        public ObservableCollection<ParidadInfo> ListaParidades { get { return listaParidades; } set { listaParidades = value; OnPropertyChange("ListaParidades"); } }

        public ParidadInfo paridadSeleccionada;
        public ParidadInfo ParidadSeleccionada { get { return paridadSeleccionada; } set { paridadSeleccionada = value; OnPropertyChange("ParidadSeleccionada"); } }

        public ObservableCollection<StopBitsInfo> listaStopBits;
        public ObservableCollection<StopBitsInfo> ListaStopBits { get { return listaStopBits; } set { listaStopBits = value; OnPropertyChange("ListaStopBits"); } }

        public StopBitsInfo stopBitsSeleccionado;
        public StopBitsInfo StopBitsSeleccionado { get { return stopBitsSeleccionado; } set { stopBitsSeleccionado = value; OnPropertyChange("StopBitsSeleccionado"); } }

        private string puerto;
        public string Puerto { get { return puerto; } set { puerto = value; OnPropertyChange("Puerto"); } }

        private string voltaje;
        public string Voltaje { get { return voltaje; } set { voltaje = value; OnPropertyChange("Voltaje"); } }

        private string frecuencia;
        public string Frecuencia { get { return frecuencia; } set { frecuencia = value; OnPropertyChange("Frecuencia"); } }

        private string kwh;
        public string Kwh { get { return kwh; } set { kwh = value; OnPropertyChange("Kwh"); } }

        private int leerBaudrate;
        public int LeerBaudrate { get { return leerBaudrate; } set { leerBaudrate = value; OnPropertyChange("LeerBaudrate"); } }

        private string leerPuerto;
        public string LeerPuerto { get { return leerPuerto; } set { leerPuerto = value; OnPropertyChange("LeerPuerto"); } }
        private byte leerId;
        public byte LeerId { get { return leerId; } set { leerId = value; OnPropertyChange("LeerId"); } }

        public VMMainWindow()
        {
            this.ParidadSeleccionada = new ParidadInfo();
            this.StopBitsSeleccionado = new StopBitsInfo();

            CargarListas();
            AceptarCommand = new RelayCommand(x => LeerDatos());

            this.ParidadSeleccionada = this.ListaParidades[0];
            this.StopBitsSeleccionado = this.ListaStopBits[1];
            this.LeerPuerto = "COM4";
            this.LeerBaudrate = 9600;
            this.LeerId = 73;
        }

        private void CargarListas()
        {
            this.ListaParidades = new ObservableCollection<ParidadInfo> { 
                new ParidadInfo(){ Id=1, Nombre="None"},
                new ParidadInfo(){ Id=2, Nombre="Even"},
                new ParidadInfo(){ Id=3, Nombre="Mark"},
                new ParidadInfo(){ Id=4, Nombre="Odd"},
                new ParidadInfo(){ Id=5, Nombre="Space"}
            };

            this.ListaStopBits = new ObservableCollection<StopBitsInfo>
            {
                new StopBitsInfo(){Id=1,Nombre="None"},
                new StopBitsInfo(){Id=2,Nombre="One"},
                new StopBitsInfo(){Id=3,Nombre="OnePointFive"},
                new StopBitsInfo(){Id=4,Nombre="Two"}
            };
        }

        DispatcherTimer timer = new DispatcherTimer();
        private void LeerDatos()
        {
            //ActualizarDato();
            timer.Stop();

            this.Puerto = "";
            this.Kwh = "";
            this.Voltaje = "";
            this.Frecuencia = "";
            
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += timer_Tick;
            timer.Start();            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            ActualizarDato();
        }

        void ActualizarDato()
        {
            try
            {
                SerialPort objetoPort = new SerialPort();

                objetoPort.PortName = this.LeerPuerto; //"COM4";
                objetoPort.BaudRate = this.LeerBaudrate; //9600;
                switch (this.ParidadSeleccionada.Id)
                {
                    case 1:
                        objetoPort.Parity = Parity.None;
                        break;
                    case 2:
                        objetoPort.Parity = Parity.Even;
                        break;
                    case 3:
                        objetoPort.Parity = Parity.Mark;
                        break;
                    case 4:
                        objetoPort.Parity = Parity.Odd;
                        break;
                    case 5:
                        objetoPort.Parity = Parity.Space;
                        break;
                    default: objetoPort.Parity = Parity.None;
                        break;
                }

                switch (this.StopBitsSeleccionado.Id)
                {
                    case 1:
                        objetoPort.StopBits = StopBits.None;
                        break;
                    case 2:
                        objetoPort.StopBits = StopBits.One;
                        break;
                    case 3:
                        objetoPort.StopBits = StopBits.OnePointFive;
                        break;
                    case 4:
                        objetoPort.StopBits = StopBits.Two;
                        break;
                    default: objetoPort.StopBits = StopBits.One;
                        break;
                }

                //objetoPort.DataBits = 8;
                //objetoPort.RtsEnable = true;
                //objetoPort.DtrEnable = true;

                ///PARA SABER TODOS LOS COMs EXISTENTES
                string[] ArrayComPortsNames = null;
                ArrayComPortsNames = SerialPort.GetPortNames();

                objetoPort.Open();

                byte slaveId = this.LeerId;
                ushort startAddress = 0;
                ushort numRegisters = 12;

                // read five registers

                IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(objetoPort);
                objetoPort.ReadTimeout = 5000;

                ushort[] registers = master.ReadHoldingRegisters(slaveId, startAddress, numRegisters);

                string abc = Convert.ToDouble(registers[0]) / 10 + " kwh" + Environment.NewLine + "Voltaje " + Convert.ToDouble(registers[2]) / 100 + " V" + Environment.NewLine;

                this.Puerto = this.LeerPuerto;
                this.Kwh = Convert.ToDouble(registers[0]) / 10 + " kWh";
                this.Voltaje = Convert.ToDouble(registers[2]) / 100 + " V";
                this.Frecuencia = Convert.ToDouble(registers[11]) / 100 + " Hz";

                objetoPort.Close();
            }
            catch (Exception excepcion)
            {
                MessageBox.Show(excepcion.Message, "Error Inesperado", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Puerto = "";
                this.Kwh = "";
                this.Voltaje = "";
                this.Frecuencia = "";
                timer.Stop();
            }
        }
    }
}
