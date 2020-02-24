using NUnit.Framework;
using Tru;
using Utility;

namespace Tests
{
    public class TruExprTest
    {
        [Test]
        public void TestTruEquals() {
            Assert.True(  new TruValue(true).Equals( new TruValue(true) ));
            Assert.False( new TruValue(true).Equals( new TruValue(false) ));
            Assert.False( new TruValue(true).Equals( new ExprList(new List<ExprTree>()) ));
            Assert.False( new TruValue(true).Equals( null ));

            Assert.True(
                new TruAnd(
                    new TruValue(true),
                    new TruNot(
                        new TruValue(false)
                    )
                ).Equals(
                new TruAnd(
                    new TruValue(true),
                    new TruNot(
                        new TruValue(false)
                    )
                )
            ));

            Assert.False(
                new TruOr(
                    new TruValue(true),
                    new TruNot(
                        new TruValue(false)
                    )
                ).Equals(
                new TruAnd(
                    new TruValue(true),
                    new TruNot(
                        new TruValue(false)
                    )
                )
            ));
        }

        [Test]
        public void TestTruParse() {
            Assert.That( TruExpr.Parse("#t"),
                Is.EqualTo( new TruValue(true) ));

            Assert.That( TruExpr.Parse("#f"),
                Is.EqualTo( new TruValue(false) ));

            Assert.That( TruExpr.Parse("{and #t #t}"),
                Is.EqualTo( new TruAnd(new TruValue(true), new TruValue(true)) ));

            Assert.That( TruExpr.Parse("{and #t #f}"),
                Is.EqualTo( new TruAnd(new TruValue(true), new TruValue(false)) ));

            Assert.That( TruExpr.Parse("{or #t #f}"),
                Is.EqualTo( new TruOr(new TruValue(true), new TruValue(false)) ));

            Assert.That( TruExpr.Parse("{or #f #f}"),
                Is.EqualTo( new TruOr(new TruValue(false), new TruValue(false)) ));

            Assert.That( TruExpr.Parse("{not #f}"),
                Is.EqualTo( new TruNot(new TruValue(false)) ));

            Assert.That( TruExpr.Parse("{not #t}"),
                Is.EqualTo( new TruNot(new TruValue(true)) ));

            Assert.That( TruExpr.Parse("{and {not #f} #t}"),
                Is.EqualTo( new TruAnd(new TruNot(new TruValue(false)), new TruValue(true)) ));
        }

        [Test]
        public void TestTruInterpret() {
            Assert.True(  TruExpr.Parse("#t").Interpret() );
            Assert.False( TruExpr.Parse("#f").Interpret() );

            Assert.True(  TruExpr.Parse("{and #t #t}").Interpret() );
            Assert.False( TruExpr.Parse("{and #t #f}").Interpret() );

            Assert.True(  TruExpr.Parse("{or #t #f}").Interpret() );
            Assert.False( TruExpr.Parse("{or #f #f}").Interpret() );

            Assert.True(  TruExpr.Parse("{not #f}").Interpret() );
            Assert.False( TruExpr.Parse("{not #t}").Interpret() );
            Assert.True(  TruExpr.Parse("{and {not #f} #t}").Interpret() );
        }
    }
}