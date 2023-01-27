using System.Threading.Tasks;

namespace YellowDuck.Api.Services.Mailer
{
    public interface IMailer
    {
        Task Send<T>(T model) where T : EmailModel;
    }
}