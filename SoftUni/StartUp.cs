using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SoftUni.Data;
using SoftUni.Models;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();
            Console.WriteLine(RemoveTown(context));
        }
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var peopleSalaryOver = context.Employees
                .Where(e => e.Salary > 50000)
                .Select(e => new { e.FirstName, e.Salary })
                .OrderBy(e => e.FirstName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in peopleSalaryOver)
            {
                sb.AppendLine($"{item.FirstName} - {item.Salary:f2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees.Select(e => new { e.FirstName, e.LastName, e.Salary, e.Department })
                .Where(e => e.Department.Name == "Research and Development").OrderBy(e => e.Salary).ThenByDescending(e => e.FirstName);

            var sb = new StringBuilder();
            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} from {item.Department.Name} - ${item.Salary:f2}");
            }
            return sb.ToString().Trim();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address newAddress = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var nakov = context.Employees.FirstOrDefault(e => e.LastName == "Nakov");

            if (nakov != null)
            {
                nakov.Address = newAddress;
                context.SaveChanges();
            }

            var addresses = context.Addresses.Select(e => new { e.AddressId, e.AddressText }).OrderByDescending(e => e.AddressId);
            StringBuilder stringBuilder = new StringBuilder();
            int i = 0;
            foreach (var item in addresses)
            {
                stringBuilder.AppendLine(item.AddressText);
                i++;
                if (i == 10)
                {
                    break;
                }
            }

            return stringBuilder.ToString().Trim();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees.Take(10).Select(e => new
            {
                EmployeeName = $"{e.FirstName} {e.LastName}",
                ManagerName = $"{e.Manager.FirstName} {e.Manager.LastName}",
                Projects = e.EmployeesProjects.Where(ep =>
                ep.Project.StartDate.Year >= 2001 &&
                ep.Project.StartDate.Year <= 2003)
                .Select(ep => new
                {
                    ProjectName = ep.Project.Name,
                    ep.Project.StartDate,
                    EndDate = ep.Project.EndDate.HasValue ?
                    ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") :
                    "not finished"

                })
                .ToList()
            });

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in employees)
            {
                stringBuilder.AppendLine($"{item.EmployeeName} - Manager: {item.ManagerName}");
                if (item.Projects.Any())
                {
                    foreach (var p in item.Projects)
                    {
                        stringBuilder.AppendLine($"--{p.ProjectName} - {p.StartDate:M/d/yyyy h:mm:ss tt} - {p.EndDate}");
                    }
                }
            }
            return stringBuilder.ToString().Trim();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses.Select(a => new
            {
                a.AddressText,
                a.Town.Name,
                EmployeeCount = a.Employees.Where(a => a.AddressId == a.Address.AddressId).Count()
            }).OrderByDescending(a => a.EmployeeCount).ThenBy(a => a.Name).ThenBy(a => a.AddressText).Take(10);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in addresses)
            {
                stringBuilder.AppendLine($"{item.AddressText}, {item.Name} - {item.EmployeeCount} employees");
            }

            return stringBuilder.ToString().Trim();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147 = context.Employees.Select(e => new
            {
                e.EmployeeId,
                e.FirstName,
                e.LastName,
                e.JobTitle,
                Projects = e.EmployeesProjects.Select(ep => new { ProjectName = ep.Project.Name }).ToList()
            }).Where(e => e.EmployeeId == 147).ToList();
            var sb = new StringBuilder();
            foreach (var item in employee147)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} - {item.JobTitle}");
                foreach (var project in item.Projects.OrderBy(p => p.ProjectName).ToList())
                {
                    sb.AppendLine($"{project.ProjectName}");
                }
            }
            return sb.ToString().Trim();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments.Where(d => d.Employees.Count() > 5).Select(d => new
            {
                d.Name,
                ManagerFullName = $"{d.Manager.FirstName} {d.Manager.LastName}",
                EmployeeFullInfo = d.Employees.Select(e => new { e.FirstName, e.LastName, e.JobTitle }).OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList(),
            }).ToList().OrderBy(x => x.EmployeeFullInfo.Count()).ThenBy(x => x.Name);

            var sb = new StringBuilder();
            foreach (var item in departments)
            {
                sb.AppendLine($"{item.Name} - {item.ManagerFullName}");
                foreach (var emp in item.EmployeeFullInfo)
                {
                    sb.AppendLine($"{emp.FirstName} {emp.LastName} - {emp.JobTitle}");
                }
            }

            return sb.ToString().Trim();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects.OrderByDescending(x => x.StartDate).Take(10).Select(p => new
            {
                p.Name,
                p.Description,
                StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt")
            }).OrderBy(x => x.Name);

            var sb = new StringBuilder();
            foreach (var item in projects)
            {
                sb.AppendLine($"{item.Name}\n{item.Description}\n{item.StartDate}");
            }
            return sb.ToString().Trim();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var employees = context.Employees.Where(x => x.Department.Name == "Engineering" ||
            x.Department.Name == "Tool Design" ||
            x.Department.Name == "Marketing" ||
            x.Department.Name == "Information Services").Select(e => new
            {
                e.FirstName,
                e.LastName,
                Salary = ((double)e.Salary) + ((double)e.Salary) * 0.12
            }).OrderBy(x => x.FirstName).ThenBy(x => x.LastName);

            var sb = new StringBuilder();
            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} (${item.Salary:f2})");
            }

            return sb.ToString().Trim();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new { e.FirstName, e.LastName, e.JobTitle, e.Salary })
                .Where(e => e.FirstName.StartsWith("Sa"))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName);
            var sb = new StringBuilder();
            foreach (var item in employees)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} - {item.JobTitle} - (${item.Salary:f2})");
            }
            return sb.ToString().Trim();
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.First(p => p.ProjectId == 2);
            context.EmployeesProjects.ToList().RemoveAll(x => x.ProjectId == 2);
            context.Projects.Remove(project);
            context.SaveChanges();
            var projects = context.Projects.Take(10).Select(p => new { p.Name }).ToList();
            var sb = new StringBuilder();
            foreach (var item in projects)
            {
                sb.AppendLine($"{item.Name}");
            }
            return sb.ToString().Trim();
        }

        public static string RemoveTown(SoftUniContext context)
        {
            var townToDelete = context.Towns.First(x => x.Name == "Seattle");
            IQueryable<Address> addressesToDelete = context.Addresses.Where(a => a.TownId == townToDelete.TownId);
            int count = addressesToDelete.Count();
            IQueryable<Employee> empoloyeesOnAddressToDelete = context.Employees.Where(e => addressesToDelete.Any(a => a.AddressId == e.AddressId));
            foreach (var emp in empoloyeesOnAddressToDelete)
            {
                emp.AddressId = null;
            }
            foreach (var address in addressesToDelete)
            {
                context.Addresses.Remove(address);
            }
            context.Remove(townToDelete);
            context.SaveChanges();
            return $"{count} addresses in Seattle were deleted";

        }
    }

}
