using NUnit.Framework;
using Tru;
using Utility;

namespace Tests
{
    public class TruLibraryTest
    {
        [Test]
        public void TestGetLibrary() {
            Assert.False( object.ReferenceEquals(TruLibrary.Library, TruLibrary.Library) ); // TruLibrary copies the Environment each time.
        }

        [Test]
        public void TestBuiltIns() {
            Assert.That( TruStatement.Interpret("{nand true true}"),   Is.EqualTo(new TruBool(false)));
            Assert.That( TruStatement.Interpret("{and  true true}"),   Is.EqualTo(new TruBool(true)));
            Assert.That( TruStatement.Interpret("{or   false false}"), Is.EqualTo(new TruBool(false)));
            Assert.That( TruStatement.Interpret("{not  true}"),        Is.EqualTo(new TruBool(false)));
            Assert.That( TruStatement.Interpret("{equals true true}"), Is.EqualTo(new TruBool(true)));
            Assert.That( TruStatement.Interpret("{equals {lambda {} true} {lambda {} true}}"), Is.EqualTo(new TruBool(false)));
            Assert.That( TruStatement.Interpret("{let {[f {lambda {} true}]} {equals f f}}"), Is.EqualTo(new TruBool(true)));
            Assert.That( TruStatement.Interpret("{if true true false}"), Is.EqualTo(new TruBool(true)));
        }

        [Test]
        public void TestLibrary() {
            Assert.That( TruStatement.Interpret("{nor  true true}"),   Is.EqualTo(new TruBool(false)));
            Assert.That( TruStatement.Interpret("{xor  true true}"),   Is.EqualTo(new TruBool(false)));
            Assert.That( TruStatement.Interpret("{xnor false false}"), Is.EqualTo(new TruBool(true)));
            Assert.That( TruStatement.Interpret("{implies true false}"), Is.EqualTo(new TruBool(false)));
            Assert.That( TruStatement.Interpret("{majority true false false}"), Is.EqualTo(new TruBool(false)));
        }
    }
}