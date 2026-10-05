using Npgsql;
using NpgsqlTypes;

namespace ClinicManagement.Infrastructure.Data;

internal static class PostgresCommandHelper
{
    /// <summary>
    /// PostgreSQL function overloads require <c>date</c>, not <c>timestamp</c>.
    /// </summary>
    public static NpgsqlParameter DateParameter(string name, DateTime value) =>
        new(name, NpgsqlDbType.Date) { Value = value.Date };
}
