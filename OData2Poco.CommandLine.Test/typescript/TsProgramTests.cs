// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.CommandLine.Test;

public partial class ProgramTests
{
    [Test]
    public async Task Default_type_for_typescript_should_be_interface_Test()
    {
        //Arrange
        var cli = $"-r {TestSample.TripPin4} --lang ts -v";
        var expected = @"
export interface City  {
	 CountryRegion: string; //Not null
	 Name: string; //Not null
	 Region: string; //Not null
    }
";
        //Act
        var tuble = await RunCommand(cli).ConfigureAwait(false);
        var output = tuble.Item2;

        //Assert
        output.ShouldContain(expected);
    }

    [Test]
    public async Task Typescript_class_with_inheritance_should_baseclass_preceed_child_test()
    {
        //Arrange
        var cli = $"-r {TestSample.TripPin4} --lang ts -v -B -G class";
        var expected = @"
export class Location  {
    		public Address: string; //Not null
    		public City?: City;
    	}
    
    	export class EventLocation extends Location {
    		public BuildingInfo?: string;
    	}
";
        //Act
        var tuble = await RunCommand(cli).ConfigureAwait(false);
        var output = tuble.Item2;

        //Assert
        output.ShouldContain(expected);
    }

    [Test]
    [TestCase("-b")]
    [TestCase("-B")]
    public async Task Optional_property_Test(string arg)
    {
        //Arrange
        var cli = $"-r {TestSample.TripPin4} --lang ts -v {arg}";
        var expected = @"
export interface PlanItem  {
	  PlanItemId: number; //Not null, Primary key, ReadOnly
	  ConfirmationCode?: string;
	  StartsAt?: Date;
	  EndsAt?: Date;
	  Duration?: Date;
}
";
        //Act
        var tuple = await RunCommand(cli).ConfigureAwait(false);
        //Assert
        var output = tuple.Item2;
        output.ShouldContain(expected, false);
    }

    [Test]
    public async Task Poco_generation_include_namespace_test()
    {
        //Arrange
        var cli = $"-r {TestSample.TripPin4} --lang ts -v ";
        var expected = @"
export namespace MicrosoftODataSampleServiceModelsTripPin {
export interface Airline  {
	  AirlineCode: string; //Not null, Primary key, ReadOnly
	  Name: string; //Not null
}
";
        //Act
        var tuble = await RunCommand(cli).ConfigureAwait(false);
        var output = tuble.Item2;
        //Assert
        output.ShouldContain(expected);
    }
}
