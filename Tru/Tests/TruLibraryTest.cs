using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tru;
using Utility;

namespace Tests
{
    [TestClass]
    public class TruLibraryTest
    {
        [TestMethod]
        public void TestGetLibrary() {
            Assert.IsFalse( object.ReferenceEquals(TruLibrary.Library, TruLibrary.Library) ); // TruLibrary copies the Environment each time.
        }

        [TestMethod]
        public void TestBuiltIns() {
            Assert.AreEqual( TruStatement.Interpret("{nand true true}"),   new TruBool(false));
            Assert.AreEqual( TruStatement.Interpret("{and  true true}"),   new TruBool(true));
            Assert.AreEqual( TruStatement.Interpret("{or   false false}"), new TruBool(false));
            Assert.AreEqual( TruStatement.Interpret("{not  true}"),        new TruBool(false));
            Assert.AreEqual( TruStatement.Interpret("{equals true true}"), new TruBool(true));
            Assert.AreEqual( TruStatement.Interpret("{equals {lambda {} true} {lambda {} true}}"), new TruBool(false));
            Assert.AreEqual( TruStatement.Interpret("{let {[f {lambda {} true}]} {equals f f}}"), new TruBool(true));
            Assert.AreEqual( TruStatement.Interpret("{if true true false}"), new TruBool(true));

            Assert.AreEqual( TruStatement.Interpret("{and  false {lambda {} true}}"), new TruBool(false)); // Short circuit evaluation
            Assert.ThrowsException<TruRuntimeException>( () => TruStatement.Interpret("{and  true {lambda {} true}}") );
        }

        [TestMethod]
        public void TestLibrary() {
            Assert.AreEqual( TruStatement.Interpret("{nor  true true}"),   new TruBool(false));
            Assert.AreEqual( TruStatement.Interpret("{xor  true true}"),   new TruBool(false));
            Assert.AreEqual( TruStatement.Interpret("{xnor false false}"), new TruBool(true));
            Assert.AreEqual( TruStatement.Interpret("{implies true false}"), new TruBool(false));
            Assert.AreEqual( TruStatement.Interpret("{majority true false false}"), new TruBool(false));
        }
    }
}