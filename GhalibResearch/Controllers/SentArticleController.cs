using Dapper;
using GhalibResearch.Models;
using GhalibResearch.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ShamisDateTime;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static GhalibResearch.DataAccess.ZTable;

namespace GhalibResearch.Controllers
{
    public class SentArticleController : Controller
    {
     
        public IActionResult Index()
        {
            var model = new AddOrEditSentArticleViewModel { };
            using SqlConnection sql = new(Startup.ConnectionString);
            model.Organization = sql.Query<OrganizationModel>("GetOrganizationList", commandType: CommandType.StoredProcedure);
            model.Article = Article.GetAll().Select(a => new SelectListItem
            {
                Value = a.ArticleId.ToString(),
                Text = a.ArticleTitle.ToString()
            });
            return View(model);
        }
   
        public IActionResult AddSentArticle([FromBody] IEnumerable<SentArticleModel> model)
        {
            DynamicParameters param = new();

            using SqlConnection sql = new(Startup.ConnectionString);
            foreach (var item in model)
            {
                param.Add("OrganizationId", item.OrganizationId);
                param.Add("ArticleId", item.ArticleId);
                item.SentDate = PersianDateTime.Parse(item.SentDateString).ToDateTime();
                param.Add("SentDate", item.SentDate);
                param.Add("CopiesCount", item.CopiesCount);
                param.Add("UserName", User.Identity.Name);

                sql.Query("AddSentArticle", param, commandType: CommandType.StoredProcedure);
            }
            return Json("موفقانه ثبت شد");
        }

        public IActionResult SentArticleList()
        {
            var model = new AddOrEditSentArticleViewModel() { };
            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
            model.SentArticleList = sql.Query<SentArticleListViewModel>("GetSentArticleList", commandType: CommandType.StoredProcedure);

            return View(model);
        }

        public IActionResult DeleteSentArticle(int id)
        {
            var template = new
            {
                SentArticleId = id
            };
            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
            sql.Query("DeleteSentArticle", template,commandType: CommandType.StoredProcedure);

            return RedirectToAction("SentArticleList");
        }

        public IActionResult EditSentArticle(int id)
        {
            var template = new
            {
                SentArticleId = id
            };
            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
            var model = new AddOrEditSentArticleViewModel()
            {
                SentArticle = sql.QuerySingle<SentArticleModel>("SelectSentArticleToEdit", template, commandType: CommandType.StoredProcedure)
            };
            model.Article = Article.GetAll().Select(a => new SelectListItem
            {
                Value = a.ArticleId.ToString(),
                Text = a.ArticleTitle.ToString(),
                Selected =a.ArticleId==model.SentArticle.ArticleId
            });

            return View(model);
        }

     
        public IActionResult EditSentArticlePost([FromBody] IEnumerable<SentArticleModel> model)
        {
            DynamicParameters param = new DynamicParameters();
            foreach (var item in model)
            {

                param.Add("SentArticleId", item.SentArticleId);
                param.Add("ArticleId", item.ArticleId);
                item.SentDate = PersianDateTime.Parse(item.SentDateString).ToDateTime();
                param.Add("SentDate", item.SentDate);
                param.Add("CopiesCount", item.CopiesCount);


                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                sql.Query("EditSentArticle", param, commandType: CommandType.StoredProcedure);
            }
            return RedirectToAction("Index");
           
        }
    }
}
