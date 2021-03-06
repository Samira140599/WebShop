using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web_Shop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace Web_Shop.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var db = new ShopContext();

            ViewBag.Books = from book in db.Books select book.BookName;

            return View();
        }

        public ActionResult About()
        {
            // AddRoles();
            AddCustomerWithRole("vasya@ukr.net", "Vasya99!","kozel");

            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        private void AddCustomerWithRole(string custMail,string custPassw, string custRole)
        {
            if (string.IsNullOrWhiteSpace(custMail)
                || string.IsNullOrWhiteSpace(custPassw)
                || string.IsNullOrWhiteSpace(custRole)) return;

            // create user
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));          
                var myUser = new ApplicationUser { Email = custMail, UserName = custMail };
                var userCreated = userManager.Create(myUser,custPassw);

                // Create or use role
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                var roleExists = roleManager.Roles.Any(x => x.Name == custRole);

                if (!roleExists)
                {
                    var roleAdmin = new IdentityRole { Name = custRole };
                    roleManager.Create(roleAdmin);
                }

                if(userCreated.Succeeded && roleManager.Roles.Any(x => x.Name == custRole))
                {
                    // use this role
                    userManager.AddToRole(myUser.Id, custRole);  
                }
                else
                {
                    // Log - "ERROR - AddCustomerWithRole : " +  userCreated.Errors;
                }

                context.SaveChanges();              
            }
        }
                
        private void AddRoles()
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                //? создаем две роли
                var roleAdmin = new IdentityRole { Name = "admin" };
                var roleOwner = new IdentityRole { Name = "owner"};

                //? добавляем роли в бд
                roleManager.Create(roleAdmin);
                roleManager.Create(roleOwner);

                #region создаем пользователей
           
                //var user = new ApplicationUser { Email = "alex@mail.ru", UserName = "alex@mail.ru" };
                //string password = "Alex!2";
                //var result = userManager.Create(user, password);

                //var user = new ApplicationUser { Email = "alex@mail.ru", UserName = "alex@mail.ru" };
                //var myUser = new ApplicationUser { Email = "samira.dehestani@ukr.net", UserName = "samira.dehestani@ukr.net" };
                //string password = "samira99";
                //var myRes = userManager.Create(myUser, password);
                //if (myRes.Succeeded)
                //{
                //    userManager.AddToRole(myUser.Id, myRole.Name);
                //}
                //if (result.Succeeded)
                //{

                #endregion

                //? добавляем  роль для пользователя Samira ("07ad7e74-fffa-4728-bcca-a82e485a33f7")  
                userManager.AddToRole("07ad7e74-fffa-4728-bcca-a82e485a33f7", roleAdmin.Name);
                userManager.AddToRole("07ad7e74-fffa-4728-bcca-a82e485a33f7", "owner");

                context.SaveChanges();
            }
        }

    }
}