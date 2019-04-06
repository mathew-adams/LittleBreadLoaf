using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class SourceToTag
    {

        [Key]
        public Guid SourceTagID { get; set; }

        public Guid TagID { get; set; }

        [StringLength(30)]
        public string SourceArea { get; set; }

        public Guid SourceID { get; set; }

        [DataType(DataType.Url)]
        [StringLength(512)]
        public string SourceURL { get; set; }
    }
}
