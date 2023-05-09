using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.enums
{
    public enum modules
    {
        UserCreate
              , RoleCreate
              , RoleIndex
              , CustomerPaymentIndex
              , CustomerPaymentCreate
              , CustomerPaymentEdit
              , CustomerPaymentDelete


    }
    public class RoleModule
    {

        private static Dictionary<string, string> ControllerNames;

        public static Dictionary<string, string> GetController()
        {
            ControllerNames = new Dictionary<string, string>();
            ControllerNames.Add("CustomerPaymentIndex", "دفعات العملاء");
            ControllerNames.Add("CustomerPaymentCreate", "اضافه دفعات العملاء");
            ControllerNames.Add("CustomerPaymentEdit", "تعديل دفعات العملاء");
            ControllerNames.Add("CustomerPaymentDelete", "حذف دفعات العملاء");

            ControllerNames.Add("CustomerRentIndex", "فواتير الايجار");
            ControllerNames.Add("CustomerRentCreate", "اضافه فاتورة ايجار");
            ControllerNames.Add("CustomerRentEdit", "تعديل فاتورة ايجار");
            ControllerNames.Add("CustomerRentDelete", "حذف  فاتورة ايجار");

            ControllerNames.Add("CustomersIndex", "العملاء");
            ControllerNames.Add("CustomersCreate", "اضافة عميل");
            ControllerNames.Add("CustomersEdit", "تعديل العملاء");
            ControllerNames.Add("CustomersDelete", "حذف عميل");

            ControllerNames.Add("CustomerDeptIndex", "مديونية العملاء");
            ControllerNames.Add("CustomerDeptCreate", "اضافة مديونية لعميل");
            ControllerNames.Add("CustomerDeptEdit", "تعديل مديونية عميل");
            ControllerNames.Add("CustomerDeptDelete", "حذف مديونية عميل");

            ControllerNames.Add("CareStatsAvailable", "السيارات المتاحة ");
            ControllerNames.Add("CareStatsAvailableToday", "السيارات القادمة اليوم ");
            ControllerNames.Add("CareStatsNotAvailable", "السيارات الموجرة ");
            ControllerNames.Add("CareStatsEditRentBack", "استلام السياراة المؤجرة");


            ControllerNames.Add("UsersIndex", " المستخدمين ");
            ControllerNames.Add("UsersCreate", " اضافه مستخدم ");
            ControllerNames.Add("UsersEdit", " تعديل مستخدم ");
            ControllerNames.Add("UsersDelete", " حذف مستخدم ");

            ControllerNames.Add("RolesIndex", " المجموعات ");
            ControllerNames.Add("RolesCreate", " اضافه مجموعه ");
            ControllerNames.Add("RolesEdit", " تعديل مجموعه ");
            ControllerNames.Add("RolesDelete", " حذف مجموعه ");
            ControllerNames.Add("MangeRolePermition", " صلاحيات مجموعه  ");

            ControllerNames.Add("CompaniesIndex", "الشركات");
            ControllerNames.Add("CompaniesCreate", "اضافة شركه");
            ControllerNames.Add("CompaniesEdit", "تعديل شركة");
            ControllerNames.Add("CompaniesDelete", "حذف شركه");

            ControllerNames.Add("EmployeesIndex", "الموظفيين");
            ControllerNames.Add("EmployeesCreate", " اضافة موظف");
            ControllerNames.Add("EmployeesEdit", " تعديل موظف");
            ControllerNames.Add("EmployeesDelete", " حذف موظف");

            ControllerNames.Add("MarketersIndex", "المسوقين");
            ControllerNames.Add("MarketersCreate", " اضافة مسوق");
            ControllerNames.Add("MarketersEdit", " تعديل مسوق");
            ControllerNames.Add("MarketersDelete", " حذف مسوق");

            ControllerNames.Add("GovernmentIndex", "المحافظات");
            ControllerNames.Add("GovernmentCreate", " اضافة محافظه");
            ControllerNames.Add("GovernmentEdit", " تعديل محافظة");
            ControllerNames.Add("GovernmentDelete", " حذف محافظة");

            ControllerNames.Add("CashDepositIndex", "ايداع");
            ControllerNames.Add("CashDepositCreate", " اضافة ايداع");
            ControllerNames.Add("CashDepositEdit", " تعديل ايداع");
            ControllerNames.Add("CashDepositDelete", " حذف ايداع");

            ControllerNames.Add("CashwithdrawalIndex", "السحب");
            ControllerNames.Add("CashwithdrawalCreate", " اضافة سحب");
            ControllerNames.Add("CashwithdrawalEdit", " تعديل سحب");
            ControllerNames.Add("CashwithdrawalDelete", " حذف سحب");

            ControllerNames.Add("ExpensesIndex", "المصروفات");
            ControllerNames.Add("ExpensesCreate", " اضافة مصروف");
            ControllerNames.Add("ExpensesEdit", " تعديل مصروف");
            ControllerNames.Add("ExpensesDelete", " حذف مصروف");




            ControllerNames.Add("ExpenseTypeIndex", "انواع المصروفات");
            ControllerNames.Add("ExpenseTypeCreate", " اضافة نوع مصروف");
            ControllerNames.Add("ExpenseTypeEdit", " تعديل نوع مصروف");
            ControllerNames.Add("ExpenseTypeDelete", " حذف نوع مصروف");


            ControllerNames.Add("StockIndex", "الخزن ");
            ControllerNames.Add("StockCreate", " اضافة خزنة ");
            ControllerNames.Add("StockEdit", " تعديل  خزنة");
            ControllerNames.Add("StockDelete", " حذف  خزنة");


            ControllerNames.Add("StockTransferIndex", "تحويلات الخزنة ");
            ControllerNames.Add("StockTransferCreate", " اضافة تحويل خزنة ");
            ControllerNames.Add("StockTransferEdit", " تعديل  تحويل خزنة");
            ControllerNames.Add("StockTransferDelete", " حذف  تحويل خزنة");


            ControllerNames.Add("CarAccidentIndex", "حوادث العربيات");
            ControllerNames.Add("CarAccidentCreate", " اضافة حادثة ");
            ControllerNames.Add("CarAccidentEdit", " تعديل  حادثة");
            ControllerNames.Add("CarAccidentDelete", " حذف  حادثة");


            

            ControllerNames.Add("CarIndex", " السيارات");
            ControllerNames.Add("CarCreate", " اضافة سيارة ");
            ControllerNames.Add("CarEdit", " تعديل  سيارة"); 
            ControllerNames.Add("CarDelete", " حذف  سيارة");

            ControllerNames.Add("CarMaintenanceIndex", " صيانة السيارات");
            ControllerNames.Add("CarMaintenanceCreate", " اضافة صيانة سيارة ");
            ControllerNames.Add("CarMaintenanceEdit", " تعديل  صيانة سيارة");
            ControllerNames.Add("CarMaintenanceDelete", " حذف  صيانة سيارة");

            ControllerNames.Add("OwnerIndex", " الملاك");
            ControllerNames.Add("OwnerCreate", " اضافة ملاك ");
            ControllerNames.Add("OwnerEdit", " تعديل  ملاك"); 
            ControllerNames.Add("OwnerDelete", " حذف  مالك");


            ControllerNames.Add("OwnerPaymentIndex", " مدفوعات الملاك ");
            ControllerNames.Add("OwnerPaymentCreate", " اضافة مدفوعات ملاك ");
            ControllerNames.Add("OwnerPaymentEdit", " تعديل  مدفوعات ملاك");
            ControllerNames.Add("OwnerPaymentDelete", " حذف  مدفوعات الملاك");

            ControllerNames.Add("OwnerRentIndex", "  ايجار الملاك ");
            ControllerNames.Add("OwnerRentCreate", " اضافة  عقد ايجار للمالك ");
            ControllerNames.Add("OwnerRentEdit", " تعديل  عقد ايجار مالك ");
            ControllerNames.Add("OwnerRentDelete", " حذف   عقد ايجار مالك");

            ControllerNames.Add("OwnerAccountIndex", " كشف حساب الملاك ");
            ControllerNames.Add("OwnerAccountCreate", "عرض كشف حساب مالك معين");

            ControllerNames.Add("CustomerAccountIndex", " كشف حساب العملاء ");
            ControllerNames.Add("CustomerAccountCreate", "عرض كشف حساب عميل معين");

            ControllerNames.Add("StockMovementIndex", " حركة الخزنة");
            ControllerNames.Add("StockMovementCreate", "عرض حركة  خزنة معينة");

            ControllerNames.Add("CarStatusReportIndex", "  تقرير التأجير");
            ControllerNames.Add("CarStatusReportCreate", "عرض تأجيرات معينة");

            ControllerNames.Add("CarAvailableByDateIndex", "   عودة السيارات");
            ControllerNames.Add("CarAvailableByDateCreate", "عودة السيارات في تاريخ معين");

            ControllerNames.Add("CustomerReportIndex", "تقرير تقييم العملاء");
            ControllerNames.Add("CustomerReportCreate", "عرض تقييم معين");

            ControllerNames.Add("CustomerEvaluationIndex", " تقييمات العملاء");
            ControllerNames.Add("CustomerEvaluationCreate", " اضافة تقييم جديد ");
            ControllerNames.Add("CustomerEvaluationEdit", "تعديل التقييمات");
            ControllerNames.Add("CustomerEvaluationDelete", "حذف التقييمات");


            ControllerNames.Add("CustomerReservationIndex", " حجز السيارات");
            ControllerNames.Add("CustomerReservationCreate", " اضافة حجز جديد ");
            ControllerNames.Add("CustomerReservationEdit", "تعديل الحجز");
            ControllerNames.Add("CustomerReservationDelete", "حذف الحجز");


            ControllerNames.Add("CustomerViolationIndex", "  مخالفات السيارات");
            ControllerNames.Add("CustomerViolationCreate", " اضافة مخالفة جديدة ");
            ControllerNames.Add("CustomerViolationEdit", "تعديل المخالفات");
            ControllerNames.Add("CustomerViolationDelete", "حذف المخالفات");



            ControllerNames.Add("ViolationReportIndex", "  تقرير المخالفات");
            ControllerNames.Add("ViolationReportCreate", "عرض مخالفة سيارة معينة");


            return ControllerNames;

        }


    }

}
