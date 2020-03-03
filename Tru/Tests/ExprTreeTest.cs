using NUnit.Framework;
using Tru;
using Utility;

namespace Tests
{
    public class ExprTreeTest
    {
        [Test]
        public void TestExprTreeEquals() {
            Assert.True(  new ExprLiteral("a").Equals( new ExprLiteral("a") ));
            Assert.False( new ExprLiteral("a").Equals( new ExprLiteral("b") ));
            Assert.False( new ExprLiteral("a").Equals( new ExprList() ));
            Assert.False( new ExprLiteral("a").Equals( null ));

            Assert.True(
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

            Assert.False(
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

        [Test]
        public void TestTokenize() {
            Assert.That(ExprTree.Tokenize("x"),
                Is.EqualTo( new List<string>{"x"} ));

            Assert.That(ExprTree.Tokenize("hello world"),
                Is.EqualTo( new List<string>{"hello", "world"} ));

            Assert.That(ExprTree.Tokenize(" hello  world "),
                Is.EqualTo( new List<string>{"hello", "world"} ));

            Assert.That(ExprTree.Tokenize("{a list }"),
                Is.EqualTo( new List<string>{"{", "a", "list", "}"} ));

            Assert.That(ExprTree.Tokenize("{a \n{nested}\n list}"),
                Is.EqualTo( new List<string>{"{", "a", "{", "nested", "}", "list", "}"} ));

            Assert.That(ExprTree.Tokenize("[other (types of) brackets]"),
                Is.EqualTo( new List<string>{"[", "other", "(", "types", "of", ")", "brackets", "]"} ));

            Assert.That(ExprTree.Tokenize("{}"),
                Is.EqualTo( new List<string>{"{", "}"} ));

            Assert.That(ExprTree.Tokenize(""),
                Is.EqualTo( new List<string>{} ));

            Assert.That(ExprTree.Tokenize(
                @"{a; this is a comment
                    {nested} ; another comment}
                list}"),
                Is.EqualTo( new List<string>{"{", "a", "{", "nested", "}", "list", "}"} ));
        }


        [Test]
        public void TestExprTreeParse() {
            Assert.That(ExprTree.Parse("x"),
                Is.EqualTo( new ExprLiteral("x") ));

            Assert.That(ExprTree.Parse("{a  list }"),
                Is.EqualTo( new ExprList(new ExprTree[]{ new ExprLiteral("a"), new ExprLiteral("list") }) ));

            Assert.That(ExprTree.Parse("{a {nested} list}"),
                Is.EqualTo( new ExprList(new ExprTree[]{
                    new ExprLiteral("a"),
                    new ExprList(new ExprTree[]{
                        new ExprLiteral("nested")
                    }),
                    new ExprLiteral("list")
                }) ));

            Assert.That(ExprTree.Parse("{}"),
                Is.EqualTo( new ExprList() ));

            Assert.That(ExprTree.Parse("[other (types of) brackets]"),
                Is.EqualTo( new ExprList(new ExprTree[]{
                    new ExprLiteral("other"),
                    new ExprList(new ExprTree[]{
                        new ExprLiteral("types"),
                        new ExprLiteral("of")
                    }),
                    new ExprLiteral("brackets")
                }) ));


            Assert.Throws<System.ArgumentException>( () => ExprTree.Parse("hello world") );
            Assert.Throws<System.ArgumentException>( () => ExprTree.Parse("") );
            Assert.Throws<System.ArgumentException>( () => ExprTree.Parse("}") );
            Assert.Throws<System.ArgumentException>( () => ExprTree.Parse("{") );
            Assert.Throws<System.ArgumentException>( () => ExprTree.Parse("[mismatched}") );
            Assert.Throws<System.ArgumentException>( () => ExprTree.Parse("(missing") );
            Assert.Throws<System.ArgumentException>( () => ExprTree.Parse("missing)") );
        }

        [Test]
        public void TestExprTreeParseAll() {
            Assert.True( Helpers.ArrayEquals(ExprTree.ParseAll("x"), new[]{new ExprLiteral("x")}) );

            Assert.True( Helpers.ArrayEquals(ExprTree.ParseAll("hello world"),
                new[]{new ExprLiteral("hello"), new ExprLiteral("world")}) );


            Assert.True( Helpers.ArrayEquals(ExprTree.ParseAll("{a list} x"),
                new ExprTree[]{
                    new ExprList(new ExprTree[]{
                        new ExprLiteral("a"),
                        new ExprLiteral("list")
                    }),
                new ExprLiteral("x")
            } ));

            Assert.Throws<System.ArgumentException>( () => ExprTree.Parse("missing)") );
        }

        [Test]
        public void TestMatch() {
            Assert.True(  ExprTree.Match("ANY", ExprTree.Parse("words"))  );

            Assert.True(  ExprTree.Match("ANY", ExprTree.Parse("{a list}"))  );

            Assert.True(  ExprTree.Match("ANY", ExprTree.Parse("ANY"))  );

            Assert.False( ExprTree.Match("{ANY}", ExprTree.Parse("word"))  );


            Assert.True(  ExprTree.Match("LITERAL", ExprTree.Parse("words"))  );

            Assert.True(  ExprTree.Match("LITERAL", ExprTree.Parse("LITERAL"))  );

            Assert.False( ExprTree.Match("LITERAL", ExprTree.Parse("{a list}"))  );


            Assert.True(  ExprTree.Match("{a list}", ExprTree.Parse("{a  list}"))  );

            Assert.True(  ExprTree.Match("{}", ExprTree.Parse("{}"))  );

            Assert.False( ExprTree.Match("{a list}", ExprTree.Parse("{a different list}"))  );


            Assert.True(  ExprTree.Match("{append ANY ANY}", ExprTree.Parse("{append 'word' {reverse 'palindrome'}}"))  );

            Assert.False( ExprTree.Match("{append ANY LITERAL}", ExprTree.Parse("{append 'word' {reverse 'palindrome'}}"))  );

            Assert.False( ExprTree.Match("{append ANY ANY}", ExprTree.Parse("{+ 'word' {reverse 'palindrome'}}"))  );

            Assert.False( ExprTree.Match("{append ANY ANY}", ExprTree.Parse("{append 'word'}"))  );

            Assert.False( ExprTree.Match("{append ANY ANY}", ExprTree.Parse("{append 'word' 'word1' 'word2'}"))  );

            Assert.False( ExprTree.Match("{append {LITERAL ANY} ANY}", ExprTree.Parse("{append 'word' 'word1'}"))  );

            Assert.True(  ExprTree.Match("{append {LITERAL ANY end} ANY}", ExprTree.Parse("{append {reverse 'palindrome' end} 'word1'}"))  );

            Assert.False( ExprTree.Match("{append {LITERAL ANY end} ANY}", ExprTree.Parse("{append {reverse 'palindrome' start} 'word1'}"))  );

            Assert.True( ExprTree.Match("LIST", ExprTree.Parse("{a list}")) );

            Assert.True(  ExprTree.Match("{a ...}", ExprTree.Parse("{}")) );
            Assert.True(  ExprTree.Match("{a ...}", ExprTree.Parse("{a}")) );
            Assert.True(  ExprTree.Match("{[LITERAL LITERAL] ...}", ExprTree.Parse("{[a b]}")) );
            Assert.True(  ExprTree.Match("{a ...}", ExprTree.Parse("{a a a}")) );
            Assert.False( ExprTree.Match("{a b ...}", ExprTree.Parse("{a a b b}")) );
            Assert.True(  ExprTree.Match("{a b ... b}", ExprTree.Parse("{a b}")) );
            Assert.False( ExprTree.Match("{a b ... b}", ExprTree.Parse("{a b c}")) );
            Assert.False( ExprTree.Match("{a b ... b c}", ExprTree.Parse("{a b b}")) );
            Assert.False( ExprTree.Match("{a b ... b}", ExprTree.Parse("{a}")) );
            Assert.True(  ExprTree.Match("{a b b ...}", ExprTree.Parse("{a b}")) );
            Assert.False( ExprTree.Match("{b b ...}", ExprTree.Parse("{a b}")) );
            Assert.True(  ExprTree.Match("{b b ...}", ExprTree.Parse("{b}")) );
            Assert.False( ExprTree.Match("{b b b ...}", ExprTree.Parse("{b}")) );
            Assert.False( ExprTree.Match("{a b b ... c}", ExprTree.Parse("{a b b}")) );
            Assert.False( ExprTree.Match("{a b b ...}", ExprTree.Parse("{a}")) );
            Assert.True(  ExprTree.Match("{b ... c ...}", ExprTree.Parse("{b b b c c c}")) );
            Assert.False( ExprTree.Match("{b ... c ...}", ExprTree.Parse("{b b b c c c d}")) );
        }
    }
}