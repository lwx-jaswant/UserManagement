using System.Net;
using API.APIStarterKit.Services;
using Core.Data.Context;
using Core.Data.Models;
using Core.Data.Models.CommonViewModel;
using Core.Data.Models.CompanyInfoViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.APIStarterKit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommonDataAPIController : ControllerBase
    {
        private readonly ICommonService _iCommonService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthenticationAPIController> _logger;

        public CommonDataAPIController(ICommonService iCommonService, ApplicationDbContext context, ILogger<AuthenticationAPIController> logger)
        {
            _iCommonService = iCommonService;
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        [Route("GetDepartmentDropDownData")]
        public ActionResult<List<Department>> GetDepartmentDropDownData()
        {
            var data = _iCommonService.GetDropDownListData<Department>();
            var result = (from tblObj in data.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                          select new ItemDropdownListViewModel
                          {
                              Id = tblObj.Id,
                              Name = tblObj.Name,
                          });
            return Ok(result);
        }
        [HttpGet]
        [Route("GetManageUserRolesDropDownData")]
        public ActionResult<List<ManageUserRoles>> GetManageUserRolesDropDownData()
        {
            var data = _iCommonService.GetDropDownListData<ManageUserRoles>();
            var result = (from tblObj in data.Where(x => x.Cancelled == false).OrderBy(x => x.Id)
                          select new ItemDropdownListViewModel
                          {
                              Id = tblObj.Id,
                              Name = tblObj.Name,
                          });
            return Ok(result);
        }
        [HttpPost]
        [Route("AddUserLoginHistory")]
        public async Task<IActionResult> AddUserLoginHistory(LoginHistory _LoginHistory)
        {
            try
            {
                _LoginHistory.PublicIP = GetPublicIP();
                var result = _context.LoginHistory.Where(x => x.UserName == _LoginHistory.UserName && x.Action == "Login").OrderByDescending(x => x.CreatedDate).Take(1).SingleOrDefault();
                if (result != null && _LoginHistory.Action == "Logout")
                {
                    _LoginHistory.LoginTime = result.LoginTime;
                    _LoginHistory.Duration = (DateTime.Now - result.LoginTime).TotalMinutes;
                }
                _LoginHistory.CreatedDate = DateTime.Now;
                _LoginHistory.ModifiedDate = DateTime.Now;
                _context.Add(_LoginHistory);
                await _context.SaveChangesAsync();
                return Ok(_LoginHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        [Route("GetCompanyInfo/{id}")]
        public async Task<ActionResult<CompanyInfoCRUDViewModel>> GetCompanyInfo(Int64 id)
        {
            var result = await _context.CompanyInfo.FirstOrDefaultAsync(m => m.Id == id);
            return Ok(result);
        }
        [HttpPost]
        [Route("UpdateCompanyInfo")]
        public async Task<IActionResult> UpdateCompanyInfo(CompanyInfoCRUDViewModel vm)
        {
            JsonResultViewModel _JsonResultViewModel = new();
            try
            {
                CompanyInfo _CompanyInfo = await _context.CompanyInfo.FindAsync(vm.Id);
                vm.ModifiedDate = DateTime.Now;
                vm.ModifiedBy = HttpContext.User.Identity.Name;
                _context.Entry(_CompanyInfo).CurrentValues.SetValues(vm);
                await _context.SaveChangesAsync();

                _JsonResultViewModel.AlertMessage = "Company Info Updated Successfully. Company Name: " + _CompanyInfo.Name;
                _JsonResultViewModel.IsSuccess = true;
                return new JsonResult(_JsonResultViewModel);
            }
            catch (Exception ex)
            {
                _JsonResultViewModel.IsSuccess = false;
                return new JsonResult(ex.Message);
                throw;
            }
        }
        public static string GetPublicIP()
        {
            try
            {
                string _URL = "http://checkip.dyndns.org/";
                WebRequest _WebRequest = WebRequest.Create(_URL);              
                WebResponse resp = _WebRequest.GetResponse();
                StreamReader _StreamReader = new StreamReader(resp.GetResponseStream());
                string response = _StreamReader.ReadToEnd().Trim();
                string[] a = response.Split(':');
                string a2 = a[1].Substring(1);
                string[] a3 = a2.Split('<');
                string a4 = a3[0];
                return a4;
            }
            catch (Exception ex)
            {
                return ex.Message;
                throw new Exception("No network adapters with an IPv4 address in the system!");
            }
        }
    }
}
