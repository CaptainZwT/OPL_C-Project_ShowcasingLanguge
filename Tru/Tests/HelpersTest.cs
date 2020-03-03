using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utility;

namespace Tests
{
    [TestClass]
    public class HelpersTest
    {

        [TestMethod]
        public void TestList() {
            List<int> empty = new List<int>();

            List<string> list = new List<string>();
            list.Add("Hello");
            list.Add("World");

            // Assert.IsTrue( list.Equals(new List<string> {"Hello", "World"}) ); // Initializer syntax tests GetEnumerator
            // Assert.IsTrue( empty.Equals( new List<int>() ));
            // Assert.IsFalse( list.Equals(new List<string> {"Hello", "World", "2"}) );

            Assert.AreEqual( empty.Count,  0 ) ;
            Assert.AreEqual( list.Count,  2 ) ;

            Assert.AreEqual( list[0],  "Hello" ) ;
            Assert.AreEqual( list[1],  "World" ) ;
            Assert.ThrowsException<System.IndexOutOfRangeException>( () => list[2].ToString() );

            Assert.AreEqual( list.ToArray().Length, 2) ;
            Assert.AreEqual( list.ToArray()[0], "Hello") ;
            Assert.AreEqual( list.ToArray()[1], "World") ;
        }

        [TestMethod]
        public void TestLargeList() {
            List<int> list = new List<int>();

            for (int i = 0; i < 100; i++) {
                list.Add(i);
            }

            Assert.AreEqual( list.Count,  100 ) ;
            for (int i = 0; i < 100; i++) {
                Assert.AreEqual( list[i],  i ) ;
            }
        }

        [TestMethod]
        public void TestArrayEquals() {
            string[] list = new string[] {"Hello", "World"};
            int[] empty = new int[] {};

            Assert.IsTrue( Helpers.ArrayEquals(list, new string[] {"Hello", "World"}) );
            Assert.IsTrue( Helpers.ArrayEquals(empty, new int[] {} ));
            Assert.IsFalse( Helpers.ArrayEquals(list, new string[] {"Hello", "World", "2"}) );
        }

    }
}