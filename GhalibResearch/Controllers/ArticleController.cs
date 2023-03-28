using Dapper;
using GhalibResearch.Models;
using GhalibResearch.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using ShamisDateTime;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static GhalibResearch.DataAccess.ZTable;

namespace GhalibResearch.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        #region Clear

        private IWebHostEnvironment Environment;

        public ArticleController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }
        [HttpGet]
        public IActionResult Add()
        {
            AddOrEditArticleViewModel ViewModel = new()
            {
                ArticleTypeList = ArticleType.GetAll().Select(a => new SelectListItem
                {
                    Value = a.ArticleTypeId.ToString(),
                    Text = a.ArticleTypeDari.ToString()
                }),
                FacultyList = Faculty.GetAll().Select(f => new SelectListItem
                {
                    Value = f.FacultyId.ToString(),
                    Text = f.FacultyName.ToString()
                }).Prepend(new SelectListItem
                {
                    Value = "0",
                    Text = " -- انتخاب پوهنځی -- "
                }),
                /*SubjectList = Subject.GetAll().Select(s => new SelectListItem
                {
                    Value = s.SubjectId.ToString(),
                    Text = s.SubjectName.ToString()
                })
                .Prepend(new SelectListItem
                {
                    Value = "0",
                    Text = " -- انتخاب مضمون -- "
                }),*/
                Degree = Degree.GetAll().Select(s => new SelectListItem
                {
                    Value = s.DegreeId.ToString(),
                    Text = s.DegreeDari.ToString()
                }),
                EducationField = EducationField.GetAll().Select(s => new SelectListItem
                {
                    Value = s.EducationFieldId.ToString(),
                    Text = s.EducationFieldDari
                }),
                MemberType = MemberType.GetAll().Select(s => new SelectListItem
                {
                    Value = s.MemberTypeId.ToString(),
                    Text = s.MemberTypeDari.ToString()
                }),
                Role = Role.GetAll().Select(s => new SelectListItem
                {
                    Value = s.RoleId.ToString(),
                    Text = s.RoleDari.ToString()
                }),
                Member = Member.GetAll().Select(s => new SelectListItem
                {
                    Value = s.MemberId.ToString(),
                    Text = s.FullName.ToString(),
                })
            };
            using SqlConnection sql = new(Startup.ConnectionString);
            var d = sql.Query<DepartmentModel>("dbo.GetDepartmentByFaculty", new { FacultyId = ViewModel.FacultyList.FirstOrDefault().Value }, commandType: CommandType.StoredProcedure);

            ViewModel.DepartmentList = d.Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName.ToString()
            }).Prepend( new SelectListItem 
            {
                Value = "0",
                Text = "-- انتخاب دیپارتمنت --" 
            });
            return View(ViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ArticleModel model, IFormCollection Collection)
        {
            if (!string.IsNullOrEmpty(model.EndingYearString))
                model.EndingYear = PersianDateTime.Parse(model.EndingYearString).ToDateTime();
            if (!string.IsNullOrEmpty(model.StartingYearString))
                model.StartingYear = PersianDateTime.Parse(model.StartingYearString).ToDateTime();
            if (!string.IsNullOrEmpty(model.AcademicCouncilProtocolDateString))
                model.AcademicCouncilProtocolDate = PersianDateTime.Parse(model.AcademicCouncilProtocolDateString).ToDateTime();
            if (!string.IsNullOrEmpty(model.ResearchComitteeProtocolDateString))
                model.ResearchComitteeProtocolDate = PersianDateTime.Parse(model.ResearchComitteeProtocolDateString).ToDateTime();

            string wwwPath = this.Environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathType = Path.Combine(path, model.ArticleTypeId.ToString());
            string filepath = Path.Combine(pathType, model.PublishedYearString);

            //string[] dtxt = model.StartingYearString.Split('/');

           // string pathYear = Path.Combine(pathType, dtxt[0], dtxt[1], dtxt[2]);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!Directory.Exists(pathType))
            {
                Directory.CreateDirectory(pathType);
            }

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            if (model.AbstractFile != null)
            {
                string AbstractfileName = Path.GetFileName(model.AbstractFile.FileName);
                using FileStream stream = new(Path.Combine(filepath, AbstractfileName), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                model.AbstractFile.CopyTo(stream);
                model.AbstractFileString = Path.Combine("\\Uploads", model.ArticleTypeId.ToString(), model.PublishedYearString, Path.GetFileName(model.AbstractFile.FileName));
            }

            if (model.DocumentFile != null)
            {
                string DocumentFileName = Path.GetFileName(model.DocumentFile.FileName);
                using FileStream stream1 = new(Path.Combine(filepath, DocumentFileName), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                model.DocumentFile.CopyTo(stream1);
                model.DocumentFileString = Path.Combine("\\Uploads", model.ArticleTypeId.ToString(), model.PublishedYearString, Path.GetFileName(model.DocumentFile.FileName));
            }

            using SqlConnection sql = new(Startup.ConnectionString);

            var template = new
            {
                model.AbstractFileString,
                model.DocumentFileString,
                model.ArticleTitle,
                model.ArticleTypeId,
                model.DepartmentId,
                model.EndingYear,
                model.PublishedYearString,
                model.AcademicCouncilProtocolNumber,
                model.AcademicCouncilProtocolDate,
                model.PublishedInJournal,
                model.Budget,
                model.StartingYear,
                model.FacultyId,
                model.EthicCode,
                model.ResearchComitteeProtocolNumber,
                model.ResearchComitteeProtocolDate,
                model.ChapterStartingNumber
            };

            DynamicParameters parameters = new(template);

            // ArticleMember Table
            int i = 0;
            var dt0 = new DataTable();
            dt0.Columns.Add("MemberId", typeof(int));
            dt0.Columns.Add("MemberTypeId", typeof(int));
            dt0.Columns.Add("RoleId", typeof(int));

            while (Collection.Where(s => s.Key == "Member[" + i + "][MemberId]").Any())
            {
                var row = dt0.NewRow();
                row["MemberId"] = Convert.ToInt32(Collection["Member[" + i + "][MemberId]"]);
                row["MemberTypeId"] = Convert.ToInt32(Collection["Member[" + i + "][MemberTypeId]"]);
                row["RoleId"] = Convert.ToInt32(Collection["Member[" + i + "][RoleId]"]);

                dt0.Rows.Add(row);
                i++;
            }
            parameters.Add("MemberTable", dt0.AsTableValuedParameter("tvpMember"));
            i = 0; dt0.Dispose();
            // End Of ArticleMember Table

            parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

            await sql.QueryAsync("AddArticle", parameters, commandType: CommandType.StoredProcedure);
            TempData["Message"] = parameters.Get<string>("Msg");
            return RedirectToAction("Add");

            /*else
            {
                AddOrEditArticleViewModel temp = new ();
                temp.AddModel = model;
                return View(temp);
            }*/
        }

        public IActionResult AddMember()
        {
            var ViewModel = new AddOrEditArticleViewModel()
            {
                FacultyList = Faculty.GetAll().Select(f => new SelectListItem
                {
                    Value = f.FacultyId.ToString(),
                    Text = f.FacultyName.ToString()
                }),

                Degree = Degree.GetAll().Select(s => new SelectListItem
                {
                    Value = s.DegreeId.ToString(),
                    Text = s.DegreeDari.ToString()
                }),
                EducationField = EducationField.GetAll().Select(s => new SelectListItem
                {
                    Value = s.EducationFieldId.ToString(),
                    Text = s.EducationFieldDari
                }),
                MemberType = MemberType.GetAll().Select(s => new SelectListItem
                {
                    Value = s.MemberTypeId.ToString(),
                    Text = s.MemberTypeDari.ToString()
                }),
                Role = Role.GetAll().Select(s => new SelectListItem
                {
                    Value = s.RoleId.ToString(),
                    Text = s.RoleDari.ToString()
                }),
                Member = Member.GetAll().Select(s => new SelectListItem
                {
                    Value = s.MemberId.ToString(),
                    Text = s.FullName.ToString(),
                }),
            };

            return View(ViewModel);
        }

        //Modal post method

        [HttpPost]
        public async Task<IActionResult> AddMember([FromBody] MemberModel model)
        {
            if (ModelState.IsValid)
            {
                var template = new
                {
                    model.FullName,
                    model.Email,
                    model.PhoneNo,
                    model.EducationFieldId,
                    model.DegreeId,
                    model.FacultyId
                };
                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                DynamicParameters parameters = new DynamicParameters(template);
                await sql.QueryAsync("AddMember", parameters, commandType: CommandType.StoredProcedure);
                return RedirectToAction("AddMember");
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult List(int? id)
        {
            var ViewModel = new AddOrEditArticleViewModel() { };
            using SqlConnection sql = new(Startup.ConnectionString);
            ViewModel.ArticleList = sql.Query<ArticleListViewModel>("dbo.GetArticleList", new { ArticleTypeId = id }, commandType: CommandType.StoredProcedure);
            ViewModel.ArticleTypeDari = sql.Query<string>("dbo.GetArticleTypeDari", new { ArticleTypeId = id }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            return View(ViewModel);
        }


        public IActionResult DeleteArticle(int id)
        {
            var template = new
            {
                ArticleId = id
            };

            //deleting physical files from path
            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
            DynamicParameters parameters0 = new DynamicParameters(new { ArticleId = id });
            var delete = new ArticleDeleteModel() { };
            delete = sql.Query<ArticleDeleteModel>("SelectFilesNameToDelete", parameters0, commandType: CommandType.StoredProcedure).SingleOrDefault();
            string wwwPath = this.Environment.WebRootPath;
            string DocumentFile = wwwPath + delete.DocumentFile;
            if (System.IO.File.Exists(DocumentFile))
            {
                System.IO.File.Delete(DocumentFile);
            }

            string AbstracFile = wwwPath + delete.AbstractFile;
            if (System.IO.File.Exists(AbstracFile))
            {
                System.IO.File.Delete(AbstracFile);
            }

            //deleting recored from database
            DynamicParameters parameters = new DynamicParameters(template);
            sql.Query("DeleteArticle", parameters, commandType: CommandType.StoredProcedure);
            return RedirectToAction("List");
        }



        [HttpGet]
        public IActionResult EditArticle(int id)
        {
            var template = new
            {
                ArticleId = id
            };

            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
            DynamicParameters parameters = new DynamicParameters(template);

            var ViewModel = new AddOrEditArticleViewModel()
            {
                ArticleEditModel = sql.QuerySingle<ArticleModel>("SelectArticleToEdit", parameters, commandType: CommandType.StoredProcedure)
            };
            ViewModel.ArticleMemberList = sql.Query<ArticleMember>("SelectArticleMembersToEdit", parameters, commandType: CommandType.StoredProcedure);
           /* if (ViewModel.ArticleEditModel.EndingYearString != null)
                ViewModel.ArticleEditModel.EndingYear = PersianDateTime.Parse(ViewModel.ArticleEditModel.EndingYearString).ToDateTime();
            if (ViewModel.ArticleEditModel.StartingYearString != null)
                ViewModel.ArticleEditModel.StartingYear = PersianDateTime.Parse(ViewModel.ArticleEditModel.StartingYearString).ToDateTime();
            if (ViewModel.ArticleEditModel.AcademicCouncilProtocolDateString != null)
                ViewModel.ArticleEditModel.AcademicCouncilProtocolDate = PersianDateTime.Parse(ViewModel.ArticleEditModel.AcademicCouncilProtocolDateString).ToDateTime();
            if (ViewModel.ArticleEditModel.ResearchComitteeProtocolDateString != null)
                ViewModel.ArticleEditModel.ResearchComitteeProtocolDate = PersianDateTime.Parse(ViewModel.ArticleEditModel.ResearchComitteeProtocolDateString).ToDateTime();
*/
            ViewModel.ArticleTypeList = ArticleType.GetAll().Select(a => new SelectListItem
            {
                Value = a.ArticleTypeId.ToString(),
                Text = a.ArticleTypeDari.ToString(),
                Selected = a.ArticleTypeId == ViewModel.ArticleEditModel.ArticleTypeId
            });
            ViewModel.DepartmentList = Department.GetAll().Select(d => new SelectListItem
            {
                Value = d.DepartmentId.ToString(),
                Text = d.DepartmentName.ToString(),
                Selected = d.DepartmentId == ViewModel.ArticleEditModel.DepartmentId
            });
            ViewModel.FacultyList = Faculty.GetAll().Select(d => new SelectListItem
            {
                Value = d.FacultyId.ToString(),
                Text = d.FacultyName,
                Selected = d.FacultyId == ViewModel.ArticleEditModel.FacultyId
            });

            ViewModel.Degree = Degree.GetAll().Select(s => new SelectListItem
            {
                Value = s.DegreeId.ToString(),
                Text = s.DegreeDari
            });
            ViewModel.EducationField = EducationField.GetAll().Select(s => new SelectListItem
            {
                Value = s.EducationFieldId.ToString(),
                Text = s.EducationFieldDari
            });

            foreach (var item in ViewModel.ArticleMemberList)
            {
                ViewModel.MemberType = MemberType.GetAll().Select(s => new SelectListItem
                {
                    Value = s.MemberTypeId.ToString(),
                    Text = s.MemberTypeDari.ToString(),
                    Selected = s.MemberTypeId == item.MemberTypeId
                });
                ViewModel.Role = Role.GetAll().Select(s => new SelectListItem
                {
                    Value = s.RoleId.ToString(),
                    Text = s.RoleDari.ToString(),
                    Selected = s.RoleId == item.RoleId
                });
                ViewModel.Member = Member.GetAll().Select(s => new SelectListItem
                {
                    Value = s.MemberId.ToString(),
                    Text = s.FullName.ToString(),
                    Selected = s.MemberId == item.MemberId
                });
            }

            ViewModel.ArticleEditModel.AbstractFileString = Path.Combine(this.Environment.WebRootPath, ViewModel.ArticleEditModel.AbstractFileString);
            ViewModel.ArticleEditModel.DocumentFileString = Path.Combine(this.Environment.WebRootPath, ViewModel.ArticleEditModel.DocumentFileString);
            return View(ViewModel);
        }

        [HttpPost]
        public IActionResult EditArticle(ArticleModel model)
        {
            
            model.EndingYear = PersianDateTime.Parse(model.EndingYearString).ToDateTime();
            model.AcademicCouncilProtocolDate = PersianDateTime.Parse(model.AcademicCouncilProtocolDateString).ToDateTime();
            model.StartingYear = PersianDateTime.Parse(model.StartingYearString).ToDateTime();

            //Deleting Old Document File
            if (model.DocumentFile != null)
            {
                using SqlConnection sqltemp = new SqlConnection(Startup.ConnectionString);
                DynamicParameters parameters = new DynamicParameters(new { ArticleId = model.ArticleId });
                var delete = new ArticleDeleteModel() { };
                delete = sqltemp.Query<ArticleDeleteModel>("SelectFilesNameToDelete", parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();

                string wwwPath = this.Environment.WebRootPath;
                string DocumentFile = wwwPath + delete.DocumentFile;

                if (System.IO.File.Exists(DocumentFile))
                {
                    System.IO.File.Delete(DocumentFile);
                }



               
                string path = Path.Combine(wwwPath, "Uploads");
                string pathType = Path.Combine(path, model.ArticleTypeId.ToString());
                string filepath = Path.Combine(pathType, model.PublishedYearString);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!Directory.Exists(pathType))
                {
                    Directory.CreateDirectory(pathType);
                }

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                string DocumentFileName = Path.GetFileName(model.DocumentFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(filepath, DocumentFileName), FileMode.Create))
                {
                    model.DocumentFile.CopyTo(stream);
                }

                model.DocumentFileString = Path.Combine("\\Uploads", model.ArticleTypeId.ToString(), model.PublishedYearString, Path.GetFileName(model.DocumentFile.FileName));
                model.AbstractFileString = delete.AbstractFile;
            }
            else
            {
                using SqlConnection sqltemp = new SqlConnection(Startup.ConnectionString);
                DynamicParameters parameters = new DynamicParameters(new { ArticleId = model.ArticleId });
                var delete = new ArticleDeleteModel() { };
                delete = sqltemp.Query<ArticleDeleteModel>("SelectFilesNameToDelete", parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
                model.DocumentFileString = delete.DocumentFile;
                model.AbstractFileString = delete.AbstractFile;
            }

            //Deleting Old Abstract File
            if (model.AbstractFile != null)
            {
                using SqlConnection sqltemp = new SqlConnection(Startup.ConnectionString);
                DynamicParameters parameters = new DynamicParameters(new { ArticleId = model.ArticleId });
                var delete = new ArticleDeleteModel() { };
                delete = sqltemp.Query<ArticleDeleteModel>("SelectFilesNameToDelete", parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();

                string wwwPath = this.Environment.WebRootPath;
                string AbstractFile = wwwPath + delete.AbstractFile;

                if (System.IO.File.Exists(AbstractFile))
                {
                    System.IO.File.Delete(AbstractFile);
                }

                //String new Abstract File file
                string path = Path.Combine(wwwPath, "Uploads");
                string pathType = Path.Combine(path, model.ArticleTypeId.ToString());
                string filepath = Path.Combine(pathType, model.PublishedYearString);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (!Directory.Exists(pathType))
                {
                    Directory.CreateDirectory(pathType);
                }

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }

                string AbstractFileName = Path.GetFileName(model.AbstractFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(filepath, AbstractFileName), FileMode.Create))
                {
                    model.AbstractFile.CopyTo(stream);
                }

                model.AbstractFileString = Path.Combine("\\Uploads", model.ArticleTypeId.ToString(), model.PublishedYearString, Path.GetFileName(model.AbstractFile.FileName));
                model.DocumentFileString = delete.DocumentFile;
            }
            else
            {
                using SqlConnection sqltemp = new SqlConnection(Startup.ConnectionString);
                DynamicParameters parameters = new DynamicParameters(new { ArticleId = model.ArticleId });
                var delete = new ArticleDeleteModel() { };
                delete = sqltemp.Query<ArticleDeleteModel>("SelectFilesNameToDelete", parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
                // model.DocumentFileString = delete.DocumentFile;
                model.AbstractFileString = delete.AbstractFile;
            }

            var template = new
            {
                model.AbstractFileString,
                model.DocumentFileString,
                model.ArticleId,
                model.ArticleTitle,
                model.ArticleTypeId,
                model.ChapterStartingNumber,
                model.DepartmentId,
                model.EndingYear,
                model.AcademicCouncilProtocolDate,
                model.AcademicCouncilProtocolNumber,
                model.ResearchComitteeProtocolDate,
                model.ResearchComitteeProtocolNumber,
                model.PublishedInJournal,
                model.PublishedYearString,
                model.StartingYear,
                model.MemberTypeId,
                model.RoleId,
                model.MemberId,
                model.FacultyId
            };

            DynamicParameters param = new DynamicParameters(template);
            using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
            sql.Query("EditArticle", param, commandType: CommandType.StoredProcedure);

           return RedirectToAction("List");
           
        }

        // Called on Ajax Request
        [HttpPost]
        public IActionResult GetSubjectList()
        {
            return Json(Subject.GetAll()
                .Select(s => new SelectListItem
                {
                    Value = s.SubjectId.ToString(),
                    Text = s.SubjectName
                }));
        }

        [HttpPost]
        public IActionResult GetMemberList()
        {
            return Json(Member.GetAll()
                .Select(m => new SelectListItem
                {
                    Value = m.MemberId.ToString(),
                    Text = m.FullName
                }));
        }

        [HttpPost]
        public IActionResult GetDepartmentByFaculty([FromBody] FacultyModel f)
        {
            using SqlConnection sql = new(Startup.ConnectionString);
            return Json(sql.Query<DepartmentModel>("dbo.GetDepartmentByFaculty", new { f.FacultyId }, commandType: CommandType.StoredProcedure)
                .Select(m => new SelectListItem
                {
                    Value = m.DepartmentId.ToString(),
                    Text = m.DepartmentName
                }));
        }

        [HttpPost]
        public async Task<IActionResult> AddSubject([FromBody] SubjectModel model)
        {
            if (ModelState.IsValid)
            {
                var template = new
                {
                    model.SubjectName
                };
                using SqlConnection sql = new(Startup.ConnectionString);
                DynamicParameters parameters = new(template);
                await sql.QueryAsync("AddSubject", parameters, commandType: CommandType.StoredProcedure);

                return Json(new { Message = "موفقانه ثبت شد." });
            }
            else
            {
                return Json(new { Message = "معلومات را کامل درج نمایید." });
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddFaculty([FromBody] FacultyModel model)
        {
            if (ModelState.IsValid)
            {
                var template = new
                {
                    model.FacultyName,
                    model.FacultyNameEnglish
                };
                using SqlConnection sql = new(Startup.ConnectionString);
                DynamicParameters parameters = new(template);
                await sql.QueryAsync("AddFaculty", parameters, commandType: CommandType.StoredProcedure);

                return Json(new { Message = "موفقانه ثبت شد." });
            }
            else
            {

                return Json(new { Message = "معلومات را کامل درج نمایید." });
            }
        }

        [HttpPost]
        public IActionResult GetFacultyList()
        {
            return Json(Faculty.GetAll()
                .Select(s => new SelectListItem
                {
                    Value = s.FacultyId.ToString(),
                    Text = s.FacultyName
                }));
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentModel model)
        {
            string message = null;
            if (ModelState.IsValid)
            {
                var template = new
                {
                    model.DepartmentName,
                    model.FacultyId
                };

                using SqlConnection sql = new(Startup.ConnectionString);
                DynamicParameters parameters = new(template);
                parameters.Add("@Msg", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);
                await sql.QueryAsync("AddDepartment", parameters, commandType: CommandType.StoredProcedure);
                message = parameters.Get<string>("Msg");
                return Json(new { Message = message });
            }
            else
            {
                if (message != null)
                {
                    return Json(new { Message = message });
                }
                return Json(new { Message = "معلومات را کامل درج نمایید." });
            }
        }

        [HttpPost]
        public IActionResult GetDepartmentList()
        {
            return Json(Department.GetAll()
                .Select(s => new SelectListItem
                {
                    Value = s.DepartmentId.ToString(),
                    Text = s.DepartmentName
                }));
        }

        #endregion Clear

        public IActionResult Detail(int id)
        {
            var template = new
            {
                ArticleId = id
            };

            using SqlConnection sql = new(Startup.ConnectionString);
            DynamicParameters parameters = new(template);
            var model = new ArticleDetailViewModel() { };
            model = sql.Query<ArticleDetailViewModel>("dbo.GetArticleDetail", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
           // model.ArticleMemberList = sql.Query<MemberDetailViewModel>("GetArticleMemberDetail", parameters, commandType: CommandType.StoredProcedure);
            model.AbstractFile = Path.Combine(this.Environment.WebRootPath, model.AbstractFile);
            model.DocumentFile = Path.Combine(this.Environment.WebRootPath, model.DocumentFile);

            model.StartingYearString = new PersianDateTime(model.StartingYear).ToShortDateString();
            model.EndingYearString = new PersianDateTime(model.EndingYear).ToShortDateString();
            model.AcademicCouncilProtocolDateString = new PersianDateTime(model.AcademicCouncilProtocolDate).ToShortDateString();
            model.ResearchComitteeProtocolDateString = new PersianDateTime(model.ResearchComitteeProtocolDate).ToShortDateString();


            return View(model);
        }

        [HttpGet]
        public IActionResult ResearchProjectRPT()
        {
            var ViewModel = new AddOrEditArticleViewModel() { };
            using SqlConnection sql = new(Startup.ConnectionString);
            ViewModel.ArticleList = sql.Query<ArticleListViewModel>("dbo.ResearchProjectRPT", commandType: CommandType.StoredProcedure);
            ViewModel.ArticleTypeDari = sql.Query<string>("dbo.GetArticleTypeDari", new { ArticleTypeId = 10 }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            return View(ViewModel);
        }
        [HttpGet]
        public IActionResult GhalibJournalArticleRPT()
        {
            var ViewModel = new AddOrEditArticleViewModel() { };
            using SqlConnection sql = new(Startup.ConnectionString);
            ViewModel.ArticleList = sql.Query<ArticleListViewModel>("dbo.GhalibJournalArticleRPT", commandType: CommandType.StoredProcedure);
            ViewModel.ArticleTypeDari = sql.Query<string>("dbo.GetArticleTypeDari", new { ArticleTypeId = 11 }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            return View(ViewModel);
        }

        [HttpGet]
        public IActionResult ExternalArticleRPT()
        {
            var ViewModel = new AddOrEditArticleViewModel() { };
            using SqlConnection sql = new(Startup.ConnectionString);
            ViewModel.ArticleList = sql.Query<ArticleListViewModel>("dbo.ExternalArticleRPT", commandType: CommandType.StoredProcedure);
            ViewModel.ArticleTypeDari = sql.Query<string>("dbo.GetArticleTypeDari", new { ArticleTypeId = 13 }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            return View(ViewModel);
        }
        [HttpGet]
        public IActionResult GhalibQuarterlyJournalRPT()
        {
            var ViewModel = new AddOrEditArticleViewModel() { };
            using SqlConnection sql = new(Startup.ConnectionString);
            ViewModel.ArticleList = sql.Query<ArticleListViewModel>("dbo.GhalibQuarterlyJournalRPT", commandType: CommandType.StoredProcedure);
            ViewModel.ArticleTypeDari = sql.Query<string>("dbo.GetArticleTypeDari", new { ArticleTypeId = 12 }, commandType: CommandType.StoredProcedure).SingleOrDefault();
            return View(ViewModel);
        }
    }

}