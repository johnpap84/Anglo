using AngloRota.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngloRota.Data
{
    public class Repository : IRepository
    {
        private AngloRotaContext _ctx;
        private ILogger<AngloRotaContext> _logger;

        public Repository(AngloRotaContext ctx, ILogger<AngloRotaContext> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        public void Add<T>(T entity) where T : class
        {
            _ctx.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _ctx.Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _ctx.SaveChangesAsync()) > 0;
        }

        #region EMPLOYEES
        ///// RETURNS ALL EMPLOYEES /////
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                IQueryable<Employee> employees = _ctx.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .OrderBy(e => e.EmployeeId);

                return await employees.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load All Employees: {ex}");
                return null;
            }

        }

        ///// RETURNS 1 EMPLOYEE SEARCHING BY ID /////
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            try
            {
                IQueryable<Employee> employeeById = _ctx.Employees
                .Where(e => e.EmployeeId == id)
                .Include(e => e.Department)
                .Include(e => e.JobTitle);

                return await employeeById.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load the Employee by Id: {id}: {ex}");
                return null;
            }

        }

        ///// RETURNS 1 EMPLOYEE SEARCHING BY EMAIL /////
        public async Task<Employee> GetEmployeeByEmailAsync(string email)
        {
            try
            {
                IQueryable<Employee> employeeByEmail = _ctx.Employees
                    .Where(e => e.Email.Equals(email))
                    .Include(e => e.Department)
                    .Include(e => e.JobTitle);

                return await employeeByEmail.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load the Employee by Email: {email}: {ex}");
                return null;
            }
        }
        
        ///// RETURNS ALL EMPLOYEES FROM A DEPARTMENT /////
        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            try
            {
                IQueryable<Employee> employeesByDepartment = _ctx.Employees
                    .Where(e => e.Department.DepartmentId == departmentId)
                    .Include(e => e.Department)
                    .Include(e => e.JobTitle);

                return await employeesByDepartment.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load all the Employees from Department: {departmentId}: {ex}");
                return null;
            }
        }
        
        ///// RETURNS ALL EMPLOYEES BY THEIR JOB TITLE /////
        public async Task<IEnumerable<Employee>> GetEmployeesByJobTitleAsync(int jobTitleId)
        {
            try
            {
                IQueryable<Employee> employeesByJobTitle = _ctx.Employees
                    .Where(e => e.JobTitle.JobTitleId == jobTitleId)
                    .Include(e => e.Department)
                    .Include(e => e.JobTitle);

                return await employeesByJobTitle.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load all the Employees with Job Title Id: {jobTitleId}: {ex}");
                return null;
            }
        }
        #endregion
        
        #region ROTA DATA 
        ///// RETURNS ALL ROTA DATA /////
        public async Task<IEnumerable<RotaData>> GetAllRotaAsync()
        {
            try
            {
                IQueryable<RotaData> allRota = _ctx.RotaData
                    .Include(r => r.RotaForEmployee)
                    .Include(r => r.Shift)
                    .OrderBy(r => r.RotaForEmployee.EmployeeId);

                return await allRota.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load all the Rota Data: {ex}");
                return null;
            }
        }

        ///// RETURNS ROTA DATA SEARCHING EMPLOYEE ID, DATE //////
        public async Task<RotaData> GetRotaAsync(int employeeId, DateTime date)
        {
            try
            {
                IQueryable<RotaData> rotaData = _ctx.RotaData
                    .Where(r => r.RotaForEmployee.EmployeeId == employeeId)
                    .Where(r => r.Date == date);

                return await rotaData.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load all the Rota Data for Employee: {employeeId} and for: {date}: {ex}");
                return null;
            }
        }

        ///// RETURNS TRUE IF SHIFT WAS FOUND ANYWHERE IN ROTADATA ////
        public async Task<bool> GetRotaByShiftAsync(Shift shift)
        {
            try
            {
                IQueryable<RotaData> rotaDataByShift = _ctx.RotaData.Where(r => r.Shift.Equals(shift));

                if (await rotaDataByShift.FirstOrDefaultAsync() == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to check if Shift: {shift.ShiftName} used in Rota Data: {ex}");
                return false;
            }
            
        }
        #endregion

        #region SHIFTS
        ///// RETURNS ALL SHIFTS /////
        public async Task<IEnumerable<Shift>> GetAllShiftsAsync()
        {
            try
            {
                IQueryable<Shift> allShifts = _ctx.Shifts.OrderBy(s => s.ShiftId);

                return await allShifts.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load All Shifts: {ex}");
                return null;
            }
            
        }

        ///// RETURNS A SHIFT SEARCHING BY NAME /////
        public async Task<Shift> GetShiftByNameAsync(string shiftName)
        {
            try
            {
                IQueryable<Shift> shiftByName = _ctx.Shifts
                    .Where(s => s.ShiftName.Equals(shiftName));

                return await shiftByName.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load the Shift with name: {shiftName}: {ex}");
                return null;
            }
        }
        
        ///// RETURNS A SHIFT SEARCHING BY ID ////
        public async Task<Shift> GetShiftByIdAsync(int id)
        {
            try
            {
                IQueryable<Shift> shiftById = _ctx.Shifts.Where(s => s.ShiftId == id);

                return await shiftById.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load the Shift with Id: {id}: {ex}");
                return null;
            }
        }
        #endregion
        
        #region DEPARTMENTS
        ///// RETURNS ALL DEPARTMENTS /////
        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            try
            {
                IQueryable<Department> allDepartment = _ctx.Departments
                .Include(d => d.JobTitles)
                .Include(d => d.EmployeesInDepartment)
                .OrderBy(d => d.DepartmentId);

                return await allDepartment.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load all Departments: {ex}");
                return null;
            } 
        }
        
        ///// RETURNS 1 DEPARTMENT SEACHING BY DEPARTMNET NAME /////
        public async Task<Department> GetDepartmentByNameAsync(string departmentName)
        {
            try
            {
                IQueryable<Department> departmentByName = _ctx.Departments
                    .Where(d => d.Name.Equals(departmentName))
                    .Include(d => d.EmployeesInDepartment)
                    .Include(d => d.JobTitles);

                return await departmentByName.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load the Department by Name:{departmentName}: {ex}");
                return null;
            }
            
        }
        
        ///// RETURNS 1 DEPARTMENT SEARCHING BY ID /////
        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            try
            {
                IQueryable<Department> departmentById = _ctx.Departments
                    .Where(d => d.DepartmentId == id)
                    .Include(d => d.JobTitles)
                    .Include(d => d.EmployeesInDepartment);

                return await departmentById.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load the Department by Id: {id}: {ex}");
                return null;
            }
        }
        #endregion
        
        #region JOB TITLES
        ///// RETURNS ALL JOB TITLES /////
        public async Task<IEnumerable<JobTitle>> GetAllJobTitlesAsync()
        {
            try
            {
                IQueryable<JobTitle> allJobTitles = _ctx.JobTitles
                    .Include(j => j.Department)
                    .Include(j => j.EmployeesInJob)
                    .OrderBy(j => j.JobTitleId);

                return await allJobTitles.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load all the job Titles: {ex}");
                return null;
            }
            
        }

        ///// RETURNS 1 JOB TITLE SEARCHING BY NAME /////
        public async Task<JobTitle> GetJobTitleByNameAsync(string jobTitleName)
        {
            try
            {
                IQueryable<JobTitle> jobTitleByName = _ctx.JobTitles
                    .Where(j => j.Name.Equals(jobTitleName))
                    .Include(j => j.Department)
                    .Include(j => j.EmployeesInJob);

                return await jobTitleByName.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load the job Title by name: {jobTitleName}: {ex}");
                return null;
            }
            
        }

        ///// RETURNS 1 JOB TITLE SEARCHING BY ID /////
        public async Task<JobTitle> GetJobTitleByIdAsync(int id)
        {
            try
            {
                IQueryable<JobTitle> jobTitleById = _ctx.JobTitles
                    .Where(j => j.JobTitleId == id)
                    .Include(j => j.Department)
                    .Include(j => j.EmployeesInJob);

                return await jobTitleById.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to load the job Title by Id: {id}: {ex}");
                return null;
            }
            
        }
        #endregion

        
        ///// RETURNS USER FOR USERMANAGER /////
        public async Task<User> GetUserByName(string Name)
        {
            IQueryable<User> user = _ctx.Users.Where(u => u.UserName.Equals(Name));

            return await user.FirstOrDefaultAsync();

            //return await _ctx.Users.Where(u => u.UserName.Equals(Name)).FirstOrDefaultAsync();
        }
    }
}
