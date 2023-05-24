using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using books.Models;
using books.Models.Entities;
using books.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using books.Models.AdminViewModels;
using System.Security.Claims;

namespace books.Controllers.Admin;

[Authorize]
public class AdminController : Controller
{
    private readonly KitapDbContext db = new KitapDbContext(); // dependency injection nesnesi
    public AdminController(KitapDbContext _db) // Dep'i parametre olarak ekledik.
    {
        db = _db; // dependency injection yaptık. 
    }

    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(UserVM postedData)
    {
        if (!ModelState.IsValid)
        {
            return View(postedData);
        }

        var user = (from x in db.Users
                    where x.Username == postedData.username && x.Password == postedData.password
                    select x
                    ).FirstOrDefault();

        if (user != null)
        {
            var claims = new List<Claim>{
                new Claim("user",user.Id.ToString()),
                new Claim("role","admin")
            };

            var claimsIdendity = new ClaimsIdentity(claims, "Cookies", "user", "role");
            var claimsPrinciple = new ClaimsPrincipal(claimsIdendity);
            await HttpContext.SignInAsync(claimsPrinciple);

            return Redirect("/Admin/Index");
        }
        else
        {
            TempData["NotFound"] = "Böyle bir kullanıcı bulunamadı!";
        }
        return View();
    }

    [Route("/Admin/Logout")]
    public async Task<IActionResult> Signout()
    {
        await HttpContext.SignOutAsync();
        return Redirect("/Admin");
    }

    [Route("/Admin/Turler")]
    public IActionResult Turler()
    {
        List<TurlerVm> TurListesi = (from x in db.Turlers
                                     select new TurlerVm
                                     {
                                         Id = x.Id,
                                         Sira = x.Sira,
                                         TurAdi = x.TurAdi
                                     }).ToList();

        return View(TurListesi);
    }

    [HttpGet]
    public IActionResult TurForm(int? id)
    {
        if (id != null)
        {
            TurlerVm duzenlenecekTur = (from x in db.Turlers
                                        where x.Id == id
                                        select new TurlerVm
                                        {
                                            Id = x.Id,
                                            Sira = x.Sira,
                                            TurAdi = x.TurAdi
                                        }).FirstOrDefault();

            ViewBag.PageTitle = "Tür Düzenle";
            return View(duzenlenecekTur);
        }
        else if (id == null)
        {
            ViewBag.PageTitle = "Tür Ekle";
            return View();
        }
        else
        {
            return View();
        }

    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TurForm(TurlerVm gelenData)
    {
        if (!ModelState.IsValid)
        {
            return View(gelenData);
        }

        if (gelenData.Id != 0)
        {
            Turler duzenelenecekTur = db.Turlers.Find(gelenData.Id);
            if (duzenelenecekTur != null)
            {
                duzenelenecekTur.Sira = gelenData.Sira;
                duzenelenecekTur.TurAdi = gelenData.TurAdi;
            }
        }
        else if (gelenData.Id == 0)
        {
            Turler yeniTur = new Turler
            {
                TurAdi = gelenData.TurAdi,
                Sira = gelenData.Sira
            };
            await db.AddAsync(yeniTur);
        }
        await db.SaveChangesAsync();

        return Redirect("/Admin/Turler");
    }


    public async Task<IActionResult> TurSil(int id)
    {
        Turler silinecekTur = db.Turlers.Find(id);
        if (silinecekTur != null)
        {
            db.Turlers.Remove(silinecekTur);
            await db.SaveChangesAsync();
        }

        return Redirect("/Admin/Turler");
    }
}