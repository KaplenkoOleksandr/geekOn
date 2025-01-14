﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Geekon.Models;
using Microsoft.AspNetCore.Identity;
using Geekon.Data;

namespace Geekon.Controllers
{
    public class ProjectUsersController : Controller
    {
        private readonly GeekOnDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectUsersController(GeekOnDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ProjectUsersController()
        {
        }

        // GET: ProjectUsers
        public async Task<IActionResult> Index()
        {
            string userId = _userManager.GetUserId(User);

            var _projContext = from p in _context.Projects
                               join us in _context.ProjectUsers on p.ProjectId equals us.ProjectProjectId
                               where us.UserId == userId
                               select p;

            return View(await _projContext.ToListAsync());
        }

        // GET: ProjectUsers/Details/5
        public IActionResult Details(int? projId)
        {
            if (projId == null)
            {
                return NotFound();
            }

            var _usersContext = from us in _context.ProjectUsers
                                where us.ProjectProjectId == projId
                                select us;

            List<ApplicationUser> users = new List<ApplicationUser>();
            foreach (var user in _usersContext)
            {
                users.Add(_userManager.FindByIdAsync(user.UserId).Result);
            }

            return PartialView("_partialDetails", users);
        }

        // GET: ProjectUsers/Create
        public IActionResult Create()
        {
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId");
            return View();
        }

        // POST: ProjectUsers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectProjectId,UserId")] ProjectUsers projectUsers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectUsers);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", projectUsers.ProjectProjectId);
            return View(projectUsers);
        }

        // GET: ProjectUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUsers = await _context.ProjectUsers.FindAsync(id);
            if (projectUsers == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "ProjectId", "ProjectId", projectUsers.ProjectProjectId);
            return View(projectUsers);
        }

        // POST: ProjectUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit(int projId, string emails)
        {
            if(projId == null)
            {
                NotFound();
            }
            try
            {
                string[] email = emails.Split();
                foreach (var e in email)
                {
                    var user = _userManager.FindByEmailAsync(e).Result;
                    if (user == null)
                    {
                        
                    }
                    else
                    {
                        var projUser = await _context.ProjectUsers.FirstOrDefaultAsync(p => p.UserId == user.Id && p.ProjectProjectId == projId);
                        if (projUser == null)
                        {
                            ProjectUsers projectUser = new ProjectUsers();
                            projectUser.ProjectProjectId = projId;
                            projectUser.UserId = user.Id;
                            _context.Add(projectUser);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                return StatusCode(200);
            }
            catch
            {
                return NotFound();
            }

        }

        // GET: ProjectUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectUsers = await _context.ProjectUsers
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (projectUsers == null)
            {
                return NotFound();
            }

            return View(projectUsers);
        }

        // POST: ProjectUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectUsers = await _context.ProjectUsers.FindAsync(id);
            _context.ProjectUsers.Remove(projectUsers);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectUsersExists(int id)
        {
            return _context.ProjectUsers.Any(e => e.Id == id);
        }
    }
}
