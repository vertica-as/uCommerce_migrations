using System.Collections.Generic;
using System.Globalization;
using UCommerce.EntitiesV2;
using UCommerce.Security;

namespace uCommerce.Migrations.Core
{
	internal class DummyUserService : IUserService
	{
		public User GetCurrentUser()
		{
			return new User { Name = "Migration", ExternalId = "Migration", UserName = "Migration", IsAdmin = true };
		}

		public User GetUser(string userName)
		{
			return new User { Name = userName, ExternalId = userName, UserName = userName, IsAdmin = true };
		}

		public IList<User> GetAllUsers()
		{
			return new[] { GetCurrentUser() };
		}

		public string GetCurrentUserName()
		{
			return GetCurrentUser().Name;
		}

		public CultureInfo GetCurrentUserCulture()
		{
			return CultureInfo.InvariantCulture;
		}
	}
}
