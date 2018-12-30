using System;
using System.Xml.Serialization;

namespace SSS.ImageAnnotater.Model
{
    [Serializable]
    public class ImageSize
    {
        [XmlElement(elementName: "width")]
        public double Width { get; set; }

        [XmlElement(elementName: "height")]
        public double Height { get; set; }

        [XmlElement(elementName: "depth")]
        public double Depth { get; set; }
    }
}
