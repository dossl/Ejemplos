using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Resource;
using StoreProject.Models;

namespace StoreProject.ViewModels
{
    public class StoreOptimizedViewModel
    {


        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "StoreCode")]
        [Required]
        [Key]
        public string StoreCodeId { get; set; }

        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "Name")]
        [Required]
        public string StoreName { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "Contactname")]
        public string ContactName { get; set; }


        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "Phone")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "Address")]
        public string Address { get; set; }


        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "Address2")]
        public string Address2 { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "BankName")]
        public string BankName { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "AccountNumber")]
        public string AccountNumber { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "BankingRoute")]
        public string BankingRoute { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "AccountOwner")]
        public string AccountOwner { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "User")]
        public string UserClaro { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "Password")]
        public string PassClaro { get; set; }

       
        public virtual State State { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "State")]
        public string StateCodeId { get; set; }

        public virtual City City { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "City")]
        public int CityId { get; set; }

        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "ZipCode")]
        public string ZipCode { get; set; }
        
        [Required]
        [Display(ResourceType = typeof(ResourceStore_enUS), Name = "SocialNumber")]
        public string SocialNumber { get; set; }

        public string Arr { get; set; }

        public List<PhoneActivation> PhoneActivationLists { get; set; }

    }
}