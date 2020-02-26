using NUnit.Framework;
using Utility;

namespace Tests
{
    public class ListTest
    {

        [Test]
        public void TestList() {
            List<int> empty = new List<int>();

            List<string> list = new List<string>();
            list.Add("Hello");
            list.Add("World");


            // Assert.True( list.Equals(new List<string> {"Hello", "World"}) ); // Initializer syntax tests GetEnumerator
            // Assert.True( empty.Equals( new List<int>() ));
            // Assert.False( list.Equals(new List<string> {"Hello", "World", "2"}) );

            Assert.That( empty.Count, Is.EqualTo( 0 ) );
            Assert.That( list.Count, Is.EqualTo( 2 ) );

            Assert.That( list[0], Is.EqualTo( "Hello" ) );
            Assert.That( list[1], Is.EqualTo( "World" ) );
            Assert.Throws<System.IndexOutOfRangeException>( () => list[2].ToString() );

            Assert.That( list.ToArray().Length, Is.EqualTo(2) );
            Assert.That( list.ToArray()[0], Is.EqualTo("Hello") );
            Assert.That( list.ToArray()[1], Is.EqualTo("World") );
        }

    }
}