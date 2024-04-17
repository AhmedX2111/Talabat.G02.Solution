using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications.Employee_specs;


namespace Talabat.APIS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : BaseApiController
    {
        private readonly IGenericRepository<Employee> _employeeRepo;

        public EmployeeController(IGenericRepository<Employee> employeeRepo)
        {
            _employeeRepo = employeeRepo;
        }

        [HttpGet] // Get : /api/Employees
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var spec = new EmployeeWithDepartmentSpecifications();

            var employees = await _employeeRepo.GetAllWithSpecAsync(spec);

            return Ok(employees);
        }

        [HttpGet("{id}")] // Get : /api/Employees/1
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var spec = new EmployeeWithDepartmentSpecifications(id);

            var employee = await _employeeRepo.GetWithSpecAsync(spec);

            if (employee is null)
                return NotFound();

            return Ok(employee);
        }


    }
}
