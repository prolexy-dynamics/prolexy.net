namespace Prolexy.Compiler.Ast;

public interface IAst 
{
    TResult Visit<TC, TResult>(IAstVisitor<TC, TResult> visitor, TC context);
}