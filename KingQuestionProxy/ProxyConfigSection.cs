using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace KingQuestionProxy
{
    /// <summary>
    /// 代理设置
    /// </summary>
    public class ProxyConfigSection : ConfigurationSection
    {
        /// <summary>
        /// 自动代理设置
        /// </summary>
        [ConfigurationProperty("pac")]
        public PacCollection PAC
        {
            get
            {
                return (PacCollection)this["pac"];
            }
        }

        /// <summary>
        /// 是否仅代理pac列表的域名
        /// </summary>
        [ConfigurationProperty("proxyPacOnly")]
        public bool ProxyPacOnly
        {
            get
            {
                return (bool)this["proxyPacOnly"];
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
