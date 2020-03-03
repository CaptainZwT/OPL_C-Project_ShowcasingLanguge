using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tru;

namespace Tests
{
    [TestClass]
    public class EnvironmentTest
    {
        [TestMethod]
        public void TestEnvironment() {
            Environment env1 = new Environment( new (string, TruVal)[] {
                ("a", new TruBool(true) ),
                ("b", new TruBool(false)),
            });
            Environment env2 = new Environment( new (string, TruVal)[] {
                ("b", new TruBool(true)),
                ("c", new TruBool(false)),
                ("c", new TruBool(true)),
            });
            Environment env3 = env1.ExtendLocal("a", new TruBool(false));
            Environment empty = new Environment();

            Assert.AreEqual(env1.Find("a"), new TruBool(true));
            Assert.AreEqual(env3.Find("a"), new TruBool(false));

            Assert.ThrowsException<TruRuntimeException>( () => env1.Find("notHere") );
            Assert.ThrowsException<TruRuntimeException>( () => empty.Find("a") );

            Assert.AreEqual(env2.Find("c"), new TruBool(true)); // Later values take precedence

            Environment env4 = empty.ExtendLocal("c", new TruBool(true)); // make sure that empty doesn't crash when added to.
            Assert.AreEqual(env4.Find("c"), new TruBool(true));

            Environment env5 = env1.ExtendLocalAll(env2);
            Assert.AreEqual(env5.Find("a"), new TruBool(true));
            Assert.AreEqual(env5.Find("b"), new TruBool(true));
            Assert.AreEqual(env5.Find("c"), new TruBool(true));

            Environment env6 = env2.ExtendLocalAll(empty); // make sure that empty doesn't crash when added all to.
            Environment env7 = empty.ExtendLocalAll(env2);

            env1.ExtendGlobal("z", new TruBool(true));
            Assert.AreEqual(env1.Find("z"), new TruBool(true));
            env1.ExtendGlobal(env2);
            Assert.AreEqual(env1.Find("c"), new TruBool(true));
        }
    }
}