using MontesAcc.Ayuda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socatrix.Ayuda
{
    class Objetos
    {
    }

    public class ParidadInfo : ViewModelBase
    {
        private int id;
        public int Id { get { return id; } set { id = value; OnPropertyChange("Id"); } }

        private string nombre;
        public string Nombre { get { return nombre; } set { nombre = value; OnPropertyChange("Nombre"); } }
    }

    public class StopBitsInfo : ViewModelBase
    {
        private int id;
        public int Id { get { return id; } set { id = value; OnPropertyChange("Id"); } }

        private string nombre;
        public string Nombre { get { return nombre; } set { nombre = value; OnPropertyChange("Nombre"); } }
    }
}
