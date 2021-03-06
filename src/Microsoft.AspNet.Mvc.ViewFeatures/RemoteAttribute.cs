// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Mvc.Internal;
using Microsoft.AspNet.Mvc.ModelBinding.Validation;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Mvc.Routing;

namespace Microsoft.AspNet.Mvc
{
    /// <summary>
    /// A <see cref="ValidationAttribute"/> which configures Unobtrusive validation to send an Ajax request to the
    /// web site. The invoked action should return JSON indicating whether the value is valid.
    /// </summary>
    /// <remarks>Does no server-side validation of the final form submission.</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RemoteAttribute : ValidationAttribute, IClientModelValidator
    {
        private string _additionalFields = string.Empty;
        private string[] _additionalFieldsSplit = new string[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteAttribute"/> class.
        /// </summary>
        /// <remarks>
        /// Intended for subclasses that support URL generation with no route, action, or controller names.
        /// </remarks>
        protected RemoteAttribute()
            : base(Resources.RemoteAttribute_RemoteValidationFailed)
        {
            RouteData = new RouteValueDictionary();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteAttribute"/> class.
        /// </summary>
        /// <param name="routeName">
        /// The route name used when generating the URL where client should send a validation request.
        /// </param>
        /// <remarks>
        /// Finds the <paramref name="routeName"/> in any area of the application.
        /// </remarks>
        public RemoteAttribute(string routeName)
            : this()
        {
            RouteName = routeName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteAttribute"/> class.
        /// </summary>
        /// <param name="action">
        /// The action name used when generating the URL where client should send a validation request.
        /// </param>
        /// <param name="controller">
        /// The controller name used when generating the URL where client should send a validation request.
        /// </param>
        /// <remarks>
        /// <para>
        /// If either <paramref name="action"/> or <paramref name="controller"/> is <c>null</c>, uses the corresponding
        /// ambient value.
        /// </para>
        /// <para>Finds the <paramref name="controller"/> in the current area.</para>
        /// </remarks>
        public RemoteAttribute(string action, string controller)
            : this()
        {
            if (action != null)
            {
                RouteData["action"] = action;
            }

            if (controller != null)
            {
                RouteData["controller"] = controller;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteAttribute"/> class.
        /// </summary>
        /// <param name="action">
        /// The action name used when generating the URL where client should send a validation request.
        /// </param>
        /// <param name="controller">
        /// The controller name used when generating the URL where client should send a validation request.
        /// </param>
        /// <param name="areaName">The name of the area containing the <paramref name="controller"/>.</param>
        /// <remarks>
        /// <para>
        /// If either <paramref name="action"/> or <paramref name="controller"/> is <c>null</c>, uses the corresponding
        /// ambient value.
        /// </para>
        /// If <paramref name="areaName"/> is <c>null</c>, finds the <paramref name="controller"/> in the root area.
        /// Use the <see cref="RemoteAttribute(string, string)"/> overload find the <paramref name="controller"/> in
        /// the current area. Or explicitly pass the current area's name as the <paramref name="areaName"/> argument to
        /// this overload.
        /// </remarks>
        public RemoteAttribute(string action, string controller, string areaName)
            : this(action, controller)
        {
            RouteData["area"] = areaName;
        }

        /// <summary>
        /// Gets or sets the HTTP method (<c>"Get"</c> or <c>"Post"</c>) client should use when sending a validation
        /// request.
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated names of fields the client should include in a validation request.
        /// </summary>
        public string AdditionalFields
        {
            get { return _additionalFields; }
            set
            {
                _additionalFields = value ?? string.Empty;
                _additionalFieldsSplit = SplitAndTrimPropertyNames(value)
                    .Select(field => FormatPropertyForClientValidation(field))
                    .ToArray();
            }
        }

        /// <summary>
        /// Gets the <see cref="RouteValueDictionary"/> used when generating the URL where client should send a
        /// validation request.
        /// </summary>
        protected RouteValueDictionary RouteData { get; }

        /// <summary>
        /// Gets or sets the route name used when generating the URL where client should send a validation request.
        /// </summary>
        protected string RouteName { get; set; }

        /// <summary>
        /// Formats <paramref name="property"/> and <see cref="AdditionalFields"/> for use in generated HTML.
        /// </summary>
        /// <param name="property">
        /// Name of the property associated with this <see cref="RemoteAttribute"/> instance.
        /// </param>
        /// <returns>Comma-separated names of fields the client should include in a validation request.</returns>
        /// <remarks>
        /// Excludes any whitespace from <see cref="AdditionalFields"/> in the return value.
        /// Prefixes each field name in the return value with <c>"*."</c>.
        /// </remarks>
        public string FormatAdditionalFieldsForClientValidation(string property)
        {
            if (string.IsNullOrEmpty(property))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(property));
            }

            var delimitedAdditionalFields = string.Join(",", _additionalFieldsSplit);
            if (!string.IsNullOrEmpty(delimitedAdditionalFields))
            {
                delimitedAdditionalFields = "," + delimitedAdditionalFields;
            }

            var formattedString = FormatPropertyForClientValidation(property) + delimitedAdditionalFields;

            return formattedString;
        }

        /// <summary>
        /// Formats <paramref name="property"/> for use in generated HTML.
        /// </summary>
        /// <param name="property">One field name the client should include in a validation request.</param>
        /// <returns>Name of a field the client should include in a validation request.</returns>
        /// <remarks>Returns <paramref name="property"/> with a <c>"*."</c> prefix.</remarks>
        public static string FormatPropertyForClientValidation(string property)
        {
            if (string.IsNullOrEmpty(property))
            {
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(property));
            }

            return "*." + property;
        }

        /// <summary>
        /// Returns the URL where the client should send a validation request.
        /// </summary>
        /// <param name="context">The <see cref="ClientModelValidationContext"/> used to generate the URL.</param>
        /// <returns>The URL where the client should send a validation request.</returns>
        protected virtual string GetUrl(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var services = context.ActionContext.HttpContext.RequestServices;
            var factory = services.GetRequiredService<IUrlHelperFactory>();
            var urlHelper = factory.GetUrlHelper(context.ActionContext);

            var url = urlHelper.RouteUrl(new UrlRouteContext()
            {
                RouteName = this.RouteName,
                Values = RouteData,
            });

            if (url == null)
            {
                throw new InvalidOperationException(Resources.RemoteAttribute_NoUrlFound);
            }

            return url;
        }

        /// <inheritdoc />
        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Always returns <c>true</c> since this <see cref="ValidationAttribute"/> does no validation itself.
        /// Related validations occur only when the client sends a validation request.
        /// </remarks>
        public override bool IsValid(object value)
        {
            return true;
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// Thrown if unable to generate a target URL for a validation request.
        /// </exception>
        public virtual IEnumerable<ModelClientValidationRule> GetClientValidationRules(
            ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var metadata = context.ModelMetadata;
            var rule = new ModelClientValidationRemoteRule(
                FormatErrorMessage(metadata.GetDisplayName()),
                GetUrl(context),
                HttpMethod,
                FormatAdditionalFieldsForClientValidation(metadata.PropertyName));

            return new[] { rule };
        }

        private static IEnumerable<string> SplitAndTrimPropertyNames(string original)
        {
            if (string.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = original.Split(',')
                                .Select(piece => piece.Trim())
                                .Where(trimmed => !string.IsNullOrEmpty(trimmed));
            return split;
        }
    }
}
