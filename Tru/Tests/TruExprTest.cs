using NUnit.Framework;
using Tru;
using Utility;

namespace Tests
{
    public class TruExprTest
    {
        [Test]
        public void TestTruEquals() {
            Assert.True(  new TruBool(true).Equals( new TruBool(true) ));
            Assert.False( new TruBool(true).Equals( new TruBool(false) ));
            Assert.False( new TruBool(true).Equals( null ));

            Assert.True(  new TruId("x").Equals( new TruId("x") ));
            Assert.False( new TruId("x").Equals( new TruId("t") ));

            Assert.True(  TruLibrary.library.Find("and").Equals(TruLibrary.library.Find("and")) );
            Assert.False( TruLibrary.library.Find("and").Equals(TruLibrary.library.Find("or"))  );


            TruFunc func0 = new TruFunc(new string[] {}, null, null);
            TruFunc func1 = new TruFunc(new string[] {"x"}, new TruId("x"), null);
            TruFunc func2 = new TruFunc(new string[] {"x"}, new TruId("x"), null);
            TruFunc func3 = new TruFunc(new string[] {"x"}, new TruId("x"), TruLibrary.library);
            TruFunc func4 = new TruFunc(new string[] {"x"}, new TruId("y"), null);

            Assert.True(func1.Equals(func2));
            Assert.False(func0.Equals(func1));
            Assert.False(func1.Equals(func3));
            Assert.False(func1.Equals(func4));
            Assert.False(func1.Equals(func0));


            TruCall call0 = new TruCall(new TruId("and"), new TruExpr[] {
                new TruBool(true),
                new TruCall(new TruId("not"), new TruExpr[] {
                    new TruBool(false)
                })
            });
            TruCall call1 = new TruCall(new TruId("and"), new TruExpr[] {
                new TruBool(true),
                new TruCall(new TruId("not"), new TruExpr[] {
                    new TruBool(false)
                })
            });
            TruCall call2 = new TruCall(new TruId("and"), new TruExpr[] {
                new TruBool(true),
                new TruCall(new TruId("not"), new TruExpr[] {
                    new TruBool(true)
                })
            });
            TruCall call3 = new TruCall(new TruId("nand"), new TruExpr[] {
                new TruBool(true),
                new TruCall(new TruId("not"), new TruExpr[] {
                    new TruBool(false)
                })
            });

            Assert.True(  call0.Equals(call1) );
            Assert.False( call0.Equals(call2) );
            Assert.False( call0.Equals(call3) );
        }

        [Test]
        public void TestTruToString() {
            Assert.That( new TruBool(true).ToString(), Is.EqualTo( "true" ));
            Assert.That( new TruBool(false).ToString(), Is.EqualTo( "false" ));

            Assert.That(  new TruId("x").ToString(), Is.EqualTo( "x" ));

            Assert.That( new TruBuiltIn(null).ToString(), Is.EqualTo("built-in"));

            Assert.That( new TruFunc(new string[] {}, new TruBool(false)).ToString(),
                Is.EqualTo("{lambda {} false}"));
            Assert.That( new TruFunc(new string[] {"x"}, new TruCall(new TruId("not"), new[]{new TruId("x")})).ToString(),
                Is.EqualTo("{lambda {x} {not x}}"));

            Assert.That( new TruCall(new TruId("and"), new [] {new TruBool(true), new TruBool(false)}).ToString(),
                Is.EqualTo("{and true false}"));
        }

        [Test]
        public void TestTruParse() {
            Assert.That( TruExpr.Parse("true"),
                Is.EqualTo( new TruBool(true) ));

            Assert.That( TruExpr.Parse("false"),
                Is.EqualTo( new TruBool(false) ));

            Assert.That( TruExpr.Parse("{and true true}"),
                Is.EqualTo( new TruCall(new TruId("and"), new[]{ new TruBool(true), new TruBool(true) }) ));

            Assert.That( TruExpr.Parse("{or true false}"),
                Is.EqualTo( new TruCall(new TruId("or"), new[]{ new TruBool(true), new TruBool(false) }) ));

            Assert.That( TruExpr.Parse("{not false}"),
                Is.EqualTo( new TruCall(new TruId("not"), new[]{ new TruBool(false) }) ));

            Assert.That( TruExpr.Parse("{and {not false} true}"),
                Is.EqualTo( new TruCall(new TruId("and"), new TruExpr[]{
                    new TruCall(new TruId("not"), new[] {
                        new TruBool(false)
                    }),
                    new TruBool(true)
                 })));

            // Assert.That( TruExpr.Parse("{lambda {a b} a}"),
            //     Is.EqualTo( new TruFunc(new string[] {"a", "b"}, new TruId("a")) ));

            // Assert.That( TruExpr.Parse("{lambda {} {and false true}}"),
            //     Is.EqualTo( new TruFunc(new string[] {},
            //         new TruCall(new TruId("and"), new[] { new TruBool(false), new TruBool(true) })) ));

            // Assert.That( TruExpr.Parse("{{lambda {x} x} false}"),
            //     Is.EqualTo( new TruCall(
            //         new TruFunc(new string[] {"x"}, new TruId("x")),
            //         new[] { new TruBool(false) }
            // )));
        }

        [Test]
        public void TestTruBasicInterpret() {
            Assert.That( TruExpr.Parse("true").Interpret(),
                Is.EqualTo( new TruBool(true) ));

            Assert.That( TruExpr.Parse("false").Interpret(),
                Is.EqualTo( new TruBool(false) ));

            Assert.That( TruExpr.Parse("{and true true}").Interpret(),
                Is.EqualTo( new TruBool(true) ));

            Assert.That( TruExpr.Parse("{or false false}").Interpret(),
                Is.EqualTo( new TruBool(false) ));

            Assert.That( TruExpr.Parse("{not false}").Interpret(),
                Is.EqualTo( new TruBool(true) ));

            Assert.That( TruExpr.Parse("{and {not false} true}").Interpret(),
                Is.EqualTo( new TruBool(true) ));

            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{and true true true}").Interpret() );
            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{and}").Interpret() );

            // Assert.That( TruExpr.Parse("{lambda {a b} a}").Interpret(), // Interprets to a lambda.
            //     Is.EqualTo( new TruFunc(new string[] {"a", "b"}, new TruId("a")) ));

            // Assert.That( TruExpr.Parse("{lambda {} {and false true}}").Interpret(),
            //     Is.EqualTo( new TruFunc(new string[] {},
            //         new TruCall(new TruId("and"), new[] { new TruBool(false), new TruBool(true) })) ));

            // Assert.That( TruExpr.Parse("{{lambda {x} x} false}").Interpret(),
                // Is.EqualTo( new TruBool(false) ));

            Assert.That( TruExpr.Parse("and").Interpret(),
                Is.EqualTo( TruLibrary.library.Find("and") ));
        }

        // [Test]
        // public void TestTruComplexInterpret() {
        //     Environment testEnv = new Environment( new[]{
        //         ("y", new TruBool(false)),
        //         ("bad-func",  TruExpr.Parse("{lambda {x} y}").Interpret()),
        //         ("good-func", TruExpr.Parse("{lambda {x} x}").Interpret()),
        //         ("meta-func", TruExpr.Parse("{lambda {a} {lambda {} a}").Interpret()) // returns a lambda that captured a.
        //     });
        //     testEnv.AddAll(TruLibrary.library);

        //     /// y should be not found, since it isn't in the lambda's scope.
        //     Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{bad-func false}").Interpret(testEnv) );
        //     Assert.That( TruExpr.Parse("{good-func false}").Interpret(testEnv),
        //         Is.EqualTo( new TruBool(false) ));
        //     Assert.That( TruExpr.Parse("y").Interpret(testEnv),
        //         Is.EqualTo( new TruBool(false) ));
        //     Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{y}").Interpret(testEnv) ); // not callable.
        //     Assert.That( TruExpr.Parse("{{meta-func true}}").Interpret(testEnv),
        //         Is.EqualTo( new TruBool(true) ));


            // Assert.That( TruExpr.Parse(
            //     @"{let {[var1 false] [var2 true]}
            //         {and var1 var2}
            //       }").Interpret(),
            //     Is.EqualTo( new TruBool(false) ));

            // Assert.That( TruExpr.Parse(
            //     @"{let { [ my-func {lambda {a b} {and a b}} ] }
            //         {my-func true true}
            //       }").Interpret(),
            //     Is.EqualTo( new TruBool(true) ));

            // Assert.That( TruExpr.Parse(
            //     @"{let {}
            //         true
            //       }").Interpret(),
            //     Is.EqualTo( new TruBool(true) ));

            // Assert.That( TruExpr.Parse( // insure that the captured environment is separate from outer.
            //     @"{let { [a false] [my-func {meta-func true}]}
            //         {my-func}
            //       }").Interpret(testEnv),
            //     Is.EqualTo( new TruBool(true) ));
        // }

        // [Test]
        // public void Test4WayMultiplexer() {
        //     string program =
        //         @"{let {[multiplex4 ; Declare a 4-way multiplexer function and test it on multiple inputs, should return true if the tests pass.
        //             {lambda {s0 s1 i0 i1 i2 i3}
        //                 {let {[multiplex2 ; make a 2-way multiplex and combine them for the 4-way
        //                     {lambda {s0 i0 i1} {or {and {not s0} i0} {and s0 i1}}} ]}
        //                 {multiplex2 s1
        //                             {multiplex2 s0 i0 i1}
        //                             {multiplex2 s0 i2 i3}}}}]}

        //             {and ; test the multiplexer
        //                 {and
        //                 {and
        //                     {equals {multiplex4 false false  false false true false} false}
        //                     {equals {multiplex4 false false  true false false false} true }}
        //                 {and
        //                     {equals {multiplex4 false true   false false false true} false}
        //                     {equals {multiplex4 false true   true true true true}    true }}}
        //                 {and
        //                 {and
        //                     {equals {multiplex4 true false   false false false true} false}
        //                     {equals {multiplex4 true false   false true true false}  true }}
        //                 {and
        //                     {equals {multiplex4 true true    true false false false} false}
        //                     {equals {multiplex4 true true    false true false true}  true }}}}";



        //     Assert.That( TruExpr.Parse(program).Interpret(), Is.EqualTo( new TruBool(true) ));
        // }
    }
}