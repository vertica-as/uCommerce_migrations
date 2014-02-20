<configuration>
	<components>
		<!--component id="UserService"
			service="UCommerce.Security.IUserService, UCommerce"
			type="UCommerce.Umbraco.Security.UmbracoUserService, UCommerce.Umbraco" /-->
		<component id="UserService"
			service="UCommerce.Security.IUserService, UCommerce"
			type="uCommerce.Migrations.Core.DummyUserService, $AssemblyName$" />
		<component
			id="UCommerce.Tag"
			service="UCommerce.EntitiesV2.IContainsNHibernateMappingsTag, UCommerce"
			type="UCommerce.EntitiesV2.MappingAssemblyTag, UCommerce"/>
	</components>
</configuration>