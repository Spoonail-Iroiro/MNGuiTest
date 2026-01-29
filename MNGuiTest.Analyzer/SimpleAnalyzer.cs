#pragma warning disable RS2008

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class SimpleAnalyzer : DiagnosticAnalyzer {
    public readonly static string[] BannedNamespaces = [
        "MNGuiTest.MNGui"
    ];

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
        id: "BANNMSP001",
        title: "Banned namespace",
        messageFormat: "Namespace {0} can't be used. Rename it to {1}.",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true
        );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register action
        context.RegisterSyntaxNodeAction(AnalyzeUsing, SyntaxKind.UsingDirective);
        context.RegisterSyntaxNodeAction(AnalyzeDeclareNamespace, SyntaxKind.NamespaceDeclaration, SyntaxKind.FileScopedNamespaceDeclaration);
    }

    private static void AnalyzeDeclareNamespace(SyntaxNodeAnalysisContext context) {
        NameSyntax? nameSyn = null;
        if (context.Node is NamespaceDeclarationSyntax nds) {
            nameSyn = nds.Name;
        }
        else if (context.Node is FileScopedNamespaceDeclarationSyntax fsnds) {
            nameSyn = fsnds.Name;
        }

        ReportBannedNameSyntax(context, nameSyn);
    }

    private static void AnalyzeUsing(SyntaxNodeAnalysisContext context) {
        if (context.Node is UsingDirectiveSyntax usingNode) {
            ReportBannedNameSyntax(context, usingNode.Name);
        }
    }

    private static void ReportBannedNameSyntax(SyntaxNodeAnalysisContext context, NameSyntax? nameSyn) {
        if (nameSyn == null) return;

        var name = nameSyn.ToString();
        foreach (var banned in BannedNamespaces) {
            // Ignore case on comparison
            if (name.ToLowerInvariant().StartsWith(banned.ToLowerInvariant())) {
                var renamed = banned.Replace("MNGuiTest.", "");
                var diag = Diagnostic.Create(Rule, nameSyn.GetLocation(), banned, renamed);
                context.ReportDiagnostic(diag);
            }
        }
    }

}

