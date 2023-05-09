using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.enums;

namespace Infrastracture.Services.Permission
{
    public static class Permissions
    {

        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
                                   {
                                    $"Permissions.{module}.Create",
                                    $"Permissions.{module}.Index",
                                    $"Permissions.{module}.Edit",
                                    $"Permissions.{module}.Delete",

                                   };

        }

        //public static List<string> GenerateAllPermissionsForModule()
        //{
        //    var allpermissions = new List<string>();

        //    var modules = Enum.GetValues(typeof(modules));


        //    foreach (var item in modules)
        //    {
        //        //    allpermissions.AddRange(GeneratePermissionsForModule(item.ToString()));
        //        //  allpermissions.Add(item.ToString());
        //        string value = item.ToString();
        //        allpermissions.Add($"Permissions.{value}");
        //    }


        //    return allpermissions;
        //}


        public static List<SchemaOfRole> GenerateAllPermissionsForModule()
        {
            var allpermissions = new List<SchemaOfRole>();

            foreach (var item in RoleModule.GetController())
            {
                allpermissions.Add(new SchemaOfRole
                {
                    en = $"Permissions.{item.Key}",
                    ar = $"{item.Value}"
                });
            }

            return allpermissions;
        }
    }

    public class SchemaOfRole
    {
        public string ar { get; set; }
        public string en { get; set; }
    }

    public static class CustomerPayment
    {
        public static string View = $"Permissions.{modules.CustomerPaymentIndex.ToString()}.Index";
        public static string Create = $"Permissions.{modules.CustomerPaymentCreate.ToString()}.Create";
        public static string Edit = $"Permissions.{modules.CustomerPaymentEdit.ToString()}.Edit";
        public static string Delete = $"Permissions.{modules.CustomerPaymentDelete.ToString()}.Delete";
    }


}
