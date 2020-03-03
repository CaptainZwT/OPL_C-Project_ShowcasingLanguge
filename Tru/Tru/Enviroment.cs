namespace Tru {
    /// Represents the environment of a Tru program as a mapping of names to values.
    public class Environment {

        /// A linked list. It is immutable.
        private class Node {
            public readonly string name;
            public readonly TruVal val;
            public readonly Node next;
            
            public Node(string name, TruVal val, Node next) {
                this.name = name; this.val = val; this.next = next;
            }

            /// Makes creates a new list that is the joining of this and other. This will come before other.
            public Node Append(Node other) {
                return new Node(this.name, this.val, (this.next != null) ? this.next.Append(other) : other);
            }
        }

        private Node list;

        private Environment(Node list) { this.list = list; }

        /// Creates an environment from a list of (string, TruVal) tuples.
        /// Values later in bindings are "higher" priority in case of name conflicts.
        public Environment( (string name, TruVal val)[] bindings = null ) {
            Node node = null;

            if (bindings != null)
                foreach (var binding in bindings)
                    node = new Node(binding.name, binding.val, node);

            this.list = node;
        }

        /// Returns a copy of this Environment.
        public Environment Copy() {
            return new Environment(this.list);
        }

        /// Returns the value corresponding to name in the environment, or throws an exception if
        /// if it doesn't exists.
        public TruVal Find(string name) {
            Node currentNode = this.list;

            while (currentNode != null && currentNode.name != name) {
                currentNode = currentNode.next;
            }

            if (currentNode == null) {
                throw new TruRuntimeException($"Free variable {name}.");
            } else {
                return currentNode.val;
            }

        }

        /// Create a new environment with the given binding added. Returns the new environment.
        /// The new binding will be higher priority than the old ones.
        public Environment ExtendLocal(string name, TruVal val) { // basically cons function
            return new Environment( new Node(name, val, this.list) );
        }

        /// Returns a new environment that contains all the bindings in this and env.
        /// Bindings in env will be higher priority than those in this.
        public Environment ExtendLocalAll(Environment env) {
            if (env.list != null) {
                Node newList = env.list.Append(this.list);
                return new Environment(newList);
            } else {
                return new Environment(this.list); // appending an empty environment does nothing.
            }
        }


        /// Modifies the current environment to contain a new binding.
        /// The same as ExtendLocal, except it mutates this instead of returning a new environment
        /// The new binding will be higher priority than the old ones.
        public void ExtendGlobal(string name, TruVal val) {
            this.list = new Node(name, val, this.list);
        }

        /// Modifies the current environment to contain a new binding.
        /// The same as ExtendLocal, except it mutates this instead of returning a new environment
        /// The new binding will be higher priority than the old ones.
        public void ExtendGlobal(Environment env) {
            if (env.list != null) {
                this.list = env.list.Append(this.list);
            }
            // appending an empty environment does nothing.
        }
    }
}