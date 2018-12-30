using System;
using System.Xml.Serialization;

namespace SSS.ImageAnnotater.Model
{
    [Serializable]
    public class ImageMetadata
    {
        [XmlElement(elementName:"folder")]
        public string Folder { get; set; }

        [XmlElement(elementName: "path")]
        public string Path { get; set; }

        [XmlElement(elementName: "filename")]
        public string Filename { get; set; }

        [XmlElement(elementName: "size")]
        public ImageSize Size { get; set; }

        [XmlElement(elementName: "object")]
        public DetectionObject DetectionObject { get; set; }
        //public List<DetectionObject> DetectionObjects { get; set; }
    }
}
