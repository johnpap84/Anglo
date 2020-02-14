using AngloRota.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngloRota.Data
{
    public interface IRepository
    {
        void Add<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();

        ///// EMPLOYEES /////
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<Employee> GetEmployeeByEmailAsync(string email);
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);
        Task<IEnumerable<Employee>> GetEmployeesByJobTitleAsync(int jobTitleId);

        ///// ROTA /////
        Task<IEnumerable<RotaData>> GetAllRotaAsync();
        Task<RotaData> GetRotaAsync(int employeeId, DateTime date);
        Task<bool> GetRotaByShiftAsync(Shift shift);

        ///// SHIFTS /////
        Task<IEnumerable<Shift>> GetAllShiftsAsync();
        Task<Shift> GetShiftByNameAsync(string shiftName);
        Task<Shift> GetShiftByIdAsync(int id);

        ///// DEPARTMENTS /////
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByNameAsync(string departmentName);
        Task<Department> GetDepartmentByIdAsync(int id);

        ///// JOB TITLES /////
        Task<IEnumerable<JobTitle>> GetAllJobTitlesAsync();
        Task<JobTitle> GetJobTitleByNameAsync(string jobTitleName);
        Task<JobTitle> GetJobTitleByIdAsync(int id);

        Task<User> GetUserByName(string Name);
    }
}
