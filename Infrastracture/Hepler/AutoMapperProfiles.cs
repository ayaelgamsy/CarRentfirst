using AutoMapper;
using Core.Dtos;
using Core.Dtos.CarAccidentDto;
using Core.Dtos.CarAvailableByDateDto;
using Core.Dtos.CarDto;
using Core.Dtos.CarMaintenanceDto;
using Core.Dtos.CarStatusReportDto;
using Core.Dtos.CashDepositDto;
using Core.Dtos.CashwithdrawalDto;
using Core.Dtos.CompaniesDto;
using Core.Dtos.CustomerAccountDto;
using Core.Dtos.CustomerDeptDto;
using Core.Dtos.CustomerDto;
using Core.Dtos.CustomerEvaluationDto;
using Core.Dtos.CustomerPaymentDto;
using Core.Dtos.CustomerRentDto;
using Core.Dtos.CustomerReportDto;
using Core.Dtos.CustomerReservationDto;
using Core.Dtos.CustomerViolationDto;
using Core.Dtos.DropDowns;
using Core.Dtos.EmployeeDto;
using Core.Dtos.ExpenseDto;
using Core.Dtos.ExpenseTypeDto;
using Core.Dtos.GovernmentDto;
using Core.Dtos.MarketerDto;
using Core.Dtos.OwnerAccountDto;
using Core.Dtos.OwnerPaymentDto;
using Core.Dtos.OwnerRentDto;
using Core.Dtos.OwnersDto;
using Core.Dtos.RolesDto;
using Core.Dtos.StockDto;
using Core.Dtos.StockMovementDto;
using Core.Dtos.StockTransferDto;
using Core.Dtos.UserDto;
using Core.Dtos.ViolationReportDto;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastracture.Hepler
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {
            
            //roles
            CreateMap<Role, RolesGetDto>();
            CreateMap<RolesRegisterDto, Role>();
            CreateMap<Role, RolesEditDto>().ReverseMap();
            CreateMap<Role, CommonDto>();


            //users
            CreateMap<User, UserGetDto>();
            CreateMap<User, UserEditDto>().ReverseMap();
            CreateMap<User, UserRegisterDto>().ReverseMap();


            //claims 
            CreateMap<Role, EditRolesClaimDto>();


            //Government
            CreateMap<Government, GovernmentGetDto>();
            CreateMap<Government, GovernmentRegisterDto>().ReverseMap();
            CreateMap<Government, GovernmentDrop>();


            //Customer
            CreateMap<Customer, CustomerRegisterDto>().ReverseMap();
            CreateMap<Customer, CustomerGetDto>();
            //.ForMember(c=>c.Account,a=>a.MapFrom(n=>n.customerAccounts.Sum(n=>n.Dept)- n.customerAccounts.Sum(n => n.Borrower)));
            CreateMap<Customer, CustomerDataDto>();

            //CustomerDept
            CreateMap<CustomerLastDept, CustomerDeptRegisterDto>().ReverseMap();
            CreateMap<CustomerLastDept, CustomerDeptGetDto>();

            //CustomerRent
            CreateMap<CustomerRent, CustomerRentRegisterDto>().ReverseMap();
            CreateMap<CustomerRent, CustomerRentGetDto>();
            CreateMap<CustomerRent, CustomerRentBackDto>().ReverseMap();

            //CarStatusReport
            CreateMap<Car, CarStatusRegisterDto>();
            CreateMap<CustomerRent, CarStatusGetDto>();


            //CustomerPayment
            CreateMap<CustomerPayment, CustomerPayRegisterDto>().ReverseMap();
            CreateMap<CustomerPayment, CustomerPayGetDto>();

            //CustomerAccount
            CreateMap<CustomerAccount, CustomerAccountRegisterDto>();
            CreateMap<CustomerAccount, CustomerAccountGetDto>();


            //CustomerEvaluation
            CreateMap<CustomerEvaluation, CustomerEvaluationRegisterDto>().ReverseMap();
            CreateMap<CustomerEvaluation, CustomerEvaluationGetDto>();

            //CustomerReservation
            CreateMap<CustomerReservation, CustomerReservationRegisterDto>().ReverseMap();
            CreateMap<CustomerReservation, CustomerReservationGetDto>();


            //CustomerViolation
            CreateMap<CustomerViolation, CustomerViolationRegisterDto>().ReverseMap();
            CreateMap<CustomerViolation, CustomerViolationGetDto>();

            //CustomerViolationReport
            CreateMap<CustomerViolation, ViolationReportRegisterDto>();
            CreateMap<CustomerViolation, ViolationReportGetDto>();

            //CustomerReport
            CreateMap<CustomerEvaluation, CustomerReportRegisterDto>();
            CreateMap<Customer, CustomerReportGetDto>();

            //Employee
            CreateMap<Employee, EmployeeRegisterDto>().ReverseMap();
            CreateMap<Employee, EmployeeGetDto>();

            //Marketer
            CreateMap<Marketer, MarketerRegisterDto>().ReverseMap();
            CreateMap<Marketer, MarketerGetDto>();
            CreateMap<Marketer,MarketerDropDto>();


            
            //OwnerRent
            CreateMap<OwnerRentContract, OwnerRentRegisterDto>().ReverseMap();
            CreateMap<OwnerRentContract, OwnerRentGetDto>();

            //OwnerPayment
            CreateMap<CarOwnerPayment, OwnerPaymentRegisterDto>().ReverseMap();
            CreateMap<CarOwnerPayment, OwnerPaymentGetDto>();

            //OwnerAccount
            CreateMap<CarOwnerAccount, OwnerAccountRegisterDto>();
            CreateMap<CarOwnerAccount, OwnerAccountGetDto>();


            //Car
            CreateMap<Car, CarRegisterDto>().ReverseMap();
            CreateMap<Car, CarGetDto>();
            CreateMap<Car, CarAvailableGetDto>();
            CreateMap<Car, CarUnavailableGetDto>();

            //CarAccident
            CreateMap<CarAccident, CarAccidentRegisterDto>().ReverseMap();
            CreateMap<CarAccident, CarAccidentGetDto>();

            //CarMaintenance
            CreateMap<CarMaintenance, CarMaintenanceRegisterDto>().ReverseMap();
            CreateMap<CarMaintenance, CarMaintenanceGetDto>();

            //car owner 
            CreateMap<OwnerRegisterDto, CarOwner>().ReverseMap();
            CreateMap<CarOwner, OwnerGetDto>();

            //CarAvailableByDate
            CreateMap<CustomerRent, CarAvailableByDateRegisterDto>();
            CreateMap<CustomerRent, CarAvailableByDateGetDto>();

            //Company
            CreateMap<Company, CompanyGetDto>();
            CreateMap<CompanyRegisterDto, Company>().ReverseMap();

            //Stock
            CreateMap<Stock, StockRegisterDto>().ReverseMap();
            // CreateMap<Stock, StockGetDto>().ForMember(a => a.Account, b => b.MapFrom(a => a.StockMovement.Sum(a => a.DifferentValue)));

            CreateMap<Stock, StockGetDto>()
            .ForMember(a => a.Account, b => b.MapFrom(n => n.StockMovement.Sum(n => n.InValue) - n.StockMovement.Sum(n => n.OutValue)));

            CreateMap<Stock, DrpDto>();
            CreateMap<Customer, DrpDto>();
            CreateMap<Car, DrpDto>();
            CreateMap<Employee, DrpDto>();
            CreateMap<Marketer, DrpDto>();
            CreateMap<CarOwner, DrpDto>();

            //ExpenseType
            CreateMap<ExpenseType, ExpenseTypeRegisterDto>().ReverseMap();
            CreateMap<ExpenseType, ExpenseTypeGetDto>();
            //Expense
            CreateMap<Expense, ExpenseGetDto>();
            CreateMap<ExpenseType, DrpDto>();
            CreateMap<Expense, ExpenseRegisterDto>().ReverseMap();
            CreateMap<Expense, ExpenseReportDto>();
            //CashDepositGetDto
            CreateMap<CashDeposit, CashDepositGetDto>();
            CreateMap<CashDeposit, CashDepositRegisterDto>().ReverseMap();
            //Cashwithdrawal
            CreateMap<Cashwithdrawal, CashwithdrawalGetDto>();
            CreateMap<Cashwithdrawal, CashwithdrawalRegisterDto>().ReverseMap();
            //StockTransferRegisterDto
            CreateMap<StockTransfer, StockTransferRegisterDto>().ReverseMap();
            CreateMap<StockTransfer, StockTransferGetDto>();

            //StockMovement
            CreateMap<StockMovement, StockMovementRegisterDto>();
            CreateMap<StockMovement, StockMovementGetDto>();

        }
    }
}

