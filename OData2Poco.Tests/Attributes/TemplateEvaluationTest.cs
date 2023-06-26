// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using NUnit.Framework;

namespace OData2Poco.Tests.Attributes
{
    internal class TemplateEvaluationTest
    {
        PropertyTemplate prop = new()
        {
            PropName = "ProductId",
            PropType = "int",
            IsNullable = false,
            IsKey = true,
            Serial = 1,
        };

        [Test]
        [TestCase("[Key]","[Key]")]
        [TestCase("[Required]", "[Required]")]
        [TestCase("[JsonPropertyName({{PropName.Quote()}})]", "[JsonPropertyName(\"ProductId\")]")]
        public void EvaluateTemplate_test(string template, string expected)
        {
            var result = template.EvaluateTemplate(prop, out var errors);
            //Assert
            Assert.AreEqual(expected, result);
            Assert.IsEmpty(errors);

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
            var result = condition.EvaluateCondition(prop, out var error);
            //Assert
            Assert.AreEqual(expected, result);
            Assert.IsEmpty(error);

        }
    }
}
