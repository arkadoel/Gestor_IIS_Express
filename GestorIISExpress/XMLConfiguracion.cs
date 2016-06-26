using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace GestorIISExpress
{
    /// <summary>
    /// Control del archivo de configuracion y de su gestion
    /// </summary>
    public class XMLConfiguracion
    {
        private static string RUTA_XML_CONFIGURACION = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"IISExpress\config\applicationhost.config");
        private XDocument xmlFile = null;
        public List<XElement> xmlSitios { get; set; }


        public void CargarXML()
        {
            try
            {
                xmlFile = XDocument.Load(RUTA_XML_CONFIGURACION);
                var sitios = xmlFile.Elements("configuration").Elements("system.applicationHost").Elements("sites").Elements("site");

                if (sitios != null)
                {
                    xmlSitios = sitios.ToList();
                }

                Console.WriteLine("Se han cargado los sitios");
            }
            catch (Exception ex)
            {
                showError(ex);
            }
        }

        public void GuardarAplicacionesSitio(string id, string nombre, string puerto, List<Applicacion> aplicaciones)
        {
            try
            {
                xmlFile = XDocument.Load(RUTA_XML_CONFIGURACION);
                var sitio = xmlFile.Elements("configuration")
                                    .Elements("system.applicationHost")
                                    .Elements("sites")
                                    .Elements("site").Where(z => z.Attribute("id").Value == id).FirstOrDefault();

                sitio.Attribute("name").SetValue(nombre);
                //sitio.Elements("application").Remove();
                sitio.Elements().Remove();

                foreach (Applicacion app in aplicaciones)
                {
                    XElement xmlapp = new XElement("application");
                    xmlapp.SetAttributeValue("path", app.Path);
                    xmlapp.SetAttributeValue("applicationPool", "Clr4IntegratedAppPool");

                    XElement virtualDirectory = new XElement("virtualDirectory");
                    virtualDirectory.SetAttributeValue("path", "/");
                    virtualDirectory.SetAttributeValue("physicalPath", app.PhysicalPath);
                    xmlapp.Add(virtualDirectory);
                    sitio.Add(xmlapp);
                }

                XElement binding = new XElement("binding");
                binding.SetAttributeValue("protocol", "http");
                binding.SetAttributeValue("bindingInformation", string.Format("*:{0}:localhost", puerto));
                sitio.Add(new XElement("bindings", binding));

                xmlFile.Save(RUTA_XML_CONFIGURACION);
                MessageBox.Show("Se ha guardado con exito", "Guardado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                showError(ex);
            }
        }

        public void EliminarSitio(string id)
        {
            try {
                xmlFile = XDocument.Load(RUTA_XML_CONFIGURACION);
                var sitio = xmlFile.Elements("configuration")
                                    .Elements("system.applicationHost")
                                    .Elements("sites")
                                    .Elements("site").Where(z => z.Attribute("id").Value == id).FirstOrDefault();

                sitio.Remove();
                xmlFile.Save(RUTA_XML_CONFIGURACION);
                MessageBox.Show("Se ha guardado con exito", "Guardado", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                showError(ex);
            }
        }

        public string GenerarID_sitio(List<int> ids)
        {
            ids.Sort();
            int siguiente = ids.Last() + 1;
            return siguiente.ToString();
        }

        public void AgregarNuevoSitio(string nombre, List<Applicacion> aplicaciones)
        {
            try {
                xmlFile = XDocument.Load(RUTA_XML_CONFIGURACION);
                var sitios = xmlFile.Elements("configuration")
                                    .Elements("system.applicationHost")
                                    .Elements("sites").First();
                var ids = xmlFile.Elements("configuration")
                                    .Elements("system.applicationHost")
                                    .Elements("sites").Elements("site").Select(z => Convert.ToInt32( z.Attribute("id").Value)).ToList();

                string nuevoID = GenerarID_sitio(ids);

                XElement nuevoSitio = new XElement("site");
                nuevoSitio.SetAttributeValue("name", nombre);
                nuevoSitio.SetAttributeValue("id", nuevoID);
                sitios.Add(nuevoSitio);
                xmlFile.Save(RUTA_XML_CONFIGURACION);

                GuardarAplicacionesSitio(nuevoID, nombre, "1221", aplicaciones);
            }
            catch (Exception ex)
            {
                showError(ex);
            }
        }

        private void showError(Exception ex)
        {
            MessageBox.Show("Se ha producido un error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public class Applicacion
        {
            public string Path { get; set; }
            public string PhysicalPath { get; set; }
        }

    }
}
