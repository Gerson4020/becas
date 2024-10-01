using BECAS.Interfaces;
using BECAS.Models;
using BECAS.Models.VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BECAS.Controllers
{
    [Authorize(Policy = "ExternalUsers")]
    //[AllowAnonymous]
    public class PartnersController : Controller
    {
        private readonly IPersonas _persona;
        public PartnersController(IPersonas personas)
        {
            _persona = personas;
        }
        // GET: PartnersController
        public ActionResult SearchBeneficiary()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SearchBeneficiary(DTSearchBeneficiary param)
        {
            // Inicializa la lista de personas
            List<SearchBeneficiaryTable> ListPersonas = new List<SearchBeneficiaryTable>();

            // Verifica si se proporcionaron parámetros de búsqueda
            
            
            if (param.dui != null || param.nombre != null || param.aperllido != null || param.telefono1 != null || param.telefono2 != null || param.correo !=null)
            {
                // Obtiene la lista de personas basada en los parámetros proporcionados
                ListPersonas = _persona.GetSearchBeneficiaries(param);

                // Obtiene el valor de búsqueda
                string searchValue = param.Search?.Value;

                // Aplica filtro de búsqueda si se proporciona un término de búsqueda
                if (!string.IsNullOrWhiteSpace(searchValue))
                {
                    ListPersonas = ListPersonas.Where(item =>
                        item.NombreCompleto.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                        item.Nombre.Contains(searchValue, StringComparison.OrdinalIgnoreCase)
                    // Agrega más condiciones de filtrado si es necesario
                    ).ToList();
                }
            }

            // Realiza una consulta para contar el número total de registros
            var totalRegistros = param.count != null ? (int)param.count : 0;

            // Devolver los datos en formato JSON con información adicional necesaria para DataTables
            return Json(new DTResult<SearchBeneficiaryTable>
            {
                draw = param.Draw,
                recordsTotal = totalRegistros, // Total de registros sin paginar
                recordsFiltered = ListPersonas.Count, // Total de registros después de aplicar filtros
                data = ListPersonas // Registros para la página actual
            });
        }

        //[HttpPost]
        //public ActionResult ChangePassword()
        //{
        //    try
        //    {

        //    }
        //    catch (Exception e)
        //    {

        //        throw;
        //    }
        //}

    }
}
