using Utility;
using System; // For array

namespace Tru {

    /// Represents an expression in the Tru Language.
    public abstract class TruExpr {
        /// Parses a string into Tru abstract syntax that can be interpreted.
        public static TruExpr Parse(string code) { return ParseHelper( ExprTree.Parse(code) ); }

        private static TruExpr ParseHelper(ExprTree expr) {
            if (expr is ExprLiteral exprLit) {
                if ( ExprTree.Match("true", expr) ) {
                    return new TruBool(true);
                } else if ( ExprTree.Match("false", expr) ) {
                    return new TruBool(false);
                } else {
                    return new TruId(exprLit.val);
                }
            } else if (expr is ExprList exprList && exprList.list.Length > 0) { // cast expr into exprList

                TruExpr[] args = new TruExpr[exprList.list.Length - 1];
                for (int i = 0; i < exprList.list.Length - 1; i++) { // Parse all but first in list into arguments
                    args[i] = ParseHelper(exprList.list[i + 1]);
                }

                return new TruCall(ParseHelper(exprList.list[0]), args);

            }

            throw new System.ArgumentException("Invalid syntax.");
        }

        /// Interprets a TruExpr with the standard library.
        public TruVal Interpret() { return this.Interpret(TruLibrary.library); }

        /// Interprets and returns a TruVal from this TruExpr.
        public abstract TruVal Interpret(Environment env);
    }

    /// Represents a value or literal in the Tru language.
    public abstract class TruVal : TruExpr {
        public override TruVal Interpret(Environment env) { return this; } // All TruVals evaluate to themselves.
    }

    /// Represents a bool value in the Tru langauge
    public class TruBool : TruVal {
        public bool val;
        public TruBool(bool val) { this.val = val; }

        public override string ToString() { return this.val ? "true" : "false"; }
        public override bool Equals(object obj) { return obj is TruBool truVal && truVal.val == this.val; }
    }

    /// Represents a callable, ie built-in or functions.
    public abstract class TruCallable : TruVal {
        /// Calls a callable.
        public abstract TruVal call(Environment env, TruExpr[] args);
    }

    /// Represents a built-in function, ie and, or, not ...
    public class TruBuiltIn : TruCallable {
        /// Note that TruOperation takes a TruExpr[], so it is responsible for calling interpret on its inputs.
        /// This allows for short-circuit evaluation in builtins.
        public delegate TruVal TruOperation(Environment env, TruExpr[] parameters);
        public TruOperation op;

        public TruBuiltIn(TruOperation op) { this.op = op; }

        public override TruVal call(Environment env, TruExpr[] args) {
            return this.op(env, args);
        }

        public override string ToString() { return "built-in"; }
        public override bool Equals(object obj) { return obj is TruBuiltIn truBuiltIn && truBuiltIn.op == this.op; }
    }

    /// Represents a user defined function.
    public class TruFunc : TruCallable {
        public string[] parameters;
        public TruExpr body;
        public Environment env; // Lambda closures.

        public TruFunc(string[] parameters, TruExpr body, Environment env = null) {
            this.parameters = parameters;
            this.body = body;
            this.env = env ?? new Environment();
        }

        public override TruVal call(Environment env, TruExpr[] args) {
            if (this.parameters.Length == args.Length) {

                Environment localEnv = new Environment();
                for (int i = 0; i < args.Length; i++) {
                    localEnv.Add(this.parameters[i], args[i].Interpret(env));
                }

                localEnv.AddAll(this.env);
                return this.body.Interpret(localEnv);

            } else {
                throw new System.Exception("Call with wrong number of paramaters.");

            }
        }

        public override string ToString() {
            return "{lambda {" + string.Join(" ", this.parameters) + "} " + body + "}";
        }

        public override bool Equals(object obj) {
            return obj is TruFunc truFunc && Helpers.ArrayEquals(this.parameters, truFunc.parameters) &&
                this.body.Equals(truFunc.body) && this.env.Equals(truFunc.env);
        }
    }

    /// Represents a binding to a value.
    public class TruId : TruExpr {
        public string name;
        public TruId(string name) { this.name = name; }
        public override TruVal Interpret(Environment env) { return env.Find(this.name); }

        public override string ToString() { return this.name; }
        public override bool Equals(object obj) { return obj is TruId truid && this.name == truid.name; }
    }

    /// Represents a call to a function or a built-in
    public class TruCall : TruExpr {
        public TruExpr func;
        public TruExpr[] args;
        public TruCall(TruExpr func, TruExpr[] args) { this.func = func; this.args = args; }
        public override TruVal Interpret(Environment env) {
            TruVal funcVal = func.Interpret(env);

            if (funcVal is TruCallable callable) {
                return callable.call(env, this.args);
            } else {
                throw new System.ArgumentException($"{funcVal} is not callable.");
            }
        }

        public override string ToString() {
            return "{" + string.Join(" ", this.func, string.Join(" ", (object[]) this.args)) + "}";
        }
        public override bool Equals(object obj) {
            return obj is TruCall truCall && this.func.Equals(truCall.func) && Helpers.ArrayEquals(this.args, truCall.args);
        }
    }

}