using Microsoft.AspNetCore.Mvc;
using Sem_Fashion_Kids_Clothing_Website.Models;

namespace Sem_Fashion_Kids_Clothing_Website.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }
    }
}