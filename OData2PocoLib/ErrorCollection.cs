// Copyright (c) Mohamed Hassan & Contributors. All rights reserved. See License.md in the project root for license information.

using System.Collections;
using OData2Poco.InfraStructure.Logging;


namespace OData2Poco;

public class ErrorCollection : IEnumerable<OptionError>
{
    private const int Info = 0;
    private const int Warning = 1;
    private const int Error = 2;

    private readonly List<OptionError> _errors = new();
    private readonly ILog _logger = PocoLogger.Default;
    public void Add(OptionError error)
    {
        _errors.Add(error);
    }
    public void Add(string message, int level = 0)
    {
        var error = new OptionError(message, level);
        Add(error);
    }
    public void AddError(string message) => Add(message, Error);
    public void AddWarning(string message) => Add(message, Warning);
    public void AddInfo(string message) => Add(message);

    public int ShowErrors()
    {
        if (!_errors.Any()) return 0;
        foreach (var error in _errors.OrderBy(e => e.Level))
        {
            switch (error.Level)
            {
                case 2:
                    _logger.Error(error.Message);
                    break;
                case 0:
                    _logger.Info(error.Message);
                    break;
                case 1:
                    _logger.Warn(error.Message);
                    break;
                default:
                    _logger.Info(error.Message);
                    break;
            }
        }
        return _errors.Any(e => e.Level >= 2) ? 1 : 0;
    }
    public IEnumerator<OptionError> GetEnumerator()
    {
        return _errors.GetEnumerator();

    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}