using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Configuration;

namespace ConfigurationTests {
    [TestFixture]
    public class ConfigurationPersisterTest {
        [TestCase]
        public void PersistGetTokenResponse() {
            ConfigurationPersister.PersistGetTokenResponse("a", "b", "c", "d");
        }
    }
}
