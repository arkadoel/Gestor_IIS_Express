
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace GestorIISExpress
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private XMLConfiguracion archivoConfiguracion { get; set; }
        private int CONFIG_MAX = 150;
        private int CONFIG_MIN = 0;
        private XElement nodoActual { get; set; }
        private List<XMLConfiguracion.Applicacion> aplicaciones { get; set;}
        private const string RUTA_IIS_EXPRESS = @"C:\Program Files\IIS Express\iisexpress.exe";

        public MainWindow()
        {
            InitializeComponent();
            App.mainWindow = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VerConfig(false);
            this.Title = string.Format("{0} {1}", App.APP_NAME, App.APP_VERSION);

            CargarData();

        }

        private void CargarData()
        {
            archivoConfiguracion = new XMLConfiguracion();
            archivoConfiguracion.CargarXML();

            NodoSitios.Items.Clear();
            foreach (XElement sitio in archivoConfiguracion.xmlSitios)
            {
                var nodo = new TreeViewItem()
                {
                    Header = sitio.Attribute("name").Value,
                    Tag = sitio.Attribute("id").Value
                };

                nodo.Selected += nodo_Selected;

                NodoSitios.Items.Add(nodo);
            }
        }

        void nodo_Selected(object sender, RoutedEventArgs e)
        {
            nodoActual = archivoConfiguracion.xmlSitios.Where(z => z.Attribute("id").Value == (sender as TreeViewItem).Tag.ToString()).FirstOrDefault();
            if (nodoActual != null)
            {
                txtNombre.Text = nodoActual.Attribute("name").Value.ToString();
                txtID.Text = string.Format("Id: {0}", nodoActual.Attribute("id").Value.ToString());
                txtPuerto.Text = nodoActual.Elements("bindings").Elements("binding").First().Attribute("bindingInformation").Value.ToString().Replace("*:", "").Replace(":localhost", "");
                var apps = nodoActual.Elements("application");

                aplicaciones = new List<XMLConfiguracion.Applicacion>();
                foreach (var app in apps)
                {
                    aplicaciones.Add(
                    new XMLConfiguracion.Applicacion()
                    {
                        Path = app.Attribute("path").Value.ToString(),
                        PhysicalPath = app.Elements("virtualDirectory").First().Attribute("physicalPath").Value.ToString()
                    });
                }

                dtApps.ItemsSource = aplicaciones;
            }
        }

        private void VerConfig(Boolean mostrar)
        {
            if (mostrar)
            {
                pnlConfig.Visibility = System.Windows.Visibility.Visible;
                txtNewAppDir.Text = "";
                txtNewAppPath.Text = "";
                Efectos.AbrirPanel(pnlConfig, CONFIG_MIN, CONFIG_MAX);
            }
            else
            {
                //pnlConfig.Visibility = System.Windows.Visibility.Hidden;
                Efectos.CerrarPanel(pnlConfig, CONFIG_MIN, CONFIG_MAX);
            }
        }

        private void btnNuevaApp_Click(object sender, RoutedEventArgs e)
        {
            VerConfig(true);
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (nodoActual != null)
            {
                archivoConfiguracion.GuardarAplicacionesSitio( txtID.Text.Replace("Id: ",""), 
                                                               txtNombre.Text, 
                                                               txtPuerto.Text, 
                                                               aplicaciones);
                CargarData();
            }
        }

        #region Panel de configuracion
        private void btnConfigOK_Click(object sender, RoutedEventArgs e)
        {
            //guardamos lo seleccionado como nueva app en el sitio actual
            if (nodoActual != null)
            {
                
                aplicaciones.Add(new XMLConfiguracion.Applicacion()
                    {
                        Path = txtNewAppPath.Text,
                        PhysicalPath = txtNewAppDir.Text
                    });
                dtApps.ItemsSource = null;
                dtApps.ItemsSource = aplicaciones;
            }
            VerConfig(false);
        }

        private void btnConfigCancel_Click(object sender, RoutedEventArgs e)
        {
            VerConfig(false);
        }

        private void btnBuscarDestino_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new System.Windows.Forms.FolderBrowserDialog();
            dialogo.RootFolder = Environment.SpecialFolder.MyComputer;
            dialogo.ShowNewFolderButton = true;
            var resultado =  dialogo.ShowDialog();

            if (resultado == System.Windows.Forms.DialogResult.OK)
            {
                txtNewAppDir.Text = dialogo.SelectedPath;
            }
        }
        #endregion

        private void imgEjecutar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (nodoActual != null)
            {
                System.Diagnostics.Process.Start(RUTA_IIS_EXPRESS, string.Format("/siteid:{0}", txtID.Text.Replace("Id: ", "")));
            }
        }

        private void btnAddSite_Click(object sender, RoutedEventArgs e)
        {
            archivoConfiguracion.AgregarNuevoSitio("Nuevo Sitio", new List<XMLConfiguracion.Applicacion>());
            CargarData();
        }

        private void btnDelSite_Click(object sender, RoutedEventArgs e)
        {
            if (nodoActual != null)
            {
                archivoConfiguracion.EliminarSitio(txtID.Text.Replace("Id: ", ""));
                CargarData();
            }
        }
    }
}
