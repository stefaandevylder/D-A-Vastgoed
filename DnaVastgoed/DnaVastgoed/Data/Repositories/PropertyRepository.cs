using DnaVastgoed.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DnaVastgoed.Data {

    public class PropertyRepository {

        private readonly ApplicationDbContext _context;
        private readonly DbSet<DnaProperty> _properties;

        public PropertyRepository(ApplicationDbContext context) {
            _context = context;
            _properties = context.Properties;
        }

        /// <summary>
        /// Create the database if needed.
        /// </summary>
        public async Task CreateDatabase() {
            await _context.Database.EnsureCreatedAsync();
        }

        /// <summary>
        /// Get a property by ID.
        /// </summary>
        /// <param name="propertyId">The property ID</param>
        /// <returns>The found property object or null</returns>
        public DnaProperty Get(string propertyId) {
            return _properties.FirstOrDefault(p => p.Id == propertyId);
        }

        /// <summary>
        /// Add a new property to the database.
        /// </summary>
        /// <param name="property">The property to add</param>
        public void Add(DnaProperty property) {
            _properties.Add(property);
        }

        /// <summary>
        /// Remove a property from the database.
        /// </summary>
        /// <param name="property">The property we want to remove</param>
        public void Remove(DnaProperty property) {
            _properties.Remove(property);
        }

        /// <summary>
        /// Save all changes.
        /// </summary>
        public void SaveChanges() {
            _context.SaveChanges();
        }

    }

}
