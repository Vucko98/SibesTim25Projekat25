using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace AccountManagementServiceHelper
{
    public class AMS_XML : AMS
    {
        public override void Write(string dogadjaj)
        { 
            lock(resourceLock)
            {
                if (!File.Exists("xmlTempLog.xml"))
                {
                    XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                    xmlWriterSettings.Indent = true;
                    xmlWriterSettings.NewLineOnAttributes = true;
                    using (XmlWriter xmlWriter = XmlWriter.Create("xmlTempLog.xml", xmlWriterSettings))
                    {
                        xmlWriter.WriteStartDocument();
                        xmlWriter.WriteStartElement("Dogadjaji");

                        xmlWriter.WriteElementString("Dogadjaj", dogadjaj);

                        xmlWriter.WriteEndElement();
                        xmlWriter.WriteEndDocument();
                        xmlWriter.Flush();
                        xmlWriter.Close();
                    }
                }
                else
                {
                    XDocument xDocument = XDocument.Load("xmlTempLog.xml");
                    XElement root = xDocument.Element("Dogadjaji");
                    IEnumerable<XElement> rows = root.Descendants("Dogadjaj");
                    XElement firstRow = rows.First();
                    firstRow.AddBeforeSelf(new XElement("Dogadjaj", dogadjaj));
                    xDocument.Save("xmlTempLog.xml");
                }
            }
        }

        public override string[] Read()
        {
            List<string> lines = new List<string>();            
            
            lock(resourceLock)
            {
                if(File.Exists("xmlTempLog.xml"))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load("xmlTempLog.xml");

                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                    {
                        lines.Add(node.InnerText);

                        if (!File.Exists("xmlLog.xml"))
                        {
                            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                            xmlWriterSettings.Indent = true;
                            xmlWriterSettings.NewLineOnAttributes = true;
                            using (XmlWriter xmlWriter = XmlWriter.Create("xmlLog.xml", xmlWriterSettings))
                            {
                                xmlWriter.WriteStartDocument();
                                xmlWriter.WriteStartElement("Dogadjaji");

                                xmlWriter.WriteElementString("Dogadjaj", node.InnerText);

                                xmlWriter.WriteEndElement();
                                xmlWriter.WriteEndDocument();
                                xmlWriter.Flush();
                                xmlWriter.Close();
                            }
                        }
                        else
                        {
                            XDocument xDocument = XDocument.Load("xmlLog.xml");
                            XElement root = xDocument.Element("Dogadjaji");
                            IEnumerable<XElement> rows = root.Descendants("Dogadjaj");
                            XElement firstRow = rows.First();
                            firstRow.AddBeforeSelf(new XElement("Dogadjaj", node.InnerText));
                            xDocument.Save("xmlLog.xml");
                        }

                    }
                    File.Delete("xmlTempLog.xml");
                }                
            }                   
            return lines.ToArray();            
        }
    }
}
