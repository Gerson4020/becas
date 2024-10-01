using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BECAS.Filters;
using Microsoft.AspNetCore.Authorization;
using BECAS.Models.VM;
using BECASLC;
using System.Globalization;
using System.Data.SqlClient;
using BECAS.Interfaces;

namespace BECAS.Controllers
{

    //[ResponseHeader]
    [Authorize(Policy = "ADOnly")]
    public class HomeController : Controller
    {
        // GET: HomeController
        private readonly MEOBContext _ctx;
        private readonly IEncryptionService _encryptionService;
        public HomeController(MEOBContext ctx, IEncryptionService encryptionService)
        {
            _ctx = ctx;
            _encryptionService = encryptionService;
        }

        public ActionResult Index()
        {
            // Intenta obtener el email desde las claims.
            var email = User.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value
                        ?? User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            if (!string.IsNullOrEmpty(email))
            {
                email = _encryptionService.Encrypt(email);
                // Busca al usuario en la base de datos utilizando el email obtenido.
                var user = _ctx.Usuarios.FirstOrDefault(x => x.Nombre == email);

                if (user != null) // Asegúrate de que el usuario fue encontrado.
                {
                    HttpContext.Session.SetString("SessionKeyName", email);
                    HttpContext.Session.SetInt32("SessionRol", (int)user.IdRol);
                    HttpContext.Session.SetInt32("SessionId", user.IdUsuario);
                    ViewBag.rol = user.IdRol;
                }
                else
                {
                    // Maneja el caso en que no se encuentre el usuario en la base de datos.
                    ViewBag.Message = "Usuario no encontrado en la base de datos.";
                }
            }
            else
            {
                // Maneja el caso en que no se pueda obtener un email de las claims.
                ViewBag.Message = "No se pudo obtener el correo electrónico del usuario.";
            }

            ViewBag.Email = email; // Pasar el email a la vista si es necesario

            return View();
        }

        public ActionResult Error()
        {
            ViewBag.Message = "No tienes acceso a la aplicación";
            return View();
        }

        #region Encriptar
        public async Task<IActionResult> Encriptar(string id)
        {
            try
            {
                var encriptado = _encryptionService.Encrypt(id);

                var result = new
                {
                    Texto_Original = id,
                    Texto_Encriptado = encriptado
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Desencriptar(string id)
        {
            try
            {
                var original = _encryptionService.Decrypt(id);

                var result = new
                {
                    Texto_Encriptado = id,
                    Texto_Desencriptado = original
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        #endregion
    }
}
