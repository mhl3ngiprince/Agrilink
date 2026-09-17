using System.IO.Pipelines;

namespace FarmToTable.Models
{

        public class SeedResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public SeedResult Details { get; set; }
            public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        }

    }

