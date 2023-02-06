using NUnit.Framework;
using srdAutoExtractor.Logic;
using System.IO;
using System.Reflection;

namespace UnitTesting
{
    [TestFixture]
    public class HelperFunctionsTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void GetAssemblyTests()
        {
            Assert.That(HelperFunctions.GetAssemblyDirectory(), Is.EqualTo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
