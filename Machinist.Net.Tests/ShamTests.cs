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
            Sham.Generate("CoinToss", () => Rand.Next(2));
            Sham.Generate("NotUniqueCoinToss", () => Rand.Next(2), unique: false);

            Blueprint<User>().WithShams();
            Blueprint<User>("NoShams");
            Blueprint<Post>(x =>
            {
                x.Title = Sham.Get.CatchPhrase;
            }).WithShams();
            Blueprint<User>("Unique", x =>
            {
                x.Username = Sham.Get.CoinToss.ToString();
            });
            Blueprint<User>("NotUnique", x =>
            {
                x.Username = Sham.Get.NotUniqueCoinToss.ToString();
            });
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
            Assert.AreEqual("Kayley", blueprints.Make<User>().Username);
            Assert.AreEqual("Miguel", blueprints.Make<User>().Username);
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

        [TestMethod]
        public void UniqueByDefault()
        {
            Assert.AreEqual("0", blueprints.Make<User>("Unique").Username);
            Assert.AreEqual("1", blueprints.Make<User>("Unique").Username);
        }

        [TestMethod]
        public void NotUnique()
        {
            Assert.AreEqual("0", blueprints.Make<User>("NotUnique").Username);
            Assert.AreEqual("0", blueprints.Make<User>("NotUnique").Username);
            Assert.AreEqual("0", blueprints.Make<User>("NotUnique").Username);
            Assert.AreEqual("1", blueprints.Make<User>("NotUnique").Username);
        }

        [ExpectedException(typeof(ApplicationException))]
        [TestMethod]
        public void NoMoreUniqueValues()
        {
            blueprints.Make<User>("Unique");
            blueprints.Make<User>("Unique");
            blueprints.Make<User>("Unique");
        }
    }
}
