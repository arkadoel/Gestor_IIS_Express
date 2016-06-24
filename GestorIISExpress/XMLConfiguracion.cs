using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            xmlFile = XDocument.Load(RUTA_XML_CONFIGURACION);
            var sitios = xmlFile.Elements("configuration").Elements("system.applicationHost").Elements("sites").Elements("site");

            if (sitios != null)
            {
                xmlSitios = sitios.ToList();
            }

            Console.WriteLine("Se han cargado los sitios");
            /*var query = from c in xmlFile.Elements("catalog").Elements("book")
                        select c;
            foreach (XElement book in query)
            {
                book.Attribute("attr1").Value = "MyNewValue";
            }
            xmlFile.Save("books.xml");*/      
        }

        public class Applicacion
        {
            public string Path { get; set; }
            public string PhysicalPath { get; set; }
        }

    }
}
