namespace Tru {
    /// Represents the environment of a Tru program as a mapping of names to values.
    public class Environment {
        private class Node {
            public string name;
            public TruVal val;
            public Node next; // Environment will be a linked list.

            public Node(string name, TruVal val, Node next) {
                this.name = name; this.val = val; this.next = next;
            }

        }

        private Node list;

        /// Creates an environment from a list of (string, TruVal) tuples.
        /// Values later in bindings are "higher" priority in case of name conflicts.
        public Environment( (string name, TruVal val)[] bindings = null ) {
            Node node = null;

            if (bindings != null)
                foreach (var binding in bindings)
                    node = new Node(binding.name, binding.val, node);

            this.list = node;
        }

        /// Returns the value corresponding to name in the enviroment, or throws an exception if
        /// if it doesn't exists.
        public TruVal Find(string name) {
            Node currentNode = this.list;

            while (currentNode != null && currentNode.name != name) {
                currentNode = currentNode.next;
            }

            if (currentNode == null) {
                throw new System.ArgumentException($"Free variable {name}.");
            } else {
                return currentNode.val;
            }

        }

        /// Joins other to this environment. This will be mutated but not other.
        /// In case of name conflicts, bindings in this will be higher priority than those in others.
        public void AddAll(Environment other) {
            if (this.list == null) { // just set this list to other list.
                this.list = other.list;
            } else {
                Node currentNode = this.list;

                while (currentNode.next != null) {
                    currentNode = currentNode.next;
                }

                currentNode.next = other.list;
            }
        }

        /// Adds one binding to this environment.
        /// The original bindings will be higher priority than the new ones.
        /// Like plait/lisp cons.
        public void Add(string name, TruVal val) {
            this.AddAll( new Environment( new[] {(name, val)} ) );
        }
    }
}