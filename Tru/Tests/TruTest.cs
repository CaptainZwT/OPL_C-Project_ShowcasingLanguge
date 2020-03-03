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

            Assert.True(  TruLibrary.Library.Find("and").Equals(TruLibrary.Library.Find("and")) );
            Assert.False( TruLibrary.Library.Find("and").Equals(TruLibrary.Library.Find("or"))  );

            TruLambda lamb0 = new TruLambda(new string[] {"x"}, new TruId("x"));
            TruLambda lamb1 = new TruLambda(new string[] {"x"}, new TruId("x"));
            TruLambda lamb2 = new TruLambda(new string[] {},    new TruId("x"));
            TruLambda lamb3 = new TruLambda(new string[] {"y"}, new TruId("x"));
            TruLambda lamb4 = new TruLambda(new string[] {"x"}, new TruId("y"));

            Assert.True(lamb0.Equals(lamb1));
            Assert.False(lamb0.Equals(lamb2));
            Assert.False(lamb0.Equals(lamb3));
            Assert.False(lamb0.Equals(lamb4));


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

            TruExpr let0 = TruExpr.Parse("{let {[a false]} a}");
            TruExpr let1 = TruExpr.Parse("{let {[a false]} a}");
            TruExpr let2 = TruExpr.Parse("{let {} true}");

            Assert.True(  let0.Equals(let1) );
            Assert.False( let0.Equals(let2) );

            TruStatement def0 = TruStatement.Parse("{define x true}");
            TruStatement def1 = TruStatement.Parse("{define x true}");
            TruStatement def2 = TruStatement.Parse("{define x false}");

            Assert.True(  def0.Equals(def1) );
            Assert.False( def0.Equals(def2) );


        }

        [Test]
        public void TestTruToString() {
            Assert.That( new TruBool(true).ToString(), Is.EqualTo( "true" ));
            Assert.That( new TruBool(false).ToString(), Is.EqualTo( "false" ));

            Assert.That(  new TruId("x").ToString(), Is.EqualTo( "x" ));

            Assert.That( new TruBuiltIn(null).ToString(), Is.EqualTo("built-in"));

            Assert.That( new TruLambda(new string[] {}, new TruBool(false)).ToString(),
                Is.EqualTo("{lambda {} false}"));
            Assert.That( new TruLambda(new string[] {"x"}, new TruCall(new TruId("not"), new[]{new TruId("x")})).ToString(),
                Is.EqualTo("{lambda {x} {not x}}"));

            Assert.That( new TruCall(new TruId("and"), new [] {new TruBool(true), new TruBool(false)}).ToString(),
                Is.EqualTo("{and true false}"));

            Assert.That( new TruFunc(new string[] {}, new TruBool(false), null).ToString(),
                Is.EqualTo("function {}"));
            Assert.That( new TruFunc(new string[] {"x"}, new TruCall(new TruId("not"), new[]{new TruId("x")}), null).ToString(),
                Is.EqualTo("function {x}"));

            Assert.That( new TruLet(
                    new (string, TruExpr)[]{
                        ("var1", new TruBool(false)),
                        ("var2",  new TruCall(new TruId("not"), new[]{ new TruBool(true) }))
                    },
                    new TruCall(new TruId("and"), new[]{ new TruId("var1"), new TruId("var2") })
                ).ToString(),
                Is.EqualTo("{let {[var1 false] [var2 {not true}]} {and var1 var2}}"));

            Assert.That( new TruDef("x", new TruId("b")).ToString(), Is.EqualTo("{define x b}"));
        }

        [Test]
        public void TestTruParseStatement() {
            Assert.That( TruStatement.Parse("{define x true}"),
                Is.EqualTo( new TruDef("x", new TruBool(true) ) ));
            Assert.That( TruStatement.Parse("{define {my-func x} true}"),
                Is.EqualTo( new TruDef("my-func", new TruLambda(new[]{"x"}, new TruBool(true)) )));
            Assert.That( TruStatement.Parse("{not true}"), // an expression is a statement.
                Is.EqualTo( new TruCall(new TruId("not"), new[] {new TruBool(true)} ) ));
        }

        [Test]
        public void TestTruParseExpr() {
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

            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{}").Interpret() ); // Can't have an empty call.


            Assert.That( TruExpr.Parse("{lambda {a b} a}"),
                Is.EqualTo( new TruLambda(new[] {"a", "b"}, new TruId("a")) ));

            Assert.That( TruExpr.Parse("{lambda {} {and false true}}"),
                Is.EqualTo( new TruLambda(new string[] {},
                    new TruCall(new TruId("and"), new[] { new TruBool(false), new TruBool(true) })) ));

            Assert.That( TruExpr.Parse("{{lambda {x} x} false}"),
                Is.EqualTo( new TruCall(
                    new TruLambda(new[] {"x"}, new TruId("x")),
                    new[] { new TruBool(false) }
            )));

            Assert.That( TruExpr.Parse("{lambda {and} and}"), // You can shadow built-in bindings
                Is.EqualTo( new TruLambda(new[] {"and"}, new TruId("and")) ));

            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{lambda {true} true}") ); // You can't shadow true/false
            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{lambda {false} false}") );
            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{lambda {lambda} false}") );
            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{lambda {x} false false}") );
            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{lambda {{not x}} false}") );

            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{and lambda false}") );

            Assert.That( TruExpr.Parse(
                @"{let {[var1 false] [var2 {not true}]}
                    {and var1 var2}
                  }"),
                Is.EqualTo( new TruLet(
                    new (string, TruExpr)[]{
                        ("var1", new TruBool(false)),
                        ("var2",  new TruCall(new TruId("not"), new[]{ new TruBool(true) }))
                    },
                    new TruCall(new TruId("and"), new[]{ new TruId("var1"), new TruId("var2") })
                )));

            Assert.That( TruExpr.Parse("{let {} true}"),
                Is.EqualTo( new TruLet(
                    new (string, TruExpr)[]{},
                    new TruBool(true)
                )));
        }

        [Test]
        public void TestTruParseId() {
            Assert.That(TruId.Parse("x"), Is.EqualTo(new TruId("x")));
            Assert.Throws<System.ArgumentException>( () => TruId.Parse("{and}") );
            Assert.Throws<System.ArgumentException>( () => TruId.Parse("define") );
        }
        
        [Test]
        public void TestTruBasicInterpret() {
            Assert.That( TruExpr.Parse("true").Interpret(),
                Is.EqualTo( new TruBool(true) ));

            Assert.That( TruExpr.Parse("false").Interpret(),
                Is.EqualTo( new TruBool(false) ));

            Assert.That( TruExpr.Parse("{and true true}").Interpret(),
                Is.EqualTo( new TruBool(true) ));

            Assert.That( TruExpr.Parse("{and {not false} true}").Interpret(),
                Is.EqualTo( new TruBool(true) ));

            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{and true true true}").Interpret() );
            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{and}").Interpret() );

            Assert.That( TruExpr.Parse("and").Interpret(),
                Is.EqualTo( TruLibrary.Library.Find("and") ));
        }

        [Test]
        public void TestTruLambdas() {
            TruFunc func0 = (TruFunc) TruExpr.Parse("{lambda {a b} a}").Interpret();
            TruFunc func1 = (TruFunc) TruExpr.Parse("{lambda {} {and false true}}").Interpret();
            Assert.That( func0.parameters, Is.EqualTo( new string[] {"a", "b"} ) );
            Assert.That( func0.body, Is.EqualTo( new TruId("a") ) );
            Assert.That( func1.parameters, Is.EqualTo( new string[] {} ) );

            Assert.That( TruExpr.Parse("{{lambda {x} x} false}").Interpret(),
                Is.EqualTo( new TruBool(false) ));
        }

        [Test]
        public void TestTruComplexInterpret() {
            Environment testEnv = TruLibrary.Library.ExtendLocalAll(new Environment( new[]{
                ("y", new TruBool(false)),
                ("bad-func",  TruExpr.Parse("{lambda {x} y}").Interpret()),
                ("good-func", TruExpr.Parse("{lambda {x} x}").Interpret()),
                ("meta-func", TruExpr.Parse("{lambda {a} {lambda {} a}}").Interpret()) // returns a lambda that captured a.
            }));

            /// y should be not found, since it isn't in the lambda's scope.
            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{bad-func false}").Interpret(testEnv) );
            Assert.That( TruExpr.Parse("{good-func false}").Interpret(testEnv),
                Is.EqualTo( new TruBool(false) ));
            Assert.That( TruExpr.Parse("y").Interpret(testEnv),
                Is.EqualTo( new TruBool(false) ));
            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{y}").Interpret(testEnv) ); // not callable.
            Assert.That( TruExpr.Parse("{{meta-func true}}").Interpret(testEnv),
                Is.EqualTo( new TruBool(true) ));

            // the lambda return from meta-func should contain a in its environment.
            TruFunc generatedFunc = TruExpr.Parse("{meta-func true}").Interpret(testEnv) as TruFunc;
            Assert.That( generatedFunc, Is.Not.EqualTo(null));
            Assert.That( generatedFunc.env.Find("a"), Is.EqualTo( new TruBool(true) ));

            Assert.That( TruExpr.Parse(
                @"{let {[var1 false] [var2 true]}
                    {and var1 var2}
                  }").Interpret(),
                Is.EqualTo( new TruBool(false) ));

            Assert.That( TruExpr.Parse(
                @"{let { [ my-func {lambda {a b} {and a b}} ] }
                    {my-func true true}
                  }").Interpret(),
                Is.EqualTo( new TruBool(true) ));

            Assert.That( TruExpr.Parse(
                @"{let {}
                    true
                  }").Interpret(),
                Is.EqualTo( new TruBool(true) ));

            Assert.That( TruExpr.Parse( // insure that the captured environment is separate from outer.
                @"{let { [a false] [my-func {meta-func true}]}
                    {my-func}
                  }").Interpret(testEnv),
                Is.EqualTo( new TruBool(true) ));

            Assert.Throws<System.ArgumentException>( () => TruExpr.Parse("{let {x 3} x}").Interpret(testEnv) );
        }

        [Test]
        public void TestTruDef() {
            Environment global = TruLibrary.Library;
            TruStatement.Parse("{define var1 true}").Interpret(global);
            TruStatement.Parse("{define {my-func x} {and var1 x}}").Interpret(global);
            
            Assert.That( TruStatement.Parse("{my-func var1}").Interpret(global), Is.EqualTo(new TruBool(true)));
        }

        [Test]
        public void Test4WayMultiplexer() {
            string program =
                @"{let {[multiplex4 ; Declare a 4-way multiplexer function and test it on multiple inputs, should return true if the tests pass.
                    {lambda {s0 s1 i0 i1 i2 i3}
                        {let {[multiplex2 ; make a 2-way multiplex and combine them for the 4-way
                            {lambda {s0 i0 i1} {or {and {not s0} i0} {and s0 i1}}} ]}
                                {multiplex2 s1
                                    {multiplex2 s0 i0 i1}
                                    {multiplex2 s0 i2 i3}}}}]}

                    {and ; test the multiplexer
                        {and
                        {and
                            {equals {multiplex4 false false  false false true false} false}
                            {equals {multiplex4 false false  true false false false} true }}
                        {and
                            {equals {multiplex4 false true   false false false true} false}
                            {equals {multiplex4 false true   true true true true}    true }}}
                        {and
                        {and
                            {equals {multiplex4 true false   false false false true} false}
                            {equals {multiplex4 true false   false true true false}  true }}
                        {and
                            {equals {multiplex4 true true    true false false false} false}
                            {equals {multiplex4 true true    false true false true}  true }}}}}";



            Assert.That( TruExpr.Parse(program).Interpret(), Is.EqualTo( new TruBool(true) ));
        }
    }
}