using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KingQuestionProxy
{
    public class PacConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("proxy")]
        public PacCollection proxy
        {
            get
            {
                return (PacCollection)this["proxy"];
            }
        }
    }

    public class PacItem : ConfigurationElement
    {
        [ConfigurationProperty("host")]
        public string host
        {
            get
            {
                return (string)this["host"];
            }
        }
    }

    public class PacCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PacItem();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var ele = element as PacItem;
            return ele.host;
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        protected override string ElementName
        {
            get
            {
                return "add";
            }
        }
    }
}
