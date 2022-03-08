using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace VZTMXMapLoader
{
    public class TMXMapLoader
    {
        string xmlFile;

        public TMXMapLoader(string xmlFile)
        {
            this.xmlFile = xmlFile;
        }

        public TiledMap GetObjectMap()
        {
            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "map";

            XmlSerializer serializer = new XmlSerializer(typeof(TiledMap), xRoot);

            using (Stream reader = new FileStream("..\\..\\..\\Content\\stage1.tmx", FileMode.Open))
            {
                TiledMap m = (TiledMap)serializer.Deserialize(reader);
                m.layer.CVStoList();

                return m;
            }

        }
    }

    public class TiledMap
    {
        [XmlAttribute("version")]
        public float version { get; set; }
        [XmlAttribute("tiledversion")]
        public string tiledversion { get; set; }
        [XmlAttribute("orientation")]
        public string orientation { get; set; }
        [XmlAttribute("renderorder")]
        public string renderorder { get; set; }
        [XmlAttribute("width")]
        public int width { get; set; }
        [XmlAttribute("height")]
        public int height { get; set; }
        [XmlAttribute("tilewidth")]
        public int tilewidth { get; set; }
        [XmlAttribute("tileheight")]
        public int tileheight { get; set; }
        [XmlAttribute("infinite")]
        public int infinite { get; set; }
        [XmlAttribute("nextlayerid")]
        public int nextlayerid { get; set; }
        [XmlAttribute("nextobjectid")]
        public int nextobjectid { get; set; }

        [XmlElement("tileset")]
        public TileSet tileset { get; set; }
        [XmlElement("layer")]
        public Layer layer { get; set; }
        [XmlElement("objectgroup")]
        public List<ObjectGroup> objectGroups { get; set; }
    }

    public class TileSet
    {
        [XmlAttribute("firstgid")]
        public int firstgid { get; set; }
        [XmlAttribute("source")]
        public string source { get; set; }
    }

    public class Layer
    {
        [XmlAttribute("id")]
        public int id { get; set; }
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlAttribute("width")]
        public int width { get; set; }
        [XmlAttribute("height")]
        public int height { get; set; }
        [XmlElement("data")]
        public string value { get; set; }

        public List<int> map;

        public void CVStoList()
        {
            map = new List<int>(value.Split(',').Select(int.Parse));
            value = null;
        }
    }

    public class ObjectGroup
    {
        [XmlAttribute("id")]
        public int id { get; set; }
        [XmlAttribute("name")]
        public string name { get; set; }
        [XmlElement(ElementName ="object")]
        public List<ObjType> objects { get; set; }
    }

    public class ObjType
    {
        [XmlAttribute("id")]
        public int id { get; set; }
        [XmlAttribute("type")]
        public int type { get; set; }
        [XmlAttribute("x")]
        public float x { get; set; }
        [XmlAttribute("y")]
        public float y { get; set; }
        [XmlAttribute("width")]
        public float width { get; set; }
        [XmlAttribute("height")]
        public float height { get; set; }
        public bool ToDelete { get; set; }
    }
}
