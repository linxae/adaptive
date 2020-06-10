using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using AdaptiveProvider.Core.Configuration;
using System.Text.Json;
using System;

namespace AdaptiveProvider.TowerAnsible.Tests
{
    [TestClass]
    public class TowerServiceTests
    {
        private string _url = "https://ec2-18-216-9-148.us-east-2.compute.amazonaws.com/";
        private NetworkCredential _basicCredential = new NetworkCredential("admin", "azerty*123");
        private NetworkCredential _oAuthCredential = new NetworkCredential("", "fULfaR8a4j***********Ug2mmo8m");

        [TestMethod]
        public void ConnectionStringParsingTest()
        {
            var cs = "Basic>user#AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAbk5J32RSikGwAM93LvNf8QAAAAACAAAAAAAQZgAAAAEAACAAAACDsmBm5vm1JUd+z1753UkJiA0os7FJ+2rJkw/IKrI1bAAAAAAOgAAAAAIAACAAAACjMnQ4/m7E2lBuZjobSoim4O2xWzR7+K75obT8gAaDJxAAAADh/WaEirERc0d/8SzK2ejTQAAAANb3PbheyvToCjkqg3y08IgLcZx2ZP9x6wMTH9puqIiHNilKNuQLRxSHJ/wcN9GtCl+nMyw6ya+5hcCzpF+4zr8=@https://ec2-18-222-136-247.us-east-2.compute.amazonaws.com/!y";

            Assert.AreEqual(AuthenticationMethod.Basic, cs.AuthenticationMethod());
            Assert.AreEqual("user", cs.Credential().UserName);
            Assert.AreEqual("pasword", cs.Credential().Password.Decrypt());
            Assert.AreEqual("https://ec2-18-222-136-247.us-east-2.compute.amazonaws.com/", cs.Url());
            Assert.AreEqual(true, cs.SkipCertificateCheck());
        }

        [TestMethod]
        public void EncryptDecryptTest()
        {
            var cs = "user+pasword";

            var ev = cs.Encrypt();
            Assert.AreNotEqual(cs, ev);

            var dv = ev.Decrypt();
            Assert.AreEqual(cs, dv);
        }

        [TestMethod]
        public void EnsureAuthenticatedBasicTest()
        {
            using (var ts = new TowerService(_url, _basicCredential, skipCertificateValidation: true))
            {
                ts.EnsureAuthenticated().Wait();
            }
        }


        [TestMethod]
        public void EnsureAuthenticatedConnectionStringTest()
        {
            var cs = "Basic>admin#AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAbk5J32RSikGwAM93LvNf8QAAAAACAAAAAAAQZgAAAAEAACAAAAD4vmBqwwVwlxOq1BHexwYEfiPWkVoHkpkj7Fgwq+qA0AAAAAAOgAAAAAIAACAAAADtIUySFouq3oZ0gRTq3UL320zlEfOZiZHiUAl+QgEdgCAAAABlUkwN2TMBILTl8FUSsCbgV9XMeX57/Uofuuc0XaDzR0AAAAB0IhQBiDbVkf5QMI7ioe1pp6M0YqyYoUfhSGFxqOnqxFql/KheSIgd2F1lzoEg0XySY62iKQSx1Wsi6X8VWVGC@https://ec2-18-222-136-247.us-east-2.compute.amazonaws.com/!y";

            using (var ts = new TowerService(cs))
            {
                ts.EnsureAuthenticated().Wait();
            }
        }

        [TestMethod]
        public void EnsureAuthenticatedOAuthTest()
        {
            using (var ts = new TowerService(_url, _oAuthCredential.Password, skipCertificateValidation: true))
            {
                ts.EnsureAuthenticated().Wait();
            }
        }

        [TestMethod]
        public void LaunchJobTemplateTest()
        {
            using (var ts = new TowerService(_url, _basicCredential, skipCertificateValidation: true))
            {
                var job = ts.LaunchJobTemplate(7);

                Assert.IsFalse(job.Failed);
            }
        }

        [TestMethod]
        public void LaunchJobTemplateWithExtraVarsTest()
        {
            using (var ts = new TowerService(_url, _basicCredential, skipCertificateValidation: true))
            {
                var job = ts.LaunchJobTemplate(8, new {user_name = "VS" });

                Assert.IsFalse(job.Failed);
            }
        }

        [TestMethod]
        public void SerializeJobLaunch()
        {
            var payload = new JobLaunch() {
                ExtraVars = new { UserName = "admin" }
            };

            var FilledContent = JsonSerializer.Serialize<JobLaunch>(payload);

            Console.WriteLine("Filled Variables:");
            Console.WriteLine(FilledContent);

            var emptyPayload = new JobLaunch();

            var emptyContent = JsonSerializer.Serialize<JobLaunch>(emptyPayload);

            Console.WriteLine("Empty Variables:");
            Console.WriteLine(emptyContent);
        }
    }
}
