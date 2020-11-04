using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPMApi.Services
{
    public interface IEmailService
    {
        void Send(string subject, string message, string email);
    }
}
