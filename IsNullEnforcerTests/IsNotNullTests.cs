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

	public const string WillRaise = "if ({|#0:s != null|}) return;";
	public const string WillRaiseFixed = "if (s is not null) return;";
	
	public const string WillNotRaise = "if (s is not null) return;";
	
	[Theory]
	[InlineData(WillRaise, WillRaiseFixed)]
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
	public Task WillNotRaiseDiagnostic(string code)
	{
		code = string.Format(Template, code);

		return Verify.VerifyAnalyzerAsync(code, DiagnosticResult.EmptyDiagnosticResults);
	}
}