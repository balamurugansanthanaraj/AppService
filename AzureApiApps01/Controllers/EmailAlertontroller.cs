using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureApiApps01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailAlertontroller : ControllerBase
    {
        private readonly EmailAlertDbContext _emailAlertDbContext;

        public EmailAlertontroller(EmailAlertDbContext emailAlertDbContext)
        {
            _emailAlertDbContext = emailAlertDbContext;
        }

        [HttpPost]
        public async Task<ActionResult<EmailAlert>> Create(EmailAlert emailAlert)
        {
            _emailAlertDbContext.EmailAlerts.Add(emailAlert);
            await _emailAlertDbContext.SaveChangesAsync();

            return CreatedAtAction("GetAlert", new { id = emailAlert.Id }, emailAlert);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmailAlert>> GetAlert(int id)
        {
            var movie = await _emailAlertDbContext.EmailAlerts.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            return movie;
        }
    }
}
