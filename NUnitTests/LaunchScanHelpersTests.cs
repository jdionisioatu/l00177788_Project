using System.Runtime.CompilerServices;
using CodeScanning.Controllers;

namespace NUnitTests
{
    public class LaunchScanHelpersTests
    {
        private Stream _stream;
        private string _randomString;
        private Random _random;

        [SetUp]
        public void Setup()
        {
            _stream = LaunchScanHelpers.CreateTarballForDockerfileDirectory(".");
            _random = new Random();
            _randomString = LaunchScanHelpers.RandomString(_random, "ABCDEFG", (1, 3));
        }

        [Test]
        public void TestIfTarFileHasContents()
        {
            Assert.That(_stream.Length, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void TestRandomStringThreeChars()
        {
            Assert.That(_randomString, Has.Length.EqualTo(3));
        }

        [Test]
        public void TestRandomStringOneChar()
        {
            _randomString = LaunchScanHelpers.RandomString(_random, "ABCDEFG", (1, 1));
            Assert.That(_randomString, Has.Length.EqualTo(1));
        }

        [Test]
        public void TestRandomStringFiveChars()
        {
            _randomString = LaunchScanHelpers.RandomString(_random, "ABCDEFG", (5, 5));
            Assert.That(_randomString, Has.Length.EqualTo(5));
        }

        [Test]
        public void TestRandomStringTenChars()
        {
            _randomString = LaunchScanHelpers.RandomString(_random, "ABCDEFG", (10, 10));
            Assert.That(_randomString, Has.Length.EqualTo(10));
        }

        [Test]
        public void TestRandomStringFifteenChars()
        {
            _randomString = LaunchScanHelpers.RandomString(_random, "ABCDEFG", (15, 15));
            Assert.That(_randomString, Has.Length.EqualTo(15));
        }


        [TearDown]
        public void TearDown()
        {
            _stream.Dispose();
        }
    }
}