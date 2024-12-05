using CyberBezp.Areas.Identity.Data;
using CyberBezp.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberBezp.Services
{
    [Table("Logs")]
    public class SystemLog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateTime { get; set; }
    }

    public interface ISystemLogger
    {
        void Log(string name, string description);
        public List<SystemLog> GetLogs();
    }

    public class SystemLogger : ISystemLogger
    {
        private readonly ApplicationDbContext _context;

        public SystemLogger(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Log(string name, string description)
        {
            var logs = _context.Set<SystemLog>();
            logs.Add(new SystemLog
            {
                Name = name,
                Description = description,
                DateTime = DateTime.Now
            });

            _context.SaveChanges();
        }

        public List<SystemLog> GetLogs()
        {
            return _context.Set<SystemLog>()
                .OrderByDescending(x => x.DateTime)
                .ThenByDescending(x => x.Id)
                .ToList();
        }

    }
}