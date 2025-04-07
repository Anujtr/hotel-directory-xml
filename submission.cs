using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using System.Net;

namespace ConsoleApp1
{
    public class Program
    {
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

        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                XmlSchemaSet schemas = new XmlSchemaSet();
                using (WebClient client = new WebClient())
                {
                    string xsdContent = client.DownloadString(xsdUrl);
                    using (StringReader sr = new StringReader(xsdContent))
                    {
                        schemas.Add(null, XmlReader.Create(sr));
                    }
                }

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas = schemas;
                settings.ValidationType = ValidationType.Schema;

                string validationError = "No Error";
                settings.ValidationEventHandler += (sender, e) =>
                {
                    validationError = e.Message;
                };

                using (WebClient client = new WebClient())
                {
                    string xmlContent = client.DownloadString(xmlUrl);
                    using (StringReader sr = new StringReader(xmlContent))
                    using (XmlReader reader = XmlReader.Create(sr, settings))
                    {
                        while (reader.Read()) { }
                    }
                }

                return validationError;
            }
            catch (Exception ex)
            {
                return "Exception occurred: " + ex.Message;
            }
        }

        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string xmlContent = client.DownloadString(xmlUrl);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlContent);

                    string json = JsonConvert.SerializeXmlNode(doc);
                    // Test if it is deserializable
                    var test = JsonConvert.DeserializeXmlNode(json);

                    return json;
                }
            }
            catch (Exception ex)
            {
                return "Conversion Error: " + ex.Message;
            }
        }
    }
}
