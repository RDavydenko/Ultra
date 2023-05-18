using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Ultra.Core.Extensions;
using Ultra.Core.Tools;

namespace Ultra.Core.Web.GraphQL
{
    public class QueryGenerator
    {
        public static Type Generate()
        {
            return Assembly.Load(Compile()).GetExportedTypes().First(x => x.Name == "Query");
        }

        private static byte[] Compile()
        {
            using (var peStream = new MemoryStream())
            {
                var result = GenerateCode().Emit(peStream);

                if (!result.Success)
                {
                    Console.WriteLine("Compilation done with error.");

                    var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (var diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }

                    return null;
                }

                Console.WriteLine("Compilation done without any error.");

                peStream.Seek(0, SeekOrigin.Begin);

                return peStream.ToArray();
            }
        }

        private static SourceText GenerateSourceText()
        {
            const string methodTemplate = @"
                [UseOffsetPaging]
                [UseDistinct]
                [UseProjection]
                [UseFiltering]
                [UseSorting]
                // [UseDistinctBy] // Не работает
                public Task<IQueryable<{1}>> Get{0}([Service] ICrudService<{1}> crudService)
                {
                    return crudService.GetQueryable();
                }";

            const string classTemplate = @"
                using System.Linq;
                using System.Threading.Tasks;
                using Microsoft.EntityFrameworkCore;
                using HotChocolate;
                using HotChocolate.Data;
                using HotChocolate.Types;
                using Ultra.Core.Services.Abstract;
                using Ultra.Core.Web.GraphQL.Attributes;
                using Ultra.Core.Web.GraphQL.Distinct;
                using Ultra.Core.Web.GraphQL.DistinctBy;

                namespace Ultra.Core.Web.GraphQL.CodeGeneration
                {
                    public class Query
                    {
                        {0}
                    }
                }";

            var entities = Executor.WebAssembly.GetDbEntities();
            var sb = new StringBuilder(classTemplate.Length + entities.Count() * methodTemplate.Length);
            foreach (var entity in entities)
            {
                sb.Append(methodTemplate);
                sb.Replace("{0}", entity.Name);
                sb.Replace("{1}", entity.FullName);
                sb.AppendLine();
            }

            return SourceText.From(classTemplate.Replace("{0}", sb.ToString()));
        }

        private static CSharpCompilation GenerateCode()
        {
            var sourceText = GenerateSourceText();
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(sourceText, options);

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IQueryable<>).Assembly.Location),
                //MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(DbContext).Assembly.Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location),
                MetadataReference.CreateFromFile(Assembly.Load("System.ComponentModel.TypeConverter, Version=6.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location),
                //MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                //MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(HotChocolate.ServiceAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(HotChocolate.Data.UseProjectionAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(HotChocolate.Types.UseOffsetPagingAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(HotChocolate.Types.ObjectFieldDescriptorAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(HotChocolate.Types.DescriptorAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Ultra.Core.Services.CrudService.CrudService<>).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.Linq.Translations.DefaultTranslationOf<>).Assembly.Location),
                MetadataReference.CreateFromFile(Executor.WebAssembly.Location),
                MetadataReference.CreateFromFile(typeof(Ultra.Core.Web.GraphQL.Attributes.UseComputedAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Ultra.Core.Web.GraphQL.DistinctBy.UseDistinctByAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Ultra.Core.Web.GraphQL.Distinct.UseDistinctAttribute).Assembly.Location),
            };

            return CSharpCompilation.Create("Ultra.Core.Web.GraphQL.CodeGeneration.dll",
                new[] { parsedSyntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }
    }
}
