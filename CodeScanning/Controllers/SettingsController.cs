using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using CodeScanning.Data;
using CodeScanning.Models;
using CodeScanning.ViewModels;

using Docker.DotNet;
using Docker.DotNet.Models;

namespace CodeScanning.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Settings
        public async Task<IActionResult> Index()
        {
            return View(await _context.Settings.ToListAsync());
        }

        // GET: Settings/Create
        public IActionResult Create()
        {
            var settings = _context.Settings.FirstOrDefault();
            if (settings != null)
            {
                var existing = new SettingsFormViewModel();
                existing.Settings = settings;
                return View("SettingsForm", existing);
            }
            var model = new SettingsFormViewModel();

            return View("SettingsForm", model);
        }

        // POST: Settings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SettingsFormViewModel model)
        {
           
            var settings = _context.Settings.FirstOrDefault();
            if (settings != null)
            {
                var existing = new SettingsFormViewModel();
                existing.Settings = settings;
                return View("SettingForm", existing);
            }

            if (!ModelState.IsValid)
                return View("SettingsForm", model);
            
            
            _context.Add(model.Settings);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: Settings/Edit/5
        public async Task<IActionResult> Update(int? id)
        {

            var settings = await _context.Settings.FindAsync(id);
            var existing = new SettingsFormViewModel();
            existing.Settings = settings;
            return View("SettingsForm",existing);
        }

        // POST: Settings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SettingsFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("SettingsForm", model);

        
            try
            {
                _context.Update(model.Settings);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SettingsExists(model.Settings.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
                
             }

            /*DockerClient client = new DockerClientConfiguration().CreateClient();
            await client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = "fedora/memcached",
                HostConfig = new HostConfig()
                {
                    DNS = new[] { "8.8.8.8", "8.8.4.4" }
                }
            });*/
            return RedirectToAction("Index", "Home");
        }

        private bool SettingsExists(int id)
        {
            return _context.Settings.Any(e => e.Id == id);
        }
    }
}
