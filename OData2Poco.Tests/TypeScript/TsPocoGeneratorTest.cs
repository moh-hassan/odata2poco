// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Tests.TypeScript;

using System.Text.RegularExpressions;
using OData2Poco.TypeScript;

[Category("typescript")]
public class TsPocoGeneratorTest : BaseTest
{
    [Test]
    public async Task Ts_generation_is_interface_by_default_test()
    {
        //Arrange
        PocoSetting setting = new()
        {
            Lang = Language.TS
        };

        var gen = await Moq.TripPin4IgenAsync().ConfigureAwait(false);
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
        var code = tsGen.GeneratePoco()[0]?.Code;

        //Assert
        code.ShouldContain(expected, false);
    }

    [Test]
    [TestCase(GeneratorType.Class, TsTestData.Class, "export class")]
    [TestCase(GeneratorType.Interface, TsTestData.Interface, "export interface")]
    public async Task Generate_single_file_test(
        GeneratorType genType,
        string expected,
        string keyWord)
    {
        //Arrange
        PocoSetting setting = new()
        {
            Lang = Language.TS,
            GeneratorType = genType
        };

        var gen = await Moq.TripPin4IgenAsync(setting).ConfigureAwait(false);

        //Act
        var tsGen = new TsPocoGenerator(gen, setting);
        var code = tsGen.GeneratePoco()[0]?.Code;
        //Assert
        code.ShouldContain(expected);
        code.Should().Contain(keyWord, Exactly.Times(13));
        code.Should().Contain("export enum", Exactly.Once());
    }

    [Test]
    [TestCase(GeneratorType.Class, TsTestData.ClassMultiFiles, "export class")]
    [TestCase(GeneratorType.Interface, TsTestData.InterfaceMultiFiles, "export interface")]
    public async Task Generate_multi_file_test(
        GeneratorType genType,
        string expected,
        string keyword)
    {
        //Arrange
        PocoSetting setting = new()
        {
            Lang = Language.TS,
            MultiFiles = true,
            GeneratorType = genType
        };
        //Act
        var gen = await Moq.TripPin4IgenAsync(setting).ConfigureAwait(false);
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
    [TestCase(GeneratorType.Class, TsTestData.ClassMultiFilesUsingFullName, "export class")]
    [TestCase(GeneratorType.Interface, TsTestData.InterfaceMultiFilesUsingFullName, "export interface")]
    public async Task Generate_multi_file_test_using_full_name_test(
        GeneratorType genType,
        string expected,
        string keyword)
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
        var gen = await Moq.TripPin4IgenAsync(setting).ConfigureAwait(false);
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
    [TestCase(GeneratorType.Class, TsTestData.Enum)]
    [TestCase(GeneratorType.Interface, TsTestData.Enum)]
    public async Task Generate_enum_test(GeneratorType genType, string expected)
    {
        //Arrange
        PocoSetting setting = new()
        {
            Lang = Language.TS,
            GeneratorType = genType
        };

        var gen = await Moq.TripPin4IgenAsync(setting).ConfigureAwait(false);

        //Act
        var tsGen = new TsPocoGenerator(gen, setting);
        var code = tsGen.GeneratePoco().ToString();
        //Assert
        code.ShouldContain(expected);
    }

    [Test]
    public void ClassTemplates_should_be_orderered_bydefault_parent_preceed_child()
    {
        //Arrange
        var list = _classList;
        var expected =
            "Airline,Airport,City,Location,AirportLocation,EventLocation,Person,Photo,PlanItem,Event,PublicTransportation,Flight,PersonGender,Trip";

        list.Sort();
        var sut = string.Join(",", list.Select(a => a.Name));

        //Assert
        sut.Should().Be(expected);
    }

    [Test]
    [TestCase(true, MembersTrue)]
    [TestCase(false, MembersFalse)]
    public void Get_typescript_imports(bool useFullName, string expected)
    {
        //Arrange
        var setting = new PocoSetting()
        {
            UseFullName = useFullName
        };
        var ct = GetClassTemplateSample("Person");

        //Act
        var sut = ct.GetImports(_classList, setting).ToString();

        //Assert
        sut.ShouldContain(expected);
    }

    #region Expected Result

    private const string MembersTrue = @"
import {MicrosoftODataSampleServiceModelsTripPinLocation} from './MicrosoftODataSampleServiceModelsTripPinLocation';
import {MicrosoftODataSampleServiceModelsTripPinCity} from './MicrosoftODataSampleServiceModelsTripPinCity';
import {MicrosoftODataSampleServiceModelsTripPinPersonGender} from './MicrosoftODataSampleServiceModelsTripPinPersonGender';
import {MicrosoftODataSampleServiceModelsTripPinTrip} from './MicrosoftODataSampleServiceModelsTripPinTrip';
import {MicrosoftODataSampleServiceModelsTripPinPhoto} from './MicrosoftODataSampleServiceModelsTripPinPhoto';
import {MicrosoftODataSampleServiceModelsTripPinPlanItem} from './MicrosoftODataSampleServiceModelsTripPinPlanItem';
";

    //members of Person class

    private const string MembersFalse = @"
import {Location} from './Location';
import {City} from './City';
import {PersonGender} from './PersonGender';
import {Trip} from './Trip';
import {Photo} from './Photo';
import {PlanItem} from './PlanItem';
";

    #endregion
}
