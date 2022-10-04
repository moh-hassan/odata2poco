
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using OData2Poco.Api;
using OData2Poco.Extensions;
using OData2Poco.Test.TypeScript;
using OData2Poco.TypeScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OData2Poco.Tests.TypeScript
{

    [Category("typescript")]
    public class TsPocoGeneratorTest : BaseTest
    {
        [Test]        
        public async Task Ts_generation_is_interface_by_default_test()             
        {
            //Arrange             
            PocoSetting setting = new()
            {
                Lang = Language.TS,                
            };

            var gen = await Moq.TripPin4IgenAsync();
            var expected = @"  
export interface Airline
export interface Airport
export interface City
export interface Location
export interface AirportLocation extends Location
export interface EventLocation extends Location
export interface Person
export interface Photo
export interface PlanItem
export interface Event extends PlanItem
export interface PublicTransportation extends PlanItem
export interface Flight extends PublicTransportation
export enum PersonGender
export interface Trip 
";
            //Act
            var tsGen = new TsPocoGenerator(gen, setting);
            string code = tsGen.GeneratePoco()[0].Code;
            
            //Assert            
            code.ShouldContain(expected,false);
        }

        [Test]
        [TestCase(GeneratorType.Class, TsTestData.ExpectedClass, "export class")]
        [TestCase(GeneratorType.Interface, TsTestData.ExpectedInterface, "export interface")]
        public async Task Generate_single_file_test(GeneratorType genType,
            string expected, string keyWord)
        {
            //Arrange             
            PocoSetting setting = new()
            {
                Lang = Language.TS,
                GeneratorType = genType
            };
            
            var gen = await Moq.TripPin4IgenAsync(setting);

            //Act
            var tsGen = new TsPocoGenerator(gen, setting);
            string code = tsGen.GeneratePoco()[0].Code;
            //Assert
            code.ShouldContain(expected);
            code.Should().Contain(keyWord, Exactly.Times(13));
            code.Should().Contain("export enum", Exactly.Once());

        }

        [Test]
        [TestCase(GeneratorType.Class, TsTestData.ExpectedClassMultiFiles, "export class")]
        [TestCase(GeneratorType.Interface, TsTestData.ExpectedInterfaceMultiFiles, "export interface")]
        public async Task Generate_multi_file_test(GeneratorType genType,
            string expected, string keyword)
        {
            //Arrange
            PocoSetting setting = new()
            {
                Lang = Language.TS,
                MultiFiles = true,
                GeneratorType = genType
            };
            //Act            
            var gen = await Moq.TripPin4IgenAsync(setting);
            var tsGen = new TsPocoGenerator(gen, setting);            
            var pocoStore = tsGen.GeneratePoco();

            //Assert
            pocoStore.Should().HaveCount(14);            

            pocoStore["Microsoft.OData.SampleService.Models.TripPin.Flight"]?
                .Code.ShouldContain(expected);

            pocoStore.Where(a => Regex.IsMatch(a.Code, keyword))
                .Should().HaveCount(13);
            pocoStore.Where(a => Regex.IsMatch(a.Code, "export enum"))
               .Should().HaveCount(1);
        }

        [Test]
        [TestCase(GeneratorType.Class, TsTestData.ExpectedClassMultiFilesUsingFullName,
            "export class")]
        [TestCase(GeneratorType.Interface, TsTestData.ExpectedInterfaceMultiFilesUsingFullName, "export interface")]
        public async Task Generate_multi_file_test_using_full_name_test(GeneratorType genType,
            string expected, string keyword)
        {
            //Arrange
            PocoSetting setting = new()
            {
                Lang = Language.TS,
                MultiFiles = true,
                GeneratorType = genType,
                UseFullName = true
            };
            //Act
            var gen = await Moq.TripPin4IgenAsync(setting);
            var tsGen = new TsPocoGenerator(gen, setting);            
            var pocoStore = tsGen.GeneratePoco();

            //Assert
            pocoStore.Should().HaveCount(14);
            pocoStore["Microsoft.OData.SampleService.Models.TripPin.Flight"]?
                .Code.ShouldContain(expected);
            pocoStore.Where(a => Regex.IsMatch(a.Code, keyword))
                .Should().HaveCount(13);
            pocoStore.Where(a => Regex.IsMatch(a.Code, "export enum"))
               .Should().HaveCount(1);
        }
        [Test]
        [TestCase(GeneratorType.Class, TsTestData.ExpectedEnum)]
        [TestCase(GeneratorType.Interface, TsTestData.ExpectedEnum)]
        public async Task Generate_enum_test(GeneratorType genType, string expected)
        {
            //Arrange          
            PocoSetting setting = new()
            {
                Lang = Language.TS,
                GeneratorType = genType
            };
           
            var gen = await Moq.TripPin4IgenAsync(setting);

            //Act
            var tsGen = new TsPocoGenerator(gen, setting);
            string code = tsGen.GeneratePoco()[0].Code;
            //Assert
            code.ShouldContain(expected);
        }

        [Test]
        public void ClassTemplates_should_be_orderered_bydefault_parent_preceed_child()
        {          
            //Arrange
            var list = ClassList;
            var expected = "Airline,Airport,City,Location,AirportLocation,EventLocation,Person,Photo,PlanItem,Event,PublicTransportation,Flight,PersonGender,Trip";

            list.Sort();             
            var  sut = string.Join(",", list.Select(a => a.Name));
             
            //Assert
            sut.Should().Be(expected);
        }

        [Test]
        [TestCase(true, MembersTrue)]
        [TestCase(false, MembersFalse)]
        public void Get_typescript_imports(bool useFullName, string expected)
        {
            //Arrange
            var setting = new PocoSetting() { UseFullName = useFullName };
            var ct = GetClassTemplateSample("Person");

            //Act
            var sut = ct.GetImports(ClassList, setting).ToString();

            //Assert            
            sut.ShouldContain(expected);

        }

        #region Expected Result
        const string MembersTrue = @"
import {MicrosoftODataSampleServiceModelsTripPinLocation} from './MicrosoftODataSampleServiceModelsTripPinLocation';
import {MicrosoftODataSampleServiceModelsTripPinCity} from './MicrosoftODataSampleServiceModelsTripPinCity';
import {MicrosoftODataSampleServiceModelsTripPinPersonGender} from './MicrosoftODataSampleServiceModelsTripPinPersonGender';
import {MicrosoftODataSampleServiceModelsTripPinTrip} from './MicrosoftODataSampleServiceModelsTripPinTrip';
import {MicrosoftODataSampleServiceModelsTripPinPhoto} from './MicrosoftODataSampleServiceModelsTripPinPhoto';
import {MicrosoftODataSampleServiceModelsTripPinPlanItem} from './MicrosoftODataSampleServiceModelsTripPinPlanItem';
";
        //members of Person class
         
        const string MembersFalse = @"
import {Location} from './Location';
import {City} from './City';
import {PersonGender} from './PersonGender';
import {Trip} from './Trip';
import {Photo} from './Photo';
import {PlanItem} from './PlanItem';
";

        #endregion
    }
}
