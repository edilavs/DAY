using Day.DAL;
using Day.Helpers;
using Day.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Day.Areas.Manage.Controllers
{
    [Area("manage")]
    public class TeamMemberController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TeamMemberController(AppDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            var data = _context.TeamMembers.ToList();
            return View(data);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(TeamMember member)
        {
            if (member.ImageFile!=null)
            {
                if (member.ImageFile.ContentType!="image/png" && member.ImageFile.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ImageFile", "Image type must be /png or /jpeg");
                }

                if (member.ImageFile.Length>2097152)
                {
                    ModelState.AddModelError("ImageFile", "Image size must be less than 2MB!");
                }                
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Image is required!");
            }
            if (!ModelState.IsValid)
            {
                return View();
            }

            member.Image = FileManager.Save(_env.WebRootPath,"uploads/members",member.ImageFile);

            _context.TeamMembers.Add(member);
            _context.SaveChanges();
            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            var member = _context.TeamMembers.FirstOrDefault(x => x.Id == id);
            if (member==null)
            {
                return RedirectToAction("error", "dashboard");
            }
            return View(member);
        }
        [HttpPost]
        public IActionResult Edit(TeamMember member)
        {
            if (member==null)
            {
                return RedirectToAction("error", "dashboard");
            }
            var existMember = _context.TeamMembers.FirstOrDefault(x => x.Id == member.Id);
            if (member.ImageFile!=null)
            {
                if (member.ImageFile != null)
                {
                    if (member.ImageFile.ContentType != "image/png" && member.ImageFile.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ImageFile", "Image type must be /png or /jpeg");
                    }

                    if (member.ImageFile.Length > 2097152)
                    {
                        ModelState.AddModelError("ImageFile", "Image size must be less than 2MB!");
                    }
                }
                else
                {
                    ModelState.AddModelError("ImageFile", "Image is required!");
                }
                if (!ModelState.IsValid)
                {
                    return View();
                }
                FileManager.Delete(_env.WebRootPath, "uploads/members", member.ImageFile.FileName);
                existMember.Image = FileManager.Save(_env.WebRootPath, "uploads/members", member.ImageFile);
            }
            existMember.FullName = member.FullName;
            existMember.Position = member.Position;
            existMember.Desc = member.Desc;
            _context.SaveChanges();
            return RedirectToAction("index");



        }
        public IActionResult Delete(int id)
        {
            var member = _context.TeamMembers.FirstOrDefault(x => x.Id == id);
            if (member == null)
            {
                return RedirectToAction("error", "dashboard");
            }
            return View(member);
        }
        [HttpPost]
        public IActionResult Delete(TeamMember member)
        {
            var existTM = _context.TeamMembers.FirstOrDefault(x => x.Id == member.Id);
            FileManager.Delete(_env.WebRootPath, "uploads/members", existTM.Image);
            _context.TeamMembers.Remove(existTM);
            _context.SaveChanges();
            

            return RedirectToAction("index");
           
        }

    }
}
