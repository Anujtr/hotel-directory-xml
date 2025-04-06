using System;
using System.Xml;
using System.Xml.Schema;
using Newtonsoft.Json;
using System.Net;
using System.IO;

namespace ConsoleApp1
{
    public class Program
    {
        // Replace these URLs with your actual raw GitHub file links
        public static string xmlURL = "https://github.com/Anujtr/hotel-directory-xml/raw/refs/heads/main/Hotels.xml";
        public static string xmlErrorURL = "https://github.com/Anujtr/hotel-directory-xml/raw/refs/heads/main/HotelsErrors.xml";
        public static string xsdURL = "https://github.com/Anujtr/hotel-directory-xml/raw/refs/heads/main/Hotels.xsd";

        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;

                // Load schema from URL
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                using (WebClient client = new WebClient())
                {
                    string xsdContent = client.DownloadString(xsdUrl);
                    using (StringReader sr = new StringReader(xsdContent))
                    {
                        schemaSet.Add(null, XmlReader.Create(sr));
                    }
                }

                settings.Schemas = schemaSet;

                string errorMessage = "No Error";
                settings.ValidationEventHandler += (sender, e) =>
                {
                    errorMessage = e.Message;
                };

                // Load XML from URL
                using (WebClient client = new WebClient())
                {
                    string xmlContent = client.DownloadString(xmlUrl);
                    using (StringReader sr = new StringReader(xmlContent))
                    {
                        using (XmlReader reader = XmlReader.Create(sr, settings))
                        {
                            while (reader.Read()) { }
                        }
                    }
                }

                return errorMessage;
            }
            catch (Exception ex)
            {
                return "Exception occurred: " + ex.Message;
            }
        }

        // Q2.2
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string xmlContent = client.DownloadString(xmlUrl);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlContent);

                    string jsonText = JsonConvert.SerializeXmlNode(xmlDoc, Newtonsoft.Json.Formatting.Indented, true);

                    return jsonText;
                }
            }
            catch (Exception ex)
            {
                return "Conversion Error: " + ex.Message;
            }
        }
    }
}
