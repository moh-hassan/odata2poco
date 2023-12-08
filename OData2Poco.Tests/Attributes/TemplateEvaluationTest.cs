// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests.Attributes
{
    internal class TemplateEvaluationTest
    {
        private readonly PropertyTemplate _prop = new()
        {
            PropName = "ProductId",
            PropType = "int",
            IsNullable = false,
            IsKey = true,
            Serial = 1,
        };

        [Test]
        [TestCase("[Key]", "[Key]")]
        [TestCase("[Required]", "[Required]")]
        [TestCase("[JsonPropertyName({{PropName.Quote()}})]", "[JsonPropertyName(\"ProductId\")]")]
        public void EvaluateTemplate_test(string template, string expected)
        {
            var result = template.EvaluateTemplate(_prop, out var errors);
            //Assert
            expected.Should().Be(result);
            Assert.That(errors, Is.Empty);


        }
        [Test]
        [TestCase("IsKey", true)]
        [TestCase("!IsKey", false)]
        [TestCase("IsNullable", false)]
        [TestCase("!IsNullable", true)]
        [TestCase("IsKey && !IsNullable", true)]
        [TestCase("IsKey && IsNullable", false)]
        [TestCase("IsKey || IsNullable", true)]
        [TestCase("IsKey || !IsNullable", true)]
        public void EvaluateConditionExpression_test(string condition, bool expected)
        {
            var result = condition.EvaluateCondition(_prop, out var error);
            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(expected, Is.EqualTo(result));
                Assert.That(error, Is.Empty);
            });
        }
    }
}
