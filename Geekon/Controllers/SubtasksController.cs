﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Geekon.Models;
using Microsoft.AspNetCore.Identity;

namespace Geekon.Controllers
{
    public class SubtasksController : Controller
    {
        private readonly GeekOnDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SubtasksController(GeekOnDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Subtasks
        public async Task<IActionResult> Index(int? taskId)
        {
            if (taskId == null)
                return NotFound();

            var _subtaskContext = _context.Subtasks.Where(s => s.TaskId == taskId);

            if (_subtaskContext == null)
                return NotFound();

            return View(_subtaskContext);
        }

        // GET: Subtasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var subtasks = await _context.Subtasks.FirstOrDefaultAsync(s => s.SubtaskId == id);

            if (subtasks == null)
                return NotFound();

            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == subtasks.TaskId);

            var proj = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == task.ProjId);

            var access = from ac in _context.ProjectUsers
                         where ac.UserId == _userManager.GetUserId(User) && ac.ProjectId == proj.ProjectId
                         select ac;

            if (access.Count() == 0)
                return NoContent();

            return View(subtasks);
        }

        // GET: Subtasks/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Subtasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create(int? taskId, [Bind("SubtaskId")] Subtasks subtasks)
        {
            try
            {
                subtasks.Status = Models.TaskStatus.ToDo;
                subtasks.SubtaskName = "New task";
                subtasks.TaskId = (int)taskId;
                subtasks.Date = DateTime.Today;
                _context.Add(subtasks);

                //add subtask into task
                var task = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId);
                task.Subtasks.Add(subtasks);
                //_context.Update(task);

                await _context.SaveChangesAsync();

                return View(subtasks);
            }
            catch {
                return NotFound();
            }
        }

        // GET: Subtasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subtasks = await _context.Subtasks.FindAsync(id);
            if (subtasks == null)
            {
                return NotFound();
            }
            return PartialView("_PartialSubtaskEdit", subtasks);
        }

        // POST: Subtasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("SubtaskId,SubtaskName,TaskId,Status,UserId,Date,Comment")] Subtasks subtasks)
        {
            try
            {
                _context.Update(subtasks);
                await _context.SaveChangesAsync();
                return PartialView("_PartialSubtaskEdit", subtasks);
            }
            catch
            {
                return NotFound();
            }
        }

        // GET: Subtasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subtasks = await _context.Subtasks
                .FirstOrDefaultAsync(m => m.SubtaskId == id);
            if (subtasks == null)
            {
                return NotFound();
            }

            return View(subtasks);
        }

        // POST: Subtasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subtasks = await _context.Subtasks.FindAsync(id);
            _context.Subtasks.Remove(subtasks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubtasksExists(int id)
        {
            return _context.Subtasks.Any(e => e.SubtaskId == id);
        }
    }
}
