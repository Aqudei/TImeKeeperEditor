using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeKeeperEditor.Models;

namespace TimeKeeperEditor.Data
{
    public class Database
    {
        private string _connectionString;
        private SqliteConnection _connection;

        public Database()
        {
            _connectionString = $"Data Source={Properties.Settings.Default.Database}";
        }

        public SqliteConnection GetOpenConnection()
        {
            if (_connection == null)
            {
                _connection = new SqliteConnection(_connectionString);

                _connection.Open();

            }
            else
            {
                if (_connection.State != System.Data.ConnectionState.Open)
                {
                    _connection.Open();
                }
            }

            return _connection;
        }

        public IEnumerable<Log> FindLogs(DateTime? logDate, string? pid)
        {
            var commandText = """
                SELECT *
                FROM attendance
                WHERE 1=1
            """;

            using var connection = GetOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;

            if (logDate.HasValue)
            {
                commandText += " AND log_date = @logDate";
                var dateParam = command.CreateParameter();
                dateParam.ParameterName = "@logDate";
                dateParam.Value = logDate.Value.ToString("yyyy-MM-dd");
                command.Parameters.Add(dateParam);
            }

            if (!string.IsNullOrWhiteSpace(pid))
            {
                commandText += " AND pid = @pid";
                var pidParam = command.CreateParameter();
                pidParam.ParameterName = "@pid";
                pidParam.Value = pid;
                command.Parameters.Add(pidParam);
            }

            command.CommandText = commandText;

            using var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    yield return new Log
                    {
                        LogRefNo = reader["log_refno"] is DBNull or null ? "" : reader["log_refno"].ToString(),
                        LogId = reader["log_id"] is DBNull or null ? "" : reader["log_id"].ToString(),
                        LogMethod = reader["log_method"] is DBNull or null ? "" : reader["log_method"].ToString(),
                        LogDate = reader["log_date"] is DBNull or null ? "" : reader["log_date"].ToString(),
                        LogInAM = reader["log_inAM"] is DBNull or null ? "" : reader["log_inAM"].ToString(),
                        LogOutAM = reader["log_outAM"] is DBNull or null ? "" : reader["log_outAM"].ToString(),
                        LogInPM = reader["log_inPM"] is DBNull or null ? "" : reader["log_inPM"].ToString(),
                        LogOutPM = reader["log_outPM"] is DBNull or null ? "" : reader["log_outPM"].ToString(),
                        LogDevice = reader["log_device"] is DBNull or null ? "" : reader["log_device"].ToString(),
                        SchedIn = reader["sched_in"] is DBNull or null ? "" : reader["sched_in"].ToString(),
                        SchedOut = reader["sched_out"] is DBNull or null ? "" : reader["sched_out"].ToString(),
                        ////TardyMin = reader["tardy_min"] is DBNull or null ? 0 : Convert.ToInt32(reader["tardy_min"]),
                        ////TardyHour = reader["tardy_hour"] is DBNull or null ? 0 : Convert.ToInt32(reader["tardy_hour"]),
                        LeaveReference = reader["leave_reference"] is DBNull or null ? "" : reader["leave_reference"].ToString(),
                        //AdminId = reader["admin_id"] is DBNull or null ? 0 : Convert.ToInt32(reader["admin_id"]),
                        ActualIN = reader["actual_IN"] is DBNull or null ? "" : reader["actual_IN"].ToString(),
                        ActualOUT = reader["actual_OUT"] is DBNull or null ? "" : reader["actual_OUT"].ToString(),
                        Remarks = reader["remarks"] is DBNull or null ? "" : reader["remarks"].ToString(),
                        DatePosted = reader["date_posted"] is DBNull or null ? DateTime.MinValue : Convert.ToDateTime(reader["date_posted"]),
                        PID = reader["pid"] is DBNull or null ? "" : reader["pid"].ToString()
                    };
                }
            }

            yield break;
        }


        public int UpdateLog(Log? log)
        {
            if (log == null)
                return 0;

            const string commandText = @"
                UPDATE attendance
                SET 
                    log_inAM = @LogInAM,
                    log_outAM = @LogOutAM,
                    log_inPM = @LogInPM,
                    log_outPM = @LogOutPM,
                    sched_in = @SchedIn,
                    sched_out = @SchedOut,
                    actual_IN = @ActualIN,
                    actual_OUT = @ActualOUT,
                    remarks = @Remarks,
                    date_posted = @DatePosted,
                WHERE log_refno = @LogRefNo
            ";

            using var connection = GetOpenConnection();
            using var command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = commandText;

            // Parameters
            command.Parameters.Add(CreateParam(command, "@LogRefNo", log.LogRefNo));
            command.Parameters.Add(CreateParam(command, "@LogInAM", log.LogInAM));
            command.Parameters.Add(CreateParam(command, "@LogOutAM", log.LogOutAM));
            command.Parameters.Add(CreateParam(command, "@LogInPM", log.LogInPM));
            command.Parameters.Add(CreateParam(command, "@LogOutPM", log.LogOutPM));
            command.Parameters.Add(CreateParam(command, "@SchedIn", log.SchedIn));
            command.Parameters.Add(CreateParam(command, "@SchedOut", log.SchedOut));
            command.Parameters.Add(CreateParam(command, "@ActualIN", log.ActualIN));
            command.Parameters.Add(CreateParam(command, "@ActualOUT", log.ActualOUT));
            command.Parameters.Add(CreateParam(command, "@Remarks", log.Remarks));
            command.Parameters.Add(CreateParam(command, "@DatePosted", log.DatePosted));

            return command.ExecuteNonQuery();
        }

        // Small helper to avoid repeating null checks
        private static System.Data.IDbDataParameter CreateParam(System.Data.IDbCommand cmd, string name, object? value)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            return param;
        }



        public IEnumerable<Employee> ListEmployees()
        {
            var commandText = """
                SELECT * FROM employee
            """;

            using var connection = GetOpenConnection();
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = System.Data.CommandType.Text;
            using var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    yield return new Employee
                    {
                        BioId = reader["bioid"] is DBNull or null ? "" : reader["bioid"].ToString(),
                        FirstName = reader["fname"] is DBNull or null ? "" : reader["fname"].ToString(),
                        MiddleName = reader["mname"] is DBNull or null ? "" : reader["mname"].ToString(),
                        LastName = reader["lname"] is DBNull or null ? "" : reader["lname"].ToString(),
                        PersonId = reader["pid"] is DBNull or null ? "" : reader["pid"].ToString(),
                        ScheduledTimeIn = reader["sched_in"] is DBNull or null ? "" : reader["sched_in"].ToString(),
                        ScheduledTimeOut = reader["sched_out"] is DBNull or null ? "" : reader["sched_out"].ToString(),
                    };
                }
            }

            yield break;
        }
    }
}
