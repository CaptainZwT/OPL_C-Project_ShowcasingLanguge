using Utility;

namespace Tru {
    public abstract class ExprTree {
        private static string opening = "{[(";
        private static string closing = "}])";
        private static string whitespace = " \n\t";


        /// Splits the string into tokens. Tokens are seperated by spaces, and braces are always a token by themselves.
        public static List<string> Tokenize(string code) {
            List<string> tokens = new List<string>();
            string token = "";

            foreach (char c in code) {
                if (whitespace.Contains(c)) {
                    if (token != "") tokens.Add(token);
                    token = "";
                } else if ( opening.Contains(c) || closing.Contains(c) ) {
                    if (token != "") tokens.Add(token);
                    tokens.Add( c.ToString() ); // Add the bracket as its own token.
                    token = "";
                } else {
                    token += c;
                }
            }

            if (token != "") tokens.Add( token );

            return tokens;
        }

        public static ExprTree Parse(string code) {
            List<string> tokens = Tokenize(code);

            if (tokens.Count == 0) {
                throw new System.ArgumentException("No expressions given.");
            }

            (ExprTree expr, int index) = ParseHelper(tokens, 0);

            if (index < tokens.Count) {
                throw new System.ArgumentException("Multiple expressions given or missmatched brackets.");
            } else {
                return expr;
            }
        }

        /// Parses a list of tokens into an ExprTree. Returns a tuple of ExprTree and the index where it left off in
        /// the token list, ie the place to parse next.!-- If its a literal string, it will return it, else, it will
        /// return the expr up until the matching close bracket.
        private static (ExprTree expr, int index) ParseHelper(List<string> tokens, int index) {
            if ( opening.Contains(tokens[index]) ) {
                char close = closing[ opening.IndexOf(tokens[index]) ]; // Closing bracket we're looking for

                index += 1;
                ExprList exprList = new ExprList();

                while (index < tokens.Count && tokens[index] != close.ToString()) {
                    ExprTree expr;
                    (expr, index) = ParseHelper(tokens, index); // returns tuple containing expr and the index it left off at.
                    exprList.list.Add(expr);
                }

                if (index >= tokens.Count) { // Reached end of string and didn't find close bracket.
                    throw new System.ArgumentException("Mismatched or missing brackets.");
                }

                return (exprList, index + 1);
            } else if ( closing.Contains(tokens[index]) ) { // Closing bracket with no match.
                throw new System.ArgumentException("Mismatched or missing brackets.");
            } else { // just a string literal.
                return (new ExprLiteral(tokens[index]), index + 1);
            }
        }

        /// Checks if expr matches pattern, meaning it has the same structure,
        /// Special tags can be used in the pattern as wildcards:
        ///     ANY: Will match any ExprTree
        ///     LITERAL: will match any ExprLiteral
        public static bool Match(string pattern, ExprTree expr) {
            return MatchHelper( Parse(pattern), expr );
        }

        private static bool MatchHelper(ExprTree pattern, ExprTree expr) {
            if (pattern is ExprLiteral patternLit) {
                return patternLit.val == "ANY" ||
                    ( patternLit.val == "LITERAL" && expr is ExprLiteral ) ||
                    pattern.Equals(expr);
            } else {
                ExprList patternList = (ExprList) pattern;

                if (expr is ExprList exprList && patternList.list.Count == exprList.list.Count ) {
                    for ( int i = 0; i < patternList.list.Count; i++) {
                        if ( !MatchHelper(patternList.list[i], exprList.list[i]) ) return false;
                    }
                    return true;
                } else {
                    return false;
                }
            }
        }

    }

    public class ExprLiteral : ExprTree {
        public string val;
        public ExprLiteral(string val) { this.val = val; }

        public override bool Equals(object obj)
        {
            return obj is ExprLiteral && ((ExprLiteral) obj).val == this.val;
        }
    }

    public class ExprList : ExprTree {
        public List<ExprTree> list;
        public ExprList(List<ExprTree> list) { this.list = list; }
        public ExprList() : this(new List<ExprTree>()) {}

        public override bool Equals(object obj)
        {
            return obj is ExprList exprList && this.list.Equals( exprList.list );
        }
    }


}