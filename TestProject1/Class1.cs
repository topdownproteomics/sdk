using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class Class1
    {
        #region Public Methods

        [Test]
        public static void TestFunction1()
        {
            Assert.AreEqual(1, TestLibNamespace.Class1.Return1());
        }

        #endregion Public Methods
    }
}