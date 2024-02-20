// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

namespace OData2Poco.Fake;

internal static class TsTestData
{
    #region data without navigation

    internal const string Class = @"  
export class Flight extends PublicTransportation {
	public FlightNumber: string; //Not null
	// public From: Airport; //navigator
	// public To: Airport; //navigator
	// public Airline: Airline; //navigator
}
";

    internal const string Interface = @"
export interface Flight extends PublicTransportation {
	FlightNumber: string; //Not null
	// From: Airport; //navigator
	// To: Airport; //navigator
	// Airline: Airline; //navigator
}
";

    internal const string ClassMultiFiles = @"  
import {Airport} from './Airport';
import {AirportLocation} from './AirportLocation';
import {Location} from './Location';
import {City} from './City';
import {Airline} from './Airline';
import {PublicTransportation} from './PublicTransportation';
import {PlanItem} from './PlanItem';

export class Flight extends PublicTransportation {
	public FlightNumber: string; //Not null
	// public From: Airport; //navigator
	// public To: Airport; //navigator
	// public Airline: Airline; //navigator
}
";

    internal const string InterfaceMultiFiles = @"
import {Airport} from './Airport';
import {AirportLocation} from './AirportLocation';
import {Location} from './Location';
import {City} from './City';
import {Airline} from './Airline';
import {PublicTransportation} from './PublicTransportation';
import {PlanItem} from './PlanItem';

export interface Flight extends PublicTransportation {
	FlightNumber: string; //Not null
	// From: Airport; //navigator
	// To: Airport; //navigator
	// Airline: Airline; //navigator
}
";

    internal const string ImportMultiFile = @"

";

    internal const string ClassMultiFilesUsingFullName = @"  
import {MicrosoftODataSampleServiceModelsTripPinAirport} from './MicrosoftODataSampleServiceModelsTripPinAirport';
import {MicrosoftODataSampleServiceModelsTripPinAirportLocation} from './MicrosoftODataSampleServiceModelsTripPinAirportLocation';
import {MicrosoftODataSampleServiceModelsTripPinLocation} from './MicrosoftODataSampleServiceModelsTripPinLocation';
import {MicrosoftODataSampleServiceModelsTripPinCity} from './MicrosoftODataSampleServiceModelsTripPinCity';
import {MicrosoftODataSampleServiceModelsTripPinAirline} from './MicrosoftODataSampleServiceModelsTripPinAirline';
import {MicrosoftODataSampleServiceModelsTripPinPublicTransportation} from './MicrosoftODataSampleServiceModelsTripPinPublicTransportation';
import {MicrosoftODataSampleServiceModelsTripPinPlanItem} from './MicrosoftODataSampleServiceModelsTripPinPlanItem';


export class MicrosoftODataSampleServiceModelsTripPinFlight extends MicrosoftODataSampleServiceModelsTripPinPublicTransportation {
	public FlightNumber: string; //Not null
	// public From: MicrosoftODataSampleServiceModelsTripPinAirport; //navigator
	// public To: MicrosoftODataSampleServiceModelsTripPinAirport; //navigator
	// public Airline: MicrosoftODataSampleServiceModelsTripPinAirline; //navigator
}
";

    internal const string InterfaceMultiFilesUsingFullName = @"
import {MicrosoftODataSampleServiceModelsTripPinAirport} from './MicrosoftODataSampleServiceModelsTripPinAirport';
import {MicrosoftODataSampleServiceModelsTripPinAirportLocation} from './MicrosoftODataSampleServiceModelsTripPinAirportLocation';
import {MicrosoftODataSampleServiceModelsTripPinLocation} from './MicrosoftODataSampleServiceModelsTripPinLocation';
import {MicrosoftODataSampleServiceModelsTripPinCity} from './MicrosoftODataSampleServiceModelsTripPinCity';
import {MicrosoftODataSampleServiceModelsTripPinAirline} from './MicrosoftODataSampleServiceModelsTripPinAirline';
import {MicrosoftODataSampleServiceModelsTripPinPublicTransportation} from './MicrosoftODataSampleServiceModelsTripPinPublicTransportation';
import {MicrosoftODataSampleServiceModelsTripPinPlanItem} from './MicrosoftODataSampleServiceModelsTripPinPlanItem';


export interface MicrosoftODataSampleServiceModelsTripPinFlight extends MicrosoftODataSampleServiceModelsTripPinPublicTransportation {
	FlightNumber: string; //Not null
	// From: MicrosoftODataSampleServiceModelsTripPinAirport; //navigator
	// To: MicrosoftODataSampleServiceModelsTripPinAirport; //navigator
	// Airline: MicrosoftODataSampleServiceModelsTripPinAirline; //navigator
}
";

    internal const string Enum = @"
	export enum PersonGender {
		Male=0 ,
		Female=1 ,
		Unknown=2 
	}
";

    #endregion

    #region data with navigation

    internal const string NavClass = @"  
export class Flight extends PublicTransportation {
	public FlightNumber: string; //Not null
	// public From: Airport; //navigator
	// public To: Airport; //navigator
	// public Airline: Airline; //navigator
}
";

    internal const string NavInterface = @"
export interface Flight extends PublicTransportation {
	FlightNumber: string; //Not null
	// From: Airport; //navigator
	// To: Airport; //navigator
	// Airline: Airline; //navigator
}
";

    internal const string NavClassMultiFiles = @"  
import { Airport } from './Airport';
import { Airline } from './Airline';
import { PublicTransportation } from './PublicTransportation';

export class Flight extends PublicTransportation {
	public FlightNumber: string; //Not null
	// public From: Airport; //navigator
	// public To: Airport; //navigator
	// public Airline: Airline; //navigator
}
";

    internal const string NavInterfaceMultiFiles = @"
import { Airport } from './Airport';
import { Airline } from './Airline';
import { PublicTransportation } from './PublicTransportation';

export interface Flight extends PublicTransportation {
	FlightNumber: string; //Not null
	// From: Airport; //navigator
	// To: Airport; //navigator
	// Airline: Airline; //navigator
}
";

    #endregion
}
