using System;
using System.Collections.Generic;

namespace SystemX.CommandLine;

public class CmdParser<T> where T : new()
{
    readonly List<string> PositionalArguments = new();
    string? PostionalVararg = null;
    readonly Dictionary<string, string> NamedArguments = new();
    readonly Dictionary<string, string> Flags = new();
    readonly Dictionary<string, string> Shortcuts = new();
    public delegate void OptionHandler(T result, string Value);
    public delegate void VarargOptionHandler(T result, string[] Value);
    public delegate void FlagHandler(T result);
    VarargOptionHandler? m_VarargOptionHandler = null;
    readonly Dictionary<string, OptionHandler> m_OptionHandlers = new();
    readonly Dictionary<string, FlagHandler> m_FlagHandlers = new();
    string HelpDoc = "";

    public CmdParser()
    {
    }

    public void Help(string helpDoc)
    {
        HelpDoc = helpDoc;
    }

    public void PrintHelp()
    {
        Console.WriteLine(HelpDoc);
    }

    public void Positional(string storeName, OptionHandler f)
    {
        PositionalArguments.Add(storeName);
        m_OptionHandlers[storeName] = f;
    }

    public void Named(string name, string storeName, OptionHandler f)
    {
        NamedArguments[name] = storeName;
        m_OptionHandlers[storeName] = f;
    }

    public void Named(string name, OptionHandler f) => Named(name, name, f);

    public void Vararg(string storeName, VarargOptionHandler f)
    {
        PostionalVararg = storeName;
        m_VarargOptionHandler = f;
    }

    public void Flag(string name, string storeName, FlagHandler f)
    {
        Flags[name] = storeName;

        m_FlagHandlers[storeName] = f;
    }

    public void Shortcut(string name, string storeName, OptionHandler f)
    {
        Shortcuts[name] = storeName;

        m_OptionHandlers[storeName] = f;
    }

    public void Shortcut(string name, OptionHandler f) => Shortcut(name, name, f);

    public void Flag(string name, FlagHandler f) => Flag(name, name, f);

    public T Parse(string[] args)
    {
        try
        {
            return ParseImpl(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.WriteLine(HelpDoc);
            System.Environment.Exit(1);
            throw default;
        }
    }

    T ParseImpl(string[] args)
    {
        var result = new T();
        var positionalIndex = 0;
        var varargs = new List<string>();

        int i = 0;
        while (i < args.Length)
        {
            var arg = args[i];
            if (arg.StartsWith("--"))
            {
                var name = arg.Substring(2);
                if (Flags.TryGetValue(name, out var flagStoreName))
                {
                    m_FlagHandlers[flagStoreName](result);
                    i++;
                    continue;
                }

                if (!NamedArguments.TryGetValue(name, out var optionStoreName))
                {
                    Console.WriteLine("Unknown named option [{0}]", name);
                    i += 1;
                    continue;
                }

                if (i + 1 < args.Length)
                {
                    m_OptionHandlers[optionStoreName](result, args[i + 1]);
                    i += 2;
                }
                else
                {
                    throw new Exception($"Missing value for option [{name}]");
                }
            }
            else if (arg.StartsWith("-"))
            {
                var name = arg.Substring(1);
                if (!Shortcuts.TryGetValue(name, out var storeName))
                {
                    Console.WriteLine("Unknown shortcut [{0}]", name);
                    i += 1;
                    continue;
                }

                if (i + 1 < args.Length)
                {
                    m_OptionHandlers[storeName](result, args[i + 1]);
                    i += 2;
                }
                else
                {
                    throw new Exception($"Missing value for option [{name}]");
                }
            }
            else
            {
                if (positionalIndex < PositionalArguments.Count)
                {
                    var name = PositionalArguments[positionalIndex];
                    m_OptionHandlers[name](result, arg);
                    positionalIndex++;
                }
                else
                {
                    if (PostionalVararg == null)
                    {
                        throw new Exception($"Unknown positional argument [{arg}]");
                    }
                    varargs.Add(arg);
                }
                i++;
            }
        }

        if (varargs.Count > 0)
        {
            m_VarargOptionHandler!(result, varargs.ToArray());
        }
        return result;
    }
}