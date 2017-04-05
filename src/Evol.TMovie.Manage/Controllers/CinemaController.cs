using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Evol.TMovie.Domain.QueryEntries;
using Evol.TMovie.Domain.QueryEntries.Parameters;
using Evol.TMovie.Domain.Commands.Dto;
using Evol.Domain.Messaging;
using Evol.TMovie.Domain.Commands;

namespace Evol.TMovie.Manage.Controllers
{
    public class CinemaController : Controller
    {
        public ICinemaQueryEntry CinemaQueryEntry { get; set; }

        public ICommandBus CommandBus { get; set; }

        public CinemaController(ICinemaQueryEntry cinemaQueryEntry)
        {
            CinemaQueryEntry = cinemaQueryEntry;
        }

        // GET: Cinema
        public async Task<ActionResult> Index(CinemaQueryParameter param = null, int pageIndex = 1, int pageSize = 10)
        {
            var paged = await CinemaQueryEntry.PagedAsync(param, pageIndex, pageSize);
            return View(paged);
        }

        // GET: Cinema/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var item = await CinemaQueryEntry.FetchAsync(id);
            return View(item);
        }

        // GET: Cinema/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cinema/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CinemaCreateDto dto)
        {
            if (! await TryUpdateModelAsync(dto))
            {
                return View(dto);
            }

            await CommandBus.SendAsync(new CinemaCreateCommand() { Input = dto });

            return RedirectToAction("Index");
        }

        // GET: Cinema/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var item = await CinemaQueryEntry.FetchAsync(id);
            return View(item);
        }

        // POST: Cinema/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, CinemaUpdateDto dto)
        {
            if (!await TryUpdateModelAsync(dto))
            {
                return View(dto);
            }

            await CommandBus.SendAsync(new CinemaUpdateCommand() { Input = dto });

            return View(dto);
        }

        // GET: Cinema/Delete/5
        public async Task<bool> Delete(Guid id)
        {
            await CommandBus.SendAsync(new CinemaDeleteCommand() { Input = new CinemaDeleteDto() { Id = id } });
            return true;
        }

    }
}