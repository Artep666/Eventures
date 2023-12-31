﻿using EventuresApp.Data;
using EventuresApp.Domain;
using EventuresApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace EventuresApp.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext context;
        public EventsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult All()
        {
            List<EventAllViewModel> events=context.Events
                .Select(eventFromDb => new EventAllViewModel
                {
                    Name = eventFromDb.Name,
                    Place = eventFromDb.Place,
                    Start =eventFromDb.Start.ToString("dd-MMM-yyyy HH:mm",CultureInfo.InvariantCulture),
                    End = eventFromDb.End.ToString("dd-MMM-yyyy HH:mm", CultureInfo.InvariantCulture),
                    Owner =eventFromDb.Owner.UserName

                })
                .ToList();
            return this.View(events);
                

        }
        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Create(EventCreateBindingModel bindingModel)
        {
            if (this.ModelState.IsValid)
            {
                string currentUserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
                Event eventForDb = new Event
                {
                    Name = bindingModel.Name,
                    Place = bindingModel.Place,
                    Start = bindingModel.Start,
                    End = bindingModel.End,
                    TotalTickets = bindingModel.TotalTickets,
                    PricePerTicket = bindingModel.PricePerTicket,
                    OwnerId = currentUserId
                };
                context.Events.Add(eventForDb);
                context.SaveChanges();

                return this.RedirectToAction("All");

            }

            return this.View();
        }
    }
}
