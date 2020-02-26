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
            Assert.That( TruExpr.Parse("true"),
                Is.EqualTo( new TruValue(true) ));

            Assert.That( TruExpr.Parse("false"),
                Is.EqualTo( new TruValue(false) ));

            Assert.That( TruExpr.Parse("{and true true}"),
                Is.EqualTo( new TruAnd(new TruValue(true), new TruValue(true)) ));

            Assert.That( TruExpr.Parse("{and true false}"),
                Is.EqualTo( new TruAnd(new TruValue(true), new TruValue(false)) ));

            Assert.That( TruExpr.Parse("{or true false}"),
                Is.EqualTo( new TruOr(new TruValue(true), new TruValue(false)) ));

            Assert.That( TruExpr.Parse("{or false false}"),
                Is.EqualTo( new TruOr(new TruValue(false), new TruValue(false)) ));

            Assert.That( TruExpr.Parse("{not false}"),
                Is.EqualTo( new TruNot(new TruValue(false)) ));

            Assert.That( TruExpr.Parse("{not true}"),
                Is.EqualTo( new TruNot(new TruValue(true)) ));

            Assert.That( TruExpr.Parse("{and {not false} true}"),
                Is.EqualTo( new TruAnd(new TruNot(new TruValue(false)), new TruValue(true)) ));
        }

        [Test]
        public void TestTruInterpret() {
            Assert.True(  TruExpr.Parse("true").Interpret() );
            Assert.False( TruExpr.Parse("false").Interpret() );

            Assert.True(  TruExpr.Parse("{and true true}").Interpret() );
            Assert.False( TruExpr.Parse("{and true false}").Interpret() );

            Assert.True(  TruExpr.Parse("{or true false}").Interpret() );
            Assert.False( TruExpr.Parse("{or false false}").Interpret() );

            Assert.True(  TruExpr.Parse("{not false}").Interpret() );
            Assert.False( TruExpr.Parse("{not true}").Interpret() );
            Assert.True(  TruExpr.Parse("{and {not false} true}").Interpret() );
        }
    }
}