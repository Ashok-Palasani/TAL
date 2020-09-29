using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnitWorksCCS.Models;
using System.IO;
using System.Text;
using DNC;
using UnitWorksCCS;
using static UnitWorksCCS.FTPLibrary;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Net.Mime;
using System.Configuration;

namespace UnitWorks_CCS.Controllers
{
    public class ProgramTransferController : Controller
    {
        i_facility_talEntities db = new i_facility_talEntities();
        
        //
        // GET: /ProgramTransfer/

        public ActionResult Index()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            ViewBag.GetConnected = 0;
            ViewData["GetConnected"] = 0;
            Session["statusc"] = null;
            Session["statusn"] = null;
            ViewData["PlantID"] = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["WorkCenterID"] = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineInvNo");
            ViewData["PartNo"] = new SelectList(db.tblparts.Where(m => m.IsDeleted == 0), "PartID", "PartNo");

            return View();
        }

        [HttpPost]
        public ActionResult Index(IEnumerable<HttpPostedFileBase> file, string PlantID, string ShopID = null, string CellID = null, string WorkCenterID = null, int PartNo = 0, int OperationNo = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            string Path = @"C:\TataReport\ProgramTransferData";

            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];
            int roleId = Convert.ToInt32(Session["RoleID"]);
            Session["MachineID"] = WorkCenterID;
            int userID = Convert.ToInt32(Session["UserId"]);
            ViewBag.ReturnStatusMsg = null;
            
            if (System.IO.Directory.Exists(Path))
            {
                DirectoryInfo dir = new DirectoryInfo(Path);
                dir.Delete(true);
            }

            foreach (var filerow in file)
            {
                if (filerow != null)
                {
                    if (filerow.ContentLength > 0)
                    {


                        int WCID = Convert.ToInt32(WorkCenterID);
                        string fileLocation = Server.MapPath("~/Content/") + filerow.FileName;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        filerow.SaveAs(fileLocation);

                        //Copy File into machine Folder
                        string filename = filerow.FileName;

                        string MacWiseFolder = Path;
                        if (!System.IO.Directory.Exists(MacWiseFolder))
                        {
                            System.IO.Directory.CreateDirectory(MacWiseFolder);
                        }
                        string destinationPathWithFileName = "";
                        destinationPathWithFileName = Path + @"\" + filename;

                        System.IO.File.Copy(fileLocation, destinationPathWithFileName, true);
                    }

                    else
                    {
                        //Not a Valid File.
                        try
                        {
                            //Log this event.
                            tblprogramtransferhistory pth = new tblprogramtransferhistory();
                            pth.IsDeleted = 0;
                            pth.MachineID = Convert.ToInt32(WorkCenterID);
                            pth.ProgramName = filerow.FileName;
                            pth.UploadedTime = DateTime.Now;
                            pth.UserID = Convert.ToInt32(Session["UserId"]);
                            pth.ReturnTime = DateTime.Now;
                            pth.ReturnStatus = 999;
                            pth.ReturnDesc = "Not a Valid File(FileLength).";
                            pth.IsCompleted = 1;
                            db.tblprogramtransferhistories.Add(pth);
                            db.SaveChanges();

                            ViewBag.ReturnStatusMsg = "Not a Valid File(FileLength).";
                            TempData["toaster_error"] = "Not a Valid File(FileLength).";

                            // int pthID = pth.PTHID;
                            // Session["pthID"] = pthID;
                            //System.Threading.Thread.Sleep(30000);
                        }
                        catch (Exception e)
                        {
                            ViewBag.ReturnStatusMsg = "Error." + e;
                        }
                    }
                }
            }
            string count = file.Count() + "files";

            Session["filesCount"] = count;

            ViewBag.GetConnected = 0;
            ViewData["GetConnected"] = 0;
            int PlantIDInt = 0, ShopIDInt = 0, CellIDInt = 0, MacIDInt = 0;
            int.TryParse(PlantID, out PlantIDInt);
            int.TryParse(ShopID, out ShopIDInt);
            int.TryParse(CellID, out CellIDInt);
            int.TryParse(WorkCenterID, out MacIDInt);
            ViewData["PlantID"] = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", PlantIDInt);
            ViewData["ShopID"] = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == PlantIDInt), "ShopID", "ShopName", ShopIDInt);
            ViewData["CellID"] = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == PlantIDInt && m.ShopID == ShopIDInt), "CellID", "CellName", CellIDInt);
            ViewData["WorkCenterID"] = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == PlantIDInt && m.ShopID == ShopIDInt && m.CellID == CellIDInt), "MachineID", "MachineInvNo", MacIDInt);
            ViewData["PartNo"] = new SelectList(db.tblparts.Where(m => m.IsDeleted == 0 && m.PartID == PartNo), "PartID", "PartNo", PartNo);
            ViewData["OperationNo"] = OperationNo;
            if (roleId == 3)
            {
                if (Path != "")
                {
                    DirectoryInfo di = new DirectoryInfo(Path);
                    FileInfo[] dirfiles = di.GetFiles();
                    foreach (var files in dirfiles)
                    {
                        string fileName = Convert.ToString(files);
                        string[] filearry = fileName.Split('.');
                        string indfile = filearry[0];
                        var prodet = db.tblNcProgramTransferMains.Where(m => m.McId == MacIDInt && m.PartId == PartNo && m.OperationNo == OperationNo && m.ProgramNumber == indfile).FirstOrDefault();
                        if (prodet != null)
                        {
                            ViewData["progval"] = indfile;
                            Session["statusc"] = "1";
                            
                            break;
                        }
                        else
                        {
                            ViewData["files"] = files;
                            Session["statusn"] = "1";
                           
                            break;
                        }
                    }
                }
            }
            else if (roleId == 4)
            {
                ViewData["Programmer"] = "UploadProgram";
            }
            return View();
        }

        public string GetIndividualFile(int PartNo, int OperationNo, int WorkCenterID)
        {
            string res = "";
            string Path = @"C:\TataReport\ProgramTransferData";
            if (System.IO.Directory.Exists(Path))
            {
                DirectoryInfo di = new DirectoryInfo(Path);
                FileInfo[] dirfiles = di.GetFiles();
                if (dirfiles.Length > 0)
                {
                    foreach (var files in dirfiles)
                    {
                        string fileName = Convert.ToString(files);
                        string[] filearry = fileName.Split('.');
                        string indfile = filearry[0];
                        var prodet = db.tblNcProgramTransferMains.Where(m => m.McId == WorkCenterID && m.PartId == PartNo && m.OperationNo == OperationNo && m.ProgramNumber == indfile).FirstOrDefault();
                        if (prodet != null)
                        {
                            res = indfile;
                            Session["statusc"] = "1";
                            break;
                        }
                        else
                        {
                            res = indfile;
                            Session["statusn"] = "1";
                            break;
                        }
                    }
                }
                else
                {
                    res = "fail";
                }
            }
            
            return res;
        }

        //0: upload failed, 
        //1: Successfull.
        public JsonResult CheckEndStatus()
        {
            string retValue = "";
            using (i_facility_talEntities dbpupload = new i_facility_talEntities())
            {
                int macID = Convert.ToInt32(Session["MachineID"]);
                int pthistroyID = 0;
                Int32.TryParse(Convert.ToString(Session["pthID"]), out pthistroyID);

                if (pthistroyID == 0)
                {
                    pthistroyID = dbpupload.tblprogramtransferhistories.Where(m => m.MachineID == macID).OrderByDescending(m => m.UploadedTime).Select(m => m.PTHID).FirstOrDefault();
                }

                var RetStatusData = dbpupload.tblprogramtransferhistories.Where(m => m.PTHID == pthistroyID).FirstOrDefault();
                if (RetStatusData != null && RetStatusData.IsCompleted == 0)
                {
                    int retStatusInt = 0;
                    if (int.TryParse(Convert.ToString(RetStatusData.ReturnStatus), out retStatusInt))
                    {
                        if (retStatusInt == 0)
                        {
                            retValue = RetStatusData.ReturnDesc;
                            Session["pthID"] = null;
                        }
                        else
                        {
                            retValue = "Upload Successfull.";
                            Session["pthID"] = null;
                        }

                        //RetStatusData.IsCompleted = 1;
                        dbpupload.Entry(RetStatusData).State = System.Data.Entity.EntityState.Modified;
                        dbpupload.SaveChanges();

                    }
                    // else return null
                }
            }
            return Json(retValue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditPrograms()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];

            ViewData["PlantID"] = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["WorkCenterID"] = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");
            ViewData["PartNo"] = new SelectList(db.tblparts.Where(m => m.IsDeleted == 0), "PartID", "PartNo");

            List<Product> plist = new List<Product>();
            Product p1 = new Product();
            p1.Description = "p1";
            p1.Id = 1;
            p1.Quantity = 10;
            plist.Add(p1);

            Product p2 = new Product();
            p2.Description = "p2";
            p2.Id = 1;
            p2.Quantity = 10;
            plist.Add(p2);

            p1.plist = plist;

            return View(p1);

        }

        [HttpPost]
        public ActionResult EditPrograms(string PlantID, string ShopID = null, string CellID = null, string WorkCenterID = null, string PartID = null, int OperationNo = 0)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"].ToString().ToUpper();
            ViewBag.roleid = Session["RoleID"];

            //Get All ProgramNo's for this WorkCenterID and send to view.


            int PlantIDInt = 0, ShopIDInt = 0, CellIDInt = 0, MacIDInt = 0, PartIDInt = 0;
            int.TryParse(PlantID, out PlantIDInt);
            int.TryParse(ShopID, out ShopIDInt);
            int.TryParse(CellID, out CellIDInt);
            int.TryParse(WorkCenterID, out MacIDInt);
            int.TryParse(PartID, out PartIDInt);
            ViewData["PlantID"] = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", PlantIDInt);
            ViewData["ShopID"] = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == PlantIDInt), "ShopID", "ShopName", ShopIDInt);
            ViewData["CellID"] = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == PlantIDInt && m.ShopID == ShopIDInt), "CellID", "CellName", CellIDInt);
            ViewData["WorkCenterID"] = new SelectList(db.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == PlantIDInt && m.ShopID == ShopIDInt && m.CellID == CellIDInt), "MachineID", "MachineDisplayName", MacIDInt);
            ViewData["PartNo"] = new SelectList(db.tblparts.Where(m => m.IsDeleted == 0 && m.PartID == PartIDInt), "PartID", "PartNo");
            ViewData["OperationNo"] = OperationNo;
            return View();
        }

        public ActionResult GetView(int CellID = 1)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            string viewName = "_Product";
            object model = null;

            using (i_facility_talEntities db = new i_facility_talEntities())
            {
                model = db.tblmachinedetails.Where(o => o.CellID == CellID)
                          .OrderBy(o => o.CellID).ToList();
            }

            return PartialView(viewName, model);
        }

        //get program data from ftp
        //private string Getftp(string ip, string filePath)
        //{
        //    string Result = "";
        //    string[] file = filePath.Split('/');
        //    try
        //    {


        //        //ip = "192.168.0.202";
        //        //FtpWebRequest request1 = WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}/", ip, "NCProgram"))) as FtpWebRequest;
        //        //request1.Method = WebRequestMethods.Ftp.MakeDirectory;
        //        //MessageBox.Show(request1.Method);
        //        //FtpWebResponse ftpResponse = (FtpWebResponse)request1.GetResponse();

        //        // FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://Hostname.com/");    
        //        FtpWebRequest request1 = (FtpWebRequest)WebRequest.Create(new Uri(string.Format(@"ftp://{0}/{1}", ip, file[1])));
        //        request1.Method = WebRequestMethods.Ftp.UploadFile;


        //        request1.Credentials = new NetworkCredential("Administrator", "admin");
        //        request1.KeepAlive = false;
        //        request1.UseBinary = true;
        //        request1.UsePassive = true;


        //        FtpWebResponse response = (FtpWebResponse)request1.GetResponse();

        //        //Stream responseStream = response.GetResponseStream();
        //        //StreamReader reader = new StreamReader(responseStream);
        //        //MessageBox.Show(reader.ReadToEnd());


        //        using (FileStream fileStream = new FileStream(file[1], FileMode.Open, FileAccess.Read))
        //        using (Stream requestStream = request1.GetRequestStream())
        //        {
        //            //await fileStream.CopyToAsync(requestStream);
        //            fileStream.CopyTo(requestStream);
        //            Result = requestStream.ToString();
        //        }


        //        //reader.Close();
        //        response.Close();

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return Result;
        //}



        //get Program data

        public string ProgramData(int MacID, string ProgNo, string FoldIds, int partId, int OpNo,string compare)
        {
            String ProgData = null;// = new StringBuilder();
           int userId = Convert.ToInt32(Session["UserId"]);
            var promasterdet = db.tblProgramTransferDetailsMasters.Where(m => m.PTdMID == MacID).Select(m => m.tblprogramType.TypeName).FirstOrDefault();
            if (promasterdet != null)
            {
                if (promasterdet == "NCProgram")
                {
                    object ip = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.IPAddress).FirstOrDefault();
                    ProgramTransfer PT = new ProgramTransfer(ip.ToString());
                    ProgData = PT.GetProgramDataNC(ProgNo, out ProgData);
                    if(compare != "")
                    {
                        tbllogreport obj = new tbllogreport();
                        obj.LogDescription = "Compare";
                        obj.LogDate = DateTime.Now.Date;
                        obj.LogCapturedTime = DateTime.Now;
                        obj.MachineID = MacID;
                        obj.partNo = partId;
                        obj.operationNo = OpNo;
                        obj.programNumber = ProgNo;
                        obj.userId = userId;
                        db.tbllogreports.Add(obj);
                        db.SaveChanges();
                    }
                   
                }

                else if (promasterdet == "FTP")
                {
                    object ip = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.IPAddress).FirstOrDefault();
                    ProgramTransfer PT = new ProgramTransfer(ip.ToString());
                    //PT.GetProgramDataNC(ProgNo, out ProgData);


                    var FTPMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == MacID).FirstOrDefault();
                    string Username = FTPMachineDet.UserName;
                    string Password = FTPMachineDet.Password;
                    string IPAddress = FTPMachineDet.IpAddress;
                    FTPclient objFtp = new FTPclient(IPAddress, Username, Password);
                    objFtp.CurrentDirectory = "/";
                    string filepath = "";
                    if (FoldIds != "")
                    {
                        filepath = objFtp.Hostname + "/" + FoldIds + ProgNo;
                    }
                    else
                    {
                        filepath = objFtp.Hostname + "/" + ProgNo;
                    }


                    WebClient request = new WebClient();
                    string url = filepath;
                    request.Credentials = new NetworkCredential(Username, Password);

                    try
                    {
                        byte[] newFileData = request.DownloadData(url);
                        ProgData = System.Text.Encoding.UTF8.GetString(newFileData);
                        if (compare != "")
                        {
                            tbllogreport obj = new tbllogreport();
                            obj.LogDescription = "Compare";
                            obj.LogDate = DateTime.Now.Date;
                            obj.LogCapturedTime = DateTime.Now;
                            obj.MachineID = MacID;
                            obj.partNo = partId;
                            obj.operationNo = OpNo;
                            obj.programNumber = ProgNo;
                            obj.userId = userId;
                            db.tbllogreports.Add(obj);
                            db.SaveChanges();
                        }

                    }
                    catch (WebException e)
                    {

                    }
                }

                else if (promasterdet == "NetworkSharing")
                {
                    var NetWorkMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == MacID).FirstOrDefault();
                    string path1 = NetWorkMachineDet.MachineProgramPath;
                    string username1 = NetWorkMachineDet.UserName;
                    string password1 = NetWorkMachineDet.Password;
                    string domainname1 = NetWorkMachineDet.Domain;
                    string fileContent = "";
                    try
                    {
                        //using (new Impersonator(username1, domainname1, password1))
                        //{

                        DirectoryInfo di = new DirectoryInfo(path1);
                        FileInfo[] subFiles = di.GetFiles();
                        foreach (var filesrow in subFiles)
                        {
                            string file = Convert.ToString(filesrow);
                            if (file == ProgNo)
                            {
                                string filePath = path1 + "\\" + file;
                                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                                {
                                    fileContent = streamReader.ReadToEnd();

                                    ProgData = fileContent;
                                    if (compare != "")
                                    {
                                        tbllogreport obj = new tbllogreport();
                                        obj.LogDescription = "Compare";
                                        obj.LogDate = DateTime.Now.Date;
                                        obj.LogCapturedTime = DateTime.Now;
                                        obj.MachineID = MacID;
                                        obj.partNo = partId;
                                        obj.operationNo = OpNo;
                                        obj.programNumber = ProgNo;
                                        obj.userId = userId;
                                        db.tbllogreports.Add(obj);
                                        db.SaveChanges();
                                    }

                                }
                                //}
                            }




                        }

                        //path details
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show("Path Error: " + e);
                    }
                }
            }
            return ProgData;
        }

        [HttpPost]
        public string StoreAllProgram()
        {
            string res = "";
            string Path1 = @"C:\TataReport\ServerProgram";
            int filecount = Convert.ToInt32(System.Web.HttpContext.Current.Request.Params["filecount"]);
            int cont = Convert.ToInt32(filecount);
            if (System.IO.Directory.Exists(Path1))
            {
                DirectoryInfo dir = new DirectoryInfo(Path1);
                dir.Delete(true);
            }
            for (int i = 0; i < cont; i++)
            {
                var file = System.Web.HttpContext.Current.Request.Files["files" + i];

                string fileLocation = Server.MapPath("~/Content/") + file.FileName;
                if (System.IO.File.Exists(fileLocation))
                {
                    System.IO.File.Delete(fileLocation);
                }

                file.SaveAs(fileLocation);

                //Copy File into machine Folder
                string filename = file.FileName;

                string MacWiseFolder = Path1;
                if (!System.IO.Directory.Exists(MacWiseFolder))
                {
                    System.IO.Directory.CreateDirectory(MacWiseFolder);
                }
                string destinationPathWithFileName = "";
                destinationPathWithFileName = Path1 + @"\" + filename;



                System.IO.File.Copy(fileLocation, destinationPathWithFileName, true);
            }






            return res;
        }

        public string ServerUpload()
        {
            string progData = "";
            string fileContent = "";
            string Path = @"C:\TataReport\ServerProgram";
            if (System.IO.Directory.Exists(Path))
            {
                DirectoryInfo di = new DirectoryInfo(Path);
                FileInfo[] subFiles = di.GetFiles();
                if (subFiles.Length > 0)
                {
                    foreach (var filesrow in subFiles)
                    {
                        string file = Convert.ToString(filesrow);

                        string filePath = Path + "\\" + file;
                        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                        using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            fileContent = streamReader.ReadToEnd();

                            progData = fileContent;
                            break;
                        }
                    }
                }
                else
                {
                    progData = "fail";
                }
            }
           

           

            return progData;
        }

        public string GetPrgData(string filename)
        {
            string res = "";
            var path = @"C:\TataReport\ProgramTransferData";
            string filePath = path + "\\" + filename;
            string fileContent = "";
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                fileContent = streamReader.ReadToEnd();

                res = fileContent;

            }
            return res;
        }

        //To get latest versions selected program Content
        public string GetLatestVersionProgData(int machineID, string progNo, int partId, int OpNo)
        {
            string progdata = "";
            int userId = Convert.ToInt32(Session["UserId"]);
            var dbItem = db.tblNcProgramTransferMains.Where(m => m.IsDeleted == false).Where(m => m.McId == machineID && m.ProgramNumber == progNo && m.PartId == partId && m.OperationNo == OpNo).OrderByDescending(m => m.VersionNumber).FirstOrDefault();
            if (dbItem != null)
            {
                progdata = dbItem.ProgramData;
                tbllogreport obj = new tbllogreport();
                obj.LogDescription = "Getting selected program data from the server and Display it in Screen";
                obj.LogDate = DateTime.Now.Date;
                obj.LogCapturedTime = DateTime.Now;
                obj.MachineID = machineID;
                obj.userId = userId;
                db.tbllogreports.Add(obj);
            }
            return progdata;
        }

        //To get the latest version of the program 
        public string GetLatestVersion(int machineID, string progNo, int partId, int OpNo)
        {
            string versionData = "";
            var versionNo = db.tblNcProgramTransferMains.Where(m => m.IsDeleted == false).Where(m => m.McId == machineID && m.ProgramNumber == progNo && m.PartId == partId && m.OperationNo == OpNo).OrderByDescending(m => m.VersionNumber).Select(m => m.VersionNumber).FirstOrDefault();
            if (versionNo != null)
            {
                versionData = Convert.ToString(versionNo);
            }
            return versionData;
        }

        public JsonResult GetProgramListToUpload(int macID)
        {
            int userID = Convert.ToInt32(Session["UserId"]);
            string path1 = @"C:\TataReport\ProgramTransferData";
            string fileLocation = "";
            if (System.IO.Directory.Exists(path1))
            {
                DirectoryInfo di = new DirectoryInfo(path1);
                FileInfo[] subFiles = di.GetFiles();
                PrgDetList = new List<ProgramListDet>();
                foreach (var row in subFiles)
                {
                    string fileName = Convert.ToString(row);
                    fileLocation = path1 + "\\" + fileName;
                    String MachineInv = db.tblmachinedetails.Where(m => m.MachineID == macID).Select(m => m.MachineInvNo).FirstOrDefault();
                    var promasterdet = db.tblProgramTransferDetailsMasters.Where(m => m.PTdMID == macID).Select(m => m.tblprogramType.TypeName).FirstOrDefault();
                    if (promasterdet != null)
                    {

                        if (promasterdet == "NCProgram")
                        {
                            //Log this event.
                            tblprogramtransferhistory pth = new tblprogramtransferhistory();
                            pth.IsDeleted = 0;
                            pth.MachineID = macID;
                            pth.ProgramName = fileName;
                            pth.UploadedTime = DateTime.Now;
                            pth.Version = 1;
                            pth.UserID = userID;
                            db.tblprogramtransferhistories.Add(pth);
                            db.SaveChanges();


                            //Based on WorkCenterID get IpAddress.
                            object ip = db.tblmachinedetails.Where(m => m.MachineID == macID).Select(m => m.IPAddress).FirstOrDefault();
                            ushort port = 8193;            //  FOCAS1/Ethernet or FOCAS2/Ethernet (TCP) function
                            int timeout = 0;           //seconds if 0 infinitely waits

                            int RetVal = 0;
                            int pthID = pth.PTHID;

                            ProgramTransfer PT = new ProgramTransfer(ip.ToString());
                            string retString = PT.UploadCNCProgram(pthID, fileLocation, out RetVal);

                            if (retString == "Success")
                            {
                                var pthData = db.tblprogramtransferhistories.Find(pthID);
                                if (pthData != null)
                                {
                                    pthData.ReturnStatus = 1;
                                    pthData.ReturnDesc = "Success";
                                    pthData.ReturnTime = DateTime.Now;
                                    pthData.IsCompleted = 1;
                                    db.SaveChanges();
                                    TempData["toaster_success"] = "The NC Program : " + fileName.Split('.')[0].ToString() + " wsa successfully uploaded on the CNC Machine " + MachineInv;
                                    System.IO.File.Delete(fileLocation);
                                }
                                else //Hope fully this don't get executed.
                                {
                                    retString = "Success but Unable to complete.";
                                    TempData["toaster_warning"] = retString;
                                }
                            }
                            else //Upload failed.
                            {
                                var pthData = db.tblprogramtransferhistories.Find(pthID);
                                if (pthData != null)
                                {
                                    pthData.ReturnStatus = 0;
                                    pthData.ReturnDesc = retString;
                                    pthData.ReturnTime = DateTime.Now;
                                    pthData.IsCompleted = 1;
                                    db.SaveChanges();
                                }
                                else //Hope fully this don't get executed.
                                {
                                    retString += "Failure and Unable to complete.";
                                    TempData["toaster_error"] = retString;
                                }
                            }
                        }

                        else if (promasterdet == "FTP")
                        {
                            string reply = "";

                            tblprogramtransferhistory pth = new tblprogramtransferhistory();
                            pth.IsDeleted = 0;
                            pth.MachineID = macID;
                            pth.ProgramName = fileName;
                            pth.UploadedTime = DateTime.Now;
                            pth.Version = 1;
                            pth.UserID = userID;
                            db.tblprogramtransferhistories.Add(pth);
                            db.SaveChanges();

                            int RetVal = 0;
                            int pthID = pth.PTHID;

                            var FTPMachineDet = new tblProgramTransferDetailsMaster();

                            FTPMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == macID).FirstOrDefault();


                            string Username = FTPMachineDet.UserName;
                            string Password = FTPMachineDet.Password;
                            string IPAddress = FTPMachineDet.IpAddress;
                            FTPclient objFtp = new FTPclient(IPAddress, Username, Password);
                            objFtp.CurrentDirectory = "/";
                            FtpMain Main = new FtpMain();

                            //Set FTP Client in MAIN form
                            Main.SetFtpClient(objFtp);
                            fileupload_ftp objftp = new fileupload_ftp();
                            bool ret = objftp.CheckIfFileExistsOnServer(fileName, objFtp);
                            if (ret == false)
                            {
                                string retString = objftp.fileuploadftp(fileLocation, objFtp.CurrentDirectory, objFtp);
                                if (retString == "Success")
                                {
                                    var pthData = db.tblprogramtransferhistories.Find(pthID);
                                    if (pthData != null)
                                    {
                                        pthData.ReturnStatus = 1;
                                        pthData.ReturnDesc = "Success";
                                        pthData.ReturnTime = DateTime.Now;
                                        pthData.IsCompleted = 1;
                                        db.SaveChanges();
                                        TempData["toaster_success"] = "The FTP Program : " + fileName.Split('.')[0].ToString() + " wsa successfully uploaded on the CNC Machine " + MachineInv;
                                        System.IO.File.Delete(fileLocation);
                                    }
                                    else
                                    {
                                        retString += "Failure and Unable to complete.";
                                        TempData["toaster_error"] = retString;
                                    }
                                }

                            }
                            else
                            {
                                ProgramListDet PLD = new ProgramListDet();
                                PLD.ProgName = fileName;
                                PLD.ProgNo = fileName;
                                PrgDetList.Add(PLD);
                                System.IO.File.Delete(fileLocation);
                            }
                        }

                        else if (promasterdet == "NetworkSharing")
                        {
                            tblprogramtransferhistory pth = new tblprogramtransferhistory();
                            pth.IsDeleted = 0;
                            pth.MachineID = macID;
                            pth.ProgramName = fileName;
                            pth.UploadedTime = DateTime.Now;
                            pth.Version = 1;
                            pth.UserID = userID;
                            db.tblprogramtransferhistories.Add(pth);
                            db.SaveChanges();

                            int RetVal = 0;
                            int pthID = pth.PTHID;

                            //validate the 
                            var NetWorkMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == macID).FirstOrDefault();
                            string path2 = NetWorkMachineDet.MachineProgramPath;
                            string username1 = NetWorkMachineDet.UserName;
                            string password1 = NetWorkMachineDet.Password;
                            string domainname1 = NetWorkMachineDet.Domain;
                            try
                            {

                                //using (new Impersonator(username1, domainname1, password1))
                                //{
                                string filePath = path2 + "\\" + fileName;
                                bool fileExists = (System.IO.File.Exists(filePath) ? true : false);

                                if (fileExists == false)
                                {
                                    System.IO.File.Copy(fileLocation, filePath, true);

                                    var pthData = db.tblprogramtransferhistories.Find(pthID);
                                    if (pthData != null)
                                    {
                                        pthData.ReturnStatus = 1;
                                        pthData.ReturnDesc = "Success";
                                        pthData.ReturnTime = DateTime.Now;
                                        pthData.IsCompleted = 1;
                                        db.SaveChanges();
                                        TempData["toaster_success"] = "The FTP Program : " + fileName.Split('.')[0].ToString() + " wsa successfully uploaded on the CNC Machine " + MachineInv;
                                        System.IO.File.Delete(fileLocation);
                                    }

                                }
                                else
                                {

                                    ProgramListDet PLD = new ProgramListDet();
                                    PLD.ProgName = fileName;
                                    PLD.ProgNo = fileName;
                                    PrgDetList.Add(PLD);
                                    System.IO.File.Delete(fileLocation);
                                }

                                //}


                            }
                            catch (Exception e)
                            {

                            }

                        }

                    }

                }
            }
            return Json(PrgDetList, JsonRequestBehavior.AllowGet);

        }

        public string UploadToMachine(int machineId, string fileName)
        {
            int userID = Convert.ToInt32(Session["UserId"]);
            string res = "";
            string path = @"C:\TataReport\ProgramTransferData";
            if (System.IO.Directory.Exists(path))
            { 
                string fileLocation = path + "\\" + fileName;
            

            String MachineInv = db.tblmachinedetails.Where(m => m.MachineID == machineId).Select(m => m.MachineInvNo).FirstOrDefault();
            var promasterdet = db.tblProgramTransferDetailsMasters.Where(m => m.PTdMID == machineId).Select(m => m.tblprogramType.TypeName).FirstOrDefault();
            if (promasterdet != null)
            {

                if (promasterdet == "NCProgram")
                {
                    //Log this event.
                    tblprogramtransferhistory pth = new tblprogramtransferhistory();
                    pth.IsDeleted = 0;
                    pth.MachineID = machineId;
                    pth.ProgramName = fileName;
                    pth.UploadedTime = DateTime.Now;
                    pth.correcteddate = DateTime.Now.ToString("yyyy-MM-dd");
                    pth.Version = 1;
                    pth.UserID = userID;
                    db.tblprogramtransferhistories.Add(pth);
                    db.SaveChanges();

                    
                    //Based on WorkCenterID get IpAddress.
                    object ip = db.tblmachinedetails.Where(m => m.MachineID == machineId).Select(m => m.IPAddress).FirstOrDefault();
                    ushort port = 8193;            //  FOCAS1/Ethernet or FOCAS2/Ethernet (TCP) function
                    int timeout = 0;           //seconds if 0 infinitely waits

                    int RetVal = 0;
                    int pthID = pth.PTHID;

                    ProgramTransfer PT = new ProgramTransfer(ip.ToString());
                    string retString = PT.UploadCNCProgram(pthID, fileLocation, out RetVal);

                    if (retString == "Success")
                    {
                        var pthData = db.tblprogramtransferhistories.Find(pthID);
                        if (pthData != null)
                        {
                            pthData.ReturnStatus = 1;
                            pthData.ReturnDesc = "Success";
                            pthData.ReturnTime = DateTime.Now;
                            pthData.IsCompleted = 1;
                            db.SaveChanges();
                            res = "Success";
                            TempData["toaster_success"] = "The NC Program : " + fileName.Split('.')[0].ToString() + " wsa successfully uploaded on the CNC Machine " + MachineInv;

                            tbllogreport obj1 = new tbllogreport();
                            obj1.LogDescription = "UploadToMachine";
                            obj1.LogDate = DateTime.Now.Date;
                            obj1.LogCapturedTime = DateTime.Now;
                            obj1.MachineID = machineId;
                            obj1.programNumber = fileName;
                            obj1.userId = userID;
                            db.tbllogreports.Add(obj1);
                            db.SaveChanges();

                            System.IO.File.Delete(fileLocation);
                        }
                        else //Hope fully this don't get executed.
                        {
                            res = "Error";
                            retString = "Success but Unable to complete.";
                            TempData["toaster_warning"] = retString;
                        }
                    }
                    else //Upload failed.
                    {
                        var pthData = db.tblprogramtransferhistories.Find(pthID);
                        if (pthData != null)
                        {
                            pthData.ReturnStatus = 0;
                            pthData.ReturnDesc = retString;
                            pthData.ReturnTime = DateTime.Now;
                            pthData.IsCompleted = 0;
                            db.SaveChanges();

                           
                        }
                        else //Hope fully this don't get executed.
                        {
                            retString += "Failure and Unable to complete.";
                            TempData["toaster_error"] = retString;
                        }
                    }
                }

                else if (promasterdet == "FTP")
                {
                    string reply = "";

                    tblprogramtransferhistory pth = new tblprogramtransferhistory();
                    pth.IsDeleted = 0;
                    pth.MachineID = machineId;
                    pth.ProgramName = fileName;
                    pth.UploadedTime = DateTime.Now;
                    pth.correcteddate = DateTime.Now.ToString("yyyy-MM-dd");
                    pth.Version = 1;
                    pth.UserID = userID;
                    db.tblprogramtransferhistories.Add(pth);
                    db.SaveChanges();

                   

                    int RetVal = 0;
                    int pthID = pth.PTHID;

                    var FTPMachineDet = new tblProgramTransferDetailsMaster();

                    FTPMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == machineId).FirstOrDefault();


                    string Username = FTPMachineDet.UserName;
                    string Password = FTPMachineDet.Password;
                    string IPAddress = FTPMachineDet.IpAddress;
                    FTPclient objFtp = new FTPclient(IPAddress, Username, Password);
                    objFtp.CurrentDirectory = "/";
                    FtpMain Main = new FtpMain();

                    //Set FTP Client in MAIN form
                    Main.SetFtpClient(objFtp);
                    fileupload_ftp objftp = new fileupload_ftp();
                    bool ret = objftp.CheckIfFileExistsOnServer(fileName, objFtp);
                    if (ret == false)
                    {
                        string retString = objftp.fileuploadftp(fileLocation, objFtp.CurrentDirectory, objFtp);
                        if (retString == "Success")
                        {
                            var pthData = db.tblprogramtransferhistories.Find(pthID);
                            if (pthData != null)
                            {
                                pthData.ReturnStatus = 1;
                                pthData.ReturnDesc = "Success";
                                pthData.ReturnTime = DateTime.Now;
                                pthData.IsCompleted = 1;
                                db.SaveChanges();
                                res = "Success";
                                TempData["toaster_success"] = "The Program : " + fileName.Split('.')[0].ToString() + " was successfully uploaded on the FTP Machine " + MachineInv;

                                    tbllogreport obj1 = new tbllogreport();
                                    obj1.LogDescription = "UploadToMachine";
                                    obj1.LogDate = DateTime.Now.Date;
                                    obj1.LogCapturedTime = DateTime.Now;
                                    obj1.MachineID = machineId;
                                    obj1.programNumber = fileName;
                                    obj1.userId = userID;
                                    db.tbllogreports.Add(obj1);
                                    db.SaveChanges();

                                    System.IO.File.Delete(fileLocation);
                            }
                            else
                            {
                                retString += "Failure and Unable to complete.";
                                TempData["toaster_error"] = retString;
                            }
                        }
                        else //Upload failed.
                        {
                            var pthData = db.tblprogramtransferhistories.Find(pthID);
                            if (pthData != null)
                            {
                                pthData.ReturnStatus = 0;
                                pthData.ReturnDesc = retString;
                                pthData.ReturnTime = DateTime.Now;
                                pthData.IsCompleted = 0;
                                db.SaveChanges();

                               
                            }
                            else //Hope fully this don't get executed.
                            {
                                retString += "Failure and Unable to complete.";
                                TempData["toaster_error"] = retString;
                            }
                        }

                    }
                    else
                    {
                        var pthData = db.tblprogramtransferhistories.Find(pthID);
                        if (pthData != null)
                        {
                            pthData.deletedDate = Convert.ToString(DateTime.Now);
                            pthData.IsDeleted = 1;
                            db.SaveChanges();
                            res = "Delete";
                        }

                    }
                }

                else if (promasterdet == "NetworkSharing")
                {


                    tblprogramtransferhistory pth = new tblprogramtransferhistory();
                    pth.IsDeleted = 0;
                    pth.MachineID = machineId;
                    pth.ProgramName = fileName;
                    pth.UploadedTime = DateTime.Now;
                    pth.correcteddate = DateTime.Now.ToString("yyyy-MM-dd");
                    pth.Version = 1;
                    pth.UserID = userID;
                    db.tblprogramtransferhistories.Add(pth);
                    db.SaveChanges();

                    

                    int RetVal = 0;
                    int pthID = pth.PTHID;

                    //validate the 
                    var NetWorkMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == machineId).FirstOrDefault();
                    string path1 = NetWorkMachineDet.MachineProgramPath;
                    string username1 = NetWorkMachineDet.UserName;
                    string password1 = NetWorkMachineDet.Password;
                    string domainname1 = NetWorkMachineDet.Domain;
                    try
                    {

                        //using (new Impersonator(username1, domainname1, password1))
                        //{
                        string filePath = path1 + "\\" + fileName;
                        bool fileExists = (System.IO.File.Exists(filePath) ? true : false);

                        if (fileExists == false)
                        {
                            System.IO.File.Copy(fileLocation, filePath, true);

                            var pthData = db.tblprogramtransferhistories.Find(pthID);
                            if (pthData != null)
                            {
                                pthData.ReturnStatus = 1;
                                pthData.ReturnDesc = "Success";
                                pthData.ReturnTime = DateTime.Now;
                                pthData.IsCompleted = 1;
                                db.SaveChanges();
                                res = "Success";
                                TempData["toaster_success"] = "The Program : " + fileName.Split('.')[0].ToString() + " was successfully uploaded on the Network Sharing " + MachineInv;

                                    tbllogreport obj1 = new tbllogreport();
                                    obj1.LogDescription = "UploadToMachine";
                                    obj1.LogDate = DateTime.Now.Date;
                                    obj1.LogCapturedTime = DateTime.Now;
                                    obj1.MachineID = machineId;
                                    obj1.programNumber = fileName;
                                    obj1.userId = userID;
                                    db.tbllogreports.Add(obj1);
                                    db.SaveChanges();

                                    System.IO.File.Delete(fileLocation);
                            }

                        }
                        else
                        {
                            res = "Delete";
                        }

                        //}


                    }
                    catch (Exception e)
                    {

                    }

                }

            }
        }
            return res;
        }

        public string CheckFileorFolder(int MacID, string Prog, string FoldIds)
        {
            string Result = "";
            object ip = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.IPAddress).FirstOrDefault();
            ProgramTransfer PT = new ProgramTransfer(ip.ToString());
            string ProgData = "";

            var FTPMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == MacID).FirstOrDefault();
            string Username = FTPMachineDet.UserName;
            string Password = FTPMachineDet.Password;
            string IPAddress = FTPMachineDet.IpAddress;
            FTPclient objFtp = new FTPclient(IPAddress, Username, Password);
            objFtp.CurrentDirectory = "/";
            string filepath = "";
            WebClient request = new WebClient();
            string url = "";
            if (FoldIds != "")
            {
                filepath = objFtp.Hostname + "/" + FoldIds + Prog;
                url = filepath;
            }
            else
            {
                filepath = objFtp.Hostname + Prog;
                url = filepath;
            }

            request.Credentials = new NetworkCredential(Username, Password);

            try
            {
                byte[] newFileData = request.DownloadData(url);
                ProgData = System.Text.Encoding.UTF8.GetString(newFileData);
                //if(ProgData!="")
                //{
                //    Result = "Fail";
                //}
                // Console.WriteLine(fileString);
            }
            catch (WebException e)
            {
                IPAddress = IPAddress + "/" + Prog;
                Result = "true";
                // Do something such as log error, but this is based on OP's original code
                // so for now we do nothing.
            }
            return Result;
        }

        public JsonResult SubProgListr(int MachID, string ProgNO)
        {
            object ip = db.tblmachinedetails.Where(m => m.MachineID == MachID).Select(m => m.IPAddress).FirstOrDefault();
            ProgramTransfer PT = new ProgramTransfer(ip.ToString());
            //PT.GetProgramDataNC(ProgNo, out ProgData);


            var FTPMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == MachID).FirstOrDefault();
            string Username = FTPMachineDet.UserName;
            string Password = FTPMachineDet.Password;
            string IPAddress = FTPMachineDet.IpAddress;

            //IPAddress = "192.168.0.110/" + ProgNO;
            //Username = "Administrator";
            //Password = "admin";
            if (ProgNO != "")
            {
                IPAddress = IPAddress + "/" + ProgNO;
            }
            //ProgData = Getftp(IPAddress, ProgNo);
            FTPclient objFtp = new FTPclient(IPAddress, Username, Password);
            objFtp.CurrentDirectory = "/";
            FtpMain Main = new FtpMain();
            PrgDetList = new List<ProgramListDet>();
            PrgDetList = Main.SetFtpClient(objFtp);
            return Json(PrgDetList, JsonRequestBehavior.AllowGet);
        }


        //Get Program data from the Version selected of the Program or by default the Latest Version Number Present
        public JsonResult ProgramDataPC(int MacID, string ProgNo, int VerNo)
        {
            String ProgData; //= "";
            int userId = Convert.ToInt32(Session["UserId"]);
            object ip = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.IPAddress).FirstOrDefault();
            String MacInv = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.MachineInvNo).FirstOrDefault();
            ProgramTransfer PT = new ProgramTransfer(ip.ToString());
            PT.GetProgramDataPC(MacInv, ProgNo.ToString(), VerNo, out ProgData);
            
            return Json(ProgData.ToString(), JsonRequestBehavior.AllowGet);
        }



        public string SendMail(string filename, int PlantID, int ShopID, int CellID, int macID, int partNo, int opNo, string message, string tolist, string cclist)
        {
            string res = "fail";
            string path = @"C:\TataReport\ProgramTransferData";
            string fileLocation = path + "\\" + filename;

            int userId = Convert.ToInt32(Session["UserId"]);
            var partName = db.tblparts.Where(m => m.PartID == partNo).Select(m => m.PartNo).FirstOrDefault();
            var plantName = db.tblplants.Where(m => m.PlantID == PlantID).Select(m => m.PlantName).FirstOrDefault();
            var shopName = db.tblshops.Where(m => m.ShopID == ShopID).Select(m => m.ShopName).FirstOrDefault();
            var cellName = db.tblcells.Where(m => m.CellID == CellID).Select(m => m.CellName).FirstOrDefault();
            var machineName = db.tblProgramTransferDetailsMasters.Where(m => m.PTdMID == macID).Select(m => m.MachineInvNo).FirstOrDefault();
            string partNumber = Convert.ToString(partName);
            string operationNo = Convert.ToString(opNo);
            string subject = "Regarding Program Transfer";

            var reader = Path.Combine(@"C:\TataReport\TCFTemplate\notifyTemplate.html");
            string htmlStr = System.IO.File.ReadAllText(reader);

            string logo = @"C:\TataReport\TCFTemplate\120px-Tata_logo.Jpeg";
            String[] seperator = { "{{reasonStart}}" };
            string[] htmlArr = htmlStr.Split(seperator, 2, StringSplitOptions.RemoveEmptyEntries);

            var reasonHtml = htmlArr[1].Split(new String[] { "{{reasonEnd}}" }, 2, StringSplitOptions.RemoveEmptyEntries)[0];
            htmlStr = htmlStr.Replace("{{reasonStart}}", "");
            htmlStr = htmlStr.Replace("{{reasonEnd}}", "");

            htmlStr = htmlStr.Replace("{{PlantName}}", plantName);
            htmlStr = htmlStr.Replace("{{ShopName}}", shopName);
            htmlStr = htmlStr.Replace("{{CellName}}", cellName);
            htmlStr = htmlStr.Replace("{{MachineName}}", machineName);
            htmlStr = htmlStr.Replace("{{PartNo}}", partNumber);
            htmlStr = htmlStr.Replace("{{OperationNo}}", operationNo);
            htmlStr = htmlStr.Replace("{{ProgramNumber}}", filename);

            htmlStr = htmlStr.Replace(reasonHtml, "");
            htmlStr = htmlStr.Replace("{{message}}", message);
            bool ret = mailsend(htmlStr, tolist, cclist, 2, subject);
            if (ret == true)
            {
                res = "Success";
                System.IO.File.Delete(fileLocation);
                tbllogreport obj = new tbllogreport();
                obj.LogDescription = "Notify";
                obj.LogDate = DateTime.Now.Date;
                obj.LogCapturedTime = DateTime.Now;
                obj.MachineID = macID;
                obj.programNumber = filename;
                obj.LogInfo = message;
                obj.userId = userId;
                db.tbllogreports.Add(obj);
                db.SaveChanges();
                
            }
            else
            {
                res = "fail";
               
            }
            return res;
        }

        public bool mailsend(string message, string tolist, string cclist, int image, string subject)
        {
            bool ret = false;

            try
            {
                tolist = tolist.Remove(tolist.Length -1);
                cclist = cclist.Remove(cclist.Length -1);
                if (message != "" && tolist != "")
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(tolist);
                    if (cclist != "")
                    {
                        mail.CC.Add(cclist);
                    }

                    var smtpConn = db.smtpdetails.Where(x => x.IsDeleted == true).FirstOrDefault();
                    string hostName = smtpConn.Host;
                    int port = smtpConn.Port;
                    bool enableSsl = smtpConn.EnableSsl;
                    bool useDefaultCredentials = smtpConn.UseDefaultCredentials;
                    string emailId = smtpConn.EmailId;
                    string password = smtpConn.password;
                    string fromMail = smtpConn.FromMailId;
                    string connectType = "";
                    if (smtpConn.ConnectType != "" || smtpConn.ConnectType != null)
                    {
                        connectType = smtpConn.ConnectType;//domain
                    }

                    mail.From = new MailAddress(fromMail);
                    mail.Subject = subject;
                    mail.Body = "" + message;
                    mail.IsBodyHtml = true;

                    if (image == 2)
                    {
                        AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message, Encoding.UTF8, MediaTypeNames.Text.Html);
                        // Create a plain text message for client that don't support HTML
                        AlternateView plainView = AlternateView.CreateAlternateViewFromString(Regex.Replace(message, "<[^>]+?>", string.Empty), Encoding.UTF8, MediaTypeNames.Text.Plain);
                        string mediaType = MediaTypeNames.Image.Jpeg;
                        LinkedResource img = new LinkedResource(@"C:\TataReport\TCFTemplate\120px-Tata_logo.Jpeg", mediaType);
                        // Make sure you set all these values!!!
                        img.ContentId = "EmbeddedContent_1";
                        img.ContentType.MediaType = mediaType;
                        img.TransferEncoding = TransferEncoding.Base64;
                        img.ContentType.Name = img.ContentId;
                        img.ContentLink = new Uri("cid:" + img.ContentId);
                        htmlView.LinkedResources.Add(img);
                        mail.AlternateViews.Add(plainView);
                        mail.AlternateViews.Add(htmlView);

                    }

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = hostName;
                    smtp.Port = port;
                    smtp.EnableSsl = enableSsl;
                    smtp.UseDefaultCredentials = useDefaultCredentials;
                    if (connectType == "")
                    {
                        smtp.Credentials = new System.Net.NetworkCredential(emailId, password);
                    }
                    else
                    {
                        smtp.Credentials = new System.Net.NetworkCredential(emailId, password, connectType);
                    }
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(mail);

                    ret = true;
                }
            }

            catch (Exception ex)
            {

            }
            return ret;
        }


        //To send the mail to programmer
        //public string SendMail(string filename, int PlantID, int ShopID, int CellID, int macID, int partNo, int opNo, string message, string tolist, string cclist)
        //{
        //    string ret = "fail";
        //    try
        //    {
        //        var partName = db.tblparts.Where(m => m.PartID == partNo).Select(m => m.PartNo).FirstOrDefault();
        //        var plantName = db.tblplants.Where(m => m.PlantID == PlantID).Select(m => m.PlantName).FirstOrDefault();
        //        var shopName = db.tblshops.Where(m => m.ShopID == ShopID).Select(m => m.ShopName).FirstOrDefault();
        //        var cellName = db.tblcells.Where(m => m.CellID == CellID).Select(m => m.CellName).FirstOrDefault();
        //        var machineName = db.tblmachinedetails.Where(m => m.MachineID == macID).Select(m => m.MachineInvNo).FirstOrDefault();

        //        var transferDet = db.tblprogramtransferhistories.Where(m => m.MachineID == macID && m.ProgramName == filename && m.PartId == partNo && m.OperationNo == opNo).FirstOrDefault();
        //        if(transferDet != null)
        //        {
        //            transferDet.Message = message;
        //            db.SaveChanges();
        //        }
        //        if (message != "" && tolist != "")
        //        {
        //            MailMessage mail = new MailMessage();
        //            mail.To.Add(tolist);
        //            if (cclist != "")
        //            {
        //                mail.CC.Add(cclist);
        //            }
        //            int image = 1;
        //            string subject = "Regarding Program Transfer";

        //            //var smtpConn = db.Smtpdetails.Where(x => x.IsDeleted == true && x.TcfModuleId == 1).FirstOrDefault();
        //            //string hostName = smtpConn.Host;
        //            //int port = smtpConn.Port;
        //            //bool enableSsl = smtpConn.EnableSsl;
        //            //bool useDefaultCredentials = smtpConn.UseDefaultCredentials;
        //            //string emailId = smtpConn.EmailId;
        //            //string password = smtpConn.Password;
        //            //string fromMail = smtpConn.FromMailId;
        //            //string connectType = "";
        //            //if (smtpConn.ConnectType != "" || smtpConn.ConnectType != null)
        //            //{
        //            //    connectType = smtpConn.ConnectType;//domain
        //            //}
        //            int port = 587;
        //            bool enableSsl = true;
        //            bool useDefaultCredentials = false;
        //            string emailId = "monika.ms@srkssolutions.com";
        //            string password = "monika.ms10$";
        //            string fromMail = "monika.ms@srkssolutions.com";
        //            string connectType = "";


        //            mail.From = new MailAddress(fromMail);
        //            //mail.From = new MailAddress("unitworks@tasl.aero");
        //            mail.Subject = subject;
        //            mail.IsBodyHtml = true;

        //            mail.Body = "<p><b>Dear Concerned,</b></p>" +
        //                    "<p><b></b></p>" +
        //                    "<p><b>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;This is you to inform that, In This Plant -->" + plantName + ", shop-->" + shopName + ", Cell-->" + cellName + ", Machine-->" + machineName + ", Part-->" + partName + ", and Operation NO-->" + opNo + " " + message + " in the <span style='color: blue'>'" + filename + "'</span> Program." +

        //                     "<p><b></b></p>" +
        //                     "<p><b></b></p>" +
        //                     "<p><b> Thank you,</b></p>" +

        //                     "<p><b></b></p>" +
        //                     "<p><b></b></p>";

        //            SmtpClient smtp = new SmtpClient();
        //            smtp.Host = hostName;
        //            smtp.Port = port;
        //            smtp.EnableSsl = enableSsl;
        //            smtp.UseDefaultCredentials = useDefaultCredentials;
        //            if (connectType == "")
        //            {
        //                smtp.Credentials = new System.Net.NetworkCredential(emailId, password);
        //            }
        //            else
        //            {
        //                smtp.Credentials = new System.Net.NetworkCredential(emailId, password, connectType);
        //            }
        //            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //            smtp.Send(mail);

        //            ret = "Success";
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //    }
        //    return ret;
        //}

        List<ProgramListDet> PrgDetList;

        //Get Program List for the Machine Connected
        public JsonResult ProgramList(int MacID)
        {
            //List<ProgramListDet> PrgDetList;
            //PrgDetList = new List<ProgramListDet>();
            int userId = Convert.ToInt32(Session["UserId"]);
            var promasterdet = db.tblProgramTransferDetailsMasters.Where(m => m.PTdMID == MacID).Select(m => m.tblprogramType.TypeName).FirstOrDefault();
            if (promasterdet != null)
            {

                if (promasterdet == "NCProgram")
                {


                    object ip = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.IPAddress).FirstOrDefault();
                    ProgramTransfer PT = new ProgramTransfer(ip.ToString());
                    PrgDetList = PT.GetProgramList();
                    

                }

                else if (promasterdet == "FTP")
                {
                    //List<ProgramListDet> PrgDetList;
                    var FTPMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == MacID).FirstOrDefault();
                    string Username = FTPMachineDet.UserName;
                    string Password = FTPMachineDet.Password;
                    string IPAddress = FTPMachineDet.IpAddress;
                    FTPclient objFtp = new FTPclient(IPAddress, Username, Password);
                    objFtp.CurrentDirectory = "/";
                    FtpMain Main = new FtpMain();
                    PrgDetList = new List<ProgramListDet>();
                    PrgDetList = Main.SetFtpClient(objFtp);
                    

                }

                else if (promasterdet == "NetworkSharing")
                {
                    //List<ProgramListDet> PrgDetList = new List<ProgramListDet>();
                    var NetWorkMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == MacID).FirstOrDefault();
                    string path1 = NetWorkMachineDet.MachineProgramPath;
                    string username1 = NetWorkMachineDet.UserName;
                    string password1 = NetWorkMachineDet.Password;
                    string domainname1 = NetWorkMachineDet.Domain;
                    try
                    {
                        //using (new Impersonator(username1, domainname1, password1))
                        //{
                        DirectoryInfo di = new DirectoryInfo(path1);
                        FileInfo[] subFiles = di.GetFiles();
                        PrgDetList = new List<ProgramListDet>();
                        foreach (var row in subFiles)
                        {
                            string fileName = Convert.ToString(row);
                            ProgramListDet PLD = new ProgramListDet();
                            PLD.ProgName = fileName;
                            PLD.ProgNo = fileName;
                            PrgDetList.Add(PLD);
                           
                        }

                        //}

                        //path details
                    }
                    catch (Exception e)
                    {
                    }
                }

            }

            return Json(PrgDetList, JsonRequestBehavior.AllowGet);
        }

        //Get the version list of the program from the Program selected for the connected machine.
        public JsonResult PCProgramList(int MacID, String ProgNo)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            List<ProgramVersionListDet> PrgVerDetList = new List<ProgramVersionListDet>();
            object ip = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.IPAddress).FirstOrDefault();
            String MacInv = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.MachineDispName).FirstOrDefault();
            ProgramTransfer PT = new ProgramTransfer(ip.ToString());
            PT.GetVersionListPCProgram(MacInv.ToString(), ProgNo.ToString(), out PrgVerDetList);
            
            return Json(PrgVerDetList, JsonRequestBehavior.AllowGet);
        }

        //Get all the programs which are present in the server based on machineId,partId,OperatioNo
        public JsonResult ServerProgramList(int MachineID, int PartId, int OpNo)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            List<ProgramVersionListDet> ServerPrgDetList = new List<ProgramVersionListDet>();
            var serverList = db.tblNcProgramTransferMains.Where(m => m.IsDeleted == false && m.McId == MachineID && m.PartId == PartId && m.OperationNo == OpNo).Select(m => m.ProgramNumber).Distinct().ToList();  //partid ,opNo  latest version distinct one
            if (serverList.Count > 0)
            {
                foreach (var row in serverList)
                {
                    ProgramVersionListDet obj1 = new ProgramVersionListDet();
                    obj1.ProgNo = row;
                    //obj1.ProgVer = Convert.ToString(row.VersionNumber);
                    //obj1.ProgDate = row.CreatedDate.ToString();
                    ServerPrgDetList.Add(obj1);
                   
                }
            }
            return Json(ServerPrgDetList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetShop(int PlantID)
        {
            var ShopData = (from row in db.tblshops
                            where row.IsDeleted == 0 && row.PlantID == PlantID
                            select new { Value = row.ShopID, Text = row.ShopName });
            return Json(ShopData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCell(int ShopID)
        {
            var CellData = (from row in db.tblcells
                            where row.IsDeleted == 0 && row.ShopID == ShopID
                            select new { Value = row.CellID, Text = row.CellName });

            return Json(CellData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWC_Cell(int CellID)
        {
            var MachineData = (from row in db.tblProgramTransferDetailsMasters
                               where row.Isdeleted == 0 && row.CellId == CellID
                               select new { Value = row.PTdMID, Text = row.MachineDispName });
            return Json(MachineData, JsonRequestBehavior.AllowGet);
        }

        //To Delete the program from machine
        public string ProgramDelete(int MacID, string ProgNo, string FoldIds, int partId, int OpNo)
        {
            string retStatus = null;
            int userId = Convert.ToInt32(Session["UserId"]);
            try
            {
                var promasterdet = db.tblProgramTransferDetailsMasters.Where(m => m.PTdMID == MacID).Select(m => m.tblprogramType.TypeName).FirstOrDefault();
                if (promasterdet != null)
                {
                    if (promasterdet == "NCProgram")
                    {
                        object ip = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.IPAddress).FirstOrDefault();
                        String MacInv = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.MachineInvNo).FirstOrDefault();
                        ProgramTransfer PT = new ProgramTransfer(ip.ToString());
                        retStatus = PT.DeleteProgram(MacInv, ProgNo);
                        tbllogreport obj = new tbllogreport();
                        obj.LogDescription = "DeleteProgram";
                        obj.LogDate = DateTime.Now.Date;
                        obj.LogCapturedTime = DateTime.Now;
                        obj.MachineID = MacID;
                        obj.partNo = partId;
                        obj.operationNo = OpNo;
                        obj.programNumber = ProgNo;
                        obj.userId = userId;
                        db.tbllogreports.Add(obj);
                        db.SaveChanges();
                    }
                    else if (promasterdet == "FTP")
                    {
                        bool ret = false;
                        object ip = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.IPAddress).FirstOrDefault();
                        String MacInv = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.MachineInvNo).FirstOrDefault();
                        ProgramTransfer PT = new ProgramTransfer(ip.ToString());

                        var FTPMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == MacID).FirstOrDefault();
                        string Username = FTPMachineDet.UserName;
                        string Password = FTPMachineDet.Password;
                        string IPAddress = FTPMachineDet.IpAddress;
                        FTPclient objFtp = new FTPclient(IPAddress, Username, Password);
                        objFtp.CurrentDirectory = "/";
                        string delefile = "";
                        if (FoldIds != "")
                        {
                            string filepath = objFtp.Hostname + "/" + FoldIds + ProgNo;
                            delefile = FoldIds + ProgNo;

                        }
                        else
                        {
                            string filepath = objFtp.Hostname + ProgNo;
                            delefile = ProgNo;
                        }
                        //string filepath = objFtp.Hostname + ProgNo;

                        ret = objFtp.FtpDelete(delefile);
                        if (ret == false)
                        {
                            retStatus = "Success";
                            tbllogreport obj = new tbllogreport();
                            obj.LogDescription = "DeleteProgram";
                            obj.LogDate = DateTime.Now.Date;
                            obj.LogCapturedTime = DateTime.Now;
                            obj.MachineID = MacID;
                            obj.partNo = partId;
                            obj.operationNo = OpNo;
                            obj.programNumber = ProgNo;
                            obj.userId = userId;
                            db.tbllogreports.Add(obj);
                            db.SaveChanges();
                        }
                    }
                    else if (promasterdet == "NetworkSharing")
                    {
                        //validate the 
                        var NetWorkMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == MacID).FirstOrDefault();
                        string path1 = NetWorkMachineDet.MachineProgramPath;
                        string username1 = NetWorkMachineDet.UserName;
                        string password1 = NetWorkMachineDet.Password;
                        string domainname1 = NetWorkMachineDet.Domain;
                        try
                        {

                            //using (new Impersonator(username1, domainname1, password1))
                            //{
                            string filePath = path1 + "\\" + ProgNo;
                            bool fileExists = (System.IO.File.Exists(filePath) ? true : false);

                            if (fileExists == true)
                            {
                                System.IO.File.Delete(filePath);
                                retStatus = "Success";
                                tbllogreport obj = new tbllogreport();
                                obj.LogDescription = "DeleteProgram";
                                obj.LogDate = DateTime.Now.Date;
                                obj.LogCapturedTime = DateTime.Now;
                                obj.MachineID = MacID;
                                obj.partNo = partId;
                                obj.operationNo = OpNo;
                                obj.programNumber = ProgNo;
                                obj.userId = userId;
                                db.tbllogreports.Add(obj);
                                db.SaveChanges();
                            }

                            //}


                        }
                        catch (Exception e)
                        {

                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return retStatus;
        }

        [HttpPost]
        public string SaveProgramData(string updateOrsave, string ncProgramData, string MacId, string filename, int partId, int opNo)
        {
            string data = "success";
            if (filename == "" || filename == null)
            {
                data = "filenull";
            }
            else
            {
                if (filename.Contains('\r'))
                {
                    filename = filename.Trim();
                }
                string Path = @"C:\TataReport\ServerProgram";
                string filePath = Path + "\\" + filename;

                System.IO.File.Delete(filePath);

                int userId = 0;
                try
                {
                    userId = Convert.ToInt32(Session["UserId"]);
                }
                catch
                {

                }
                try
                {
                    int macid = Convert.ToInt32(MacId);

                    var fileCheck = db.tblNcProgramTransferMains.Where(m => m.ProgramNumber == filename).ToList();

                    if (fileCheck.Count == 0)
                    {//add fresh data
                        tblNcProgramTransferMain dataObj = new tblNcProgramTransferMain();
                        dataObj.McId = macid;
                        dataObj.ProgramNumber = filename;
                        dataObj.VersionNumber = 1;
                        dataObj.ProgramData = ncProgramData;
                        dataObj.CreatedDate = DateTime.Now;
                        dataObj.CorrectedDate = Convert.ToString(DateTime.Now.Date);
                        dataObj.CreatedBy = userId;
                        dataObj.PartId = partId;
                        dataObj.OperationNo = opNo;
                        dataObj.IsDeleted = false;
                        db.tblNcProgramTransferMains.Add(dataObj);
                        db.SaveChanges();

                        tbllogreport obj = new tbllogreport();
                        obj.LogDescription = "UploadToServer";
                        obj.LogDate = DateTime.Now.Date;
                        obj.LogCapturedTime = DateTime.Now;
                        obj.MachineID = macid;
                        obj.partNo = partId;
                        obj.operationNo = opNo;
                        obj.programNumber = filename;
                        obj.userId = userId;
                        db.tbllogreports.Add(obj);
                        db.SaveChanges();
                    }
                    else
                    {
                        //update old version or add new version
                        var item = db.tblNcProgramTransferMains.Where(m => m.ProgramNumber == filename).OrderByDescending(m => m.VersionNumber).Take(1).Single();
                        if (updateOrsave == "1")
                        {//update
                            item.ProgramData = ncProgramData;
                            db.SaveChanges();
                            tbllogreport obj = new tbllogreport();
                            obj.LogDescription = "UploadAsPreviousVersion";
                            obj.LogDate = DateTime.Now.Date;
                            obj.LogCapturedTime = DateTime.Now;
                            obj.MachineID = macid;
                            obj.partNo = partId;
                            obj.operationNo = opNo;
                            obj.version = item.VersionNumber;
                            obj.programNumber = filename;
                            obj.userId = userId;
                            db.tbllogreports.Add(obj);
                            db.SaveChanges();
                        }
                        else
                        {//add
                            int? versionNumber = item.VersionNumber;
                            tblNcProgramTransferMain dataObj = new tblNcProgramTransferMain();
                            dataObj.McId = macid;
                            dataObj.ProgramNumber = filename;
                            dataObj.VersionNumber = versionNumber + 1;
                            dataObj.ProgramData = ncProgramData;
                            dataObj.CreatedDate = DateTime.Now;
                            dataObj.CorrectedDate = Convert.ToString(DateTime.Now.Date);
                            dataObj.PartId = partId;
                            dataObj.OperationNo = opNo;
                            dataObj.CreatedBy = userId;
                            dataObj.IsDeleted = false;
                            db.tblNcProgramTransferMains.Add(dataObj);
                            db.SaveChanges();

                            tbllogreport obj = new tbllogreport();
                            obj.LogDescription = "UploadAsNewversion";
                            obj.LogDate = DateTime.Now.Date;
                            obj.LogCapturedTime = DateTime.Now;
                            obj.MachineID = macid;
                            obj.partNo = partId;
                            obj.operationNo = opNo;
                            obj.version = versionNumber + 1;
                            obj.programNumber = filename;
                            obj.userId = userId;
                            db.tbllogreports.Add(obj);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    data = "UnSuccessFul";
                }
            }
            return data;
        }

        public string PcToCNC(string ncProgramData, string MacId, string filename, string vernoval,int partId,int opNo)
        {
            string reply = "";
            int RetVal = 0;
            int userId = Convert.ToInt32(Session["UserId"]);
            int MacID = Convert.ToInt32(MacId);
            int versionNo = Convert.ToInt32(vernoval);

            string filelocation = @"D:\" + filename;
            System.IO.File.WriteAllText(filelocation, ncProgramData);

            String MachineInv = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.MachineInvNo).FirstOrDefault();
            var promasterdet = db.tblProgramTransferDetailsMasters.Where(m => m.PTdMID == MacID).Select(m => m.tblprogramType.TypeName).FirstOrDefault();
            if (promasterdet != null)
            {

                if (promasterdet == "NCProgram")
                {
                    //Log this event.
                    tblprogramtransferhistory pth = new tblprogramtransferhistory();
                    pth.IsDeleted = 0;
                    pth.MachineID = MacID;
                    pth.ProgramName = filename;
                    pth.UploadedTime = DateTime.Now;
                    pth.correcteddate = DateTime.Now.ToString("yyyy-MM-dd");
                    pth.Version = 1;
                    pth.PartId = partId;
                    pth.OperationNo = opNo;
                    pth.UserID = userId;
                    db.tblprogramtransferhistories.Add(pth);
                    db.SaveChanges();


                    //Based on WorkCenterID get IpAddress.
                    object ip = db.tblmachinedetails.Where(m => m.MachineID == MacID).Select(m => m.IPAddress).FirstOrDefault();
                    ushort port = 8193;            //  FOCAS1/Ethernet or FOCAS2/Ethernet (TCP) function
                    int timeout = 0;           //seconds if 0 infinitely waits

                    int pthID = pth.PTHID;

                    ProgramTransfer PT = new ProgramTransfer(ip.ToString());
                    string retString = PT.UploadCNCProgram(pthID, filelocation, out RetVal);

                    if (retString == "Success")
                    {
                        var pthData = db.tblprogramtransferhistories.Find(pthID);
                        if (pthData != null)
                        {
                            pthData.ReturnStatus = 1;
                            pthData.ReturnDesc = "Success";
                            pthData.ReturnTime = DateTime.Now;
                            pthData.IsCompleted = 1;
                            db.SaveChanges();
                            TempData["toaster_success"] = "The NC Program : " + filename.Split('.')[0].ToString() + " was successfully uploaded on the CNC Machine " + MachineInv;


                            tbllogreport obj = new tbllogreport();
                            obj.LogDescription = "UploadToCNC";
                            obj.LogDate = DateTime.Now.Date;
                            obj.LogCapturedTime = DateTime.Now;
                            obj.MachineID = MacID;
                            obj.partNo = partId;
                            obj.operationNo = opNo;
                            obj.version = Convert.ToInt32(vernoval);
                            obj.programNumber = filename;
                            obj.userId = userId;
                            db.tbllogreports.Add(obj);
                            db.SaveChanges();

                            System.IO.File.Delete(filelocation);
                        }
                        else //Hope fully this don't get executed.
                        {
                            reply = "Error";
                            retString = "Success but Unable to complete.";
                            TempData["toaster_warning"] = retString;
                        }
                    }
                    else //Upload failed.
                    {
                        var pthData = db.tblprogramtransferhistories.Find(pthID);
                        if (pthData != null)
                        {
                            pthData.ReturnStatus = 0;
                            pthData.ReturnDesc = retString;
                            pthData.ReturnTime = DateTime.Now;
                            pthData.IsCompleted = 0;
                            db.SaveChanges();

                            
                        }
                        else //Hope fully this don't get executed.
                        {
                            retString += "Failure and Unable to complete.";
                            TempData["toaster_error"] = retString;
                        }
                    }
                }

                else if (promasterdet == "FTP")
                {
                    tblprogramtransferhistory pth = new tblprogramtransferhistory();
                    pth.IsDeleted = 0;
                    pth.MachineID = MacID;
                    pth.ProgramName = filename;
                    pth.UploadedTime = DateTime.Now;
                    pth.correcteddate = DateTime.Now.ToString("yyyy-MM-dd");
                    pth.Version = 1;
                    pth.UserID = userId;
                    pth.PartId = partId;
                    pth.OperationNo = opNo;
                    db.tblprogramtransferhistories.Add(pth);
                    db.SaveChanges();

                    int pthID = pth.PTHID;

                    var FTPMachineDet = new tblProgramTransferDetailsMaster();

                    FTPMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == MacID).FirstOrDefault();


                    string Username = FTPMachineDet.UserName;
                    string Password = FTPMachineDet.Password;
                    string IPAddress = FTPMachineDet.IpAddress;
                    FTPclient objFtp = new FTPclient(IPAddress, Username, Password);
                    objFtp.CurrentDirectory = "/";
                    FtpMain Main = new FtpMain();

                    //Set FTP Client in MAIN form
                    Main.SetFtpClient(objFtp);
                    fileupload_ftp objftp = new fileupload_ftp();
                    string retString = objftp.fileuploadftp(filelocation, objFtp.CurrentDirectory, objFtp);
                    if (retString == "Success")
                    {
                        var pthData = db.tblprogramtransferhistories.Find(pthID);
                        if (pthData != null)
                        {
                            pthData.ReturnStatus = 1;
                            pthData.ReturnDesc = "Success";
                            pthData.ReturnTime = DateTime.Now;
                            pthData.IsCompleted = 1;
                            db.SaveChanges();
                            TempData["toaster_success"] = "The Program : " + filename.Split('.')[0].ToString() + " was successfully uploaded on the FTP Machine " + MachineInv;

                            tbllogreport obj = new tbllogreport();
                            obj.LogDescription = "UploadToCNC";
                            obj.LogDate = DateTime.Now.Date;
                            obj.LogCapturedTime = DateTime.Now;
                            obj.MachineID = MacID;
                            obj.partNo = partId;
                            obj.operationNo = opNo;
                            obj.version = Convert.ToInt32(vernoval);
                            obj.programNumber = filename;
                            obj.userId = userId;
                            db.tbllogreports.Add(obj);
                            db.SaveChanges();

                            System.IO.File.Delete(filelocation);
                        }
                        else
                        {
                            retString += "Failure and Unable to complete.";
                            TempData["toaster_error"] = retString;
                        }
                    }

                }

                else if (promasterdet == "NetworkSharing")
                {
                    tblprogramtransferhistory pth = new tblprogramtransferhistory();
                    pth.IsDeleted = 0;
                    pth.MachineID = MacID;
                    pth.ProgramName = filename;
                    pth.UploadedTime = DateTime.Now;
                    pth.correcteddate = DateTime.Now.ToString("yyyy-MM-dd");
                    pth.Version = 1;
                    pth.PartId = partId;
                    pth.OperationNo = opNo;
                    pth.UserID = userId;
                    db.tblprogramtransferhistories.Add(pth);
                    db.SaveChanges();

                    int pthID = pth.PTHID;

                    //validate the 
                    var NetWorkMachineDet = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == MacID).FirstOrDefault();
                    string path1 = NetWorkMachineDet.MachineProgramPath;
                    string username1 = NetWorkMachineDet.UserName;
                    string password1 = NetWorkMachineDet.Password;
                    string domainname1 = NetWorkMachineDet.Domain;
                    try
                    {

                        //using (new Impersonator(username1, domainname1, password1))
                        //{
                        string filePath = path1 + "\\" + filename;

                        System.IO.File.Copy(filelocation, filePath, true);

                        var pthData = db.tblprogramtransferhistories.Find(pthID);
                        if (pthData != null)
                        {
                            pthData.ReturnStatus = 1;
                            pthData.ReturnDesc = "Success";
                            pthData.ReturnTime = DateTime.Now;
                            pthData.IsCompleted = 1;
                            db.SaveChanges();
                            TempData["toaster_success"] = "The Program : " + filename.Split('.')[0].ToString() + " wsa successfully uploaded on the Network Sharing " + MachineInv;

                            tbllogreport obj = new tbllogreport();
                            obj.LogDescription = "UploadToCNC";
                            obj.LogDate = DateTime.Now.Date;
                            obj.LogCapturedTime = DateTime.Now;
                            obj.MachineID = MacID;
                            obj.partNo = partId;
                            obj.operationNo = opNo;
                            obj.version = Convert.ToInt32(vernoval);
                            obj.programNumber = filename;
                            obj.userId = userId;
                            db.tbllogreports.Add(obj);
                            db.SaveChanges();

                            System.IO.File.Delete(filelocation);
                        }

                        //}


                    }
                    catch (Exception e)
                    {

                    }

                }

            }




            return reply;
        }
    }

    public class DownloadNCProg
    {
        i_facility_talEntities db = new i_facility_talEntities();

        static object ip;   // "192.168.0.1" or "CNC-1.FACTORY"
        static ushort port;            //  FOCAS1/Ethernet or FOCAS2/Ethernet (TCP) function
        static int timeout;           //seconds if 0 infinitely waits
        static ushort FlibHndl;
        string FilePath;
        //ushort h;
        //Focas1.ODBST buf;


        public DownloadNCProg(object ip1, ushort port1, int timeout1, string FilePath)
        {
            ip = ip1;                 // "192.168.0.1" or "CNC-1.FACTORY"
            port = port1;            //  FOCAS1/Ethernet or FOCAS2/Ethernet (TCP) function
            timeout = timeout1;           //seconds if 0 infinitely waits
            //ushort FlibHndl = FlibHndl1; 
            this.FilePath = FilePath;
        }

        //DNC Program
        //public string BeginCalculation()
        //{
        //    string retValue = null;
        //    //try
        //    //{
        //    //    //Focas1.cnc_allclibhndl3(ip,  port,  timeout, out FlibHndl);
        //    //    Focas1.focas_ret retallclibhndl3 = (Focas1.focas_ret)Focas1.cnc_allclibhndl3(ip, port, timeout, out FlibHndl); //¨ú±olibrary handle 
        //    //    if (retallclibhndl3 == Focas1.focas_ret.EW_OK)
        //    //    {
        //    //        //send data to Machine
        //    //        //1) notify about start : FWLIBAPI short WINAPI cnc_dncstart(unsigned short FlibHndl);
        //    //        //Note : CNC parameters must be set. Mind it
        //    //        Focas1.focas_ret retdncstart = (Focas1.focas_ret)Focas1.cnc_dncstart(FlibHndl);
        //    //        if (retdncstart == Focas1.focas_ret.EW_OK)
        //    //        {
        //    //            //send nc command data to cnc(dnc)
        //    //            //FWLIBAPI short WINAPI cnc_cdnc(unsigned short FlibHndl,char *data, short number);
        //    //            //For example, to execute the commands such as
        //    //            //M3 S2000 ;        // T14 ;        // G0 X10. ;          // G0 Z-5. ;      // M30 ;  

        //    //            //send a following string using cnc_dnc function.
        //    //            //cnc_dnc( "\nM3S2000\nT14\nG0X10.\nG0Z-5.\nM30\n%", 32 ) ;  The string data can be sent by multiple cnc_dnc functions.
        //    //            // For above example, the commands can be sent block by block like this.
        //    //            //cnc_dnc( "\n", 1 ) ;      // cnc_dnc( "M3S2000\n", 8 ) ;
        //    //            // cnc_dnc( "T14\n", 4 ) ;       // cnc_dnc( "G0X10.\n", 7 ) ;
        //    //            // cnc_dnc( "G0Z-5.\n", 7 ) ;        // cnc_dnc( "M30\n", 4 ) ;      
        //    //            // cnc_dnc( "%", 1 ) ; 

        //    //            ushort number = 0;
        //    //            string DataString = "\nM3S2000\nT14\nG0X10.\nG0Z-5.\nM30\n%";

        //    //            //number = (ushort)System.Text.ASCIIEncoding.Unicode.GetByteCount(DataString);
        //    //            //number = System.Text.ASCIIEncoding.ASCII.GetByteCount(DataString);

        //    //            //As per example in given xml files (Program\cnc_dnc2.htm)
        //    //            number = (ushort)DataString.Length;

        //    //            //In a loop, keep downloading.
        //    //            var fileStream = new FileStream(@"c:\file.txt", FileMode.Open, FileAccess.Read);
        //    //            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
        //    //            {
        //    //                string line;
        //    //                while ((line = streamReader.ReadLine()) != null)
        //    //                {
        //    //                    // process the line
        //    //                    Object Data = line;
        //    //                    number = (ushort)DataString.Length;
        //    //                    Focas1.focas_ret retcdnc = (Focas1.focas_ret)Focas1.cnc_cdnc(FlibHndl, Data, number);

        //    //                    if (retcdnc == Focas1.focas_ret.EW_FUNC)
        //    //                    {
        //    //                        TextLogFile("cnc_dncstart function has not been executed. " + retcdnc.ToString());
        //    //                    }
        //    //                    else if (retcdnc == Focas1.focas_ret.EW_RESET)
        //    //                    {
        //    //                        TextLogFile("Reset or stop request. " + retcdnc.ToString());
        //    //                    }
        //    //                    else if (retcdnc == Focas1.focas_ret.EW_LENGTH)
        //    //                    {
        //    //                        TextLogFile("Data block length error. " + retcdnc.ToString());
        //    //                    }

        //    //                    //End dncstart: FWLIBAPI short WINAPI cnc_dncend(unsigned short FlibHndl);
        //    //                    Focas1.focas_ret retdncend = (Focas1.focas_ret)Focas1.cnc_dncend(FlibHndl);
        //    //                    if (retdncend == Focas1.focas_ret.EW_FUNC)
        //    //                    {
        //    //                        TextLogFile("cnc_dncstart function has not been executed. " + retdncend.ToString());
        //    //                    }
        //    //                    else if (retdncend == Focas1.focas_ret.EW_DATA)
        //    //                    {
        //    //                        TextLogFile("A character which is unavailable for NC program is detected. " + retdncend.ToString());
        //    //                    }
        //    //                }
        //    //            }



        //    //        }
        //    //        else if (retdncstart == Focas1.focas_ret.EW_BUSY)
        //    //        {
        //    //            TextLogFile("c_dncstart function has been executed. " + retdncstart.ToString());

        //    //            //End dncstart: FWLIBAPI short WINAPI cnc_dncend(unsigned short FlibHndl);
        //    //            Focas1.focas_ret retdncend = (Focas1.focas_ret)Focas1.cnc_dncend(FlibHndl);
        //    //            if (retdncend == Focas1.focas_ret.EW_FUNC)
        //    //            {
        //    //                TextLogFile("cnc_dncstart function has not been executed. " + retdncend.ToString());
        //    //            }
        //    //            else if (retdncend == Focas1.focas_ret.EW_DATA)
        //    //            {
        //    //                TextLogFile("A character which is unavailable for NC program is detected. " + retdncend.ToString());
        //    //            }

        //    //        }
        //    //        else if (retdncstart == Focas1.focas_ret.EW_PARAM)
        //    //        {
        //    //            TextLogFile("CNC parameter error. " + retdncstart.ToString());
        //    //        }

        //    //        Focas1.cnc_freelibhndl(h);
        //    //        retValue += retallclibhndl3.ToString();
        //    //    }
        //    //    else if (retallclibhndl3 == Focas1.focas_ret.EW_SOCKET)
        //    //    {
        //    //        TextLogFile("Socket communication error. " + retallclibhndl3.ToString());
        //    //    }
        //    //    else if (retallclibhndl3 == Focas1.focas_ret.EW_NODLL)
        //    //    {
        //    //        TextLogFile("There is no DLL file for each CNC series . " + retallclibhndl3.ToString());
        //    //    }
        //    //    else if (retallclibhndl3 == Focas1.focas_ret.EW_HANDLE)
        //    //    {
        //    //        TextLogFile("Allocation of handle number is failed. " + retallclibhndl3.ToString());
        //    //    }

        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    retValue += e.ToString();
        //    //}

        //    return retValue;
        //}

        //CNC Program 
        public string UploadCNCProgram(int pthID, out int retValueInt)
        {
            string retValue = null;
            //int retStatusInt = 0; //failure.
            retValueInt = 0; //EW_OK.
            try
            {
                Focas1.focas_ret retallclibhndl3 = (Focas1.focas_ret)Focas1.cnc_allclibhndl3(ip, port, timeout, out FlibHndl); //¨ú±olibrary handle 
                if (retallclibhndl3 == Focas1.focas_ret.EW_OK)
                {
                    retValueInt = (int)Focas1.focas_ret.EW_OK;
                    short type = 0;
                    var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                    using (StreamReader sr = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        StringBuilder DataString = new StringBuilder();
                        while (!sr.EndOfStream)
                        {
                            //read the data line by line into Dictionary, then access it.
                            string line = null;
                            Dictionary<int, string> AllData = new Dictionary<int, string>();
                            int LineNo = 1;
                            while ((line = sr.ReadLine()) != null)
                            {
                                AllData.Add(LineNo, line + "\n");
                                LineNo++;
                            }
                            Focas1.focas_ret retncstart = (Focas1.focas_ret)Focas1.cnc_dwnstart3(FlibHndl, type);
                            if (retncstart == Focas1.focas_ret.EW_OK)
                            {
                                retValueInt = (int)Focas1.focas_ret.EW_OK;
                                for (int row = 1; row < AllData.Count; row++)
                                {
                                    if (DataString != null)
                                    {
                                        DataString.Clear();
                                    }
                                    if (row == 0)
                                    {
                                        DataString.Append("\n");
                                    }
                                    line = null;
                                    int stringSize = DataString.Length;
                                    ushort currentLineSize = line != null ? (ushort)line.Length : (ushort)0;
                                    while (stringSize + currentLineSize < 250) //current line + old data size < 256
                                    {
                                        if (line != null)
                                        {
                                            DataString.Append(line); //include currentline
                                        }
                                        stringSize = DataString.Length;
                                        line = null;
                                        if (AllData.ContainsKey(row))
                                        {
                                            line = AllData[row++];
                                        }
                                        else // Add "%" at the end of Program.
                                        {
                                            DataString.Append("%");
                                            break;
                                        }
                                        currentLineSize = (ushort)line.Length;
                                    }
                                    row -= 2; //Above whileLoop fails only after more data, so don't consider last row + Increment in the ForLoop so -1 => Total -2.
                                    // send cur+-rent data
                                    stringSize = DataString.Length;
                                    Object Data = DataString;
                                    {
                                        Focas1.focas_ret retcnc = (Focas1.focas_ret)Focas1.cnc_download3(FlibHndl, ref stringSize, Data);
                                        if (retcnc == Focas1.focas_ret.EW_OK)
                                        {
                                            retValue = "Executed Successfully. " + retcnc.ToString();
                                            retValueInt = (int)Focas1.focas_ret.EW_OK;
                                        }
                                        else
                                        {

                                            if (retcnc == Focas1.focas_ret.EW_FUNC)
                                            {
                                                retValueInt = (int)Focas1.focas_ret.EW_FUNC;
                                                retValue = "cnc_dwnstart3 function has not been executed. " + retcnc.ToString();
                                            }
                                            else if (retcnc == Focas1.focas_ret.EW_RESET)
                                            {
                                                retValueInt = (int)Focas1.focas_ret.EW_RESET;
                                                retValue = "Reset or stop request. " + retcnc.ToString();
                                            }
                                            else if (retcnc == Focas1.focas_ret.EW_LENGTH)
                                            {
                                                retValueInt = (int)Focas1.focas_ret.EW_LENGTH;
                                                retValue = "The size of character string is negative. " + retcnc.ToString();
                                            }
                                            else if (retcnc == Focas1.focas_ret.EW_DATA)
                                            {
                                                retValueInt = (int)Focas1.focas_ret.EW_DATA;
                                                retValue = "Data error. " + retcnc.ToString();
                                            }
                                            else if (retcnc == Focas1.focas_ret.EW_PROT)
                                            {
                                                retValueInt = (int)Focas1.focas_ret.EW_PROT;
                                                retValue = "Tape memory is write-protected by the CNC parameter setting. " + retcnc.ToString();
                                            }
                                            else if (retcnc == Focas1.focas_ret.EW_OVRFLOW)
                                            {
                                                retValueInt = (int)Focas1.focas_ret.EW_OVRFLOW;
                                                retValue = "Make enough free area in CNC memory. " + retcnc.ToString();
                                            }
                                            else if (retcnc == Focas1.focas_ret.EW_BUFFER)
                                            {
                                                retValueInt = (int)Focas1.focas_ret.EW_BUFFER;
                                                retValue = "Retry because the buffer is full. " + retcnc.ToString();
                                            }
                                            else if (retcnc == Focas1.focas_ret.EW_REJECT)
                                            {
                                                retValueInt = (int)Focas1.focas_ret.EW_REJECT;
                                                retValue = "Downloading is disable in the current CNC status " + retcnc.ToString();
                                            }
                                            else if (retcnc == Focas1.focas_ret.EW_ALARM)
                                            {
                                                retValueInt = (int)Focas1.focas_ret.EW_ALARM;
                                                retValue = "Alarm has occurred while downloading. " + retcnc.ToString();
                                            }

                                        }
                                        retValue = retcnc.ToString();
                                    }
                                }

                                Focas1.focas_ret retncend = (Focas1.focas_ret)Focas1.cnc_dwnend3(FlibHndl);
                                if (retncend == Focas1.focas_ret.EW_OK)
                                {
                                    retValueInt = (int)Focas1.focas_ret.EW_OK;
                                    retValue = "Success";
                                    retValue = "cnc_dwnend3 executed succesfully " + retncend.ToString();
                                }
                                else
                                {
                                    if (retncend == Focas1.focas_ret.EW_FUNC)
                                    {
                                        retValueInt = (int)Focas1.focas_ret.EW_FUNC;
                                        retValue = "cnc_dwnstart3 function has not been executed. " + retncend.ToString();
                                    }
                                    else if (retncend == Focas1.focas_ret.EW_DATA)
                                    {
                                        retValueInt = (int)Focas1.focas_ret.EW_DATA;
                                        retValue = "Data error. " + retncend.ToString();
                                    }
                                    else if (retncend == Focas1.focas_ret.EW_OVRFLOW)
                                    {
                                        retValueInt = (int)Focas1.focas_ret.EW_OVRFLOW;
                                        retValue = "Make enough free area in CNC memory. " + retncend.ToString();
                                    }
                                    else if (retncend == Focas1.focas_ret.EW_PROT)
                                    {
                                        retValueInt = (int)Focas1.focas_ret.EW_PROT;
                                        retValue = "Tape memory is write-protected by the CNC parameter setting. " + retncend.ToString();
                                    }
                                    else if (retncend == Focas1.focas_ret.EW_REJECT)
                                    {
                                        retValueInt = (int)Focas1.focas_ret.EW_REJECT;
                                        retValue = "Downloading is disable in the current CNC status. " + retncend.ToString();
                                    }
                                    else if (retncend == Focas1.focas_ret.EW_ALARM)
                                    {
                                        retValueInt = (int)Focas1.focas_ret.EW_ALARM;
                                        retValue = "Alarm has occurred while downloading. " + retncend.ToString();
                                    }

                                }
                                retValue = retncend.ToString();
                            }
                            else
                            {
                                if (retncstart == Focas1.focas_ret.EW_BUSY)
                                {
                                    retValueInt = (int)Focas1.focas_ret.EW_BUSY;
                                    retValue = "Busy. " + retncstart.ToString();
                                }
                                else if (retncstart == Focas1.focas_ret.EW_ATTRIB)
                                {
                                    retValueInt = (int)Focas1.focas_ret.EW_ATTRIB;
                                    retValue = "Data type (type) is illegal. " + retncstart.ToString();
                                }
                                else if (retncstart == Focas1.focas_ret.EW_NOOPT)
                                {
                                    retValueInt = (int)Focas1.focas_ret.EW_NOOPT;
                                    retValue = "No option. " + retncstart.ToString();
                                }
                                else if (retncstart == Focas1.focas_ret.EW_PARAM)
                                {
                                    retValueInt = (int)Focas1.focas_ret.EW_PARAM;
                                    retValue = "CNC parameter error. " + retncstart.ToString();
                                }
                                else if (retncstart == Focas1.focas_ret.EW_MODE)
                                {
                                    retValueInt = (int)Focas1.focas_ret.EW_MODE;
                                    retValue = "CNC mode error. " + retncstart.ToString();
                                }
                                else if (retncstart == Focas1.focas_ret.EW_REJECT)
                                {
                                    retValueInt = (int)Focas1.focas_ret.EW_REJECT;
                                    retValue = "CNC is machining, so Rejected. " + retncstart.ToString();
                                }
                                else if (retncstart == Focas1.focas_ret.EW_ALARM)
                                {
                                    retValueInt = (int)Focas1.focas_ret.EW_ALARM;
                                    retValue = "Alarm State error, reset the alarm on CNC. " + retncstart.ToString();
                                }
                                else if (retncstart == Focas1.focas_ret.EW_PASSWD)
                                {
                                    retValueInt = (int)Focas1.focas_ret.EW_PASSWD;
                                    retValue = "Specified CNC data cannot be written because the data is protected.. " + retncstart.ToString();
                                }

                            }
                            retValue = retncstart.ToString();
                        }
                    }
                }
                else
                {
                    if (retallclibhndl3 == Focas1.focas_ret.EW_SOCKET)
                    {
                        retValueInt = (int)Focas1.focas_ret.EW_SOCKET;
                        retValue = "Socket communication error. " + retallclibhndl3.ToString();
                    }
                    else if (retallclibhndl3 == Focas1.focas_ret.EW_NODLL)
                    {
                        retValueInt = (int)Focas1.focas_ret.EW_NODLL;
                        retValue = "There is no DLL file for each CNC series . " + retallclibhndl3.ToString();
                    }
                    else if (retallclibhndl3 == Focas1.focas_ret.EW_HANDLE)
                    {
                        retValueInt = (int)Focas1.focas_ret.EW_HANDLE;
                        retValue = "Allocation of handle number is failed. " + retallclibhndl3.ToString();
                    }

                    retValue = retallclibhndl3.ToString();
                }

                using (i_facility_talEntities redb = new i_facility_talEntities())
                {
                    var RecordToUpdate = redb.tblprogramtransferhistories.Find(pthID);
                    if (RecordToUpdate != null)
                    {
                        RecordToUpdate.ReturnTime = DateTime.Now;
                        RecordToUpdate.ReturnStatus = retValueInt;
                        RecordToUpdate.ReturnDesc = retValue;
                        redb.Entry(RecordToUpdate).State = System.Data.Entity.EntityState.Modified;
                        redb.SaveChanges();

                    }
                    else
                    {
                        //TextLogFile("Unable to Find Latest Record to update Error/EndTime.");
                    }
                }

            }
            catch (Exception e)
            {
                retValue += e.ToString();
            }

            return retValue;
        }

        //public void TextLogFile(string msg)
        //{
        //    string FileName = @"~/Content/LogFile.txt";
        //    StreamWriter sw = new StreamWriter(FileName, true);
        //    sw.WriteLine("" + DateTime.Now);
        //    sw.WriteLine("" + msg);

        //}

    }

}
