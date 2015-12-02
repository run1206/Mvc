// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace Microsoft.AspNet.Mvc.ModelBinding.Validation
{
    public class RangeAttributeAdapter : AttributeAdapterBase<RangeAttribute>
    {
        public RangeAttributeAdapter(RangeAttribute attribute, IStringLocalizer stringLocalizer)
            : base(attribute, stringLocalizer)
        {
        }

        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var errorMessage = GetErrorMessage(context.ModelMetadata);
            return new[] { new ModelClientValidationRangeRule(errorMessage, Attribute.Minimum, Attribute.Maximum) };
        }

        public override string GetErrorMessage(ModelMetadata metadata, IModelMetadataProvider metadataProvider)
        {
            return GetErrorMessage(metadata, metadata.GetDisplayName(), Attribute.Minimum, Attribute.Maximum);
        }
    }
}