using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IBCS_Web_Portal.Models
{
    public class register
    {
        [Required(ErrorMessage = "Name is Required")]
        public string name { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        //[RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",

        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$",
        ErrorMessage = "Please Enter Proper Email")]
        public string email { get; set; }

        //[Required(ErrorMessage = "Mobile is Required")]
        //[RegularExpression(@"\d{11}", ErrorMessage = "Please enter 11 digit Mobile No.")]
        [RegularExpression(@"^0*(300[1-8][0-9]{6}|3009[0-8][0-9]{5}|30099[0-8][0-9]{4}|300999[0-8][0-9]{3}|3009999[0-8][0-9]{2}|30099999[0-8][0-9]|300999999[0-9]|30[1-9][0-9]{7}|3[1-9][0-9]{8})$",
        ErrorMessage = "Please enter 11 digit Mobile No.")]
        public string mobile { get; set; }

        //[Required(ErrorMessage = "Password is required")]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }

        //[Required(ErrorMessage = "Confirm Password is required")]
        //[DataType(DataType.Password)]
        //[Compare("Password")]
        //public string ConfirmPassword { get; set; }

        //[Required(ErrorMessage = "Age is required")]
        //[Range(typeof(int), "18", "40", ErrorMessage = "Age can only be between 18 and 40")]
        //public string age { get; set; }
    }
}