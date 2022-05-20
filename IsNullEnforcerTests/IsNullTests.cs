using IsNullEnforcer;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace IsNullEnforcerTests;

using Verify = CodeFixVerifier<NullComparisonAnalyzer, NullComparisonFixer, CSharpCodeFixTest<NullComparisonAnalyzer, NullComparisonFixer, XUnitVerifier>, XUnitVerifier>;

public class IsNullTests
{
	private const string Template = @"
public class Test {{
	public void Method() {{
		string? s = null;
		{0}
	}}
}}
";

	private const string EqualCheck = "if ({|#0:s == null|}) return;";
	private const string EqualCheckFixed = "if (s is null) return;";
	
	private const string MultipleConditions = "if ({|#0:s == null|} || s.Length == 0) return;";
	private const string MultipleConditionsFixed = "if (s is null || s.Length == 0) return;";
	
	private const string WillNotRaise = "if (s is null) return;";
	private const string WillNotRaise2 = "if (s is null || s.Length == 0) return;";
	private const string WillNotRaise3 = "if (s is \"\") return;";
	
	[Theory]
	[InlineData(EqualCheck, EqualCheckFixed)]
	[InlineData(MultipleConditions, MultipleConditionsFixed)]
	public Task WillRaiseDiagnostic(string code, string fixedCode)
	{
		code = string.Format(Template, code);
		fixedCode = string.Format(Template, fixedCode);

		var expectedDiagnostic = new DiagnosticResult(DiagnosticDescriptors.IsNullDiagnostic)
			.WithLocation(0)
			.WithArguments("s");

		return Verify.VerifyCodeFixAsync(code, expectedDiagnostic, fixedCode);
	}

	[Theory]
	[InlineData(WillNotRaise)]
	[InlineData(WillNotRaise2)]
	[InlineData(WillNotRaise3)]
	public Task WillNotRaiseDiagnostic(string code)
	{
		code = string.Format(Template, code);

		return Verify.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
	}
}