using System;
using System.Xml.Serialization;

namespace SSS.ImageAnnotater.Model
{
    [Serializable]
    public class DetectionObject
    {
        [XmlElement(ElementName ="name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "pose")]
        public string Pose { get; set; }

        [XmlElement(ElementName = "bndbox")]
        public BoundBox BoundedBox { get; set; }
    }
}
