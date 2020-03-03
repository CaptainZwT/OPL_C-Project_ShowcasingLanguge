using Utility;

#pragma warning disable CS0659 // Silence "overrides Equals() but not GetHashCode()" warnings
namespace Tru {
    public abstract class ExprTree {
        private static string opening = "{[(";
        private static string closing = "}])";
        private static string whitespace = " \r\n\t";
        private static char comment = ';';


        /// Splits the string into tokens. Tokens are seperated by spaces, and braces are always a token by themselves.
        public static List<string> Tokenize(string code) {
            List<string> tokens = new List<string>();
            string token = "";

            for (int i = 0; i < code.Length; i++) {
                char c = code[i];

                if (whitespace.Contains(c)) {
                    if (token != "") tokens.Add(token);
                    token = "";
                } else if ( opening.Contains(c) || closing.Contains(c) ) {
                    if (token != "") tokens.Add(token);
                    tokens.Add( c.ToString() ); // Add the bracket as its own token.
                    token = "";
                } else if (comment == c) {
                    while (i < code.Length && code[i] != '\n') // skip to end of line.
                        i++;
                } else {
                    token += c;
                }
            }

            if (token != "") tokens.Add( token );

            return tokens;
        }

        /// Parses a string into a single ExprTree. If multiple expressions are given, it will throw an error.
        public static ExprTree Parse(string code) {
            List<string> tokens = Tokenize(code);

            if (tokens.Count == 0) {
                throw new TruSyntaxError("No expressions given.");
            }

            (ExprTree expr, int index) = ParseHelper(tokens, 0);

            if (index < tokens.Count) {
                throw new TruSyntaxError("Multiple expressions given.");
            } else {
                return expr;
            }
        }

        /// Parses a string into an array of ExprTree.
        public static ExprTree[] ParseAll(string code) {
            List<string> tokens = Tokenize(code);
            List<ExprTree> expressions = new List<ExprTree>();

            int index = 0;
            while (index < tokens.Count) { // parse expressions until the whole token array is used.
                ExprTree expr;
                (expr, index) = ParseHelper(tokens, index);
                expressions.Add(expr);
            }

            return expressions.ToArray();
        }

        /// Parses a list of tokens into an ExprTree. Returns a tuple of ExprTree and the index where it left off in
        /// the token list, ie the place to parse next.!-- If its a literal string, it will return it, else, it will
        /// return the expr up until the matching close bracket.
        private static (ExprTree expr, int index) ParseHelper(List<string> tokens, int index) {
            if ( opening.Contains(tokens[index]) ) { // If opening bracket
                char close = closing[ opening.IndexOf(tokens[index]) ]; // Closing bracket we're looking for

                index += 1;
                List<ExprTree> exprList = new List<ExprTree>();

                while (index < tokens.Count && tokens[index] != close.ToString()) {
                    ExprTree expr;
                    (expr, index) = ParseHelper(tokens, index); // returns tuple containing expr and the index it left off at.
                    exprList.Add(expr);
                }

                if (index >= tokens.Count) { // Reached end of string and didn't find close bracket.
                    throw new TruSyntaxError($"Mismatched brackets, expected '{close}'.");
                }

                return (new ExprList(exprList.ToArray()), index + 1);
            } else if ( closing.Contains(tokens[index]) ) { // Closing bracket with no match.
                throw new TruSyntaxError($"Mismatched brackets, unexpected '{tokens[index]}'.");
            } else { // just a string literal.
                return (new ExprLiteral(tokens[index]), index + 1);
            }
        }

        /// Checks if expr matches pattern, meaning it has the same structure,
        /// Special tags can be used in the pattern as wildcards:
        ///     ANY : matches any ExprTree
        ///     LITERAL : matches any ExprLiteral
        ///     LIST : matches any ExrList
        ///     ... : Modifies the previous item to match 0 or more repetitions of the previous pattern in a list.
        public static bool Match(string pattern, ExprTree expr) {
            return MatchHelper( Parse(pattern), expr );
        }

        private static bool MatchHelper(ExprTree pattern, ExprTree expr) {
            if (pattern is ExprLiteral patternLit) {
                return patternLit.val == "ANY" ||
                    ( patternLit.val == "LITERAL" && expr is ExprLiteral ) ||
                    ( patternLit.val == "LIST" && expr is ExprList ) ||
                    pattern.Equals(expr);
            } else if (pattern is ExprList patList && expr is ExprList exprList) {
                int patI = patList.list.Length - 1, exprI = exprList.list.Length - 1;

                while ( patI >= 0 ) { // Loop through the lists backwards so that ... is visited before what it affects.
                    // If a ... has nothing before it just match it like a normal symbol
                    if ( patList.list[patI].Equals(new ExprLiteral("...")) && patI >= 1 ) {
                        ExprTree repPattern = patList.list[patI - 1];
                        int matchCount = 0;
                        for (/*exprI current value*/; exprI >= 0 && MatchHelper(repPattern, exprList.list[exprI]); exprI--)
                            matchCount++;
                        patI -= 2;

                        // to prevent ... from eating the first a in {a a ...}, we count repetitions of the pattern in pattern
                        // and make sure we found at least that many.
                        int expectedCount = 0;
                        for (/*patI current value*/; patI >= 0 && patList.list[patI].Equals(repPattern); patI--)
                            expectedCount++;

                        if (expectedCount > matchCount)
                            return false;
                    } else {
                        if ( exprI < 0 || !MatchHelper(patList.list[patI], exprList.list[exprI]) )
                            return false;
                        patI--; exprI--;
                    }
                }

                return (patI < 0 && exprI < 0);

            } else {
                return false;
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
        public ExprTree[] list;
        public ExprList(ExprTree[] list) { this.list = list; }
        public ExprList() : this(new ExprTree[0]) {}

        public override bool Equals(object obj)
        {
            if (obj is ExprList exprList && this.list.Length == exprList.list.Length) {
                for (int i = 0; i < this.list.Length; i++) {
                    if (!this.list[i].Equals(exprList.list[i])) return false;
                }
                return true;
            };
            return false;
        }
    }


}