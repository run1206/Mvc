﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNet.Mvc.Filters;

namespace Microsoft.AspNet.Mvc.ViewFeatures
{
    /// <summary>
    /// A marker interface for filters which define a policy for antiforgery token validation.
    /// </summary>
    public interface IAntiforgeryPolicy : IFilterMetadata
    {
    }
}
