// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNet.Mvc.DataAnnotations;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.OptionsModel;

namespace Microsoft.AspNet.Mvc.ModelBinding.Validation
{
    /// <summary>
    /// A factory for validators based on ValidationAttribute.
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="stringLocalizer"></param>
    /// <returns></returns>
    public delegate IAttributeAdapter ValidationAttributeAdapterFactory(
        ValidationAttribute attribute,
        IStringLocalizer stringLocalizer);

    /// <summary>
    /// An implementation of <see cref="IClientModelValidatorProvider"/> which provides client validators
    /// for attributes which derive from <see cref="ValidationAttribute"/>. It also provides
    /// a validator for types which implement <see cref="IClientModelValidator"/>.
    /// The logic to support <see cref="IClientModelValidator"/>
    /// is implemented in <see cref="ValidationAttributeAdapter{}"/>.
    /// </summary>
    public class DataAnnotationsClientModelValidatorProvider : IClientModelValidatorProvider
    {
        private readonly Dictionary<Type, ValidationAttributeAdapterFactory> _attributeFactories =
            BuildAttributeFactoriesDictionary();
        private readonly IOptions<MvcDataAnnotationsLocalizationOptions> _options;
        private readonly IStringLocalizerFactory _stringLocalizerFactory;

        /// <summary>
        /// Create a new instance of <see cref="DataAnnotationsClientModelValidatorProvider"/>.
        /// </summary>
        /// <param name="options">The <see cref="IOptions{MvcDataAnnotationsLocalizationOptions}"/>.</param>
        /// <param name="stringLocalizerFactory">The <see cref="IStringLocalizerFactory"/>.</param>
        public DataAnnotationsClientModelValidatorProvider(
            IOptions<MvcDataAnnotationsLocalizationOptions> options,
            IStringLocalizerFactory stringLocalizerFactory)
        {
            _options = options;
            _stringLocalizerFactory = stringLocalizerFactory;
        }

        /// <inheritdoc />
        public void GetValidators(ClientValidatorProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            IStringLocalizer stringLocalizer = null;
            if (_options.Value.DataAnnotationLocalizerProvider != null && _stringLocalizerFactory != null)
            {
                // This will pass first non-null type (either containerType or modelType) to delegate.
                // Pass the root model type(container type) if it is non null, else pass the model type.
                stringLocalizer = _options.Value.DataAnnotationLocalizerProvider(
                    context.ModelMetadata.ContainerType ?? context.ModelMetadata.ModelType,
                    _stringLocalizerFactory);
            }

            var hasRequiredAttribute = false;

            foreach (var attribute in context.ValidatorMetadata.OfType<ValidationAttribute>())
            {
                hasRequiredAttribute |= attribute is RequiredAttribute;

                ValidationAttributeAdapterFactory factory;
                if (_attributeFactories.TryGetValue(attribute.GetType(), out factory))
                {
                    context.Validators.Add(factory(attribute, stringLocalizer));
                }
            }

            if (!hasRequiredAttribute && context.ModelMetadata.IsRequired)
            {
                // Add a default '[Required]' validator for generating HTML if necessary.
                context.Validators.Add(new RequiredAttributeAdapter(new RequiredAttribute(), stringLocalizer));
            }
        }

        private static Dictionary<Type, ValidationAttributeAdapterFactory> BuildAttributeFactoriesDictionary()
        {
            return ValidationAttributeAdapterTable.AttributeFactories;
        }
    }
}
