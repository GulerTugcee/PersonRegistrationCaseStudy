using KisiKayitMVCApp.Models.Context;
using KisiKayitMVCApp.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace KisiKayitMVCApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly KisiKayitDbContext _context;        
        public HomeController(KisiKayitDbContext context)
        {
            _context = context;            
        }
        public async Task<IActionResult> Index()
        {
            List<Kisi> kisiler = new();
            //kisiler = await _context.Kisiler.ToListAsync();            
            kisiler = _context.Kisiler.FromSqlRaw("KisileriGetir").ToList();

            return View(kisiler);
        }       

        public PartialViewResult KisiEkle()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> KisiEkle(Kisi kisi, IFormFile file)
        {
            kisi.Resim = FileSaveToServer(file, "./wwwroot/files/");

            await _context.Kisiler.AddAsync(kisi);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public PartialViewResult KisiGetir()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<JsonResult> KisiGetir(int id)
        {
            var result = await _context.Kisiler.FindAsync(id);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> KisiGuncelle(Kisi kisi, IFormFile file)
        {
            var currentKisi = await _context.Kisiler.AsNoTracking().FirstOrDefaultAsync(x => x.Id == kisi.Id);
            if (file != null)
            {
                FileDeleteToServer("./wwwroot/files/" + currentKisi.Resim);
                kisi.Resim = FileSaveToServer(file, "./wwwroot/files/");
            }
            _context.Update(kisi);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<JsonResult> KisiSil(int id)
        {
            var result = await _context.Kisiler.FindAsync(id);
            _context.Remove(result);
            await _context.SaveChangesAsync();

            FileDeleteToServer("./wwwroot/files/" + result.Resim);
            return Json(result);
        }

        public string FileSaveToServer(IFormFile file, string filePath)
        {
            var fileFormat = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            fileFormat = fileFormat.ToLower();
            string fileName = Guid.NewGuid().ToString() + fileFormat;
            string path = filePath + fileName;
            using (var stream = System.IO.File.Create(path))
            {
                file.CopyTo(stream);
            }
            return fileName;
        }

        public void FileDeleteToServer(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception)
            {
            }
        }

    }
}