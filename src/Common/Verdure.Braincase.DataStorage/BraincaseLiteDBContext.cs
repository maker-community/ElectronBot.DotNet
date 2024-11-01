using System.Globalization;
using LiteDB;
using Verdure.Braincase.DataStorage.Collections;

namespace Verdure.Braincase.DataStorage;

public class BraincaseLiteDBContext : IDisposable
{
    private readonly LiteDatabase _liteDBClient;
    private readonly string _collectionPrefix;

    private const string DB_NAME_INDEX = "authSource";

    public BraincaseLiteDBContext(BraincaseDatabaseSettings dbSettings)
    {
        var mapper = new BsonMapper();
        mapper.RegisterType(
            value => value.ToString("o", CultureInfo.InvariantCulture),
            bson => DateTime.ParseExact
            (bson, "o", CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind));

        mapper.RegisterType(
            value => value.ToString("o", CultureInfo.InvariantCulture),
            bson => DateTimeOffset.ParseExact
            (bson, "o", CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind));

        var dbConnectionString = dbSettings.BraincaseLiteDB;
        var connectionString = new ConnectionString(dbConnectionString)
        {
            Connection = ConnectionType.Shared
        };
        _liteDBClient = new LiteDatabase(connectionString, mapper);

        _collectionPrefix = dbSettings.TablePrefix ?? "Braincase";
    }

    private LiteDatabase Database => _liteDBClient;

    public void Dispose()
    {
        _liteDBClient?.Dispose();
    }

    public ILiteCollection<LingxiSpaceDocument> LingxiSpaces
        => Database.GetCollection<LingxiSpaceDocument>($"{_collectionPrefix}_LingxiSpaces");

    public ILiteCollection<LocalSettingDocument> LocalSettings
        => Database.GetCollection<LocalSettingDocument>($"{_collectionPrefix}_LocalSettings");

    public ILiteCollection<EmojisDocument> Emojis
        => Database.GetCollection<EmojisDocument>($"{_collectionPrefix}_Emojis");

    public ILiteStorage<string> FileStorage => _liteDBClient.FileStorage;
}
