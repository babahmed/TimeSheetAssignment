using HimamaTimesheet.Application.DTOs.Mail;
using System.Threading.Tasks;

namespace HimamaTimesheet.Application.Interfaces.Shared
{
    public interface IMailService
    {
        Task SendAsync(MailRequest request);
    }
}