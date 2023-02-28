using BECASLC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BECAS.Controllers
{
    public class LoginController : Controller
    {
        private readonly MEOBContext _ctx;
        public LoginController(MEOBContext ctx)
        {
            _ctx = ctx;
        }
        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string email, string password)
        {
            bool estado = false;
            if (email != null && password != null)
            {
                var user = _ctx.Usuarios.FirstOrDefault(x => x.Nombre.Equals(email));
                if (user != null)
                {
                    if (user.Password.Equals(password))
                    {
                        estado = true;
                        HttpContext.Session.SetString("SessionKeyName", email);
                        HttpContext.Session.SetInt32("SessionRol", (int)user.IdRol);
                        HttpContext.Session.SetInt32("SessionId", (int)user.IdUsuario);
                    }
                }
            }
            if (estado)
            {
                return RedirectToAction("index", "Home");
            }
            else
            {
                ViewBag.Message += string.Format("<b> Nombre o contraseña incorrectos<br />");
                return RedirectToAction("index", "Login");
            }
        }

        [HttpGet]
        public ActionResult SingOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}
