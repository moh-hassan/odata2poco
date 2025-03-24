// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using OData2Poco;

[SetUpFixture]
public class OdataServiceSetup
{
    private OdataService _odataService;

    [OneTimeSetUp]
    public void SetUp()
    {
        _odataService = OdataService.Instance;
        if (!OdataService.IsStarted)
        {
            throw new OData2PocoException("Failed to start OData service");
        }
        TestContext.Out.WriteLine($"Starting OData service in: {_odataService.MockServer.Urls[0]}");
        TestContext.Out.WriteLine($"Finding  {_odataService.MockServer.MappingModels.Count} stubs. Trippin: {OdataService.Trippin}");
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _odataService.Dispose();
    }
}
