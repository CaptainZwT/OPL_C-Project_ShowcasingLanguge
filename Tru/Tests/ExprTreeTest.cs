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
                new ExprList(new List<ExprTree>{
                    new ExprLiteral("a"),
                    new ExprList(new List<ExprTree>{
                        new ExprLiteral("b")
                    })
                }).Equals(
                new ExprList(new List<ExprTree>{
                    new ExprLiteral("a"),
                    new ExprList(new List<ExprTree>{
                        new ExprLiteral("b")
                    })
                })
            ));

            Assert.False(
                new ExprList(new List<ExprTree>{
                    new ExprLiteral("a"),
                    new ExprList(new List<ExprTree>{
                        new ExprLiteral("b")
                    })
                }).Equals(
                new ExprList(new List<ExprTree>{
                    new ExprLiteral("a"),
                    new ExprList(new List<ExprTree>{
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

            Assert.That(ExprTree.Tokenize("{a {nested} list}"),
                Is.EqualTo( new List<string>{"{", "a", "{", "nested", "}", "list", "}"} ));

            Assert.That(ExprTree.Tokenize("[other (types of) brackets]"),
                Is.EqualTo( new List<string>{"[", "other", "(", "types", "of", ")", "brackets", "]"} ));

            Assert.That(ExprTree.Tokenize("{}"),
                Is.EqualTo( new List<string>{"{", "}"} ));

            Assert.That(ExprTree.Tokenize(""),
                Is.EqualTo( new List<string>{} ));
        }


        [Test]
        public void TestExprTreeParse() {
            Assert.That(ExprTree.Parse("x"),
                Is.EqualTo( new ExprLiteral("x") ));

            Assert.That(ExprTree.Parse("{a  list }"),
                Is.EqualTo( new ExprList(new List<ExprTree>{ new ExprLiteral("a"), new ExprLiteral("list") }) ));

            Assert.That(ExprTree.Parse("{a {nested} list}"),
                Is.EqualTo( new ExprList(new List<ExprTree>{
                    new ExprLiteral("a"),
                    new ExprList(new List<ExprTree>{
                        new ExprLiteral("nested")
                    }),
                    new ExprLiteral("list")
                }) ));

            Assert.That(ExprTree.Parse("{}"),
                Is.EqualTo( new ExprList( new List<ExprTree>() ) ));

            Assert.That(ExprTree.Parse("[other (types of) brackets]"),
                Is.EqualTo( new ExprList(new List<ExprTree>{
                    new ExprLiteral("other"),
                    new ExprList(new List<ExprTree>{
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

        }
    }
}