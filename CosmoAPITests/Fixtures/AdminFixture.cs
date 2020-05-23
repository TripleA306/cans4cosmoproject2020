using CosmoAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CosmoAPITests.Fixtures
{
    public class AdminFixture
    {

        /// <summary>
        /// Method for creating a list of pre-determined regions. This will generate dates relative to today's date
        /// </summary>
        /// <returns></returns>
        public static List<Admin> GetAdminFixture()
        {
            //create a random salt for this test admin
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }


            //store the password by using the salt to encrypt the original password
            string password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: "9135f73499f84c7e70bcb5f582e4bf13ed78c899261e6b87c05454e2515c578a", //The SHA-256 hash of Cosmo123
                salt: salt, //salt value
                prf: KeyDerivationPrf.HMACSHA256, //encrypt using SHA256
                iterationCount: 10000, //run through the algorithm 10000 times
                numBytesRequested: 256 / 8 //return 64 byte string
                ));


            return new List<Admin>()   //Defining a list of Regions to load into context
            {
                new Admin { username = "Cans4Cosmo", password = password, salt = salt }
            };
        }

        /// <summary>
        /// Calls GetAdminFixture to get a list of preset regions to load to context
        /// </summary>
        /// <param name="context"></param>
        public static void Load(CosmoContext context)
        {
            context.AddRange(GetAdminFixture());
            context.SaveChanges();
        }

        /// <summary>
        /// Removes any admins currently in the context
        /// </summary>
        /// <param name="context"></param>
        public static void Unload(CosmoContext context)
        {
            context.RemoveRange(context.Admin);
            context.SaveChanges();
        }


    }
}
