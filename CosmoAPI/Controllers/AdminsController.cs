using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CosmoAPI.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Security.Claims;
using CosmoAPI.Authorization;

namespace CosmoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly CosmoContext _context;

        public AdminsController(CosmoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// This Post method will take in an administrator
        /// Check to see if the admin name already exists
        ///    If it does, the admin will not be save
        ///    If it does not, the admin will be save
        /// </summary>
        /// <param name="administrator"></param>
        /// <returns>200 if created, 401 if not created.</returns>
        // POST: api/Admin
        [HttpPost("username-c={username}")]
        [Authorize(Roles.ADMIN)]
        public async Task<IActionResult> PostAdmin([FromBody] Admin administrator)
        {
            //If we failed the Authorize claims check, the response code will be set to 401
            if (this.Response.StatusCode == 401)
            {
                return Unauthorized();
            }

            //Check to see if it is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Check to see if the username exists
            if (!AdminUserNameExists(administrator.username))
            {
                //create a random salt for this new admin
                byte[] salt = new byte[128 / 8];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
                //Set the salt to the new admin
                administrator.salt = salt;


                //hash the password value of the uiAdmin with its password salted
                string encryptedPass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: administrator.password,
                    salt: administrator.salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                    ));

                administrator.password = encryptedPass;

                //Save/add the valid admin
                _context.Admin.Add(administrator);
                await _context.SaveChangesAsync();

                //Return the created code 
                return Created("adminCreated", administrator);
            }
            else
            {
                //Return our error message for username already existing 
                ModelState.AddModelError("username", "Username already exists");
                return BadRequest(ModelState);
            }



        }


        /// <summary>
        /// This method will check to see if an administrator username already exists
        /// </summary>
        /// <param name="username">The Administrators username string</param>
        /// <returns>true if the name exists, else false</returns>
        private bool AdminUserNameExists(string username)
        {
            return _context.Admin.Any(e => e.username == username);
        }



        /// <summary>
        /// This method will take in an admin object and check the database for an admin account matching the data passed in.
        /// </summary>
        /// <param name="admin"></param>
        /// <returns>bool value according to if the admin was found in the database.</returns>
        [HttpPost]
        public async Task<IActionResult> ValidateAdmin([FromBody] Admin admin)
        {
            //get an existing admin by searching with the username of the admin passed in
            Admin dbAdmin = GetAdmin(admin.username);
            //check if an admin was found matching the username
            if (dbAdmin == null)
            {
                //return unauthorized. The username does not exist.
                return Unauthorized();
            }
            //if the password matches the admin password saved in the database
            if (checkPassword(admin, dbAdmin))
            {
                //create a JWT key
                var key = Encoding.ASCII.GetBytes("Ca5nQTanmoOutL0gTaken313578");

                IEnumerable<Claim> claims = new Claim[] { new Claim(Roles.ADMIN.ToString(), Roles.ADMIN.ToString()) };

                //creat a JWT token
                var JWToken = new JwtSecurityToken(
                    issuer: "http://localhost:5002/api", //set the issuer, this api
                    audience: "http://localhost:8080", //set the audience we except to get requests from
                    claims: claims,
                    notBefore: new DateTimeOffset(DateTime.Now).DateTime, //token will not be valid until now
                    expires: new DateTimeOffset(DateTime.Now.AddHours(2)).DateTime, //token will expire in 2 hours
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) //sign the token
                );

                //write the token out to token
                var token = new JwtSecurityTokenHandler().WriteToken(JWToken);

                //return Ok with the token
                return Ok(token);
            }
            else //password is wrong
            {
                //returned unauthorized
                return Unauthorized();
            }

        }

        /// <summary>
        /// This method will take in the admin from the UI and the admin from the db. it will salt the password retrieved from the uiAdmin and match it to the
        /// password saved in the dbAdmin
        /// </summary>
        /// <param name="uiAdmin"></param>
        /// <param name="dbAdmin"></param>
        /// <returns></returns>
        private bool checkPassword(Admin uiAdmin, Admin dbAdmin)
        {
            //hash the password value of the uiAdmin with its password salted
            string uiPass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: uiAdmin.password,
                salt: dbAdmin.salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));

            //check if the hash matches the password stored in the admin object from the database
            if (uiPass.Equals(dbAdmin.password))
            {
                //return true
                return true;
            }
            else //passwords do not match
            {
                //return false
                return false;
            }
        }

        /// <summary>
        /// This method will take in a username and return an admin with the existing username in the database
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private Admin GetAdmin(string userName)
        {
            //initialize an admin
            Admin dbAdmin = null;
            try
            {
                //get an admin with the username passed in
                dbAdmin = _context.Admin.AsQueryable<Admin>().Where(e => e.username.CompareTo(userName) == 0).FirstOrDefault();
            }
            catch (Exception e) //if the query returns null, an excpetion may be thrown
            {
                //null return value is handled outside of this method.
            }
            //return that admin
            return dbAdmin;
        }

        /// <summary>
        /// This method will take in an admin id and check the database for an admin existing with the given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool AdminExists(int id)
        {
            return _context.Admin.Any(e => e.adminID == id);
        }




        // GET: api/Admins
        //[HttpGet]
        //public IEnumerable<Admin> GetAdmin()
        //{
        //    return _context.Admin;
        //}

        // GET: api/Admins/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetAdmin([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var admin = await _context.Admin.FindAsync(id);

        //    if (admin == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(admin);
        //}

        // PUT: api/Admins/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutAdmin([FromRoute] int id, [FromBody] Admin admin)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != admin.adminID)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(admin).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!AdminExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}




        // DELETE: api/Admins/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteAdmin([FromRoute] int id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var admin = await _context.Admin.FindAsync(id);
        //    if (admin == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Admin.Remove(admin);
        //    await _context.SaveChangesAsync();

        //    return Ok(admin);
        //}



        /// <summary>
        /// This method will take in the token supplied by the HTTP request, unencrypt it and return the id if it is valid
        /// </summary>
        /// <param name="token">The token string to examine</param>
        /// <returns>The id number associated with the token, if valid, -1 otherwise</returns>
        private int ValidateToken(string token)
        {
            return 0;
        }
    }
}