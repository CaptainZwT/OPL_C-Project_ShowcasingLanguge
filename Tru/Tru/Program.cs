using System; // For System.WriteLine() and System.ReadLine()

namespace Tru
{
    class Program
    {
        /// short help string to show when starting the REPL.
        public static readonly string shortHelpStr = 
@"This is a REPL for the Tru language.
Type ""help"" for documentation, or ""quit"" to close the REPL.
";

        /// The help documentation
        public static readonly string helpStr = shortHelpStr + "\n" +
@"Enter Tru statements to execute them or see the evaluated result.

EXAMPLES:
    {and true true}
    {define x true} ; you can use ';' for comments
    {let {[y x]}
        {not y}}
    {lambda {} true}}

SYNTAX:
    {}[]()
        Curly brackets, square brackets, and parentheses are interchangeable, but curly brackets are preferred in most cases.
    ;
        Semicolon marks a one line comment.
    true false
        Boolean literals.
    keywords
       ""lambda"", ""let"", ""define"", ""true"", ""false""
    id
        A symbol that can be bound to a value. Can be any sequence of characters except whitespace, semicolon, delimeters, and reserved keywords.
    {function-expression expression...}
        Function call.
    {lambda {param...} body}
        Creates a closure with given paramaters and body. Captures the local environment, so you can use local variables in the lambda.
    {let { [id expression]... } body}
        Makes a local binding of a value to an id.
    {define id expression}
        Makes a global binding of a value to an id. Define statements can only be at the top level of a Tru statement.
    {define {func-name param...} body}
        Makes a global function. Equivalent to {define func-name {lambda {param...} body}}

    Concrete syntax described in Extended Backus-Naur Form:
        <character>   ::= ""A"" | ""B"" | ""C"" ... ""a"" | ""b"" | ""c"" ... ""0"" | ""1"" | ""2"" ... ""!"" | ""@"" | ""#"" | ""$"" | ""%"" | ""^"" ...
        <comment>     ::= "";"" <character>* ""\n""
        <statement>   ::= <expr> | <define> | <define-func>
        <expr>        ::= <bool> | <id> | <call> | <lambda> | <let>
        <bool>        ::= ""true"" | ""false""
        <id>          ::= <character>+
        <call>        ::= ""{"" <expr> <expr>* ""}""
        <lambda>      ::= ""{"" ""lambda"" ""{"" <id>* ""}"" <expr> ""}""
        <binding>     ::= ""["" <id> <expr> ""]""
        <let>         ::= ""{"" ""let"" ""{"" <binding>* ""}"" <expr> ""}""
        <define>      ::= ""{"" ""define"" <id> <expr> ""}
        <define-func> ::= ""{"" ""define"" ""{"" <id> <id>* ""}"" <expr> ""}""

VALUES:
    A Tru expression can evaluate to either a bool, a built-in function, or a user-defined function.

BUILT-IN FUNCTIONS AND STANDARD LIBRARY FUNCTIONS
    nand : (bool bool -> bool)
        Logical nand. Supports short-circuit evaluation.
    and : (bool bool -> bool)
        Logical and. Supports short-circuit evaluation.
    or : (bool bool -> bool)
        Logical or. Supports short-circuit evaluation.
    not : (bool -> bool)
        Logical not   
    equals : (any any -> bool)
        Returns true if the two arguments are equal, else false.
    if : (bool any any -> bool)
        If the bool is true, returns the 2nd parameter, else it returns the 3rd paramater.

    nor : (bool bool -> bool)
        Logical nor
    xor : (bool bool -> bool)
        Logical xor
    xnor : (bool bool -> bool)
        Logical xnor
    implies : (bool bool -> bool)
        Logical implies
    majority : (bool bool bool -> bool)
        Returns true if 2 or 3 of the inputs are true, else returns false.
";

        /// Launches a REPL where you can evaluate Tru statements.
        static void Main(string[] args)
        {
            Console.WriteLine(shortHelpStr);

            Environment global = TruLibrary.Library;
            while (true) {
                Console.Write( ">>> ");
                string input = Console.ReadLine();
                if (input == "quit" || input == "exit") {
                    return;
                } else if (input == "help") {
                    Console.WriteLine(helpStr);
                } else {
                    try {
                        TruStatement[] statements = TruStatement.ParseAll(input);
                        foreach (TruStatement statement in statements) {
                            TruVal result = statement.Interpret(global);
                            if (result != null)
                                Console.WriteLine(result);
                        }
                    } catch (TruSyntaxError e) {
                        Console.WriteLine($"Syntax Error: {e.Message}");
                    } catch (TruRuntimeException e) {
                        Console.WriteLine($"Runtime Exception: {e.Message}");
                    }
                }
            }
        }
    }
}
