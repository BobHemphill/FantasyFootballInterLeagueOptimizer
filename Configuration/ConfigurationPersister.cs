using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Configuration;

namespace Configuration {
    public static class ConfigurationPersister {
        public static void PersistGetTokenResponse(string token, string tokenSecret, string session, string yahooId) {
            var xDoc = XDocument.Load("user.config");
            var appSettings = xDoc.Element("appSettings");
            if (appSettings == null) {
                appSettings = new XElement("appSettings");
            }

            var adds = appSettings.Elements("add");
            PersistAddNodeWithKeyAttribute(appSettings, adds, Settings.TokenKeyKey, token);
            PersistAddNodeWithKeyAttribute(appSettings, adds, Settings.TokenSecretKey, tokenSecret);
            PersistAddNodeWithKeyAttribute(appSettings, adds, Settings.SessionHandleKey, session);
            PersistAddNodeWithKeyAttribute(appSettings, adds, Settings.YahooIdKey, yahooId);

            xDoc.Save("user.config");
            ConfigurationManager.RefreshSection("appSettings");
        }

        static void PersistAddNodeWithKeyAttribute(XElement appSettings, IEnumerable<XElement> adds, string key, string value) {
            var tokenNode = adds.FirstOrDefault(e => HasKeyAttribute(e, key));
            if (tokenNode != null) {
                tokenNode.SetAttributeValue("value", value);
            }
            else {
                tokenNode = new XElement("add");
                tokenNode.SetAttributeValue("key", key);
                tokenNode.SetAttributeValue("value", value);
                appSettings.Add(tokenNode);
            }
        }

        static bool HasKeyAttribute(XElement element, string key) {
            var keys = element.Attributes("key");

            return (keys.FirstOrDefault(a => a.Value == key) != null);
        }
    }
}
