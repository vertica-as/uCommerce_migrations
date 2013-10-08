namespace uCommerce.Migrations.Core
{
	internal class UCommerce
	{
		public class Catalog
		{
			public static string Name(string countryIsoCode)
			{
				return "Rosendahl_" + countryIsoCode;
			}

			public const string Group = "Rosendahl";
		}

		public class DataTypes
		{
			public const string Number = "Number";
			public const string LongText = "LongText";
			public const string ShortText = "ShortText";
			public const string Boolean = "Boolean";
			public const string Videos = "Videos";
		}

		public class Categories
		{
			public const string JustImported = "Just Imported Category";
			public const string Just_Imported = "Just_Imported";
			public const string Rosendahl = "Rosendahl Category";
			public const string RosendahlHomepage = "Rosendahl Homepage Category";
			public const string RosendahlBasketpage = "Rosendahl Basketpage Category";
			public const string RosendahlConfirmationpage = "Rosendahl Confirmationpage Category";
			public const string Basketpage = "Basketpage";
			public const string Confirmationpage = "Confirmationpage";

			public const string ArchivedProducts = "Archived Products Category";
			public const string Archived_Products = "Archived_Products";

			public class Properties
			{
				public const string MetaDescription = "MetaDescription";
				public const string MetaPageTitle = "MetaPageTitle";
			}
		}

		public class Definitions
		{
			public const string CampaignItem = "CampaignItem";
		}

		public class Products
		{
			public const string RosendahlStandardProduct = "Rosendahl Standard Product";
			public const string RosendahlInspirationalProduct = "Rosendahl Inspirational Product";
			public const string RosendahlNavigationProduct = "Rosendahl Navigation Product";

			public class Properties
			{
				public const string Color = "Color";
				public const string Material = "Material";
				public const string Depth = "Depth";
				public const string Designer = "Designer";
				public const string Height = "Height";
				public const string Width = "Width";
				public const string ProductLineShort = "ProductLineShort";
				public const string ProductLineLong = "ProductLineLong";
				public const string FunctionGroupShort = "FunctionGroupShort";
				public const string FunctionGroupLong = "FunctionGroupLong";
				public const string Brand = "Brand";
				public const string NumberOfItemsInProduct = "NumberOfItemsInProduct";
				public const string InventoryOnHand = "InventoryOnHand";
				public const string NextExpectedInventoryDeliveryDate = "NextExpectedInventoryDeliveryDate";
				public const string WebDisplayName = "WebDisplayName";
				public const string VolumeAmount = "VolumeAmount";
				public const string VolumeUnit = "VolumeUnit";
				public const string Diameter = "Diameter";
				public const string Videos = "Videos";

				public const string IsMicrowaveSafe = "IsMicrowaveSafe";
				public const string IsDishWasherSafe = "IsDishWasherSafe";
				public const string IsOvenSafe = "IsOvenSafe";
				public const string CanBeFrozen = "CanBeFrozen";
				public const string CanContainHotLiquids = "CanContainHotLiquids";
				public const string CapabilitiesText = "CapabilitiesText";
				public const string ColliSize = "ColliSize";
				public const string MetaDescription = "MetaDescription";
				public const string MetaPageTitle = "MetaPageTitle";

				public const string InspirationUrl = "InspirationUrl";

				public const string NavigationUrl = "NavigationUrl";
				public const string SearchKeywords = "SearchKeywords";

				public const string StockStatus = "StockStatus";
			}
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

		public class Campaigns
		{
			public class CampaignItem
			{
				public static string NewsletterName(string countryIsoCode)
				{
					return "Newsletter signup " + countryIsoCode;
				}

				public static string WinbackName(string countryIsoCode)
				{
					return "WinBack " + countryIsoCode;
				}

				public const string NewsletterSignUp = "Newsletter sign up";
				public const string WinBack = "Win back";
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
