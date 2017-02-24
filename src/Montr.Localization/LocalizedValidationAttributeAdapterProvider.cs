using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.DataAnnotations.Internal;
using Microsoft.Extensions.Localization;
using Montr.Localization.Resources;

namespace Montr.Localization
{
	// https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.DataAnnotations/ValidationAttributeAdapterProvider.cs
    public class LocalizedValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
	{
		public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
		{
			if (attribute == null) throw new ArgumentNullException(nameof(attribute));

			IAttributeAdapter adapter;

			var type = attribute.GetType();

			if (type == typeof(RegularExpressionAttribute))
			{
				adapter = new RegularExpressionAttributeAdapter((RegularExpressionAttribute)attribute, stringLocalizer);
			}
			else if (type == typeof(MaxLengthAttribute))
			{
				adapter = new MaxLengthAttributeAdapter((MaxLengthAttribute)attribute, stringLocalizer);
			}
			else if (type == typeof(RequiredAttribute))
			{
				adapter = new LocalizedRequiredAttributeAdapter((RequiredAttribute)attribute, stringLocalizer); // +
			}
			else if (type == typeof(CompareAttribute))
			{
				adapter = new CompareAttributeAdapter((CompareAttribute)attribute, stringLocalizer);
			}
			else if (type == typeof(MinLengthAttribute))
			{
				adapter = new MinLengthAttributeAdapter((MinLengthAttribute)attribute, stringLocalizer);
			}
			else if (type == typeof(CreditCardAttribute))
			{
				adapter = new DataTypeAttributeAdapter((DataTypeAttribute)attribute, "data-val-creditcard", stringLocalizer);
			}
			else if (type == typeof(StringLengthAttribute))
			{
				adapter = new StringLengthAttributeAdapter((StringLengthAttribute)attribute, stringLocalizer);
			}
			else if (type == typeof(RangeAttribute))
			{
				adapter = new RangeAttributeAdapter((RangeAttribute)attribute, stringLocalizer);
			}
			else if (type == typeof(EmailAddressAttribute))
			{
				adapter = new DataTypeAttributeAdapter((DataTypeAttribute)attribute, "data-val-email", stringLocalizer);
			}
			else if (type == typeof(PhoneAttribute))
			{
				adapter = new DataTypeAttributeAdapter((DataTypeAttribute)attribute, "data-val-phone", stringLocalizer);
			}
			else if (type == typeof(UrlAttribute))
			{
				adapter = new DataTypeAttributeAdapter((DataTypeAttribute)attribute, "data-val-url", stringLocalizer);
			}
			else
			{
				adapter = null;
			}

			return adapter;
		}

		public class LocalizedRequiredAttributeAdapter : RequiredAttributeAdapter
		{
			public LocalizedRequiredAttributeAdapter(RequiredAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
			{
				if (attribute.ErrorMessageResourceType == null)
					attribute.ErrorMessageResourceType = typeof(DataAnnotationResources);
				if (attribute.ErrorMessageResourceName == null)
					attribute.ErrorMessageResourceName = nameof(DataAnnotationResources.ValueRequired);
			}
		}
	}
}
