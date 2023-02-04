using NUnit.Framework;
using nxn_AutoExtractor.Classes;
using nxn_AutoExtractor.Models;
using System.IO;
using System.Reflection;

namespace UnitTesting
{
    public class Tests
    {
        string testFile1 = "test1.zip";
        string testFile2 = "test2.tar.gz";
        string testFile3 = "test3.7z";

        [SetUp]
        public void SetUp()
        {
            string s = Assembly.GetExecutingAssembly().Location;
            string ss = Path.Combine(s.Substring(0, s.IndexOf(Assembly.GetExecutingAssembly().GetName().Name)), "NUnitTestingFiles");

            this.testFile1 = Path.Combine(ss, this.testFile1);
            this.testFile2 = Path.Combine(ss, this.testFile2);
            this.testFile3 = Path.Combine(ss, this.testFile3);
        }

        [Test]
        public void ExtractZipFile()
        {
            CompressedFile compressedFile = new()
            {
                FileInfo = new FileInfo(this.testFile1)
            };

            Extraction o = new(compressedFile);

            Assert.DoesNotThrow(() => { o.Extract(); });
            Assert.IsTrue(compressedFile.WasSuccessful);
            Assert.AreEqual(3, compressedFile.EntriesFile);
            Assert.AreEqual(0, compressedFile.EntriesDirectory);
        }

        [Test]
        [Ignore("Something wrong here")]
        public void ExtractTarGZFile()
        {
            CompressedFile compressedFile = new()
            {
                FileInfo = new FileInfo(this.testFile2)
            };

            Extraction o = new(compressedFile);

            Assert.DoesNotThrow(() => { o.Extract(); });
            Assert.IsTrue(compressedFile.WasSuccessful);
            Assert.AreEqual(3, compressedFile.EntriesFile);
            Assert.AreEqual(0, compressedFile.EntriesDirectory);
        }

        [Test]
        public void Extract7ZFile()
        {
            CompressedFile compressedFile = new()
            {
                FileInfo = new FileInfo(this.testFile3)
            };

            Extraction o = new(compressedFile);

            Assert.DoesNotThrow(() => { o.Extract(); });
            Assert.IsTrue(compressedFile.WasSuccessful);
            Assert.AreEqual(3, compressedFile.EntriesFile);
            Assert.AreEqual(0, compressedFile.EntriesDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (string d in Directory.GetDirectories(Path.GetDirectoryName(this.testFile1)))
            {
                Directory.Delete(d, true);
            }
        }
    }
}