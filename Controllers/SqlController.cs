using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SqlRestWinService.Controllers
	{
	[ApiController]
	[Route("api/[controller]")]
	public class SqlController : ControllerBase
		{
		private readonly IConfiguration _configuration;
		private readonly ILogger<SqlController> _logger;

		public SqlController(IConfiguration configuration, ILogger<SqlController> logger)
			{
			_configuration = configuration;
			_logger = logger;
			}

		/// <summary>
		/// Executes a provided SQL query against the configured database.
		/// </summary>
		/// <param name="sqlRequest">The SQL query request containing the query string.</param>
		/// <returns>The result of the SQL query execution.</returns>
		[HttpPost("execute-sql")]
		[Produces("application/json")]
		public async Task<IActionResult> ExecuteSqlAsync([FromBody] string sqlRequest)
			{
			if (string.IsNullOrWhiteSpace(sqlRequest))
				{
				return BadRequest("SQL query is missing or empty.");
				}

			try
				{
				var dataSet = await ExecuteQueryAsync(sqlRequest);

				// Convert DataSet to JSON-friendly format (list of tables with rows)
				var resultList = new List<object>();

				foreach (DataTable table in dataSet.Tables)
					{
					var tableData = new List<Dictionary<string, object>>();

					foreach (DataRow row in table.Rows)
						{
						var rowData = new Dictionary<string, object>();

						foreach (DataColumn column in table.Columns)
							{
							rowData[column.ColumnName] = row[column] ?? DBNull.Value;
							}

						tableData.Add(rowData);
						}

					resultList.Add(new { TableName = table.TableName, Rows = tableData });
					}

				return Ok(resultList);
				}
			catch (Exception ex)
				{
				_logger.LogError(ex, "Error executing SQL");
				return StatusCode(500, new { Error = ex.Message });
				}
			}



		private async Task<DataSet> ExecuteQueryAsync(string sqlQuery)
			{
			var connectionString = _configuration.GetConnectionString("mssql");
			_logger.LogInformation($"Executing SQL: {sqlQuery}");

			var dataSet = new DataSet();

			try
				{
				using var connection = new SqlConnection(connectionString);
				await connection.OpenAsync();

				using var command = new SqlCommand(sqlQuery, connection);
				using var adapter = new SqlDataAdapter(command);

				// Fill the DataSet with results
				adapter.Fill(dataSet);

				return dataSet;
				}
			catch (Exception ex)
				{
				_logger.LogError(ex, "Error executing SQL");

				// Optionally, you can throw the exception or handle it as per your requirement.
				throw;
				}
			}
		}


		}
