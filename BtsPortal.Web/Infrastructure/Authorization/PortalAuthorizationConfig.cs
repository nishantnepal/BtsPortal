using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace BtsPortal.Web.Infrastructure.Authorization
{
    public class PortalAuthorizationConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("Area")]
        public string Area => base["Area"] as string;

        [ConfigurationProperty("Controller")]
        public string Controller => base["Controller"] as string;

        [ConfigurationProperty("Action")]
        public string Action => base["Action"] as string;

        [ConfigurationProperty("AllowedRoles")]
        public string AllowedRoles => base["AllowedRoles"] as string;

        [ConfigurationProperty("AllowedUsers")]
        public string AllowedUsers => base["AllowedUsers"] as string;
    }

    [ConfigurationCollection(typeof(PortalAuthorizationConfigElement), AddItemName = "PortalAuth", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class PortalAuthConfigCollection : ConfigurationElementCollection,IEnumerable<PortalAuthorizationConfigElement>
    {
        public PortalAuthorizationConfigElement this[int index]
        {
            get { return (PortalAuthorizationConfigElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(PortalAuthorizationConfigElement serviceConfig)
        {
            BaseAdd(serviceConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PortalAuthorizationConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PortalAuthorizationConfigElement)element);
        }

        public void Remove(PortalAuthorizationConfigElement serviceConfig)
        {
            BaseRemove(serviceConfig);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(String name)
        {
            BaseRemove(name);
        }

        public new IEnumerator<PortalAuthorizationConfigElement> GetEnumerator()
        {
            int count = base.Count;
            for (int i = 0; i < count; i++)
            {
                yield return base.BaseGet(i) as PortalAuthorizationConfigElement;
            }
        }
    }

    public class PortalAuthConfigSection : ConfigurationSection
    {
        public const string SECTION_NAME = "PortalAuthConfig";

        [ConfigurationProperty("PortalAuths")]
        public PortalAuthConfigCollection PortalAuths => base["PortalAuths"] as PortalAuthConfigCollection;
        
    }


}