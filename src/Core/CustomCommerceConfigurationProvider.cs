using UCommerce.Infrastructure.Configuration;

namespace uCommerce.Migrations.Core
{
	public class CustomCommerceConfigurationProvider : CommerceConfigurationProvider
	{
		private readonly string _connectionString;

		public CustomCommerceConfigurationProvider(string connectionString)
		{
			_connectionString = connectionString;
		}

		public override RuntimeConfigurationSection GetRuntimeConfiguration()
		{
			return new RuntimeConfigurationSection
			{
				EnableCache = false,
				ConnectionString = _connectionString,
			};
		}

		public override SecurityConfigurationSection GetSecurityConfiguration()
		{
			return new SecurityConfigurationSection()
			{
				Enable = false,
			};
		}
	}
}