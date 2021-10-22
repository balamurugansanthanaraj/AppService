using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blobStorage
{
    public class EmailAlert
    {
        public int Id { get; set; }
        public string ToAddress { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }

        public bool EmailSent { get; set; }
    }
}
