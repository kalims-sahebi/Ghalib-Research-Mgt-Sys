using Dapper;
using GhalibResearch.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GhalibResearch.DataAccess
{
    public static class ZTable
    {
        public static class ArticleType
        {
            public static IEnumerable<ArticleTypeModel> GetAll()
            {
                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                return sql.Query<ArticleTypeModel>("GetArticleType");
            }

        }

        public static class Department
        {
            public static IEnumerable<DepartmentModel> GetAll()
            {
                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                return sql.Query<DepartmentModel>("GetDepartment");
            }
        }

        public static class Faculty
        {
            public static IEnumerable<FacultyModel> GetAll()
            {
                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                return sql.Query<FacultyModel>("GetFaculty");
            }
        }

        public static class Subject
        {
            public static IEnumerable<SubjectModel> GetAll()
            {
                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                return sql.Query<SubjectModel>("GetSubject");
            }

        }

        public static class Degree
        {
            public static IEnumerable<DegreeModel> GetAll()
            {
                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                return sql.Query<DegreeModel>("GetDegree");
            }

        }

        public static class EducationField
        {
            public static IEnumerable<EducationFieldModel> GetAll()
            {
                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                return sql.Query<EducationFieldModel>("GetEducationField");
            }

        }

        public static class MemberType
        {
            public static IEnumerable<MemberTypeModel> GetAll()
            {
                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                return sql.Query<MemberTypeModel>("GetMemberType");
            }

        }

        public static class Role
        {
            public static IEnumerable<RoleModel> GetAll()
            {
                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                return sql.Query<RoleModel>("GetRole");
            }

        }

        public static class Member
        {
            public static IEnumerable<MemberModel> GetAll()
            {
                using SqlConnection sql = new SqlConnection(Startup.ConnectionString);
                return sql.Query<MemberModel>("GetMember");
            }

        }

        public static class Article
        {
            public static IEnumerable<ArticleModel> GetAll()
            {
                using SqlConnection sql = new(Startup.ConnectionString);
                return sql.Query<ArticleModel>("GetArticleld");
            }

        }
    }
}
