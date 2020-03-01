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

        private static TruVal equals(Environment env, TruExpr[] parms) {
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

        // private static (string name, string def)[] _library = new (string, string)[] {
        //     ("identity", "{lambda {x} x}"),
        //     ("nor",      "{lambda {a b} {not {or a b}}}"),
        //     ("xor",      "{lambda {a b} {and {or a b} {nand a b}}}"),
        //     ("xnor",     "{lambda {a b} {not {xor a b}}}"),
        //     ("implies",  "{lambda {a b} {or {not a} b}}"),
        //     ("equals",   "{lambda {a b} {xnor a b}}"),
        //     ("majority", "{lambda {a b c} {or {or {and a b} {and a c}} {and b c}}})))"),
        // };


        public static Environment library;

        /// Parses and combines _builtins and _library into library.
        static TruLibrary() {
            library = new Environment(
                Array.ConvertAll(_builtins, (x) => (x.name, (TruVal) new TruBuiltIn(x.op)))
            );

            // (string, TruVal)[] libraryParsed = new (string, TruVal)[_library.Length];
            // for (int i = 0; i < _library.Length; i++) {
            //     TruVal func = TruExpr.Parse(_library[i].def).Interpret(library); // All standard library functions have the entire standard library in scope.
            //     libraryParsed[i] = (_library[i].name, func);
            // }

            // library.Add(Environment.MakeEnv(libraryParsed)); // add to standard library. This mutates it so that all the functions will have the entire library.
        }
    }
}