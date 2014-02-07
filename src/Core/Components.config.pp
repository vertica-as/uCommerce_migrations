<configuration>
	<components>
		<!--component id="UserService"
			service="UCommerce.Security.IUserService, UCommerce"
			type="UCommerce.Umbraco.Security.UmbracoUserService, UCommerce.Umbraco" /-->
		<component id="UserService"
			service="UCommerce.Security.IUserService, UCommerce"
			type="uCommerce.Migrations.Core.DummyUserService, $AssemblyName$" />
	</components>
</configuration>