using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ComputingSystemSimulation
{
    public class CompSystemParams
    {
        public int coresCount { get; private set; }
        public int reserveCoresCount { get; private set; }
        public int memoryCount { get; private set; }
        public double maxTaskWorkTime { get; private set; }
        public double maxFreeMemoryTime { get; private set; }

        public CompSystemParams() { }

        public bool ReadParamsFromXMLFile()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("../../CompSystemParams.xml");
                coresCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[0].InnerText);
                memoryCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[1].InnerText);
                maxTaskWorkTime = Convert.ToDouble(doc.DocumentElement.ChildNodes[2].InnerText);
                maxFreeMemoryTime = Convert.ToDouble(doc.DocumentElement.ChildNodes[3].InnerText);
                reserveCoresCount = Convert.ToInt32(doc.DocumentElement.ChildNodes[4].InnerText);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        } 
    }
}
