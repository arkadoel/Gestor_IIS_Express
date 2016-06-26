using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GestorIISExpress
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string APP_NAME = "Gestor IIS Express"; //diario de programacion local
        public const string APP_VERSION = "Alpha 0.2016.06.26";
        public static MainWindow mainWindow { get; set; }
    }
}
