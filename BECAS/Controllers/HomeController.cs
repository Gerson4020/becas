using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BECAS.Filters;
using Microsoft.AspNetCore.Authorization;
using BECAS.Models.VM;
using BECASLC;
using System.Globalization;

namespace BECAS.Controllers
{

    //[ResponseHeader]
    public class HomeController : Controller
    {
        // GET: HomeController
        private readonly MEOBContext _ctx;
        public HomeController(MEOBContext ctx)
        {
            _ctx = ctx;
        }
        public ActionResult Index()
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userDB = _ctx.Usuarios.FirstOrDefault(x => x.Nombre.Equals(User.Identity.Name));
                    if (userDB != null)
                    {
                        HttpContext.Session.SetString("SessionKeyName", userDB.Nombre);
                        HttpContext.Session.SetInt32("SessionRol", (int)userDB.IdRol);
                        HttpContext.Session.SetInt32("SessionId", (int)userDB.IdUsuario);
                    }
                    else
                    {
                        return RedirectToAction("error","Home");
                        
                    }
                }
                
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public ActionResult Error()
        {
            ViewBag.Message = "No tienes acceso a la aplicación";
            return View();
        }
    }
}
