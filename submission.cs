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
        // URLs to the XML data and XSD schema
        public static string xmlURL = "https://github.com/Anujtr/hotel-directory-xml/raw/refs/heads/main/Hotels.xml";
        public static string xmlErrorURL = "https://github.com/Anujtr/hotel-directory-xml/raw/refs/heads/main/HotelsErrors.xml";
        public static string xsdURL = "https://github.com/Anujtr/hotel-directory-xml/raw/refs/heads/main/Hotels.xsd";

        // Main method that runs the validation and conversion process
        public static void Main(string[] args)
        {
            // Validate XML against the XSD schema for correct format
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);

            // Validate faulty XML against the XSD schema
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);

            // Convert valid XML to JSON format
            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Method for validating XML against the XSD schema
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                // Load the XSD schema
                XmlSchemaSet schemas = new XmlSchemaSet();
                using (WebClient client = new WebClient())
                {
                    string xsdContent = client.DownloadString(xsdUrl);
                    using (StringReader sr = new StringReader(xsdContent))
                    {
                        schemas.Add(null, XmlReader.Create(sr));
                    }
                }

                // Set up XML reader settings for schema validation
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.Schemas = schemas;
                settings.ValidationType = ValidationType.Schema;

                string validationError = "No Error"; // Default validation result
                settings.ValidationEventHandler += (sender, e) =>
                {
                    // Capture validation error messages
                    validationError = e.Message;
                };

                // Read and validate the XML
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
                // Return any exceptions that occur during validation
                return "Exception occurred: " + ex.Message;
            }
        }

        // Method to convert XML content to JSON format
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                // Download XML content and convert to JSON
                using (WebClient client = new WebClient())
                {
                    string xmlContent = client.DownloadString(xmlUrl);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlContent);

                    string json = JsonConvert.SerializeXmlNode(doc); // Convert XML to JSON

                    // Test if JSON is valid by attempting to deserialize it
                    var test = JsonConvert.DeserializeXmlNode(json);

                    return json; // Return the converted JSON string
                }
            }
            catch (Exception ex)
            {
                // Return any errors during the conversion
                return "Conversion Error: " + ex.Message;
            }
        }
    }
}
