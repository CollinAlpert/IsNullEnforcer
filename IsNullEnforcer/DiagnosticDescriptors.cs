using Microsoft.CodeAnalysis;

namespace IsNullEnforcer;

public class DiagnosticDescriptors
{
	public static readonly DiagnosticDescriptor IsNullDiagnostic = new(
		"INE01",
		"Use 'is null'",
		"Use '{0} is null' instead of '{0} == null'",
		"Readability",
		DiagnosticSeverity.Error,
		true,
		"Prefer 'is null' over an equals check in order to increase readability.");
	
	public static readonly DiagnosticDescriptor IsNotNullDiagnostic = new(
		"INE02",
		"Use 'is not null'",
		"Use '{0} is not null' instead of '{0} != null'",
		"Readability",
		DiagnosticSeverity.Error,
		true,
		"Prefer 'is not null' over a not-equals check in order to increase readability.");
}