using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using UnitWorksCCS.Models;

namespace UnitWorksCCS.Controllers
{
    public class MachinesController : Controller
    {
        private i_facility_talEntities db = new i_facility_talEntities();
        //getting machine list
        [HttpGet]
        public ActionResult MachineList()
        {

            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"];
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();
            ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewBag.Shop = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopID", "ShopName");
            ViewBag.Cell = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellID", "CellName");
            ViewBag.ProgramType = new SelectList(db.tblprogramTypes.Where(m => m.Isdeleted == 0), "ptypeid", "TypeName");
            //ViewBag.
            MachineModel pa = new MachineModel();
            tblProgramTransferDetailsMaster mp = new tblProgramTransferDetailsMaster();
            pa.PT = mp;
            pa.PTList = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0).ToList();
            return View(pa);


        }

        [HttpGet]
        public ActionResult CreateMachine()
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }
            ViewBag.Logout = Session["Username"];
            ViewBag.roleid = Session["RoleID"];
            String Username = Session["Username"].ToString();

            ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewBag.Shop = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopID", "ShopName");
            ViewBag.Cell = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellID", "CellName");
            ViewBag.ProgramType = new SelectList(db.tblprogramTypes.Where(m => m.Isdeleted == 0), "ptypeid", "TypeName");
            return View();
        }

        [HttpPost]
        public string CreateData(int plantid, int shopid, int cellid, string MachineInvNo, string MachineModel, string ControllerType, string MachineDispName, string MachineMake, int programtype, string ipaddress, string username, string password, int port, string domain)
        {
            string res = "";

            tblProgramTransferDetailsMaster obj = new tblProgramTransferDetailsMaster();
            obj.CellId = cellid;
            obj.PlantID = plantid;
            obj.Shopid = shopid;
            obj.ProgramType = programtype;
            obj.IpAddress = ipaddress;
            obj.UserName = username;
            obj.Password = password;
            obj.Port = port;
            obj.MachineDispName = MachineDispName;
            obj.MachineInvNo = MachineInvNo;
            obj.MachineMake = MachineMake;
            obj.MachineModel = MachineModel;
            obj.ControllerType = ControllerType;
            obj.Domain = domain;
            obj.CreatedOn = DateTime.Now;
            obj.CreatedBy = Convert.ToInt32( Session["UserId"]);
            using (i_facility_talEntities db = new i_facility_talEntities())
            {
                db.tblProgramTransferDetailsMasters.Add(obj);
                db.SaveChanges();
            }
            res = "Success";


            return res;
        }

        public JsonResult GetElemet(int Id)
        {
            var Data = db.tblProgramTransferDetailsMasters.Where(m => m.PTdMID == Id).Select(m => new { programtype = m.ProgramType, IpAddres = m.IpAddress, UserName = m.UserName, password = m.Password, Port = m.Port, domain = m.Domain });
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public string GetProgramMachineDetails(int ID)
        {
            string res = "";
            var  Data = db.tblProgramTransferDetailsMasters.Where(m => m.PTdMID == ID).Select(m=>new { m.PlantID,m.Shopid,m.CellId,m.MachineDispName,m.MachineInvNo,m.MachineMake,m.MachineModel,m.ControllerType,m.IpAddress,m.Port,m.Domain,m.MachineProgramPath,m.UserName,m.Password,m.ProgramType}).FirstOrDefault();
            res = JsonConvert.SerializeObject(Data);
            return res;
        }

       
        public ActionResult EditMachine(int Id)
        {
            if ((Session["UserId"] == null) || (Session["UserId"].ToString() == String.Empty))
            {
                return RedirectToAction("Login", "Login", null);
            }

            tblProgramTransferDetailsMaster machine = db.tblProgramTransferDetailsMasters.Find(Id);
            if (machine == null)
            {
                return HttpNotFound();
            }
            int plantid = Convert.ToInt32(machine.PlantID);
            ViewBag.Plant = new SelectList(db.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName", machine.PlantID);
            ViewBag.dept = new SelectList(db.tblshops.Where(m => m.IsDeleted == 0), "ShopId", "ShopName", machine.Shopid);
            ViewBag.cell = new SelectList(db.tblcells.Where(m => m.IsDeleted == 0), "CellID", "CellName", machine.CellId);
            ViewBag.ProgramType = new SelectList(db.tblprogramTypes.Where(m => m.Isdeleted == 0), "ptypeid", "TypeName", machine.ProgramType);
            return View(machine);
        }
        //Update Machine
        public string EditData(int id, int plantid, int shopid, int cellid, string MachineInvNo, string ControllerType, string MachineDispName,string MachineMake, string MachineModel, int programtype, string ipaddress, string username, string password, int port, string domain)
        {
            string res = "";
            var obj = db.tblProgramTransferDetailsMasters.Where(m => m.Isdeleted == 0 && m.PTdMID == id).FirstOrDefault();
            if (obj != null)
            {
                obj.CellId = cellid;
                obj.PlantID = plantid;
                obj.Shopid = shopid;
                obj.ProgramType = programtype;
                obj.IpAddress = ipaddress;
                obj.UserName = username;
                obj.Password = password;
                obj.Port = port;
                obj.MachineDispName = MachineDispName;
                obj.MachineInvNo = MachineInvNo;
                obj.MachineMake = MachineMake;
                obj.MachineModel = MachineModel;
                obj.ControllerType = ControllerType;
                obj.Domain = domain;
                db.SaveChanges();
                res = "Success";
            }

            return res;
        }


        //Delete Machine
        public ActionResult DeleteMachine(int id)
        {
            
            tblProgramTransferDetailsMaster tblmc = db.tblProgramTransferDetailsMasters.Find(id);
           
            tblmc.Isdeleted = 1;
            tblmc.ModifiedBy = 1;
            tblmc.ModifiedOn = DateTime.Now;
            db.Entry(tblmc).State = EntityState.Modified;
            db.SaveChanges();

           

            return RedirectToAction("MachineList");
        }

        
        //Machine Table End

        public JsonResult FetchDept(int PID)
        {
            var DeptData = (from row in db.tblshops
                            where row.IsDeleted == 0 && row.PlantID == PID
                            select new { Value = row.ShopID, Text = row.ShopName }
                                );
            return Json(DeptData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FetchCat(int DeptID)
        {
            var CatData = (from row in db.tblcells
                           where row.IsDeleted == 0 && row.ShopID == DeptID
                           select new { Value = row.CellID, Text = row.CellName }
                                );
            return Json(CatData, JsonRequestBehavior.AllowGet);
        }



        // Get Plant Details
        public string GetPlant()
        {
            string res = "";

            var plant = db.tblplants.Where(p => p.IsDeleted == 0).Select(m => new { m.PlantID, m.PlantName }).ToList();
            res = JsonConvert.SerializeObject(plant);
            return res;
        }

        // Get Shop Details
        public string GetShop(int PlantID)
        {
            string res = "";

            var shop = db.tblshops.Where(p => p.IsDeleted == 0 && p.PlantID == PlantID).Select(m => new { m.ShopID, m.ShopName }).ToList();
            res = JsonConvert.SerializeObject(shop);
            return res;
        }

        //Get Cell Details
        public string GetCell(int ShopID)
        {
            string res = "";

            var cell = db.tblcells.Where(p => p.IsDeleted == 0 && p.ShopID == ShopID).Select(m => new { m.CellID, m.CellName }).ToList();
            res = JsonConvert.SerializeObject(cell);
            return res;
        }

        //GetProgramTypeDetails
        public string GetProgramTypeDetails(int PtypeID)
        {
            string res = "";

            var cell = db.tblprogramTypes.Where(p => p.Isdeleted == 0 ).Select(m => new { m.ptypeid, m.TypeName }).ToList();
            res = JsonConvert.SerializeObject(cell);
            return res;
        }
    }
}