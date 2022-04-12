using Microsoft.EntityFrameworkCore.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Data
{
    public class OracleCommandInterceptor : DbCommandInterceptor
    {

        public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
        {
            if (command != null && !string.IsNullOrWhiteSpace(command.CommandText))
                command.CommandText = command.CommandText.Replace("N''", "''");
            return result;
        }

        public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (command != null && !string.IsNullOrWhiteSpace(command.CommandText))
                command.CommandText = command.CommandText.Replace("N''", "''");
            return new ValueTask<InterceptionResult<int>>(result);
        }

        public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            if (command != null && !string.IsNullOrWhiteSpace(command.CommandText))
                command.CommandText = command.CommandText.Replace("N''", "''");
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
        {
            if (command != null && !string.IsNullOrWhiteSpace(command.CommandText))
                command.CommandText = command.CommandText.Replace("N''", "''");
            return new ValueTask<InterceptionResult<DbDataReader>>(result);
        }

        public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
        {
            if (command != null && !string.IsNullOrWhiteSpace(command.CommandText))
                command.CommandText = command.CommandText.Replace("N''", "''");
            return result;
        }

        public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
        {
            if (command != null && !string.IsNullOrWhiteSpace(command.CommandText))
                command.CommandText = command.CommandText.Replace("N''", "''");
            return new ValueTask<InterceptionResult<object>>(result);
        }
    }
}

