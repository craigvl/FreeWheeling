using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using FreeWheeling.Domain.Entities;
using FreeWheeling.Domain.DataContexts;

namespace FreeWheeling.UI.Controllers
{
    public class ExpandedAPIController : ApiController
    {
        private CycleDb db = new CycleDb();

        // GET api/Test
        public IQueryable<UserExpand> GetUserExpands()
        {
            return db.UserExpands;
        }

        // GET api/Test/5
        [ResponseType(typeof(UserExpand))]
        public IHttpActionResult GetUserExpand(int id)
        {
            UserExpand userexpand = db.UserExpands.Find(id);
            if (userexpand == null)
            {
                return NotFound();
            }

            return Ok(userexpand);
        }

        // PUT api/Test/5
        public IHttpActionResult PutUserExpand(int id, UserExpand userexpand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userexpand.id)
            {
                return BadRequest();
            }

            db.Entry(userexpand).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExpandExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/Test
        [ResponseType(typeof(UserExpand))]
        public IHttpActionResult PostUserExpand(UserExpand userexpand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UserExpands.Add(userexpand);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = userexpand.id }, userexpand);
        }

        // DELETE api/Test/5
        [ResponseType(typeof(UserExpand))]
        public IHttpActionResult DeleteUserExpand(int id)
        {
            UserExpand userexpand = db.UserExpands.Find(id);
            if (userexpand == null)
            {
                return NotFound();
            }

            db.UserExpands.Remove(userexpand);
            db.SaveChanges();

            return Ok(userexpand);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExpandExists(int id)
        {
            return db.UserExpands.Count(e => e.id == id) > 0;
        }
    }
}