using ClearBank.DeveloperTest.Data;
using System;
using System.Configuration;

namespace ClearBank.DeveloperTest.Services;

public interface IDataStoreProvider
{
    public IAccountDataStore Get();
}

public class DataStoreProvider : IDataStoreProvider
{
    private readonly AccountDataStore _primary;
    private readonly BackupAccountDataStore _backup;

    public DataStoreProvider(AccountDataStore primary, BackupAccountDataStore backup)
    {
        _primary = primary;
        _backup = backup;
    }

    public IAccountDataStore Get()
    {
        var type = ConfigurationManager.AppSettings["DataStoreType"];
        return string.Equals(type, "Backup", StringComparison.OrdinalIgnoreCase) ? _backup : _primary;
    }
}   