using AngloRota.Data.Entities;
using AngloRota.Models;
using AutoMapper;

namespace AngloRota.Data
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            CreateMap<Employee, EmployeeModel>()
                .ForMember(e => e.Name, o => o.MapFrom(m => m.EmployeeName))
                .ForPath(e => e.Department, o => o.MapFrom(m => m.Department.Name))
                .ForPath(e => e.JobTitle, o => o.MapFrom(m => m.JobTitle.Name));
            
            CreateMap<EmployeeModel, Employee>()
                .ForMember(e => e.EmployeeName, o => o.MapFrom(m => m.Name))
                .ForPath(e => e.Department.Name, o => o.MapFrom(m => m.Department))
                .ForPath(e => e.JobTitle.Name, o => o.MapFrom(m => m.JobTitle));
                
            CreateMap<Department, DepartmentModel>()
                .ForMember(d => d.DepartmentName, o => o.MapFrom(m => m.Name))
                .ReverseMap();

            CreateMap<JobTitle, JobTitleModel>()
                .ForMember(j => j.Id, o => o.MapFrom(m => m.JobTitleId))
                .ForMember(j => j.JobTitleName, o => o.MapFrom(m => m.Name))
                .ForPath(j => j.InDepartment, o => o.MapFrom(m => m.Department.Name))
                .ForPath(j => j.EmployeesInJob, o => o.MapFrom(m => m.EmployeesInJob))
                .ReverseMap();
            
            CreateMap<Shift, ShiftModel>().ReverseMap();

            CreateMap<RotaData, RotaDataModel>()
                .ForMember(d => d.RotaId, o => o.MapFrom(m => m.RotaId))
                .ForMember(d => d.Date, o => o.MapFrom(m => m.Date))
                .ForPath(d => d.EmployeeId, o => o.MapFrom(m => m.RotaForEmployee.EmployeeId))
                .ForPath(d => d.EmployeeName, o => o.MapFrom(m => m.RotaForEmployee.EmployeeName))
                .ForPath(d => d.ShiftName, o => o.MapFrom(m => m.Shift.ShiftName))
                .ForPath(d => d.DurationInMins, o => o.MapFrom(m => m.Shift.DurationInMins))
                .ReverseMap();

            CreateMap<User, UserModel>().ReverseMap();
        }
    }
}
