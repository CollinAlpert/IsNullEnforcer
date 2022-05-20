using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace IsNullEnforcer;

[ExportCodeFixProvider(LanguageNames.CSharp)]
public class NullComparisonFixer : CodeFixProvider
{
	public override async Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		foreach (var diagnostic in context.Diagnostics)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			if (root?.FindNode(context.Span) is not BinaryExpressionSyntax binaryExpression)
			{
				return;
			}

			var (codeFixTitle, newExpression) = GetDiagnosticAwareData(diagnostic.Id, binaryExpression.Left);
			var newRoot = root.ReplaceNode(binaryExpression, newExpression);

			var codeFix = CodeAction.Create(codeFixTitle, token => Task.FromResult(context.Document.WithSyntaxRoot(newRoot)), diagnostic.Id);
			
			context.RegisterCodeFix(codeFix, diagnostic);
		}
	}

	private static (string CodeFixTitle, IsPatternExpressionSyntax NewExpression) GetDiagnosticAwareData(string diagnosticId, ExpressionSyntax expression)
	{
		if (diagnosticId == DiagnosticDescriptors.IsNullDiagnostic.Id)
		{
			var newExpression = IsPatternExpression(expression, ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression)));
			
			return ("Use 'is null'", newExpression);
		}
		
		if (diagnosticId == DiagnosticDescriptors.IsNotNullDiagnostic.Id)
		{
			var newExpression = IsPatternExpression(expression, UnaryPattern(ConstantPattern(LiteralExpression(SyntaxKind.NullLiteralExpression))));
			return ("Use 'is not null'", newExpression);
		}
		
		throw new ArgumentException($"Diagnostic id '{diagnosticId}' is not supported.", nameof(diagnosticId));
	}

	public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

	public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(DiagnosticDescriptors.IsNullDiagnostic.Id, DiagnosticDescriptors.IsNotNullDiagnostic.Id);
}