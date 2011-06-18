using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Machinist.Net.Tests.Models.ActiveRecord;

namespace Machinist.Net.Tests
{
    public class ShamBlueprints : BlueprintDefinition
    {
        public ShamBlueprints()
        {
            Sham.Generate("CatchPhrase", Faker.Company.CatchPhrase);
            Sham.Match("Username", Faker.Name.FirstName);
            Sham.Match(".ody", () => "String");

            Blueprint<User>().WithShams();
            Blueprint<User>("NoShams");
            Blueprint<Post>(x =>
            {
                x.Title = Sham.Get.CatchPhrase;
            }).WithShams();
        }
    }

    [TestClass]
    public class ShamTests
    {
        private Blueprint blueprints;

        [TestInitialize]
        public void TestInitialize()
        {
            blueprints = new Blueprint(new ShamBlueprints());
        }

        [TestMethod]
        public void NoShamsByDefault()
        {
            Assert.IsNull(blueprints.Make<User>("NoShams").Username);
        }

        [TestMethod]
        public void BasicSham()
        {
            Assert.AreEqual("Delpha", blueprints.Make<User>().Username);
            Assert.AreEqual("Bernita", blueprints.Make<User>().Username);
        }

        [TestMethod]
        public void Regex()
        {
            Assert.AreEqual("String", blueprints.Make<Post>().Body);
        }

        [TestMethod]
        public void UseShamsInBlueprint()
        {
            Assert.AreEqual("Ergonomic background implementation", blueprints.Make<Post>().Title);
        }
    }
}
