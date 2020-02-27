using Utility;

namespace Tru {
    public abstract class TruExpr {
        public static TruExpr Parse(string code) { return ParseHelper( ExprTree.Parse(code) ); }

        private static TruExpr ParseHelper(ExprTree expr) {
            if ( ExprTree.Match("true", expr) ) {
                return new TruValue(true);
            } else if ( ExprTree.Match("false", expr) ) {
                return new TruValue(false);
            } else if ( expr is ExprList exprList ) { // cast expr into exprList

                if (ExprTree.Match("{and ANY ANY}", expr)) {
                    return new TruAnd( ParseHelper(exprList.list[1]), ParseHelper(exprList.list[2]) );
                } else if (ExprTree.Match("{or ANY ANY}", expr)) {
                    return new TruOr(  ParseHelper(exprList.list[1]), ParseHelper(exprList.list[2]) );
                } else if (ExprTree.Match("{not ANY}", expr)) {
                    return new TruNot( ParseHelper(exprList.list[1]) );
                }

            }

            throw new System.ArgumentException("Invalid syntax.");
        }

        public abstract bool Interpret();
    }

    public class TruValue : TruExpr {
        public bool val;
        public TruValue(bool val) { this.val = val; }
        public override bool Interpret() { return this.val; }

        public override bool Equals(object obj) {
            return obj is TruValue truVal && truVal.val == this.val;
        }
    }

    public class TruAnd : TruExpr {
        public TruExpr exprA, exprB;
        public TruAnd(TruExpr exprA, TruExpr exprB) { this.exprA = exprA; this.exprB = exprB; }
        public override bool Interpret() { return exprA.Interpret() && exprB.Interpret(); }

        public override bool Equals(object obj) {
            return obj is TruAnd truAnd && truAnd.exprA.Equals(this.exprA) && truAnd.exprB.Equals(this.exprB);
        }
    }

    public class TruOr : TruExpr {
        public TruExpr exprA, exprB;
        public TruOr(TruExpr exprA, TruExpr exprB) {this.exprA = exprA; this.exprB = exprB;}
        public override bool Interpret() { return exprA.Interpret() || exprB.Interpret(); }

        public override bool Equals(object obj) {
            return obj is TruOr truOr && truOr.exprA.Equals(this.exprA) && truOr.exprB.Equals(this.exprB);
        }
    }

    public class TruNot : TruExpr {
        public TruExpr expr;
        public TruNot(TruExpr expr) {this.expr = expr;}
        public override bool Interpret() { return !expr.Interpret(); }

        public override bool Equals(object obj) {
            return obj is TruNot truNot && truNot.expr.Equals(this.expr);
        }
    }
}