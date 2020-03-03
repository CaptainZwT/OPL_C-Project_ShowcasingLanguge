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
            Assert.That( TruExpr.Parse("{nand true true}").Interpret(),   Is.EqualTo(new TruBool(false)));
            Assert.That( TruExpr.Parse("{and  true true}").Interpret(),   Is.EqualTo(new TruBool(true)));
            Assert.That( TruExpr.Parse("{or   false false}").Interpret(), Is.EqualTo(new TruBool(false)));
            Assert.That( TruExpr.Parse("{not  true}").Interpret(),        Is.EqualTo(new TruBool(false)));
            Assert.That( TruExpr.Parse("{equals true true}").Interpret(), Is.EqualTo(new TruBool(true)));
            Assert.That( TruExpr.Parse("{equals {lambda {} true} {lambda {} true}}").Interpret(), Is.EqualTo(new TruBool(false)));
            Assert.That( TruExpr.Parse("{let {[f {lambda {} true}]} {equals f f}}").Interpret(), Is.EqualTo(new TruBool(true)));
            Assert.That( TruExpr.Parse("{if true true false}").Interpret(), Is.EqualTo(new TruBool(true)));
        }

        [Test]
        public void TestLibrary() {
            Assert.That( TruExpr.Parse("{nor  true true}").Interpret(),   Is.EqualTo(new TruBool(false)));
            Assert.That( TruExpr.Parse("{xor  true true}").Interpret(),   Is.EqualTo(new TruBool(false)));
            Assert.That( TruExpr.Parse("{xnor false false}").Interpret(), Is.EqualTo(new TruBool(true)));
            Assert.That( TruExpr.Parse("{implies true false}").Interpret(), Is.EqualTo(new TruBool(false)));
            Assert.That( TruExpr.Parse("{majority true false false}").Interpret(), Is.EqualTo(new TruBool(false)));
        }
    }
}