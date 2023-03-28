using Dapper;
using GhalibResearch.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.Controllers
{
   // [Authorize]
    public class OrganizationController : Controller
    {

        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddOrEditOrganizationViewModel() { };
            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
            model.OrganizationList = sql.Query<OrganizationModel>("GetOrganizationList", commandType: CommandType.StoredProcedure);
           

            return View(model);

        }

        [HttpPost]
        public async Task<IActionResult> Add(OrganizationModel model)
        {
            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
            if (ModelState.IsValid)
            {

                var template = new
                {
                    model.OrganizationName
                };

                DynamicParameters parameters = new DynamicParameters(template);


                await sql.QueryAsync("AddOrganization", parameters, commandType: CommandType.StoredProcedure);
            }


            var ViewModel = new AddOrEditOrganizationViewModel() { };
            ViewModel.AddModel = model;
            ViewModel.OrganizationList = sql.Query<OrganizationModel>("GetOrganizationList", commandType: CommandType.StoredProcedure);


            return View(ViewModel);

        }

        [HttpGet]
        public IActionResult DeleteOrganization(int id) {
            var template = new {
                OrganizationId = id
            };
            DynamicParameters param = new DynamicParameters(template);

            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
            sql.Query("DeleteOrganization", param, commandType: CommandType.StoredProcedure);
            return RedirectToAction("Add");
        }


        public IActionResult EditOrganization(int id)
        {
            var template = new
            {
                OrganizationId = id
            };
            var ViewModel = new AddOrEditOrganizationViewModel() { };

            DynamicParameters param = new DynamicParameters(template);
            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);

            ViewModel.EditModel=sql.QuerySingle<OrganizationModel>("SelectOrganizationToEdit", param, commandType: CommandType.StoredProcedure);
            ViewModel.OrganizationList = sql.Query<OrganizationModel>("GetOrganizationList", commandType: CommandType.StoredProcedure);




            return View(nameof(Add), ViewModel);

        }

        [HttpPost]
        public IActionResult EditOrganization(OrganizationModel model)
        {
            var template = new
            {
                model.OrganizationId,
                model.OrganizationName

            };

            DynamicParameters param = new DynamicParameters(template);
            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
            sql.Query("EditOrganization",param,commandType: CommandType.StoredProcedure);

            return RedirectToAction("Add");
        }

    }
}