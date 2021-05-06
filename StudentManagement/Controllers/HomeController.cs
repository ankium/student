using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Models;
using StudentManagement.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace StudentManagement.Controllers
{
    public class HomeController:Controller
    {
        private readonly IWebHostEnvironment webHostEnv;
        private readonly IStudentRepository stuRep;
        private readonly ILogger logger;
        //使用构造函数注入的方式注入IStudentRepository
        public HomeController(IStudentRepository studentRepository,IWebHostEnvironment webHostEnvironment,ILogger<HomeController> logger)
        {
            this.stuRep = studentRepository;
            this.webHostEnv = webHostEnvironment;
            this.logger = logger;
        }
        public ViewResult Index()
        {
            IEnumerable<Student> stuListModel = stuRep.GetAllStudents();
            return View(stuListModel);
        }

        public IActionResult Details(int? id){
            //throw new Exception("在Details视图中抛出异常");
            logger.LogTrace("学生详情---跟踪（Trace）Log");
            logger.LogDebug("学生详情---调试(Debug)Log");
            logger.LogInformation("学生详情---信息（Information）Log");
            logger.LogWarning("学生详情---警告(Warning)Log");
            logger.LogError("学生详情---错误（Error）Log");
            logger.LogCritical("学生详情---严重（Critical）Log");
            var student = stuRep.GetStudentById(id);
            if (student==null)
            {
                //Response.StatusCode = 404;
                //return View("StudentNotFound", id);
                ViewBag.ErrorMessage = $"学生ID为{id}的信息不存在，请重试。";
                return View("NotFound");
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel(){
                Student = stuRep.GetStudentById(id??1),
                PageTitle = "学生详情"
            };

            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public IActionResult Create(){
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(StudentCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photos!=null&&model.Photos.Count>0)
                {
                    foreach (IFormFile photo in model.Photos)
                    {
                        string uploadsFolder = Path.Combine(webHostEnv.WebRootPath, "images", "avatars");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                    }
                }
                Student newStudent = new Student
                {
                    Name = model.Name,
                    Email = model.Email,
                    Major = model.Major,
                    PhotoPath = uniqueFileName
                };
                stuRep.Insert(newStudent);
                return RedirectToAction("Details", new { id = newStudent.Id });
            }
            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Student student = stuRep.GetStudentById(id);
            if (student==null)
            {
                //Response.StatusCode = 404;
                //return View("StudentNotFound", id);
                ViewBag.ErrorMessage = $"学生ID为{id}的信息不存在，请重试。";
                return View("NotFound");
            }
            StudentEditViewModel studentEditViewModel = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                Major = student.Major,
                ExistingPhotoPath = student.PhotoPath
            };
            return View(studentEditViewModel);
        }

        //通过模型绑定，作为操作方法的参数StudentEditVewModel会接收来自POST请求的Edit表单数据
        [HttpPost]
        public IActionResult Edit(StudentEditViewModel model)
        {
            //检查提供的数据是否有效，如果没有通过验证，需要重新编辑学生信息，这样用户就可以更正并重新提交编辑表单
            if (ModelState.IsValid)
            {
                //从数据库中查询正在编辑的学生信息
                Student student = stuRep.GetStudentById(model.Id);
                //用模型对象中的数据更新student对象
                student.Name = model.Name;
                student.Email = model.Email;
                student.Major = model.Major;
                //如果用户想要更改图片，那么可以上传新图片文件，它会被模型对象上的Photos属性接收；如果用户没有上传图片，那么我们会保留现在的图片文件信息
                //因为兼容了多图上传，所以将这里的!=null判断修改为Photos的总数是否大于0
                if (model.Photos.Count>0)
                {
                    //如果上传了新的图片，则必须显示新的图片信息，因此我们会检查当前学生信息中是否有图片，如果有，则会删除它
                    if (model.ExistingPhotoPath!=null)
                    {
                        string filePath = Path.Combine(webHostEnv.WebRootPath, "images", "avatars", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    //我们将新的图片文件保存至wwwroot/images/avatars文件夹中，并且会更新Student对象中的PhotoPath属性，最终都会将它们保存至数据库中
                    student.PhotoPath = ProcessUploadedFile(model);
                }
                //调用仓储服务中的Update()方法，保存Student对象中的数据，更新数据库表中的信息
                Student updatedStudent = stuRep.Update(student);
                return RedirectToAction("index");
            }
            return View(model);
        }
        /// <summary>
        /// 将图片保存至指定的路径中，并返回唯一的文件名
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ProcessUploadedFile(StudentEditViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photos.Count>0)
            {
                foreach (var photo in model.Photos)
                {
                    //必须将图片文件上传至wwwroot/images/avatars文件夹中，而要获取wwwroot文件夹的路径，我们需要注入ASP.NET Core提供的webHostEnvironment服务去获取文件夹的路径
                    string uploadsFolder = Path.Combine(webHostEnv.WebRootPath, "images", "avatars");
                    //为了确保文件名是唯壹的，我们在文件名后附加壹个新的GUID值和壹个下划线
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    //因为使用了非托管资源，所以需要手动进行释放
                    using(var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        //使用IFormFile接口提供的CopyTo()方法将文件复制到wwwroot/images/avatars文件夹
                        photo.CopyTo(fileStream);
                    }
                }
            }
            return uniqueFileName;
        }
    }
}