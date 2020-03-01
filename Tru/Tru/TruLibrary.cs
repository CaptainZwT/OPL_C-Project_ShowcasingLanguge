using System; // For array.

namespace Tru {
    /// Contains the default standard library.
    public static class TruLibrary {

        private static TruVal nand(Environment env, TruExpr[] parms) {
            if (parms.Length == 2 && parms[0].Interpret(env) is TruBool a && parms[1].Interpret(env) is TruBool b) {
                return new TruBool( !(a.val && b.val) );
            } else {
                throw new System.ArgumentException("Invalid types or parameter count.");
            }
        }

        private static TruVal and(Environment env, TruExpr[] parms) {
            if (parms.Length == 2 && parms[0].Interpret(env) is TruBool a && parms[1].Interpret(env) is TruBool b) {
                return new TruBool( a.val && b.val );
            } else {
                throw new System.ArgumentException("Invalid types or parameter count.");
            }
        }

        private static TruVal or(Environment env, TruExpr[] parms) {
            if (parms.Length == 2 && parms[0].Interpret(env) is TruBool a && parms[1].Interpret(env) is TruBool b) {
                return new TruBool( a.val || b.val );
            } else {
                throw new System.ArgumentException("Invalid types or parameter count.");
            }
        }

        private static TruVal not(Environment env, TruExpr[] parms) {
            if (parms.Length == 1 && parms[0].Interpret(env) is TruBool a) {
                return new TruBool( !a.val );
            } else {
                throw new System.ArgumentException("Invalid types or parameter count.");
            }
        }

        private static TruVal equals(Environment env, TruExpr[] parms) { // for bools, same as xnor, but this way it works for other types.
            if (parms.Length == 2) { // equals will work on all types, and returns a TruBool
                return new TruBool( parms[0].Interpret(env).Equals(parms[1].Interpret(env)) );
            } else {
                throw new System.ArgumentException("Invalid types or parameter count.");
            }
        }

        private static TruVal ifStatement(Environment env, TruExpr[] parms) {
            if (parms.Length == 3 && parms[0].Interpret(env) is TruBool cond) {
                return cond.val ? parms[1].Interpret(env) : parms[2].Interpret(env);
            } else {
                throw new System.ArgumentException("Invalid types or parameter count.");
            }
        }



        private static (string name, TruBuiltIn.TruOperation op)[] _builtins = new (string, TruBuiltIn.TruOperation)[] {
            ("nand",    nand),
            ("and",     and),
            ("or",      or),
            ("not",     not),
            ("equals",  equals),
            ("if",      ifStatement)
        };

        private static (string name, string def)[] _library = new (string, string)[] {
            ("nor",      "{lambda {a b} {not {or a b}}}"),
            ("xor",      "{lambda {a b} {and {or a b} {nand a b}}}"),
            ("xnor",     "{lambda {a b} {not {xor a b}}}"),
            ("implies",  "{lambda {a b} {or  {not a} b}}"),
            ("majority", "{lambda {a b c} {or {or {and a b} {and a c}} {and b c}}}"),
        };


        public static Environment library;

        /// Parses and combines _builtins and _library into library.
        static TruLibrary() {
            library = new Environment(
                Array.ConvertAll(_builtins, (func) => (func.name, (TruVal) new TruBuiltIn(func.op)) )
            );

            library.AddAll(new Environment( // Add to library, all funcs in library have access to all other funcs.
                Array.ConvertAll(_library,  (func) => (func.name, TruExpr.Parse(func.def).Interpret(library)) )
            ));
        }
    }
}