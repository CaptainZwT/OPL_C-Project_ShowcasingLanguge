using NUnit.Framework;
using Tru;

namespace Tests
{
    public class EnvironmentTest
    {
        [Test]
        public void TestEnvironment() {
            Environment env = new Environment( new (string, TruVal)[] {
                ("a", new TruBool(true) ),
                ("b", new TruBool(false)),
            });
            Environment env2 = new Environment( new (string, TruVal)[] {
                ("a", new TruBool(false)),
                ("c", new TruBool(false)),
            });
            Environment empty = new Environment();

            Assert.That(env.Find("a"), Is.EqualTo(new TruBool(true)));
            Assert.That(env.Find("b"), Is.EqualTo(new TruBool(false)));


            env2.AddAll(env);
            env2.Add("d", new TruBool(true));
            env2.AddAll( new Environment() );

            Assert.That(env2.Find("a"), Is.EqualTo(new TruBool(false)));
            Assert.That(env2.Find("d"), Is.EqualTo(new TruBool(true)));
            Assert.That(env.Find("a"), Is.EqualTo(new TruBool(true))); // Original env is not modified.

            Assert.Throws<System.ArgumentException>( () => env.Find("notHere") );
            Assert.Throws<System.ArgumentException>( () => empty.Find("a") );

            empty.Add("d", new TruBool(true)); // make sure that empty doesn't crash when added to.
            Assert.That(empty.Find("d"), Is.EqualTo(new TruBool(true)));
        }
    }
}