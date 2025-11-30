using Microsoft.AspNetCore.Mvc;

namespace ERP360.Pedidos.Api.Controllers
{
    public class PedidosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
