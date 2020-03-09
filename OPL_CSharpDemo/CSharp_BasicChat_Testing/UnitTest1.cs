using CSharp_BasicChat_Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Sockets;

namespace CSharp_BasicChat_Testing
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Unit Testing

            // Create 2 TcpClients that send a message each
            // Check they both recieve their messages

            TcpClient Client1 = new TcpClient("127.0.0.1", 8888);
            TcpClient Client2 = new TcpClient("127.0.0.1", 8888);

            Client1.WriteString("CaptainTest");
            Client2.WriteString("CalvaryTest");

            // Connecting to the server (by passing a username) per normal server functionality

            string joinmsg1 = "#(CaptainTest joined.";
            string joinmsg2 = "#(CalvaryTest joined.";

            string incmsg1 = Client1.ReadString();
            string incmsg2 = Client2.ReadString();
            string incmsg3 = Client1.ReadString();

            // checking that the output of both clients should be their own join messages
            Assert.AreEqual(joinmsg1, incmsg1);
            Assert.AreEqual(joinmsg2, incmsg2);

            // Client 1 should then have a join message for Client2
            StringAssert.Contains(incmsg3, joinmsg2);


            // Message Sequence 1

            string _msg1 = "hello";

            Client1.WriteString(_msg1);
            string msg1 = Client1.ReadString();
            string msg2 = Client2.ReadString();


            // Testing Message Sequences
            StringAssert.Contains(msg1, _msg1);
            StringAssert.Contains(msg2, _msg1);

            // Message Sequence 2

            string _msg2 = "sup";

            Client2.WriteString(_msg2);
            string msg3 = Client1.ReadString();
            string msg4 = Client2.ReadString();


            // Testing Message Sequences
            StringAssert.Contains(msg3, _msg2);
            StringAssert.Contains(msg4, _msg2);

        }
    }
}
