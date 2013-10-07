using System;
using NHibernate.Util;
using UCommerce.EntitiesV2;

namespace uCommerce.Migrations.Core
{
	public class Migrator
	{
		 private readonly SessionProvider _sessionProvider;

		public Migrator(string connectionString)
		{
			_sessionProvider = new SessionProvider(new CustomCommerceConfigurationProvider(connectionString), new DummyUserService());

			_definitions = new Lazy<Repository<Definition>>(() => new Repository<Definition>(_sessionProvider));
		}

		#region categories

		private readonly Lazy<Repository<Definition>> _definitions;

		public Definition CategoryDefinition(string name, string description)
		{
			Definition category = _definitions.Value.SingleOrDefault(x => x.Name == name);

			if (category == null)
			{
				var definitionTypes = new Repository<DefinitionType>(_sessionProvider);
				var categoryType = (DefinitionType)definitionTypes.Select().First();
				category = new Definition { Name = name, Description = description, DefinitionType = categoryType, };
			}
			category.Deleted = false;

			return category;
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
	}
}