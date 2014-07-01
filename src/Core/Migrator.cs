using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Util;
using UCommerce.EntitiesV2;

namespace uCommerce.Migrations.Core
{
	#region Category DefinitionField

	public class DefinitionFieldService
	{
		private readonly IRepository<DefinitionField> _repository;

		public DefinitionFieldService(IRepository<DefinitionField> repository)
		{
			_repository = repository;
		}

		public DefinitionField AddDefinitionField(string name, Definition category, DataType type, bool multilingual = false)
		{
			DefinitionField field = _repository.SingleOrDefault(x => x.Name == name && x.Definition == category) ?? new DefinitionField();
			field.Definition = category;
			field.DataType = type;
			field.Name = name;
			field.DisplayOnSite = true;
			field.Multilingual = multilingual;
			field.RenderInEditor = true;
			field.Deleted = false;

			category.DefinitionFields.Add(field);

			return field;
		}

		public void RemoveDefinitionField(Definition category, params string[] names)
		{
			foreach (var name in names)
			{
				try
				{
					DefinitionField toBeRemoved = _repository.SingleOrDefault(x => x.Name == name && x.Definition == category && !x.Deleted);

					if (toBeRemoved != null)
						category.DefinitionFields.Remove(toBeRemoved);
				}
				catch (Exception ex)
				{
					string message = string.Format("'{0}' category definition field doesn't exist.", name);
					throw new InvalidOperationException(message, ex);
				}
			}
		}
	}

	#endregion

	public class Migrator
	{
		private readonly ISessionProvider _sessionProvider;

		public Migrator(string connectionString)
		{
			_sessionProvider = new SessionProvider(new CustomCommerceConfigurationProvider(connectionString), new DummyUserService());

			_dataTypes = repository<DataType>(_sessionProvider);
			_definitions = repository<Definition>(_sessionProvider);
			_definitionFields = new Lazy<DefinitionFieldService>(() => new DefinitionFieldService(new Repository<DefinitionField>(_sessionProvider)));
			_productDefinitions = repository<ProductDefinition>(_sessionProvider);
			_productDefinitionFields = repository<ProductDefinitionField>(_sessionProvider);
			_currencies = repository<Currency>(_sessionProvider);
			_priceGroups = repository<PriceGroup>(_sessionProvider);
			_productRelationTypes = repository<ProductRelationType>(_sessionProvider);

			_catalogGroups = repository<ProductCatalogGroup>(_sessionProvider);
			_emailProfiles = repository<EmailProfile>(_sessionProvider);
			_orderNumbers = repository<OrderNumberSerie>(_sessionProvider);
			_catalogs = repository<ProductCatalog>(_sessionProvider);

			_categories = repository<Category>(_sessionProvider);
			_shippingMethods = repository<ShippingMethod>(_sessionProvider);
			_shippingMethodPrices = repository<ShippingMethodPrice>(_sessionProvider);

			_countries = repository<Country>(_sessionProvider);
			_paymentRepository = repository<PaymentMethod>(_sessionProvider);
			_orderNumberSeries = repository<OrderNumberSerie>(_sessionProvider);

			_campaigns = repository<Campaign>(_sessionProvider);
			_campaignItems = repository<CampaignItem>(_sessionProvider);

			_status = repository<OrderStatus>(_sessionProvider);
		}

		private static Lazy<Repository<TEntity>> repository<TEntity>(ISessionProvider provider) where TEntity : class
		{
			return new Lazy<Repository<TEntity>>(() => new Repository<TEntity>(provider));
		}

		#region DataTypes

		private readonly Lazy<Repository<DataType>> _dataTypes;
		public DataType DataType(string name)
		{
			var dataType = _dataTypes.Value
				.Single(t => t.DefinitionName == name);
			return dataType;
		}

		public void Save(DataType dataType)
		{
			_dataTypes.Value.Save(dataType);
		}

		public void DeleteDataType(string name)
		{
			var toBeDeleted = _dataTypes.Value.Single(x => x.TypeName == name);
			_dataTypes.Value.Delete(toBeDeleted);
		}

		public DataType CustomDataType(string typeName, string definitionName = null, bool nullable = true)
		{
			var type = _dataTypes.Value
				.SingleOrDefault(x => x.TypeName == typeName);

			if (type == null)
			{
				type = new DataType();
			}

			type.BuiltIn = false;
			type.DefinitionName = definitionName;
			type.Deleted = false;
			type.TypeName = typeName;
			type.Nullable = nullable;
			type.ValidationExpression = string.Empty;

			return type;
		}

		#endregion

		#region category management

		public IEnumerable<ProductCatalogGroup> GetAllProductCatalogGroups()
		{
			return _catalogGroups.Value.Select(x => !x.Deleted);
		}

		#region Category definitions

		private readonly Lazy<Repository<Definition>> _definitions;
		public Definition CategoryDefinition(string name, string description)
		{
			Definition category = _definitions.Value.SingleOrDefault(x => x.Name == name);

			if (category == null)
			{
				var definitionTypes = new Repository<DefinitionType>(_sessionProvider);
				var categoryType = (DefinitionType)EnumerableExtensions.First(definitionTypes.Select());
				category = new Definition { Name = name, Description = description, DefinitionType = categoryType, };
			}
			category.Deleted = false;

			return category;
		}

		public Definition ExistingCategoryDefinition(string name)
		{
			try
			{
				var existing = _definitions.Value.Single(x => x.Name == name && !x.Deleted);
				return existing;
			}
			catch (Exception ex)
			{
				string message = string.Format("'{0}' category definition doesn't exist.", name);
				throw new InvalidOperationException(message, ex);
			}
		}

		public void Save(Definition category)
		{
			_definitions.Value.Save(category);
		}

		public void DeleteCategoryDefinition(string categoryName)
		{
			var toBeDeleted = _definitions.Value.Select(t => t.Name == categoryName);
			_definitions.Value.Delete(toBeDeleted);
		}

		#endregion

		
		private readonly Lazy<DefinitionFieldService> _definitionFields;
		public DefinitionField AddDefinitionField(string name, Definition category, DataType type, bool multilingual = false)
		{
			return _definitionFields.Value.AddDefinitionField(name, category, type, multilingual);
		}

		public void RemoveDefinitionFields(Definition category, params string[] names)
		{
			_definitionFields.Value.RemoveDefinitionField(category, names);
		}

		
		private readonly Lazy<Repository<Category>> _categories;
		public void AssignCategoryToCatalogs(string name, Definition definition, ProductCatalogGroup group)
		{
			var catalogs = _catalogs.Value.Select(c => c.ProductCatalogGroup == group && !c.Deleted);

			foreach (var catalog in catalogs)
			{
				Category category = _categories.Value.SingleOrDefault(c => c.Name == name && c.ProductCatalog.Name == catalog.Name) ??
					new Category();

				category.Name = name;
				category.Definition = definition;
				category.Deleted = false;

				catalog.AddCategory(category);
			}
			_catalogs.Value.Save(catalogs);
		}

		public void AssignCategoryToCatalog(string name, Definition definition, ProductCatalog catalog)
		{
			Category category = _categories.Value
				.SingleOrDefault(c => c.Name == name && c.ProductCatalog.Name == catalog.Name) ??
				new Category();

			category.Name = name;
			category.Definition = definition;
			category.Deleted = false;

			catalog.AddCategory(category);
			_catalogs.Value.Save(catalog);
		}

		public void UnassignCategoryFromCatalogs(string name, ProductCatalogGroup group)
		{
			var catalogs = _catalogs.Value.Select(c => c.ProductCatalogGroup == group && !c.Deleted);

			foreach (var catalog in catalogs)
			{
				Category toBeUnassigned = catalog.Categories.Single(c => c.Name == name);
				catalog.Categories.Remove(toBeUnassigned);
				_categories.Value.Delete(toBeUnassigned);
			}
			_catalogs.Value.Save(catalogs);
		}

		public void UnassignCategoriesFromCatalog(ProductCatalog catalog, params string[] names)
		{
			foreach (string name in names)
			{
				Category toBeUnassigned = catalog.Categories.Single(c => c.Name == name);
				catalog.Categories.Remove(toBeUnassigned);
				_categories.Value.Delete(toBeUnassigned);
			}
			_catalogs.Value.Save(catalog);
		}

		#endregion

		#region product management

		private readonly Lazy<Repository<ProductDefinition>> _productDefinitions;

		public ProductDefinition ProductDefinition(string name, string description)
		{
			var productDefinition = _productDefinitions.Value.SingleOrDefault(x => x.Name == name);
			if (productDefinition == null)
			{
				productDefinition = new ProductDefinition { Name = name, Description = description, };
			}
			productDefinition.Deleted = false;

			return productDefinition;
		}

		public ProductDefinition ExistingProductDefinition(string name)
		{
			try
			{
				var existing = _productDefinitions.Value.Single(x => x.Name == name && !x.Deleted);
				return existing;
			}
			catch (Exception ex)
			{
				string message = string.Format("'{0}' product definition doesn't exist.", name);
				throw new InvalidOperationException(message, ex);
			}
		}

		private readonly Lazy<Repository<ProductDefinitionField>> _productDefinitionFields;

		public ProductDefinitionField AddDefinitionField(string name, ProductDefinition product, DataType type, bool multilingual = false)
		{
			ProductDefinitionField field = _productDefinitionFields.Value.SingleOrDefault(x => x.Name == name && x.ProductDefinition == product);

			if (field == null)
			{
				field = new ProductDefinitionField();
			}
			field.DataType = type;
			field.DisplayOnSite = true;
			field.Name = name;
			field.ProductDefinition = product;
			field.RenderInEditor = true;
			field.Multilingual = multilingual;
			field.Deleted = false;

			product.ProductDefinitionFields.Add(field);
			return field;
		}

		public void RemoveDefinitionFields(ProductDefinition product, params string[] names)
		{
			foreach (var name in names)
			{
				try
				{

					ProductDefinitionField toBeRemoved = _productDefinitionFields.Value.SingleOrDefault(x => x.Name == name && x.ProductDefinition == product && !x.Deleted);
					if (toBeRemoved != null) product.ProductDefinitionFields.Remove(toBeRemoved);
				}
				catch (Exception ex)
				{
					string message = string.Format("'{0}' product definition field doesn't exist.", name);
					throw new InvalidOperationException(message, ex);
				}
			}
		}

		public void Save(ProductDefinition product)
		{
			_productDefinitions.Value.Save(product);
		}

		public void DeleteProductDefinition(string productName)
		{
			var toBeDeleted = _productDefinitions.Value.Select(t => t.Name == productName);
			_productDefinitions.Value.Delete(toBeDeleted);
		}

		#endregion

		#region ProductRelationType management

		private readonly Lazy<Repository<ProductRelationType>> _productRelationTypes;

		public void AddProductRelationType(string name, string description)
		{
			var relationType = _productRelationTypes.Value
				.SingleOrDefault(x => x.Name == name);

			if (relationType == null)
				relationType = new ProductRelationType();

			relationType.Name = name;
			relationType.Description = description;

			_productRelationTypes.Value.Save(relationType);
		}

		public void RemoveProductRelationType(string name)
		{
			var relationType = _productRelationTypes.Value
				.SingleOrDefault(x => x.Name == name);

			if (relationType == null)
				throw new InvalidOperationException(
					string.Format("ProductRelationType {0} doesn't exist.", name));

			_productRelationTypes.Value.Delete(relationType);
		}

		#endregion

		#region currency management

		private readonly Lazy<Repository<Currency>> _currencies;
		public Currency EnsureCurrency(string currencyIsoCode)
		{
			Currency currency = _currencies.Value
				.SingleOrDefault(x => x.ISOCode == currencyIsoCode);

			if (currency == null)
			{
				currency = new Currency
				{
					ExchangeRate = 0,
					ISOCode = currencyIsoCode,
				};
			}
			currency.Deleted = false;
			_currencies.Value.Save(currency);
			return currency;
		}

		public void DeleteCurrency(string currencyIsoCode)
		{
			var toBeDeleted = _currencies.Value.Select(x => x.ISOCode == currencyIsoCode);
			_currencies.Value.Delete(toBeDeleted);
		}

		#endregion

		#region price group management

		private readonly Lazy<Repository<PriceGroup>> _priceGroups;
		public PriceGroup PriceGroup(Currency currency, decimal vatRate, string countryIsoCode = null)
		{
			string name = PriceGroupName(currency.ISOCode, countryIsoCode);
			return PriceGroup(name, currency, vatRate);
		}

		public PriceGroup PriceGroup(string name, Currency currency, decimal vatRate)
		{
			PriceGroup priceGroup = _priceGroups.Value
				.SingleOrDefault(x => x.Name == name);

			if (priceGroup == null)
			{
				priceGroup = new PriceGroup
				{
					Name = name,
					Currency = currency
				};
			}
			priceGroup.VATRate = vatRate;
			priceGroup.Deleted = false;

			return priceGroup;
		}

		public PriceGroup ExistingPriceGroup(string currencyIsoCode, string countryIsoCode = null)
		{
			var name = PriceGroupName(currencyIsoCode, countryIsoCode);

			return _priceGroups.Value.Single(x => x.Name == name);
		}

		public string PriceGroupName(string currencyIsoCode, string countryIsoCode = null, bool previous = false)
		{
			Func<string> optionalCountry = () => string.IsNullOrEmpty(countryIsoCode) ? string.Empty : "_" + countryIsoCode;
			Func<string> optionalPrevious = () => previous ? "_Previous" : string.Empty;

			return string.Format("Web{0}{1}{2}",
				currencyIsoCode,
				optionalCountry(),
				optionalPrevious());
		}

		public void Save(PriceGroup priceGroup)
		{
			_priceGroups.Value.Save(priceGroup);
		}

		public void DeletePriceGroup(string currencyIsoCode, string countryIsocode = null)
		{
			string name = PriceGroupName(currencyIsoCode, countryIsocode);
			DeletePriceGroupByName(name);
		}

		public void DeletePriceGroupByName(string name)
		{
			var toBeDeleted = _priceGroups.Value.Select(t => t.Name == name);
			_priceGroups.Value.Delete(toBeDeleted);
		}

		#endregion

		#region catalog management

		private readonly Lazy<Repository<ProductCatalogGroup>> _catalogGroups;
		private readonly Lazy<Repository<EmailProfile>> _emailProfiles;
		private readonly Lazy<Repository<OrderNumberSerie>> _orderNumbers;
		public ProductCatalogGroup CatalogGroup(string name, string description, Currency defaultCurrency)
		{
			ProductCatalogGroup catalogGroup = _catalogGroups.Value.SingleOrDefault(x => x.Name == name);
			if (catalogGroup == null)
			{
				EmailProfile defaultEmailProfile = _emailProfiles.Value.Select(x => x.Name == "Default" && !x.Deleted).First();
				var defaultOrderNumbers = _orderNumbers.Value.Select(x => x.OrderNumberName == "Default" && !x.Deleted).First();
				catalogGroup = new ProductCatalogGroup
				{
					EmailProfile = defaultEmailProfile,
					OrderNumberSerie = defaultOrderNumbers
				};
			}

			catalogGroup.Name = name;
			catalogGroup.Currency = defaultCurrency;
			catalogGroup.Description = description;
			catalogGroup.MemberGroupId = "-1";
			catalogGroup.MemberTypeId = "-1";
			catalogGroup.Deleted = false;

			return catalogGroup;
		}

		public ProductCatalogGroup ExistingCatalogGroup(string name)
		{
			try
			{
				var existing = _catalogGroups.Value.Single(x => x.Name == name && !x.Deleted);
				return existing;
			}
			catch (Exception ex)
			{
				string message = string.Format("'{0}' catalog group doesn't exist.", name);
				throw new InvalidOperationException(message, ex);
			}
		}

		public void Save(ProductCatalogGroup catalogGroup)
		{
			_catalogGroups.Value.Save(catalogGroup);
		}

		public void DeleteCatalogGroup(string name)
		{
			var toBeDeleted = _catalogGroups.Value.Select(x => x.Name == name);
			_catalogGroups.Value.Delete(toBeDeleted);
		}

		private readonly Lazy<Repository<ProductCatalog>> _catalogs;
		public ProductCatalog Catalog(string name, ProductCatalogGroup catalogGroup, PriceGroup priceGroup)
		{
			var catalog = _catalogs.Value.SingleOrDefault(x => x.Name == name) ?? new ProductCatalog();
			catalog.Name = name;
			catalog.DisplayOnWebSite = false;
			catalog.ProductCatalogGroup = catalogGroup;
			catalog.PriceGroup = priceGroup;
			catalog.Deleted = false;
			return catalog;
		}

		public ProductCatalog ExistingCatalog(string name)
		{
			try
			{
				var existing = _catalogs.Value.Single(x => x.Name == name && !x.Deleted);
				return existing;
			}
			catch (Exception ex)
			{
				string message = string.Format("'{0}' catalog doesn't exist.", name);
				throw new InvalidOperationException(message, ex);
			}
		}

		public void Save(ProductCatalog catalog)
		{
			_catalogs.Value.Save(catalog);
		}

		public void DeleteCatalog(string name)
		{
			var toBeDeleted = _catalogs.Value.Select(x => x.Name == name);
			_catalogs.Value.Delete(toBeDeleted);
		}

		#endregion

		#region role management

		// we refuse to work with roles. point and click at will

		#endregion

		#region Shipping method

		private readonly Lazy<Repository<ShippingMethod>> _shippingMethods;

		public ShippingMethod ShippingMethod(string name, IEnumerable<Country> countries, IEnumerable<ProductCatalogGroup> productCatalogGroups)
		{
			var shippingMethod = _shippingMethods.Value.SingleOrDefault(x => x.Name == name) ?? new ShippingMethod();
			shippingMethod.Name = name;
			shippingMethod.Deleted = false;
			shippingMethod.ServiceName = "SinglePriceService";

			foreach (var productCatalogGroup in productCatalogGroups)
				shippingMethod.AddEligibleProductCatalogGroup(productCatalogGroup);

			foreach (var country in countries)
				shippingMethod.AddEligibleCountry(country);

			return shippingMethod;
		}

		public void Save(ShippingMethod shippingMethod)
		{
			_shippingMethods.Value.Save(shippingMethod);
		}

		public void DeleteShippingMethod(string name)
		{
			var shippingMethod = _shippingMethods.Value.SingleOrDefault(x => x.Name == name);

			if (shippingMethod != null)
				_shippingMethods.Value.Delete(shippingMethod);
		}

		private readonly Lazy<Repository<ShippingMethodPrice>> _shippingMethodPrices;
		public ShippingMethodPrice ShippingMethodPrice(PriceGroup priceGroup, decimal price)
		{
			var shippingMethodPrice = _shippingMethodPrices.Value
				.SingleOrDefault(x => x.Currency == priceGroup.Currency && x.PriceGroup == priceGroup);

			if (shippingMethodPrice == null)
				shippingMethodPrice = new ShippingMethodPrice();

			shippingMethodPrice.Currency = priceGroup.Currency;
			shippingMethodPrice.PriceGroup = priceGroup;
			shippingMethodPrice.Price = price;

			return shippingMethodPrice;
		}

		#endregion

		#region Payments

		private readonly Lazy<Repository<PaymentMethod>> _paymentRepository;

		public PaymentMethod PaymentMethod(string name, IList<Country> countries, string pipeline)
		{
			var paymentMethod = _paymentRepository.Value.SingleOrDefault(x => x.Name == name);

			if (paymentMethod == null)
				paymentMethod = new PaymentMethod();

			paymentMethod.EligibleCountries = countries;
			paymentMethod.Deleted = false;
			paymentMethod.Enabled = true;
			paymentMethod.Name = name;
			paymentMethod.PaymentMethodServiceName = name;
			paymentMethod.Pipeline = pipeline;

			return paymentMethod;
		}

		public void Save(PaymentMethod paymentMethod)
		{
			_paymentRepository.Value.Save(paymentMethod);
		}

		public void DeletePaymentMethod(string name)
		{
			var paymentMethod = _paymentRepository.Value.SingleOrDefault(x => x.Name == name);

			if (paymentMethod != null)
				_paymentRepository.Value.Delete(paymentMethod);
		}

		#endregion

		#region Countries

		private readonly Lazy<Repository<Country>> _countries;

		public IEnumerable<Country> GetAllCountries()
		{
			return _countries.Value.Select(x => !x.Deleted);
		}

		public void Save(Country country)
		{
			_countries.Value.Save(country);
		}

		public void DeleteCountry(string isoCode)
		{
			var toBeDeleted = _countries.Value.Select(x => x.Name == isoCode);
			_countries.Value.Delete(toBeDeleted);
		}

		#endregion

		#region OrderNumbers

		private readonly Lazy<Repository<OrderNumberSerie>> _orderNumberSeries;

		public OrderNumberSerie EnsureOrderNumberSeries(string name)
		{
			var sequence = _orderNumberSeries.Value.SingleOrDefault(x => x.OrderNumberName == name);

			if (sequence == null)
			{
				sequence = new OrderNumberSerie
				{
					OrderNumberName = name,
				};
			}
			sequence.Deleted = false;

			_orderNumberSeries.Value.Save(sequence);

			return sequence;
		}

		public void DeleteOrderNumberSeries(string name)
		{
			var toBeDeleted = _orderNumberSeries.Value.Select(x => x.OrderNumberName == name);
			_orderNumberSeries.Value.Delete(toBeDeleted);
		}

		public void Save(OrderNumberSerie serie)
		{
			_orderNumberSeries.Value.Save(serie);
		}

		#endregion

		#region Campaign

		public Definition CampaignItemDefinition(string name, string description)
		{
			var definitionTypes = new Repository<DefinitionType>(_sessionProvider);
			DefinitionType definitionType = definitionTypes.Single(x => x.Name == UCommerce.Definitions.CampaignItem);
			Definition campaignDefinition = _definitions.Value.SingleOrDefault(x => x.Name == name && x.DefinitionType == definitionType);

			if (campaignDefinition == null)
			{
				campaignDefinition = new Definition
				{
					Name = name,
					Description = description,
					DefinitionType = definitionType,
				};
			}
			campaignDefinition.Deleted = false;

			return campaignDefinition;
		}

		public void DeleteCampignDefinition(string name)
		{
			var definitionTypes = new Repository<DefinitionType>(_sessionProvider);
			DefinitionType definitionType = definitionTypes.Single(x => x.Name == UCommerce.Definitions.CampaignItem);
			Definition campaignDefinition = _definitions.Value.SingleOrDefault(x => x.Name == name && x.DefinitionType == definitionType);

			_definitions.Value.Delete(campaignDefinition);
		}

		private readonly Lazy<Repository<Campaign>> _campaigns;

		public Campaign Campaign(string name, string catalogGroup, DateTime campaignStart, DateTime campaignEnds)
		{
			var campaign = _campaigns.Value.SingleOrDefault(x => x.Name == name);
			var productcatalog = _catalogGroups.Value.Single(x => x.Name == catalogGroup);

			if (campaign == null)
			{
				campaign = new Campaign
				{
					Name = name,
					Enabled = true,
					EndsOn = campaignEnds,
					ProductCatalogGroups = new List<ProductCatalogGroup> { productcatalog },
					StartsOn = campaignStart,
				};
			}
			campaign.Deleted = false;

			return campaign;
		}

		public void Save(Campaign campaign)
		{
			_campaigns.Value.Save(campaign);
		}

		public void DeleteCampaign(string campaignName)
		{
			var campaignToBeDeleted = _campaigns.Value.Select(x => x.Name == campaignName);
			_campaigns.Value.Delete(campaignToBeDeleted);
		}

		private readonly Lazy<Repository<CampaignItem>> _campaignItems;

		public Campaign AddCampaignItems(Campaign campaign, Definition campaignItemDefinition, params string[] names)
		{
			if (campaign.CampaignItems == null)
			{
				campaign.CampaignItems = new List<CampaignItem>();
			}

			foreach (var name in names)
			{
				CampaignItem campaignItem = _campaignItems.Value.SingleOrDefault(x => x.Name == name && x.Campaign == campaign) ?? new CampaignItem
				{
					AllowNextCampaignItems = true, // Allow additional discounts
					Campaign = campaign,
					CampaignItemProperties = null,
					Definition = campaignItemDefinition,
					Enabled = true,
					Name = name,
				};

				campaignItem.Deleted = false;
				campaign.CampaignItems.Add(campaignItem);
			}

			Save(campaign);

			return campaign;
		}

		#endregion

		#region order status

		private readonly Lazy<Repository<OrderStatus>> _status;

		public OrderStatus ExistingOrderStatus(string name)
		{
			try
			{
				OrderStatus existing = _status.Value.Single(x => x.Name == name);
				return existing;
			}
			catch (Exception ex)
			{
				string message = string.Format("Order status named '{0}' doesn't exist.", name);
				throw new InvalidOperationException(message, ex);
			}
		}

		public void DeleteOrderStatus(string name)
		{
			var toBeDeleted = _status.Value.Select(t => t.Name == name);
			_status.Value.Delete(toBeDeleted);
		}

		#endregion
	}
}