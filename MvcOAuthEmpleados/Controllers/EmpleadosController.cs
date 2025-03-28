using Microsoft.AspNetCore.Mvc;
using MvcOAuthEmpleados.Filters;
using MvcOAuthEmpleados.Models;
using MvcOAuthEmpleados.Services;
using NuGet.Common;

namespace MvcOAuthEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;
        public EmpleadosController(ServiceEmpleados service)
        {
            this.service = service;
        }
        [AuthorizeEmpleados]
        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.service.GetEmpleadosAsync();
            return View(empleados);
        }
        public async Task<IActionResult> Details
            (int id)
        {
            
            Empleado empleado = await
                    this.service.FindEmpleadoAsync(id);
            return View(empleado);
        }
        [AuthorizeEmpleados]
        public async Task<ActionResult> Perfil()
        {
            Empleado empleado = await
                this.service.GetPerfilAsync();
            return View(empleado);
        }
        [AuthorizeEmpleados]
        public async Task<ActionResult> Compis()
        {
            List<Empleado> empleados = await
                this.service.GetCompisAsync();
            return View(empleados);
        }
    }
}
