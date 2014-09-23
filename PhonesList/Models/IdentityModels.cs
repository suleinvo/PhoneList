using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;


namespace PhonesList.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.


    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Contact> Contacts { get; set; }
    }

    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set;}
        
        [MaxLength(15)]
        public string Phone { get; set;}

        //EMAIL CHACKING
        [RegularExpression("[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}", ErrorMessage = "Unvalid Email")]
        public string Email { get; set;}
        public ApplicationUser User { get; set; }
    }

    public class EmailSend
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public System.Data.Entity.DbSet<PhonesList.Models.Contact> Contacts { get; set; }

        public System.Data.Entity.DbSet<PhonesList.Models.EmailSend> EmailSends { get; set; }
    }
}