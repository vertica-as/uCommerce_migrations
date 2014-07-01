using System;
using System.Linq;
using System.Linq.Expressions;
using NSubstitute;
using NUnit.Framework;
using UCommerce.EntitiesV2;
using uCommerce.Migrations.Core;

namespace uCommerce.Migrations.Tests.Core.Migrator
{
	[TestFixture]
	public class AddDefinitionFieldTester
	{
		public class TestableRepository : Repository<DefinitionField>
		{
			private DefinitionField[] _fields;
			
			public TestableRepository(DefinitionField[] fields)
				: base(null)
			{
				_fields = fields;
			}

			public override DefinitionField SingleOrDefault(Expression<Func<DefinitionField, bool>> expression)
			{
				return _fields.SingleOrDefault(expression.Compile());
			}
		}

		[Test]
		public void AddDefinitionField_CreatingDefinitionField_WillNotMoveExistingDefinitionField()
		{
			const string FieldName = "TEST_FIELD";

			var categoryDefinitionOne = new Definition();
			var categoryOneDefinitionField = new DefinitionField { Name = FieldName, Definition = categoryDefinitionOne };
			var fields = new[]
			{
				categoryOneDefinitionField,
			};

			var categoryTwoDefinition = new Definition();
			var someDataType = Substitute.For<DataType>();

			var repository = new TestableRepository(fields);
			var definitionFieldService = new DefinitionFieldService(repository);
			var newDefinitionField = definitionFieldService.AddDefinitionField(FieldName, categoryTwoDefinition, someDataType);

			Assert.That(categoryOneDefinitionField, Is.Not.EqualTo(newDefinitionField));
		}
	}
}