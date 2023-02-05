using NUnit.Framework;
using srd_AutoExtractor.Classes;
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
            Extraction o = new(this.testFile1);

            Assert.DoesNotThrow(() => { o.Extract(); });
            Assert.AreEqual(3, o.ExtractedFilecount);

            o = new(this.testFile1, Path.GetDirectoryName(this.testFile1));

            this.TearDown();

            Assert.DoesNotThrow(() => { o.Extract(); });
            Assert.AreEqual(3, o.ExtractedFilecount);
        }

        [Test]
        public void ExtractTarGZFile()
        {
            Extraction o = new(this.testFile2);

            Assert.DoesNotThrow(() => { o.Extract(); });
            Assert.AreEqual(39, o.ExtractedFilecount);

            o = new(this.testFile2, Path.GetDirectoryName(this.testFile2));

            this.TearDown();

            Assert.DoesNotThrow(() => { o.Extract(); });
            Assert.AreEqual(39, o.ExtractedFilecount);

            this.TearDown();

            Assert.DoesNotThrowAsync(() => o.ExtractAsync() );
            Assert.AreEqual(39, o.ExtractedFilecount);
        }

        [Test]
        public void Extract7ZFile()
        {
            Extraction o = new(this.testFile3);

            Assert.DoesNotThrow(() => { o.Extract(); });
            Assert.AreEqual(3, o.ExtractedFilecount);

            o = new(this.testFile3, Path.GetDirectoryName(this.testFile3));

            this.TearDown();

            Assert.DoesNotThrow(() => { o.Extract(); });
            Assert.AreEqual(3, o.ExtractedFilecount);
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