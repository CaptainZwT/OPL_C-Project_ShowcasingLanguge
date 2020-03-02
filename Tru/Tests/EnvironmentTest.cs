using NUnit.Framework;
using Tru;

namespace Tests
{
    public class EnvironmentTest
    {
        [Test]
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

            Assert.That(env1.Find("a"), Is.EqualTo(new TruBool(true)));
            Assert.That(env3.Find("a"), Is.EqualTo(new TruBool(false)));

            Assert.Throws<System.ArgumentException>( () => env1.Find("notHere") );
            Assert.Throws<System.ArgumentException>( () => empty.Find("a") );

            Assert.That(env2.Find("c"), Is.EqualTo(new TruBool(true))); // Later values take precedence

            Environment env4 = empty.ExtendLocal("c", new TruBool(true)); // make sure that empty doesn't crash when added to.
            Assert.That(env4.Find("c"), Is.EqualTo(new TruBool(true)));

            Environment env5 = env1.ExtendLocalAll(env2);
            Assert.That(env5.Find("a"), Is.EqualTo(new TruBool(true)));
            Assert.That(env5.Find("b"), Is.EqualTo(new TruBool(true)));
            Assert.That(env5.Find("c"), Is.EqualTo(new TruBool(true)));

            Environment env6 = env2.ExtendLocalAll(empty); // make sure that empty doesn't crash when added all to.
            Environment env7 = empty.ExtendLocalAll(env2);

            env1.ExtendGlobal("z", new TruBool(true));
            Assert.That(env1.Find("z"), Is.EqualTo(new TruBool(true)));
            env1.ExtendGlobal(env2);
            Assert.That(env1.Find("c"), Is.EqualTo(new TruBool(true)));
        }
    }
}