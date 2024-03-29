//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from src/Parser/Generated/DTSchema.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace DTSchema.Parser {
using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public partial class DTSchemaLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, T__6=7, T__7=8, T__8=9, 
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, T__15=16, T__16=17, 
		IDENTIFIER=18, LINE_COMMENT=19, SPACE=20;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "T__10", "T__11", "T__12", "T__13", "T__14", "T__15", "T__16", 
		"UNICODE", "LETTER", "DIGIT", "INT", "IDENTIFIER_HEAD", "IDENTIFIER_FOLLOW", 
		"IDENTIFIER", "LINE_COMMENT", "SPACE"
	};


	public DTSchemaLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public DTSchemaLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'|'", "'enum'", "'='", "'extern'", "'type'", "'?'", "':'", "'fn'", 
		"'json'", "'('", "')'", "','", "'['", "']'", "'{'", "'}'", "'=>'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, null, null, "IDENTIFIER", "LINE_COMMENT", "SPACE"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "DTSchema.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static DTSchemaLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,20,151,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,1,0,1,0,1,1,1,1,1,1,1,1,1,
		1,1,2,1,2,1,3,1,3,1,3,1,3,1,3,1,3,1,3,1,4,1,4,1,4,1,4,1,4,1,5,1,5,1,6,
		1,6,1,7,1,7,1,7,1,8,1,8,1,8,1,8,1,8,1,9,1,9,1,10,1,10,1,11,1,11,1,12,1,
		12,1,13,1,13,1,14,1,14,1,15,1,15,1,16,1,16,1,16,1,17,3,17,105,8,17,1,18,
		3,18,108,8,18,1,19,1,19,1,20,4,20,113,8,20,11,20,12,20,114,1,21,1,21,1,
		21,3,21,120,8,21,1,22,1,22,3,22,124,8,22,1,23,1,23,5,23,128,8,23,10,23,
		12,23,131,9,23,1,23,3,23,134,8,23,1,24,1,24,1,24,5,24,139,8,24,10,24,12,
		24,142,9,24,1,24,1,24,1,24,1,24,1,25,1,25,1,25,1,25,0,0,26,1,1,3,2,5,3,
		7,4,9,5,11,6,13,7,15,8,17,9,19,10,21,11,23,12,25,13,27,14,29,15,31,16,
		33,17,35,0,37,0,39,0,41,0,43,0,45,0,47,18,49,19,51,20,1,0,5,2,0,255,55295,
		57343,65535,2,0,65,90,97,122,1,0,48,57,1,0,10,10,3,0,9,10,13,13,32,32,
		151,0,1,1,0,0,0,0,3,1,0,0,0,0,5,1,0,0,0,0,7,1,0,0,0,0,9,1,0,0,0,0,11,1,
		0,0,0,0,13,1,0,0,0,0,15,1,0,0,0,0,17,1,0,0,0,0,19,1,0,0,0,0,21,1,0,0,0,
		0,23,1,0,0,0,0,25,1,0,0,0,0,27,1,0,0,0,0,29,1,0,0,0,0,31,1,0,0,0,0,33,
		1,0,0,0,0,47,1,0,0,0,0,49,1,0,0,0,0,51,1,0,0,0,1,53,1,0,0,0,3,55,1,0,0,
		0,5,60,1,0,0,0,7,62,1,0,0,0,9,69,1,0,0,0,11,74,1,0,0,0,13,76,1,0,0,0,15,
		78,1,0,0,0,17,81,1,0,0,0,19,86,1,0,0,0,21,88,1,0,0,0,23,90,1,0,0,0,25,
		92,1,0,0,0,27,94,1,0,0,0,29,96,1,0,0,0,31,98,1,0,0,0,33,100,1,0,0,0,35,
		104,1,0,0,0,37,107,1,0,0,0,39,109,1,0,0,0,41,112,1,0,0,0,43,119,1,0,0,
		0,45,123,1,0,0,0,47,125,1,0,0,0,49,135,1,0,0,0,51,147,1,0,0,0,53,54,5,
		124,0,0,54,2,1,0,0,0,55,56,5,101,0,0,56,57,5,110,0,0,57,58,5,117,0,0,58,
		59,5,109,0,0,59,4,1,0,0,0,60,61,5,61,0,0,61,6,1,0,0,0,62,63,5,101,0,0,
		63,64,5,120,0,0,64,65,5,116,0,0,65,66,5,101,0,0,66,67,5,114,0,0,67,68,
		5,110,0,0,68,8,1,0,0,0,69,70,5,116,0,0,70,71,5,121,0,0,71,72,5,112,0,0,
		72,73,5,101,0,0,73,10,1,0,0,0,74,75,5,63,0,0,75,12,1,0,0,0,76,77,5,58,
		0,0,77,14,1,0,0,0,78,79,5,102,0,0,79,80,5,110,0,0,80,16,1,0,0,0,81,82,
		5,106,0,0,82,83,5,115,0,0,83,84,5,111,0,0,84,85,5,110,0,0,85,18,1,0,0,
		0,86,87,5,40,0,0,87,20,1,0,0,0,88,89,5,41,0,0,89,22,1,0,0,0,90,91,5,44,
		0,0,91,24,1,0,0,0,92,93,5,91,0,0,93,26,1,0,0,0,94,95,5,93,0,0,95,28,1,
		0,0,0,96,97,5,123,0,0,97,30,1,0,0,0,98,99,5,125,0,0,99,32,1,0,0,0,100,
		101,5,61,0,0,101,102,5,62,0,0,102,34,1,0,0,0,103,105,7,0,0,0,104,103,1,
		0,0,0,105,36,1,0,0,0,106,108,7,1,0,0,107,106,1,0,0,0,108,38,1,0,0,0,109,
		110,7,2,0,0,110,40,1,0,0,0,111,113,3,39,19,0,112,111,1,0,0,0,113,114,1,
		0,0,0,114,112,1,0,0,0,114,115,1,0,0,0,115,42,1,0,0,0,116,120,3,37,18,0,
		117,120,3,35,17,0,118,120,5,95,0,0,119,116,1,0,0,0,119,117,1,0,0,0,119,
		118,1,0,0,0,120,44,1,0,0,0,121,124,3,43,21,0,122,124,3,39,19,0,123,121,
		1,0,0,0,123,122,1,0,0,0,124,46,1,0,0,0,125,129,3,43,21,0,126,128,3,45,
		22,0,127,126,1,0,0,0,128,131,1,0,0,0,129,127,1,0,0,0,129,130,1,0,0,0,130,
		133,1,0,0,0,131,129,1,0,0,0,132,134,5,33,0,0,133,132,1,0,0,0,133,134,1,
		0,0,0,134,48,1,0,0,0,135,136,5,45,0,0,136,140,5,45,0,0,137,139,8,3,0,0,
		138,137,1,0,0,0,139,142,1,0,0,0,140,138,1,0,0,0,140,141,1,0,0,0,141,143,
		1,0,0,0,142,140,1,0,0,0,143,144,5,10,0,0,144,145,1,0,0,0,145,146,6,24,
		0,0,146,50,1,0,0,0,147,148,7,4,0,0,148,149,1,0,0,0,149,150,6,25,0,0,150,
		52,1,0,0,0,9,0,104,107,114,119,123,129,133,140,1,0,1,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace DTSchema.Parser
