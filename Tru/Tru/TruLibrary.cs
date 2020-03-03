using System; // For array.

namespace Tru {
    /// Contains the default standard library.
    public static class TruLibrary {
        /// Interprets expr, checks its type, and returns it. Throws an error if the type doesn't match.
        private static T Check<T>(TruExpr expr, Environment env) where T : TruVal {
            TruVal val = expr.Interpret(env);
            if (val is T t) {
                return t;
            } else {
                throw new TruRuntimeException($"Incorrect types, given {val.GetType().Name} expected {typeof(T).Name}.");
            }
        }

        /// Checks paramater count and throws an exception if it doesn't match.
        private static void CheckCount(TruExpr[] parms, int count) {
            if (parms.Length != count)
                throw new TruRuntimeException($"Built-in call with incorrect argument count, given {parms.Length} expected {count}.");
        }
        
        private static TruVal nand(Environment env, TruExpr[] parms) {
            CheckCount(parms, 2);
            return new TruBool( !( Check<TruBool>(parms[0], env).val && Check<TruBool>(parms[1], env).val ) );
        }

        private static TruVal and(Environment env, TruExpr[] parms) {
            CheckCount(parms, 2);
            return new TruBool( Check<TruBool>(parms[0], env).val && Check<TruBool>(parms[1], env).val );
        }

        private static TruVal or(Environment env, TruExpr[] parms) {
            CheckCount(parms, 2);
            return new TruBool( Check<TruBool>(parms[0], env).val || Check<TruBool>(parms[1], env).val );
        }

        private static TruVal not(Environment env, TruExpr[] parms) {
            CheckCount(parms, 1);
            return new TruBool( !Check<TruBool>(parms[0], env).val );
        }

        private static TruVal equals(Environment env, TruExpr[] parms) { // for bools, same as xnor, but this way it works for other types.
            // equals will work on all types, and returns a TruBool   
            CheckCount(parms, 2);
            return new TruBool( parms[0].Interpret(env).Equals(parms[1].Interpret(env)) );
        }

        private static TruVal ifStatement(Environment env, TruExpr[] parms) {
            CheckCount(parms, 3);
            return Check<TruBool>(parms[0], env).val ? parms[1].Interpret(env) : parms[2].Interpret(env);
        }



        private static (string name, TruBuiltIn.TruOperation op)[] _builtins = new (string, TruBuiltIn.TruOperation)[] {
            ("nand",    nand),
            ("and",     and),
            ("or",      or),
            ("not",     not),
            ("equals",  equals),
            ("if",      ifStatement)
        };

        private static (string name, string def)[] _definitions = new (string, string)[] {
            ("nor",      "{lambda {a b} {not {or a b}}}"),
            ("xor",      "{lambda {a b} {and {or a b} {nand a b}}}"),
            ("xnor",     "{lambda {a b} {not {xor a b}}}"),
            ("implies",  "{lambda {a b} {or  {not a} b}}"),
            ("majority", "{lambda {a b c} {or {or {and a b} {and a c}} {and b c}}}"),
        };


        private static Environment _library;

        /// Returns a copy of the library, so you don't have to worry about modifying it.
        public static Environment Library {
            get { return _library.Copy(); }
        }

        /// Parses and combines _builtins and _library into library.
        static TruLibrary() {
            _library = new Environment(
                Array.ConvertAll(_builtins, (func) => (func.name, (TruVal) new TruBuiltIn(func.op)) )
            );

            _library.ExtendGlobal(new Environment( // Add to library, all funcs in library have access to all other funcs.
                Array.ConvertAll(_definitions,  (func) => (func.name, TruExpr.Parse(func.def).Interpret(_library)) )
            ));
        }
    }
}