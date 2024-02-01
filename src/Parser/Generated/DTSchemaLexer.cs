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
		T__9=10, T__10=11, T__11=12, T__12=13, T__13=14, T__14=15, IDENTIFIER=16, 
		LINE_COMMENT=17, SPACE=18;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "T__6", "T__7", "T__8", 
		"T__9", "T__10", "T__11", "T__12", "T__13", "T__14", "UNICODE", "LETTER", 
		"DIGIT", "INT", "IDENTIFIER_HEAD", "IDENTIFIER_FOLLOW", "IDENTIFIER", 
		"LINE_COMMENT", "SPACE"
	};


	public DTSchemaLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public DTSchemaLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'extern'", "'type'", "'?'", "':'", "'fn'", "'('", "')'", "'|'", 
		"'='", "','", "'['", "']'", "'{'", "'}'", "'=>'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, null, null, null, null, null, 
		null, null, null, null, "IDENTIFIER", "LINE_COMMENT", "SPACE"
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
		4,0,18,137,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,1,
		1,1,2,1,2,1,3,1,3,1,4,1,4,1,4,1,5,1,5,1,6,1,6,1,7,1,7,1,8,1,8,1,9,1,9,
		1,10,1,10,1,11,1,11,1,12,1,12,1,13,1,13,1,14,1,14,1,14,1,15,3,15,91,8,
		15,1,16,3,16,94,8,16,1,17,1,17,1,18,4,18,99,8,18,11,18,12,18,100,1,19,
		1,19,1,19,3,19,106,8,19,1,20,1,20,3,20,110,8,20,1,21,1,21,5,21,114,8,21,
		10,21,12,21,117,9,21,1,21,3,21,120,8,21,1,22,1,22,1,22,5,22,125,8,22,10,
		22,12,22,128,9,22,1,22,1,22,1,22,1,22,1,23,1,23,1,23,1,23,0,0,24,1,1,3,
		2,5,3,7,4,9,5,11,6,13,7,15,8,17,9,19,10,21,11,23,12,25,13,27,14,29,15,
		31,0,33,0,35,0,37,0,39,0,41,0,43,16,45,17,47,18,1,0,5,2,0,255,55295,57343,
		65535,2,0,65,90,97,122,1,0,48,57,1,0,10,10,3,0,9,10,13,13,32,32,137,0,
		1,1,0,0,0,0,3,1,0,0,0,0,5,1,0,0,0,0,7,1,0,0,0,0,9,1,0,0,0,0,11,1,0,0,0,
		0,13,1,0,0,0,0,15,1,0,0,0,0,17,1,0,0,0,0,19,1,0,0,0,0,21,1,0,0,0,0,23,
		1,0,0,0,0,25,1,0,0,0,0,27,1,0,0,0,0,29,1,0,0,0,0,43,1,0,0,0,0,45,1,0,0,
		0,0,47,1,0,0,0,1,49,1,0,0,0,3,56,1,0,0,0,5,61,1,0,0,0,7,63,1,0,0,0,9,65,
		1,0,0,0,11,68,1,0,0,0,13,70,1,0,0,0,15,72,1,0,0,0,17,74,1,0,0,0,19,76,
		1,0,0,0,21,78,1,0,0,0,23,80,1,0,0,0,25,82,1,0,0,0,27,84,1,0,0,0,29,86,
		1,0,0,0,31,90,1,0,0,0,33,93,1,0,0,0,35,95,1,0,0,0,37,98,1,0,0,0,39,105,
		1,0,0,0,41,109,1,0,0,0,43,111,1,0,0,0,45,121,1,0,0,0,47,133,1,0,0,0,49,
		50,5,101,0,0,50,51,5,120,0,0,51,52,5,116,0,0,52,53,5,101,0,0,53,54,5,114,
		0,0,54,55,5,110,0,0,55,2,1,0,0,0,56,57,5,116,0,0,57,58,5,121,0,0,58,59,
		5,112,0,0,59,60,5,101,0,0,60,4,1,0,0,0,61,62,5,63,0,0,62,6,1,0,0,0,63,
		64,5,58,0,0,64,8,1,0,0,0,65,66,5,102,0,0,66,67,5,110,0,0,67,10,1,0,0,0,
		68,69,5,40,0,0,69,12,1,0,0,0,70,71,5,41,0,0,71,14,1,0,0,0,72,73,5,124,
		0,0,73,16,1,0,0,0,74,75,5,61,0,0,75,18,1,0,0,0,76,77,5,44,0,0,77,20,1,
		0,0,0,78,79,5,91,0,0,79,22,1,0,0,0,80,81,5,93,0,0,81,24,1,0,0,0,82,83,
		5,123,0,0,83,26,1,0,0,0,84,85,5,125,0,0,85,28,1,0,0,0,86,87,5,61,0,0,87,
		88,5,62,0,0,88,30,1,0,0,0,89,91,7,0,0,0,90,89,1,0,0,0,91,32,1,0,0,0,92,
		94,7,1,0,0,93,92,1,0,0,0,94,34,1,0,0,0,95,96,7,2,0,0,96,36,1,0,0,0,97,
		99,3,35,17,0,98,97,1,0,0,0,99,100,1,0,0,0,100,98,1,0,0,0,100,101,1,0,0,
		0,101,38,1,0,0,0,102,106,3,33,16,0,103,106,3,31,15,0,104,106,5,95,0,0,
		105,102,1,0,0,0,105,103,1,0,0,0,105,104,1,0,0,0,106,40,1,0,0,0,107,110,
		3,39,19,0,108,110,3,35,17,0,109,107,1,0,0,0,109,108,1,0,0,0,110,42,1,0,
		0,0,111,115,3,39,19,0,112,114,3,41,20,0,113,112,1,0,0,0,114,117,1,0,0,
		0,115,113,1,0,0,0,115,116,1,0,0,0,116,119,1,0,0,0,117,115,1,0,0,0,118,
		120,5,33,0,0,119,118,1,0,0,0,119,120,1,0,0,0,120,44,1,0,0,0,121,122,5,
		45,0,0,122,126,5,45,0,0,123,125,8,3,0,0,124,123,1,0,0,0,125,128,1,0,0,
		0,126,124,1,0,0,0,126,127,1,0,0,0,127,129,1,0,0,0,128,126,1,0,0,0,129,
		130,5,10,0,0,130,131,1,0,0,0,131,132,6,22,0,0,132,46,1,0,0,0,133,134,7,
		4,0,0,134,135,1,0,0,0,135,136,6,23,0,0,136,48,1,0,0,0,9,0,90,93,100,105,
		109,115,119,126,1,0,1,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace DTSchema.Parser