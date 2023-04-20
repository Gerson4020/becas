using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BECAS.Filters;
using Microsoft.AspNetCore.Authorization;
using BECAS.Models.VM;
using BECASLC;
using System.Globalization;

namespace BECAS.Controllers
{

    [ResponseHeader]
    public class HomeController : Controller
    {
        // GET: HomeController
        //private readonly MEOBContext _ctx;
        public HomeController(MEOBContext ctx)
        {
            //_ctx = ctx;
        }
        public ActionResult Index()
        {
            try
            {
                DateTime fecha = DateTime.Now;
                string mes = fecha.AddMonths(-3).ToString("MMMM", CultureInfo.CreateSpecificCulture("es-ES"));
                HomeVM vM = new HomeVM();
                //vM.CountUsuario = _ctx.Personas.Count().ToString();
                //vM.CountEstipendios = _ctx.CargaEstipendios.Count().ToString();
                //vM.CountEducacion = _ctx.CargaEducacions.Where(x => x.DEstado.Equals("Activo") && x.RMes.Contains(mes)).Count().ToString();
                //vM.CountEvPsicosocial = _ctx.CargaEvaluacionPsicosocials.Count().ToString();
                //vM.CountSegPsicosocial = _ctx.CargaSeguimientoPsicosocials.Count().ToString();
                //vM.CountPasantillas = _ctx.CargaSeguimientoPasantias.Count().ToString();
                //vM.CountAuto = _ctx.CargaSeguimientoAutoempleos.Count().ToString();
                //vM.CountPracticas = _ctx.CargaSeguimientoPracticasPrs.Count().ToString();
                return View(vM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HomeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HomeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HomeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
