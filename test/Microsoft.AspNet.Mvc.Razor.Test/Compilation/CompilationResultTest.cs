// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNet.Diagnostics;
using Xunit;

namespace Microsoft.AspNet.Mvc.Razor.Compilation
{
    public class CompilationResultTest
    {
        [Fact]
        public void EnsureSuccessful_ThrowsIfCompilationFailed()
        {
            // Arrange
            var compilationFailure = new CompilationFailure(
                "test",
                sourceFileContent: string.Empty,
                compiledContent: string.Empty,
                messages: Enumerable.Empty<AspNet.Diagnostics.DiagnosticMessage>());
            var failures = new[] { compilationFailure };
            var result = new CompilationResult(failures);

            // Act and Assert
            Assert.Null(result.CompiledType);
            Assert.Same(failures, result.CompilationFailures);
            var exception = Assert.Throws<CompilationFailedException>(() => result.EnsureSuccessful());
            var failure = Assert.Single(exception.CompilationFailures);
            Assert.Same(compilationFailure, failure);
        }
    }
}
