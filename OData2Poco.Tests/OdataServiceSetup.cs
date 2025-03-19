// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

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
            throw new Exception("Failed to start OData service");
        }
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _odataService.Dispose();
    }
}
