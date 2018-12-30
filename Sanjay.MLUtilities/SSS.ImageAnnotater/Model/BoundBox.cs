using System;
using System.Xml.Serialization;

namespace SSS.ImageAnnotater.Model
{
    [Serializable]
    public class BoundBox
    {
        [XmlElement(elementName: "xmin")]
        public double XMin { get; set; }

        [XmlElement(elementName: "ymin")]
        public double YMin { get; set; }

        [XmlElement(elementName: "xmax")]
        public double XMax{ get; set; }

        [XmlElement(elementName: "ymax")]
        public double YMax { get; set; }
    }
}
