using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Service.Controllers
{
    [Route("api/[controller]")]
    public class ActivityController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        public ActivityController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        // GET: api/<controller>
        [HttpGet]
        public ActionResult Get()
        {
            return Json(DAL.Activity.Instance.GetCount());
        }

        // GET api/<controller>/5
        [HttpGet("verifyCount")]
        public ActionResult GetVerifyCount()
        {
            return Json(DAL.Activity.Instance.GetVerifyCount());
        }
        [HttpGet("recommend")]
        public ActionResult GetGetRecommend()
        {
            var result = DAL.Activity.Instance.GetRecommend();
            if (result != null)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("记录数为0"));
        }

        [HttpGet("end")]
        public ActionResult GetEnd()
        {
            var result = DAL.Activity.Instance.GetEnd();
            if (result != null)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("记录数为0"));
        }
        [HttpGet("names")]
        public ActionResult GetNames()
        {
            var result = DAL.Activity.Instance.GetActivityNames();
            if (result.Count()== 0)
                return Json(Result.Err("没有任何活动"));
            else
                return Json(Result.Ok(result));
        }
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var result = DAL.Activity.Instance.GetModel(id);
            result.activityIntroduction = result.activityIntroduction.Replace("scr=\"", $"scr=\"https://{HttpContext.Request.Host.Value }/");

            if (result!= null)
                return Json(Result.Ok(result));
            else
                return Json(Result.Err("activityID不存在"));
        }
        [HttpPost]
        public ActionResult Post([FromBody]Model.Activity activity)
        {
            activity.activityIntroduction = activity.activityIntroduction.Replace($"https://{HttpContext.Request.Host.Value}/", "");
            activity.recommend = "否";
            try
            {
                int n = DAL.Activity.Instance.Add(activity);
                return Json(Result.Ok("发布活动成功", n));
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("foreign key"))
                    return Json(Result.Err("合法用户才能添加记录"));
                else if (ex.Message.ToLower().Contains("null"))
                    return Json(Result.Err("活动名称，结束时间 ，活动图片，活动审核情况，用户名不能为空"));
                else
                    return Json(Result.Err(ex.Message));
            }

        }
    }
}
