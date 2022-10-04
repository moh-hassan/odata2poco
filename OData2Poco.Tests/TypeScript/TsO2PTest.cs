using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using OData2Poco.Api;
using OData2Poco.Extensions;
using OData2Poco.Tests;
using OData2Poco.TextTransform;
using OData2Poco.TypeScript;


namespace OData2Poco.Test.TypeScript
{
    [Category("typescript")]
    public class TsO2PTest : BaseTest
    {
        #region data
        string Url { get; } = TestSample.TripPin4;
        const string TsClassCode = @"
export class Location  {
		public address: string; //Not null
		public city?: City;
	}

	export class EventLocation extends Location {
		public buildingInfo?: string;
	}
";
        const string TsInterfaceCode = @"
export interface Location  {
		address: string; //Not null
		city?: City;
	}

	export interface EventLocation extends Location {
		buildingInfo?: string;
	}
";
        #endregion

        [Test]
        [TestCase(GeneratorType.Class,TsClassCode)]
        [TestCase(GeneratorType.Interface, TsInterfaceCode)]
        public async Task Generate_typescribt_as_single_file_test(GeneratorType type, 
            string expected)
        {
            //Arrange           
            PocoSetting setting = new()
            {
                Lang = Language.TS,
                NameCase = CaseEnum.Camel,
                GeneratorType = type,
                AddNavigation = true,
                EnableNullableReferenceTypes = true
            };
            var o2 = new O2P(setting);
             
            //Act             
            var cs = OdataConnectionString.Create(Url);
            var pocostore = await o2.GenerateTsAsync(cs);
            var code = pocostore[0].Code ?? "";
            //Assert
            code.ToLines().Should().ContainInOrder(expected.ToLines());
            pocostore.Count.Should().Be(1);             
        }

        [Test]
        public async Task Generate_typescribt_interface_as_single_file_test()
        {
            //Arrange
            GeneratorType type = GeneratorType.Interface;
            PocoSetting setting = new()
            {
                Lang = Language.TS,
                NameCase = CaseEnum.Camel,
                GeneratorType = type,
                AddNavigation = true,
                EnableNullableReferenceTypes = true
            };
            var o2 = new O2P(setting);
            var expected = @"
export interface Location  {
		address: string; //Not null
		city?: City;
	}

	export interface EventLocation extends Location {
		buildingInfo?: string;
	}
";
            //Act            
            var cs = OdataConnectionString.Create(Url);
            var pocostore = await o2.GenerateTsAsync(cs);
            var code = pocostore[0].Code ?? "";
            //Assert
            code.ToLines().Should().ContainInOrder(expected.ToLines());
            Regex.Matches(code, "export interface").Cast<Match>().Count().Should().Be(13);
            Regex.Matches(code, "export enum PersonGender").Cast<Match>().Count().Should().Be(1);
            Assert.That(code, Does.Not.Contain("class"));
        }
        [Test]
        public async Task Generate_typescribt_class_as_multi_files_test()
        {
            //Arrange
            GeneratorType type = GeneratorType.Class;
            PocoSetting setting = new()
            {
                Lang = Language.TS,
                NameCase = CaseEnum.Camel,
                GeneratorType = type,
                AddNavigation = true,
                EnableNullableReferenceTypes = true,
                MultiFiles = true
            };
            var o2 = new O2P(setting);
            var expected = @"
import {Photo} from './Photo';
import {PlanItem} from './PlanItem';

export class Trip  {
	public tripId: number; //Not null, Primary key, ReadOnly
	public shareId?: string;
	public description?: string;
	public name: string; //Not null
	public budget: number; //Not null
	public startsAt: Date; //Not null
	public endsAt: Date; //Not null
	public tags?: string[];
	public photos?: Photo[]; //navigator
	public planItems?: PlanItem[]; //navigator
}
";
            //Act             
            var cs = OdataConnectionString.Create(Url);
            var codes = await o2.GenerateTsAsync(cs);
            
            //Assert
            var trip = codes.FirstOrDefault(a => a.Name.EndsWith("Trip"));
            codes.Count.Should().Be(14);
            trip.Code.ToLines().Should().ContainInOrder(expected.ToLines());

        }

        [Test]
        [TestCase(GeneratorType.Interface, 13, 1, "interface")]
        [TestCase(GeneratorType.Class, 13, 1, "class")]
        public async Task Multi_files_can_generate_class_or_interface_test(GeneratorType type, int n1, int n2, string keyword)
        {
            //Arrange
            PocoSetting setting = new()
            {
                Lang = Language.TS,
                NameCase = CaseEnum.Camel,
                GeneratorType = type,
                AddNavigation = true,
                EnableNullableReferenceTypes = true,
                MultiFiles = true
            };
            var o2 = new O2P(setting);

            //Act             
            var cs = OdataConnectionString.Create(Url);
            var codes = await o2.GenerateTsAsync(cs);

            //Assert            
            codes.Count(a => a.Code.Contains(keyword))
                .Should().Be(n1);
            codes.Count(a => a.Code.Contains("enum"))
               .Should().Be(n2);
        }
    }
}

