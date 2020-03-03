using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tru;
using Utility;

namespace Tests
{
    [TestClass]
    public class ExprTreeTest
    {
        [TestMethod]
        public void TestExprTreeEquals() {
            Assert.IsTrue(  new ExprLiteral("a").Equals( new ExprLiteral("a") ));
            Assert.IsFalse( new ExprLiteral("a").Equals( new ExprLiteral("b") ));
            Assert.IsFalse( new ExprLiteral("a").Equals( new ExprList() ));
            Assert.IsFalse( new ExprLiteral("a").Equals( null ));

            Assert.IsTrue(
                new ExprList(new ExprTree[]{
                    new ExprLiteral("a"),
                    new ExprList(new ExprTree[]{
                        new ExprLiteral("b")
                    })
                }).Equals(
                new ExprList(new ExprTree[]{
                    new ExprLiteral("a"),
                    new ExprList(new ExprTree[]{
                        new ExprLiteral("b")
                    })
                })
            ));

            Assert.IsFalse(
                new ExprList(new ExprTree[]{
                    new ExprLiteral("a"),
                    new ExprList(new ExprTree[]{
                        new ExprLiteral("b")
                    })
                }).Equals(
                new ExprList(new ExprTree[]{
                    new ExprLiteral("a"),
                    new ExprList(new ExprTree[]{
                        new ExprLiteral("c")
                    })
                })
            ));
        }

        [TestMethod]
        public void TestTokenize() {
            CollectionAssert.AreEqual(ExprTree.Tokenize("x").ToArray(), new[]{"x"} );

            CollectionAssert.AreEqual(ExprTree.Tokenize("hello world").ToArray(), new[]{"hello", "world"} );

            CollectionAssert.AreEqual(ExprTree.Tokenize(" hello  world ").ToArray(), new[]{"hello", "world"} );

            CollectionAssert.AreEqual(ExprTree.Tokenize("{a list }").ToArray(), new[]{"{", "a", "list", "}"} );

            CollectionAssert.AreEqual(ExprTree.Tokenize("{a \n{nested}\n list}").ToArray(),
                new[]{"{", "a", "{", "nested", "}", "list", "}"} );

            CollectionAssert.AreEqual(ExprTree.Tokenize("[other (types of) brackets]").ToArray(),
                new[]{"[", "other", "(", "types", "of", ")", "brackets", "]"} );

            CollectionAssert.AreEqual(ExprTree.Tokenize("{}").ToArray(), new[]{"{", "}"} );

            CollectionAssert.AreEqual(ExprTree.Tokenize("").ToArray(), new string[]{} );

            CollectionAssert.AreEqual(ExprTree.Tokenize(
                @"{a; this is a comment
                    {nested} ; another comment}
                   list}").ToArray(),
                new[]{"{", "a", "{", "nested", "}", "list", "}"}
            );
        }


        [TestMethod]
        public void TestExprTreeParse() {
            Assert.AreEqual(ExprTree.Parse("x"), new ExprLiteral("x") );

            Assert.AreEqual(ExprTree.Parse("{a  list }"),
                 new ExprList(new ExprTree[]{ new ExprLiteral("a"), new ExprLiteral("list") }) );

            Assert.AreEqual(ExprTree.Parse("{a {nested} list}"),
                new ExprList(new ExprTree[]{
                    new ExprLiteral("a"),
                    new ExprList(new ExprTree[]{
                        new ExprLiteral("nested")
                    }),
                    new ExprLiteral("list")
            }) );

            Assert.AreEqual(ExprTree.Parse("{}"), new ExprList() );

            Assert.AreEqual(ExprTree.Parse("[other (types of) brackets]"),
                new ExprList(new ExprTree[]{
                    new ExprLiteral("other"),
                    new ExprList(new ExprTree[]{
                        new ExprLiteral("types"),
                        new ExprLiteral("of")
                    }),
                    new ExprLiteral("brackets")
            }) );


            Assert.ThrowsException<TruSyntaxError>( () => ExprTree.Parse("hello world") );
            Assert.ThrowsException<TruSyntaxError>( () => ExprTree.Parse("") );
            Assert.ThrowsException<TruSyntaxError>( () => ExprTree.Parse("}") );
            Assert.ThrowsException<TruSyntaxError>( () => ExprTree.Parse("{") );
            Assert.ThrowsException<TruSyntaxError>( () => ExprTree.Parse("[mismatched}") );
            Assert.ThrowsException<TruSyntaxError>( () => ExprTree.Parse("(missing") );
            Assert.ThrowsException<TruSyntaxError>( () => ExprTree.Parse("missing)") );
        }

        [TestMethod]
        public void TestExprTreeParseAll() {
            Assert.IsTrue( Helpers.ArrayEquals(ExprTree.ParseAll("x"), new[]{new ExprLiteral("x")}) );

            Assert.IsTrue( Helpers.ArrayEquals(ExprTree.ParseAll("hello world"),
                new[]{new ExprLiteral("hello"), new ExprLiteral("world")}) );


            Assert.IsTrue( Helpers.ArrayEquals(ExprTree.ParseAll("{a list} x"),
                new ExprTree[]{
                    new ExprList(new ExprTree[]{
                        new ExprLiteral("a"),
                        new ExprLiteral("list")
                    }),
                new ExprLiteral("x")
            } ));

            Assert.ThrowsException<TruSyntaxError>( () => ExprTree.Parse("missing)") );
        }

        [TestMethod]
        public void TestMatch() {
            Assert.IsTrue(  ExprTree.Match("ANY", ExprTree.Parse("words"))  );

            Assert.IsTrue(  ExprTree.Match("ANY", ExprTree.Parse("{a list}"))  );

            Assert.IsTrue(  ExprTree.Match("ANY", ExprTree.Parse("ANY"))  );

            Assert.IsFalse( ExprTree.Match("{ANY}", ExprTree.Parse("word"))  );


            Assert.IsTrue(  ExprTree.Match("LITERAL", ExprTree.Parse("words"))  );

            Assert.IsTrue(  ExprTree.Match("LITERAL", ExprTree.Parse("LITERAL"))  );

            Assert.IsFalse( ExprTree.Match("LITERAL", ExprTree.Parse("{a list}"))  );


            Assert.IsTrue(  ExprTree.Match("{a list}", ExprTree.Parse("{a  list}"))  );

            Assert.IsTrue(  ExprTree.Match("{}", ExprTree.Parse("{}"))  );

            Assert.IsFalse( ExprTree.Match("{a list}", ExprTree.Parse("{a different list}"))  );


            Assert.IsTrue(  ExprTree.Match("{append ANY ANY}", ExprTree.Parse("{append 'word' {reverse 'palindrome'}}"))  );

            Assert.IsFalse( ExprTree.Match("{append ANY LITERAL}", ExprTree.Parse("{append 'word' {reverse 'palindrome'}}"))  );

            Assert.IsFalse( ExprTree.Match("{append ANY ANY}", ExprTree.Parse("{+ 'word' {reverse 'palindrome'}}"))  );

            Assert.IsFalse( ExprTree.Match("{append ANY ANY}", ExprTree.Parse("{append 'word'}"))  );

            Assert.IsFalse( ExprTree.Match("{append ANY ANY}", ExprTree.Parse("{append 'word' 'word1' 'word2'}"))  );

            Assert.IsFalse( ExprTree.Match("{append {LITERAL ANY} ANY}", ExprTree.Parse("{append 'word' 'word1'}"))  );

            Assert.IsTrue(  ExprTree.Match("{append {LITERAL ANY end} ANY}", ExprTree.Parse("{append {reverse 'palindrome' end} 'word1'}"))  );

            Assert.IsFalse( ExprTree.Match("{append {LITERAL ANY end} ANY}", ExprTree.Parse("{append {reverse 'palindrome' start} 'word1'}"))  );

            Assert.IsTrue(  ExprTree.Match("LIST", ExprTree.Parse("{a list}")) );

            Assert.IsTrue(  ExprTree.Match("{a ...}",       ExprTree.Parse("{}")) );
            Assert.IsTrue(  ExprTree.Match("{a ...}",       ExprTree.Parse("{a}")) );
            Assert.IsTrue(  ExprTree.Match("{[LITERAL LITERAL] ...}", ExprTree.Parse("{[a b]}")) );
            Assert.IsTrue(  ExprTree.Match("{a ...}",       ExprTree.Parse("{a a a}")) );
            Assert.IsFalse( ExprTree.Match("{a b ...}",     ExprTree.Parse("{a a b b}")) );
            Assert.IsTrue(  ExprTree.Match("{a b ... b}",   ExprTree.Parse("{a b}")) );
            Assert.IsFalse( ExprTree.Match("{a b ... b}",   ExprTree.Parse("{a b c}")) );
            Assert.IsFalse( ExprTree.Match("{a b ... b c}", ExprTree.Parse("{a b b}")) );
            Assert.IsFalse( ExprTree.Match("{a b ... b}",   ExprTree.Parse("{a}")) );
            Assert.IsTrue(  ExprTree.Match("{a b b ...}",   ExprTree.Parse("{a b}")) );
            Assert.IsFalse( ExprTree.Match("{b b ...}",     ExprTree.Parse("{a b}")) );
            Assert.IsTrue(  ExprTree.Match("{b b ...}",     ExprTree.Parse("{b}")) );
            Assert.IsFalse( ExprTree.Match("{b b b ...}",   ExprTree.Parse("{b}")) );
            Assert.IsFalse( ExprTree.Match("{a b b ... c}", ExprTree.Parse("{a b b}")) );
            Assert.IsFalse( ExprTree.Match("{a b b ...}",   ExprTree.Parse("{a}")) );
            Assert.IsTrue(  ExprTree.Match("{b ... c ...}", ExprTree.Parse("{b b b c c c}")) );
            Assert.IsFalse( ExprTree.Match("{b ... c ...}", ExprTree.Parse("{b b b c c c d}")) );
        }
    }
}