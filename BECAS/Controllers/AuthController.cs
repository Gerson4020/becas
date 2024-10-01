using BECASLC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using BECAS.Models;
using Microsoft.Extensions.Options;
using System.Text;
using BECAS.Interfaces;

namespace BECAS.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly MEOBContext _ctx;
        private readonly IEncryptionService _encryptionService;

        public AuthController(MEOBContext ctx, IEncryptionService encryptionService)
        {
            _ctx = ctx;
            _encryptionService = encryptionService;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            //if (User.Identity.IsAuthenticated)
            //{
            //    // Si el usuario ya está autenticado, redirigirlo a su dashboard o página principal
            //    return RedirectToAction("SearchBeneficiary", "Partners");
            //}

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Message = "El email y la contraseña son requeridos.";
                return View();
            }

            email = _encryptionService.Encrypt(email);
            var user = _ctx.Usuarios.FirstOrDefault(x => x.Nombre == email);

            password = _encryptionService.Encrypt(password);
            if (user != null && user.Password == password)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim("UserId", user.IdUsuario.ToString()),
            new Claim(ClaimTypes.Role, user.IdRol.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, "ApplicationScheme");

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };

                await HttpContext.SignInAsync("ApplicationScheme", new ClaimsPrincipal(claimsIdentity), authProperties);

                HttpContext.Session.SetString("SessionKeyName", email);
                HttpContext.Session.SetInt32("SessionRol", (int)user.IdRol);
                HttpContext.Session.SetInt32("SessionId", user.IdUsuario);

                return RedirectToAction("SearchBeneficiary", "Partners");
            }

            ViewBag.Message = "Nombre o contraseña incorrectos.";
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Limpiar la cookie de autenticación y la sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult GenerateKeyIv()
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.GenerateKey();
                    aesAlg.GenerateIV();

                    var key = Convert.ToBase64String(aesAlg.Key);
                    var iv = Convert.ToBase64String(aesAlg.IV);

                    var result = new
                    {
                        Key = key,
                        IV = iv
                    };

                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpGet]
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


        //public string Encrypt(string plainText)
        //{
        //    var key = Convert.FromBase64String(_encryptionSettings.EncryptionKey);
        //    var iv = Convert.FromBase64String(_encryptionSettings.EncryptionIV);

        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = key;
        //        aesAlg.IV = iv;

        //        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        //        using (MemoryStream msEncrypt = new MemoryStream())
        //        {
        //            using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
        //            {
        //                swEncrypt.Write(plainText);
        //            }

        //            return Convert.ToBase64String(msEncrypt.ToArray());
        //        }
        //    }
        //}

        //public string Decrypt(string cipherText)
        //{
        //    var key = Convert.FromBase64String(_encryptionSettings.EncryptionKey);
        //    var iv = Convert.FromBase64String(_encryptionSettings.EncryptionIV);

        //    using (Aes aesAlg = Aes.Create())
        //    {
        //        aesAlg.Key = key;
        //        aesAlg.IV = iv;

        //        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        //        using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
        //        {
        //            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
        //            {
        //                return srDecrypt.ReadToEnd();
        //            }
        //        }
        //    }
        //}
    }
}
