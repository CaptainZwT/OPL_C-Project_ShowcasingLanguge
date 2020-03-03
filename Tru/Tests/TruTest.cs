using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tru;
using Utility;

namespace Tests
{
    [TestClass]
    public class TruExprTest
    {
        [TestMethod]
        public void TestTruEquals() {
            Assert.IsTrue(  new TruBool(true).Equals( new TruBool(true) ));
            Assert.IsFalse( new TruBool(true).Equals( new TruBool(false) ));
            Assert.IsFalse( new TruBool(true).Equals( null ));

            Assert.IsTrue(  new TruId("x").Equals( new TruId("x") ));
            Assert.IsFalse( new TruId("x").Equals( new TruId("t") ));

            Assert.IsTrue(  TruLibrary.Library.Find("and").Equals(TruLibrary.Library.Find("and")) );
            Assert.IsFalse( TruLibrary.Library.Find("and").Equals(TruLibrary.Library.Find("or"))  );

            TruLambda lamb0 = new TruLambda(new string[] {"x"}, new TruId("x"));
            TruLambda lamb1 = new TruLambda(new string[] {"x"}, new TruId("x"));
            TruLambda lamb2 = new TruLambda(new string[] {},    new TruId("x"));
            TruLambda lamb3 = new TruLambda(new string[] {"y"}, new TruId("x"));
            TruLambda lamb4 = new TruLambda(new string[] {"x"}, new TruId("y"));

            Assert.IsTrue(lamb0.Equals(lamb1));
            Assert.IsFalse(lamb0.Equals(lamb2));
            Assert.IsFalse(lamb0.Equals(lamb3));
            Assert.IsFalse(lamb0.Equals(lamb4));


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

            Assert.IsTrue(  call0.Equals(call1) );
            Assert.IsFalse( call0.Equals(call2) );
            Assert.IsFalse( call0.Equals(call3) );

            TruExpr let0 = TruExpr.Parse("{let {[a false]} a}");
            TruExpr let1 = TruExpr.Parse("{let {[a false]} a}");
            TruExpr let2 = TruExpr.Parse("{let {} true}");

            Assert.IsTrue(  let0.Equals(let1) );
            Assert.IsFalse( let0.Equals(let2) );

            TruStatement def0 = TruStatement.Parse("{define x true}");
            TruStatement def1 = TruStatement.Parse("{define x true}");
            TruStatement def2 = TruStatement.Parse("{define x false}");

            Assert.IsTrue(  def0.Equals(def1) );
            Assert.IsFalse( def0.Equals(def2) );
        }

        [TestMethod]
        public void TestTruToString() {
            Assert.AreEqual( new TruBool(true).ToString(),  "true" );
            Assert.AreEqual( new TruBool(false).ToString(),  "false" );

            Assert.AreEqual(  new TruId("x").ToString(),  "x" );

            Assert.AreEqual( new TruBuiltIn(null).ToString(), "built-in");

            Assert.AreEqual( new TruLambda(new string[] {}, new TruBool(false)).ToString(),
                "{lambda {} false}");
            Assert.AreEqual( new TruLambda(new string[] {"x"}, new TruCall(new TruId("not"), new[]{new TruId("x")})).ToString(),
                "{lambda {x} {not x}}");

            Assert.AreEqual( new TruCall(new TruId("and"), new [] {new TruBool(true), new TruBool(false)}).ToString(),
                "{and true false}");

            Assert.AreEqual( new TruFunc(new string[] {}, new TruBool(false), null).ToString(),
                "function {}");
            Assert.AreEqual( new TruFunc(new string[] {"x"}, new TruCall(new TruId("not"), new[]{new TruId("x")}), null).ToString(),
                "function {x}");

            Assert.AreEqual( new TruLet(
                    new (string, TruExpr)[]{
                        ("var1", new TruBool(false)),
                        ("var2",  new TruCall(new TruId("not"), new[]{ new TruBool(true) }))
                    },
                    new TruCall(new TruId("and"), new[]{ new TruId("var1"), new TruId("var2") })
                ).ToString(),
                "{let {[var1 false] [var2 {not true}]} {and var1 var2}}");

            Assert.AreEqual( new TruDef("x", new TruId("b")).ToString(), "{define x b}");
        }

        [TestMethod]
        public void TestTruParseStatement() {
            Assert.AreEqual( TruStatement.Parse("{define x true}"), new TruDef("x", new TruBool(true) ) );
            Assert.AreEqual( TruStatement.Parse("{define {my-func x} true}"),
                 new TruDef("my-func", new TruLambda(new[]{"x"}, new TruBool(true)) ));
            Assert.AreEqual( TruStatement.Parse("{not true}"), // an expression is a statement.
                 new TruCall(new TruId("not"), new[] {new TruBool(true)} ) );
        }

        [TestMethod]
        public void TestTruParseExpr() {
            Assert.AreEqual( TruExpr.Parse("true"), new TruBool(true) );

            Assert.AreEqual( TruExpr.Parse("false"), new TruBool(false) );

            Assert.AreEqual( TruExpr.Parse("{and true true}"),
                 new TruCall(new TruId("and"), new[]{ new TruBool(true), new TruBool(true) }) );

            Assert.AreEqual( TruExpr.Parse("{or true false}"),
                 new TruCall(new TruId("or"), new[]{ new TruBool(true), new TruBool(false) }) );

            Assert.AreEqual( TruExpr.Parse("{not false}"),
                 new TruCall(new TruId("not"), new[]{ new TruBool(false) }) );

            Assert.AreEqual( TruExpr.Parse("{and {not false} true}"),
                new TruCall(new TruId("and"), new TruExpr[]{
                    new TruCall(new TruId("not"), new[] {
                        new TruBool(false)
                    }),
                    new TruBool(true)
                 }));

            Assert.ThrowsException<System.ArgumentException>( () => TruExpr.Parse("{}") ); // Can't have an empty call.


            Assert.AreEqual( TruExpr.Parse("{lambda {a b} a}"),
                 new TruLambda(new[] {"a", "b"}, new TruId("a")) );

            Assert.AreEqual( TruExpr.Parse("{lambda {} {and false true}}"),
                new TruLambda(new string[] {}, new TruCall(new TruId("and"), new[] { new TruBool(false), new TruBool(true) })) );

            Assert.AreEqual( TruExpr.Parse("{{lambda {x} x} false}"),
                new TruCall(
                    new TruLambda(new[] {"x"}, new TruId("x")),
                    new[] { new TruBool(false) }
            ));

            Assert.AreEqual( TruExpr.Parse("{lambda {and} and}"), // You can shadow built-in bindings
                 new TruLambda(new[] {"and"}, new TruId("and")) );

            Assert.ThrowsException<System.ArgumentException>( () => TruExpr.Parse("{lambda {true} true}") ); // You can't shadow true/false
            Assert.ThrowsException<System.ArgumentException>( () => TruExpr.Parse("{lambda {false} false}") );
            Assert.ThrowsException<System.ArgumentException>( () => TruExpr.Parse("{lambda {lambda} false}") );
            Assert.ThrowsException<System.ArgumentException>( () => TruExpr.Parse("{lambda {x} false false}") );
            Assert.ThrowsException<System.ArgumentException>( () => TruExpr.Parse("{lambda {{not x}} false}") );

            Assert.ThrowsException<System.ArgumentException>( () => TruExpr.Parse("{and lambda false}") );

            Assert.AreEqual( TruExpr.Parse(
                @"{let {[var1 false] [var2 {not true}]}
                    {and var1 var2}
                  }"),
                new TruLet(
                    new (string, TruExpr)[]{
                        ("var1", new TruBool(false)),
                        ("var2",  new TruCall(new TruId("not"), new[]{ new TruBool(true) }))
                    },
                    new TruCall(new TruId("and"), new[]{ new TruId("var1"), new TruId("var2") })
            ));

            Assert.AreEqual( TruExpr.Parse("{let {} true}"),
                new TruLet( new (string, TruExpr)[]{}, new TruBool(true)
            ));
        }

        [TestMethod]
        public void TestTruParseId() {
            Assert.AreEqual(TruId.Parse("x"), new TruId("x"));
            Assert.ThrowsException<System.ArgumentException>( () => TruId.Parse("{and}") );
            Assert.ThrowsException<System.ArgumentException>( () => TruId.Parse("define") );
        }
        
        [TestMethod]
        public void TestTruBasicInterpret() {
            Assert.AreEqual( TruExpr.Parse("true").Interpret(), new TruBool(true) );

            Assert.AreEqual( TruStatement.Interpret("false"), new TruBool(false) );

            Assert.AreEqual( TruStatement.Interpret("{and true true}"), new TruBool(true) );

            Assert.AreEqual( TruStatement.Interpret("{and {not false} true}"), new TruBool(true) );

            Assert.ThrowsException<System.ArgumentException>( () => TruStatement.Interpret("{and true true true}") );
            Assert.ThrowsException<System.ArgumentException>( () => TruStatement.Interpret("{and}") );

            Assert.AreEqual( TruStatement.Interpret("and"), TruLibrary.Library.Find("and") );
        }

        [TestMethod]
        public void TestTruLambdas() {
            TruFunc func0 = (TruFunc) TruStatement.Interpret("{lambda {a b} a}");
            TruFunc func1 = (TruFunc) TruStatement.Interpret("{lambda {} {and false true}}");
            CollectionAssert.AreEqual( func0.parameters,  new string[] {"a", "b"} ) ;
            Assert.AreEqual( func0.body,  new TruId("a") ) ;
            CollectionAssert.AreEqual( func1.parameters,  new string[] {} ) ;

            Assert.AreEqual( TruStatement.Interpret("{{lambda {x} x} false}"), new TruBool(false) );
        }

        [TestMethod]
        public void TestTruComplexInterpret() {
            Environment testEnv = TruLibrary.Library.ExtendLocalAll(new Environment( new[]{
                ("y", new TruBool(false)),
                ("bad-func",  TruStatement.Interpret("{lambda {x} y}")),
                ("good-func", TruStatement.Interpret("{lambda {x} x}")),
                ("meta-func", TruStatement.Interpret("{lambda {a} {lambda {} a}}")) // returns a lambda that captured a.
            }));

            /// y should be not found, since it isn't in the lambda's scope.
            Assert.ThrowsException<System.ArgumentException>( () => TruExpr.Parse("{bad-func false}").Interpret(testEnv) );
            Assert.AreEqual( TruStatement.Interpret("{good-func false}", testEnv), new TruBool(false) );
            Assert.AreEqual( TruStatement.Interpret("y", testEnv), new TruBool(false) );
            Assert.ThrowsException<System.ArgumentException>( () => TruStatement.Interpret("{y}", testEnv) ); // not callable.
            Assert.AreEqual( TruStatement.Interpret("{{meta-func true}}", testEnv), new TruBool(true) );

            // the lambda return from meta-func should contain a in its environment.
            TruFunc generatedFunc = TruStatement.Interpret("{meta-func true}", testEnv) as TruFunc;
            Assert.IsNotNull( generatedFunc );
            Assert.AreEqual( generatedFunc.env.Find("a"),  new TruBool(true) );

            Assert.AreEqual( TruStatement.Interpret(
                @"{let {[var1 false] [var2 true]}
                    {and var1 var2}
                  }"),
                 new TruBool(false) );

            Assert.AreEqual( TruStatement.Interpret(
                @"{let { [ my-func {lambda {a b} {and a b}} ] }
                    {my-func true true}
                  }"),
                 new TruBool(true) );

            Assert.AreEqual( TruStatement.Interpret("{let {} true}"), new TruBool(true) );

            Assert.AreEqual( TruStatement.Interpret( // insure that the captured environment is separate from outer.
                "{let { [a false] [my-func {meta-func true}]} {my-func}}", testEnv),
                 new TruBool(true) );

            Assert.ThrowsException<System.ArgumentException>( () => TruStatement.Interpret("{let {x 3} x}", testEnv) );
        }

        [TestMethod]
        public void TestTruDef() {
            Environment global = TruLibrary.Library;
            TruStatement.Parse("{define var1 true}").Interpret(global);
            TruStatement.Parse("{define {my-func x} {and var1 x}}").Interpret(global);
            
            Assert.AreEqual( TruStatement.Parse("{my-func var1}").Interpret(global), new TruBool(true));
        }

        [TestMethod]
        public void Test4WayMultiplexer() {
            string program = @"
                ; Declare a 4-way multiplexer function and test it on multiple inputs, should return true if the tests pass.
                {define {multiplex4 s0 s1  i0 i1 i2 i3}
                    {let {[multiplex2 ; make a 2-way multiplex and combine them for the 4-way
                        {lambda {s0 i0 i1} {or {and {not s0} i0} {and s0 i1}}} ]}
                            {multiplex2 s1
                                {multiplex2 s0 i0 i1}
                                {multiplex2 s0 i2 i3}}}}

                ; test the multiplexer
                {multiplex4 false false  false false true false}
                {multiplex4 false false  true false false false}

                {multiplex4 false true   false false false true}
                {multiplex4 false true   true true true true}

                {multiplex4 true false   false false false true}
                {multiplex4 true false   false true true false}
                
                {multiplex4 true true    true false false false}
                {multiplex4 true true    false true false true}
            ";

            
            TruVal[] results = TruStatement.InterpretAll(program);
            Assert.IsTrue(Helpers.ArrayEquals(results, new TruVal[] {
                new TruBool(false), new TruBool(true), new TruBool(false), new TruBool(true),
                new TruBool(false), new TruBool(true), new TruBool(false), new TruBool(true),
            }));
        }
    }
}