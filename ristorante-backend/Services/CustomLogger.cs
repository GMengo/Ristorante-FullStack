using System.Net;
using System.Security.Claims;

namespace ristorante_backend.Services
{
    public interface ICustomLogger
    {
        public void WriteRequest(HttpContext httpContext, string utente);
        public void WriteResponse(HttpContext httpContext, string utente, int duration);
    }


    public class CustomConsoleLogger : ICustomLogger
    {
        public void WriteRequest(HttpContext httpContext, string utente)
        {
            string method = httpContext.Request.Method;
            string path = httpContext.Request.Path;
            Console.WriteLine($"{DateTime.Now.ToString("G")} Request arrivata: [utente: {utente}, method: {method}, path: {path}]");
        }
        public void WriteResponse(HttpContext httpContext, string utente, int duration)
        {
            string method = httpContext.Request.Method;
            string path = httpContext.Request.Path;
            int statusCode = httpContext.Response.StatusCode;
            Console.WriteLine($"{DateTime.Now.ToString("G")} Response in uscita: [status code: {statusCode} {(HttpStatusCode)statusCode}, utente: {utente}, method: {method}, path: {path}, durata: {duration}ms]");
        }

    }


    public class CustomFileLogger : ICustomLogger
    {
        public void WriteRequest(HttpContext httpContext, string utente)
        {
            string method = httpContext.Request.Method;
            string path = httpContext.Request.Path;
            File.AppendAllText("./log.txt", $"{DateTime.Now.ToString("G")} Request arrivata: [utente: {utente}, method: {method}, path: {path}]");
        }
        public void WriteResponse(HttpContext httpContext, string utente, int duration)
        {
            string method = httpContext.Request.Method;
            string path = httpContext.Request.Path;
            int statusCode = httpContext.Response.StatusCode;
            File.AppendAllText("./log.txt", $"{DateTime.Now.ToString("G")} Response in uscita: [status code: {statusCode} {(HttpStatusCode)statusCode}, utente: {utente}, method: {method}, path: {path}, durata: {duration}ms]");
        }

    }
}
