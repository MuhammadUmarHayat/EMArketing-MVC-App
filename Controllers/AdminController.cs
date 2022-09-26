using EMarkeetingMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using PagedList;

namespace EMarkeetingMVC.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult login()
        {
            return View();
        }
        MVCDB1Entities db = new MVCDB1Entities();//connection string

        [HttpPost]
        public ActionResult login(tbl_admin avm)
        {
            tbl_admin ad = db.tbl_admin.Where(x => x.ad_username == avm.ad_username && x.ad_password == avm.ad_password).SingleOrDefault();
            if (ad != null)
            {

                Session["ad_id"] = ad.ad_id.ToString();
                return RedirectToAction("Create");

            }
            else
            {
                ViewBag.error = "Invalid username or password";

            }

            return View();
        }


        public ActionResult Create()
        {
            if (Session["ad_id"] == null)//admin id
            {
                return RedirectToAction("login");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(tbl_category cvm, HttpPostedFileBase imgfile)
        {
            string path = uploadimgfile(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Image could not be uploaded....";
            }
            else
            {

                string constring = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=MVCDB1;Integrated Security=True;Pooling=False;MultipleActiveResultSets=True;Application Name=EntityFramework";


                int adminid = Convert.ToInt32(Session["ad_id"].ToString());
                SqlConnection conn = new SqlConnection(constring);

                string query = "INSERT INTO  tbl_category(cat_name,cat_image,cat_status,cat_fk_ad) values('" + cvm.cat_name + "','" + path + "','1','" + adminid + "')";

                SqlCommand sqlCmd = new SqlCommand(query, conn);
                conn.Open();
                sqlCmd.ExecuteNonQuery();
                conn.Close();

                //tbl_category cat = new tbl_category();
                //cat.cat_name = cvm.cat_name;
                //cat.cat_image = path;
                //cat.cat_status = 1;
                //cat.cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString());
                //db.tbl_category.Add(cat);
                //db.SaveChanges();






                return RedirectToAction("ViewCategory");
            }

            return View();
        } //end,,,,,,,,,,,,,,,,,,,

        public ActionResult ViewCategory(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
               IPagedList<tbl_category> stu = list.ToPagedList(pageindex, pagesize);


            return View(stu);

           // return View();


        }
        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }



            return path;
        }












    }
}