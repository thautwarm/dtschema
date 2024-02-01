using System.Text;

namespace DTSchema.Compiler;

public class Printer
{
    public int Indent { private set; get; } = 0;

    public StringBuilder Builder { private set; get; } = new StringBuilder();

    public static Printer operator ++(Printer p)
    {
        p.Indent++;
        return p;
    }

    public static Printer operator --(Printer p)
    {
        p.Indent--;
        return p;
    }

    public static int operator <<(Printer p, string text)
    {
        p.Builder.Append(' ', p.Indent * 4);
        p.Builder.Append(text);
        p.Builder.AppendLine();
        return 0;
    }
}