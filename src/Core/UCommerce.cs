namespace uCommerce.Migrations.Core
{
	internal class UCommerce
	{
		public class DataTypes
		{
			public const string Number = "Number";
			public const string LongText = "LongText";
			public const string ShortText = "ShortText";
			public const string Boolean = "Boolean";
		}

		public class Definitions
		{
			public const string CampaignItem = "CampaignItem";
		}

		public class ShippingMethods
		{
			public const string Default = "Default";
		}

		public class PaymentMethods
		{
			public const string Dibs = "DIBS";
		}

		public class ProductRelations
		{
			public class Names
			{
				public const string RelatedProducts = "Related Products";
			}
		}

		public class Currencies
		{
			public const string SwedishKroner = "SEK";
			public const string BritishPound = "GBP";
			public const string Euro = "EUR";
			public const string DanishKroner = "DKK";
			public const string NorwegianKroner = "NOK";
		}

		public static int Counter = 0;
	}
}
