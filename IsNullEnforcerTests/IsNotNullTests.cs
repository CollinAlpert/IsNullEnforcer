using IsNullEnforcer;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace IsNullEnforcerTests;

using Verify = CodeFixVerifier<NullComparisonAnalyzer, NullComparisonFixer, CSharpCodeFixTest<NullComparisonAnalyzer, NullComparisonFixer, XUnitVerifier>, XUnitVerifier>;

public class IsNotNullTests
{
	private const string Template = @"
public class Test {{
	public void Method() {{
		string? s = ""Hello World"";
		{0}
	}}
}}
";

	private const string NotEqualCheck = "if ({|#0:s != null|}) return;";
	private const string NotEqualCheckFixed = "if (s is not null) return;";
	
	private const string MultipleConditions = "if ({|#0:s != null|} && s.Length > 0) return;";
	private const string MultipleConditionsFixed = "if (s is not null && s.Length > 0) return;";
	
	private const string WillNotRaise = "if (s is not null) return;";
	private const string WillNotRaise2 = "if (s is not null && s.Length > 0) return;";
	private const string WillNotRaise3 = "if (s is not \"\" && s.Length > 0) return;";
	
	[Theory]
	[InlineData(NotEqualCheck, NotEqualCheckFixed)]
	[InlineData(MultipleConditions, MultipleConditionsFixed)]
	public Task WillRaiseDiagnostic(string code, string fixedCode)
	{
		code = string.Format(Template, code);
		fixedCode = string.Format(Template, fixedCode);

		var expectedDiagnostic = new DiagnosticResult(DiagnosticDescriptors.IsNotNullDiagnostic)
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