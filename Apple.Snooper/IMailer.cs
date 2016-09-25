using System.Threading.Tasks;

namespace Apple.Snooper
{
    public interface IMailer
    {
        Task<bool> Notify(EmailConfig emailConfig, string model, string store);
    }
}