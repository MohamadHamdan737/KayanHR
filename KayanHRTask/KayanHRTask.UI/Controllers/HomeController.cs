using KayanHRTask.BL.IRepositories;
using KayanHRTask.BL.Models;
using KayanHRTask.BL.Models.ViewModels;
using KayanHRTask.EF.Data;
using KayanHRTask.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace KayanHRTask.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IBaseRepository<Users> _UserBaseRepository;
        public AppDbContext _db;
        public IHostingEnvironment _hosting;
        public HomeController(ILogger<HomeController> logger ,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IBaseRepository<Users> UserBaseRepository,
            AppDbContext db,
            IHostingEnvironment hosting
            )
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _UserBaseRepository = UserBaseRepository;
             _db=db;
            _hosting = hosting;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid user email or password");
                    return View(model);
                }
                var resoult = await _signInManager.PasswordSignInAsync(user.UserName!, model.Password!, model.RememberMe, false);
                if (resoult.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminPage","Home");
                    }
                    else if (roles.Contains("User"))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid user email or password");
                    return View(model);
                }

            }
            return View(model);
        }
        public IActionResult UserRegister()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UserRegister(RegistorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fileName=string.Empty;
                if (model.Image!=null)
                {
                    if (model.Image.Length > 2 * 1024 * 1024) 
                    {
                        ModelState.AddModelError(string.Empty,"The uploaded file size exceeds the 2 MB limit.");
                        return View(model);
                    }
                    var uploads = Path.Combine(_hosting.WebRootPath, "images");
                    fileName = model.Image.FileName;
                    var fullPath=Path.Combine(uploads, fileName);
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        model.Image.CopyTo(fileStream);
                    }
                }
                var userName = model.FirstName + model.LastName;
                userName = userName?.Replace(" ", "");
                var existingemail = await _userManager.FindByEmailAsync(model.Email);
                if (existingemail!=null)
                {
                    ModelState.AddModelError(string.Empty, "Email already exists.");
                    return View(model);
                }
                IdentityUser user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = userName,
                    
                };
                var existingUser = await _userManager.FindByNameAsync(user.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Username already exists.");
                    return View(model);
                }
                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {
                   
                    Users users = new Users
                    {
                        UserName=userName, Email=model.Email,Password=model.Password,ImageName=fileName
                    };
                   _db.Add(users);
                    _db.SaveChanges();
                }
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                        return View(model);
                    }
                }
                await _userManager.AddToRoleAsync(user, "User");
                return RedirectToAction(nameof(Login));
            }
            return View(model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AdminPage()
        {
            var AllUsers =_db.Users.ToList();
            return View(AllUsers);
        }
        [HttpGet]

        public async Task<IActionResult> Edit(int id)
        {
            var user =await _UserBaseRepository.GetByIdAsync(id);
            if (user == null)
            {
                return View();
            }
            return View(user);
        }  
        [HttpPost]

        public async Task<IActionResult> Edit(int id,Users users)
        {
            var userId =await _UserBaseRepository.GetByIdAsync(id);
            var user = await _userManager.FindByEmailAsync(userId.Email!);
            if (userId == null)
            {
                return View();
            }
            else
            {
               
                userId.UserName = users.UserName;
                userId.Email = users.Email;
                _UserBaseRepository.Update(userId);
                
                if (user!=null)
                {
                    user.UserName = users.UserName;
                    user.Email = users.Email;
                   await _userManager.UpdateAsync(user);
                }
                else
                {
                    return View();
                }
                return RedirectToAction(nameof(AdminPage));
            }

        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = await _UserBaseRepository.GetByIdAsync(id);
            if (userId != null)
            {
                return View(userId);
            }
            else
            {
                return RedirectToAction(nameof(AdminPage));
            }
        } 
        [HttpPost]
        public async Task<IActionResult> Delete(int id,Users users)
        {
            var userId = await _UserBaseRepository.GetByIdAsync(id);
            var user = await _userManager.FindByEmailAsync(userId.Email);
            if (userId != null)
            {
                _UserBaseRepository.Delete(userId.UsersId);
               await _userManager.DeleteAsync(user);
                return RedirectToAction(nameof(AdminPage));
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> View(int id)
        {
            var userId = await _UserBaseRepository.GetByIdAsync(id);
            return View(userId);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(RegistorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var fileName = string.Empty;
                if (model.Image != null)
                {
                    if (model.Image.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError(string.Empty, "The uploaded file size exceeds the 2 MB limit.");
                        return View(model);
                    }
                    var uploads = Path.Combine(_hosting.WebRootPath, "images");
                    fileName = model.Image.FileName;
                    var fullPath = Path.Combine(uploads, fileName);
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        model.Image.CopyTo(fileStream);
                    }
                }
                var userName = model.FirstName + model.LastName;
                userName = userName?.Replace(" ", "");
                var existingemail = await _userManager.FindByEmailAsync(model.Email);
                if (existingemail != null)
                {
                    ModelState.AddModelError(string.Empty, "Email already exists.");
                    return View(model);
                }
                IdentityUser user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = userName,
                };
                var existingUser = await _userManager.FindByNameAsync(user.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Username already exists.");
                    return View(model);
                }
                var result = await _userManager.CreateAsync(user, model.Password!);
                if (result.Succeeded)
                {

                    Users users = new Users
                    {
                        UserName = userName,
                        Email = model.Email,
                        Password = model.Password,
                        ImageName = fileName
                    };
                    _db.Add(users);
                    _db.SaveChanges();
                }
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                    {
                        ModelState.AddModelError(err.Code, err.Description);
                        return View(model);
                    }
                }
                await _userManager.AddToRoleAsync(user, "Admin");
                return RedirectToAction(nameof(AdminPage));
            }
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}