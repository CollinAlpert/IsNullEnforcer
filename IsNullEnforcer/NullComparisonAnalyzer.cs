using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace IsNullEnforcer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NullComparisonAnalyzer : DiagnosticAnalyzer
{
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();
		context.RegisterCompilationStartAction(OnCompilationStartAction);
	}

	private static void OnCompilationStartAction(CompilationStartAnalysisContext obj)
	{
		obj.RegisterSyntaxNodeAction(OnSyntaxNodeAction, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
	}

	private static void OnSyntaxNodeAction(SyntaxNodeAnalysisContext obj)
	{
		if(obj.Node is BinaryExpressionSyntax binary && binary.Right.IsKind(SyntaxKind.NullLiteralExpression))
		{
			obj.ReportDiagnostic(Diagnostic.Create(GetDiagnostic(obj.Node.Kind()), binary.GetLocation(), binary.Left.ToString()));
		}
	}

	private static DiagnosticDescriptor GetDiagnostic(SyntaxKind kind)
	{
		if (kind == SyntaxKind.EqualsExpression)
		{
			return DiagnosticDescriptors.IsNullDiagnostic;
		}
		
		if (kind == SyntaxKind.NotEqualsExpression)
		{
			return DiagnosticDescriptors.IsNotNullDiagnostic;
		}
		
		throw new ArgumentException($"SyntaxKind '{kind}' is not supported.", nameof(kind));
	}

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(DiagnosticDescriptors.IsNullDiagnostic, DiagnosticDescriptors.IsNotNullDiagnostic);
}