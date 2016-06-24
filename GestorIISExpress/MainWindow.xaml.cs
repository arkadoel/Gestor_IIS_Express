
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            archivoConfiguracion = new XMLConfiguracion();
            archivoConfiguracion.CargarXML();

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
            var nodo = archivoConfiguracion.xmlSitios.Where(z => z.Attribute("id").Value == (sender as TreeViewItem).Tag.ToString()).FirstOrDefault();
            if (nodo != null)
            {
                txtNombre.Text = nodo.Attribute("name").Value.ToString();
                txtID.Text = string.Format("Id: {0}", nodo.Attribute("id").Value.ToString());
                txtPuerto.Text = nodo.Elements("bindings").Elements("binding").First().Attribute("bindingInformation").Value.ToString().Replace("*:","").Replace(":localhost","");
                var apps = nodo.Elements("application");

                List<XMLConfiguracion.Applicacion> aplicaciones = new List<XMLConfiguracion.Applicacion>();
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
    }
}
