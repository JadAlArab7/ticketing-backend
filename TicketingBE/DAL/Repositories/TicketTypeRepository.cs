using System.Data;
using TicketingBE.DAL.Helpers;
using TicketingBE.DAL.Interfaces;
using TicketingBE.Models;

namespace TicketingBE.DAL.Repositories
{
    /// <summary>
    /// Implementation of ITicketTypeRepository using ADO.NET for database operations
    /// This class is responsible for all database interactions related to TicketType entity
    /// </summary>
    public class TicketTypeRepository : ITicketTypeRepository
    {
        private readonly SqlHelper _sqlHelper;

        public TicketTypeRepository(SqlHelper sqlHelper)
        {
            _sqlHelper = sqlHelper ?? throw new ArgumentNullException(nameof(sqlHelper));
        }

        /// <summary>
        /// Retrieves all ticket types from the database
        /// </summary>
        public async Task<IEnumerable<TicketType>> GetAllTicketTypesAsync()
        {
            string query = @"
                SELECT 
                    id, 
                    name
                FROM tck.ticket_types
                ORDER BY name";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<List<TicketType>>(query, (dataReader) =>
                {
                    var ticketTypes = new List<TicketType>();
                    while (dataReader.Read())
                    {
                        ticketTypes.Add(new TicketType
                        {
                            Id = TypeHelper.GetGuidAsString(dataReader["id"]),
                            Name = TypeHelper.GetString(dataReader["name"])
                        });
                    }
                    return ticketTypes;
                });
            });
        }

        /// <summary>
        /// Retrieves a specific ticket type by its ID
        /// </summary>
        public async Task<TicketType?> GetTicketTypeByIdAsync(string id)
        {
            string query = @"
                SELECT 
                    id, 
                    name
                FROM tck.ticket_types
                WHERE id = @id";

            return await Task.Run(() =>
            {
                return _sqlHelper.ExecuteReader<TicketType?>(query, (dataReader) =>
                {
                    TicketType? ticketType = null;
                    if (dataReader.Read())
                    {
                        ticketType = new TicketType
                        {
                            Id = TypeHelper.GetGuidAsString(dataReader["id"]),
                            Name = TypeHelper.GetString(dataReader["name"])
                        };
                    }
                    return ticketType;
                }, _sqlHelper.CreateParam("@id", Guid.Parse(id)));
            });
        }
    }
}
