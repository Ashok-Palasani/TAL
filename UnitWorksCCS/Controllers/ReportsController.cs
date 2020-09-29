using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using System.IO;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data.SqlClient;
using UnitWorksCCS.Models;
using UnitWorksCCS.Models;
using static System.Net.Mime.MediaTypeNames;

namespace UnitWorksCCS.Controllers
{
    public class ReportsController : Controller
    {
        UnitWorksCCS.i_facility_talEntities Serverdb = new UnitWorksCCS.i_facility_talEntities();

        //GET: Reports

        public ActionResult ProgramTransferReport()
        {
            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");
            ViewData["PartNo"] = new SelectList(Serverdb.tblparts.Where(m => m.IsDeleted == 0), "PartID", "PartNo");
            return View();
        }

        [HttpPost]
        public ActionResult ProgramTransferReport(string FromDate, string Todate, int PlantID = 0, int ShopID = 0, int CellID = 0, int MachineID = 0, int PartNo = 0, int OperationNo = 0)
        {

            var getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0).ToList();

            if (MachineID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == MachineID).ToList();
            }
            else if (CellID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
            }
            else if (ShopID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID).ToList();
            }

            int dateDifference = Convert.ToDateTime(Todate).Subtract(Convert.ToDateTime(FromDate)).Days;

            FileInfo templateFile = new FileInfo(@"C:\TataReport\NewTemplates\ProgramTransferDetailsReportForSupervisor.xlsx");

            ExcelPackage templatep = new ExcelPackage(templateFile);
            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];

            String FileDir = @"C:\TataReport\ReportsList\" + Convert.ToDateTime(Todate).ToString("yyyy-MM-dd");
            bool exists = System.IO.Directory.Exists(FileDir);
            if (!exists)
                System.IO.Directory.CreateDirectory(FileDir);

            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "ProgramTransferDetailsReportForSupervisor" + Convert.ToDateTime(Todate).ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "ProgramTransferDetailsReportForSupervisor" + Convert.ToDateTime(Todate).ToString("yyyy-MM-dd") + ".xlsx"));
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
                    return View();
                }
            }
            //Using the File for generation and populating it
            ExcelPackage p = null;
            p = new ExcelPackage(newFile);
            ExcelWorksheet worksheet = null;

            //Creating the WorkSheet for populating
            try
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(Todate).ToString("dd-MM-yyyy"), Templatews);
            }
            catch { }

            if (worksheet == null)
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(Todate).ToString("dd-MM-yyyy") + "1", Templatews);
            }

            int sheetcount = p.Workbook.Worksheets.Count;
            p.Workbook.Worksheets.MoveToStart(sheetcount);
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            int StartRow = 5;
            int SlNo = 1;
            for (int i = 0; i <= dateDifference; i++)
            {
                DateTime QueryDate = Convert.ToDateTime(FromDate).AddDays(i);
                string correctedDate = QueryDate.ToString("yyyy-MM-dd");
                foreach (var Machine in getMachineList)
                {
                    var GetUtilList = Serverdb.tblprogramtransferhistories.Where(m => m.MachineID == Machine.MachineID && m.correcteddate == correctedDate && m.IsCompleted == 1).ToList();
                    foreach (var MacRow in GetUtilList)
                    {
                        List<string> hyrachy = GetHierarchyData1(MachineID);
                        var partNo = Serverdb.tblparts.Where(m => m.PartID == MacRow.PartId).FirstOrDefault();
                        var username = Serverdb.tblusers.Where(m => m.UserID == MacRow.UserID).FirstOrDefault();
                        worksheet.Cells["A" + StartRow].Value = SlNo++;
                        worksheet.Cells["B" + StartRow].Value = hyrachy[0];
                        worksheet.Cells["C" + StartRow].Value = hyrachy[1];
                        worksheet.Cells["D" + StartRow].Value = hyrachy[2];
                        worksheet.Cells["E" + StartRow].Value = hyrachy[3];
                        worksheet.Cells["F" + StartRow].Value = hyrachy[4];
                        worksheet.Cells["G" + StartRow].Value = correctedDate;
                        worksheet.Cells["H" + StartRow].Value = partNo.PartNo;
                        worksheet.Cells["I" + StartRow].Value = MacRow.OperationNo;
                        worksheet.Cells["J" + StartRow].Value = MacRow.ProgramName;
                        worksheet.Cells["K" + StartRow].Value = MacRow.Version;
                        worksheet.Cells["L" + StartRow].Value = MacRow.ReturnTime;
                        worksheet.Cells["M" + StartRow].Value = username.UserName;
                        worksheet.Cells["N" + StartRow].Value = MacRow.Message;
                        StartRow++;
                    }
                }
            }
            p.Save();

            //Downloding Excel
            string path1 = System.IO.Path.Combine(FileDir, "ProgramTransferDetailsReportForSupervisor" + FromDate + ".xlsx");
            System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
            //string Outgoingfile = "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx";
            string Outgoingfile = "ProgramTransferDetailsReportForSupervisor" + FromDate + ".xlsx";
            if (file1.Exists)
            {
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                Response.AddHeader("Content-Length", file1.Length.ToString());
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.WriteFile(file1.FullName);
                Response.Flush();
                Response.Close();
            }
            return View();
        }


        public ActionResult ProgramTransferReportForProgrammer()
        {
            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");
            ViewData["PartNo"] = new SelectList(Serverdb.tblparts.Where(m => m.IsDeleted == 0), "PartID", "PartNo");
            return View();
        }

        [HttpPost]
        public ActionResult ProgramTransferReportForProgrammer(string FromDate, string Todate, int PlantID = 0, int ShopID = 0, int CellID = 0, int MachineID = 0, int PartNo = 0, int OperationNo = 0)
        {

            var getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0).ToList();

            if (MachineID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == MachineID).ToList();
            }
            else if (CellID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
            }
            else if (ShopID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID).ToList();
            }

            int dateDifference = Convert.ToDateTime(Todate).Subtract(Convert.ToDateTime(FromDate)).Days;

            FileInfo templateFile = new FileInfo(@"C:\TataReport\NewTemplates\ProgramTransferDetailsReportForProgrammer.xlsx");

            ExcelPackage templatep = new ExcelPackage(templateFile);
            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];

            String FileDir = @"C:\TataReport\ReportsList\" + Convert.ToDateTime(Todate).ToString("yyyy-MM-dd");
            bool exists = System.IO.Directory.Exists(FileDir);
            if (!exists)
                System.IO.Directory.CreateDirectory(FileDir);

            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "ProgramTransferDetailsReportForProgrammer" + Convert.ToDateTime(Todate).ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "ProgramTransferDetailsReportForProgrammer" + Convert.ToDateTime(Todate).ToString("yyyy-MM-dd") + ".xlsx"));
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
                    return View();
                }
            }
            //Using the File for generation and populating it
            ExcelPackage p = null;
            p = new ExcelPackage(newFile);
            ExcelWorksheet worksheet = null;

            //Creating the WorkSheet for populating
            try
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(Todate).ToString("dd-MM-yyyy"), Templatews);
            }
            catch { }

            if (worksheet == null)
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(Todate).ToString("dd-MM-yyyy") + "1", Templatews);
            }

            int sheetcount = p.Workbook.Worksheets.Count;
            p.Workbook.Worksheets.MoveToStart(sheetcount);
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            int StartRow = 5;
            int SlNo = 1;
            for (int i = 0; i <= dateDifference; i++)
            {
                DateTime QueryDate = Convert.ToDateTime(FromDate).AddDays(i);
                string correctedDate = QueryDate.ToString("yyyy-MM-dd");
                foreach (var Machine in getMachineList)
                {
                    var GetUtilList = Serverdb.tblprogramtransferhistories.Where(m => m.MachineID == Machine.MachineID && m.correcteddate == correctedDate && m.IsCompleted == 1).ToList();
                    foreach (var MacRow in GetUtilList)
                    {
                        List<string> hyrachy = GetHierarchyData1(MachineID);
                        var partNo = Serverdb.tblparts.Where(m => m.PartID == MacRow.PartId).FirstOrDefault();
                        var username = Serverdb.tblusers.Where(m => m.UserID == MacRow.UserID).FirstOrDefault();
                        worksheet.Cells["A" + StartRow].Value = SlNo++;
                        worksheet.Cells["B" + StartRow].Value = hyrachy[0];
                        worksheet.Cells["C" + StartRow].Value = hyrachy[1];
                        worksheet.Cells["D" + StartRow].Value = hyrachy[2];
                        worksheet.Cells["E" + StartRow].Value = hyrachy[3];
                        worksheet.Cells["F" + StartRow].Value = hyrachy[4];
                        worksheet.Cells["G" + StartRow].Value = correctedDate;
                        worksheet.Cells["H" + StartRow].Value = partNo.PartNo;
                        worksheet.Cells["I" + StartRow].Value = MacRow.OperationNo;
                        worksheet.Cells["J" + StartRow].Value = MacRow.ProgramName;
                        worksheet.Cells["K" + StartRow].Value = MacRow.Version;
                        worksheet.Cells["L" + StartRow].Value = MacRow.ReturnTime;
                        worksheet.Cells["M" + StartRow].Value = username.UserName;
                        worksheet.Cells["N" + StartRow].Value = MacRow.Message;
                        worksheet.Cells["O" + StartRow].Value = "Yes";
                        StartRow++;
                    }

                    var GetserverprogList = Serverdb.tblNcProgramTransferMains.Where(m => m.McId == Machine.MachineID && m.CorrectedDate == correctedDate && m.IsDeleted == false).ToList();
                    foreach (var MacRow in GetserverprogList)
                    {
                        List<string> hyrachy = GetHierarchyData1(MachineID);
                        var partNo = Serverdb.tblparts.Where(m => m.PartID == MacRow.PartId).FirstOrDefault();
                        var username = Serverdb.tblusers.Where(m => m.UserID == 4).FirstOrDefault();
                        worksheet.Cells["A" + StartRow].Value = SlNo++;
                        worksheet.Cells["B" + StartRow].Value = hyrachy[0];
                        worksheet.Cells["C" + StartRow].Value = hyrachy[1];
                        worksheet.Cells["D" + StartRow].Value = hyrachy[2];
                        worksheet.Cells["E" + StartRow].Value = hyrachy[3];
                        worksheet.Cells["F" + StartRow].Value = hyrachy[4];
                        worksheet.Cells["G" + StartRow].Value = correctedDate;
                        worksheet.Cells["H" + StartRow].Value = partNo.PartNo;
                        worksheet.Cells["I" + StartRow].Value = MacRow.OperationNo;
                        worksheet.Cells["J" + StartRow].Value = MacRow.ProgramNumber;
                        worksheet.Cells["K" + StartRow].Value = MacRow.VersionNumber;
                        worksheet.Cells["L" + StartRow].Value = MacRow.CreatedDate;
                        worksheet.Cells["M" + StartRow].Value = username.UserName;
                        worksheet.Cells["P" + StartRow].Value = "Yes";
                        StartRow++;
                    }
                }
            }
            p.Save();

            //Downloding Excel
            string path1 = System.IO.Path.Combine(FileDir, "ProgramTransferDetailsReportForProgrammer" + FromDate + ".xlsx");
            System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
            //string Outgoingfile = "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx";
            string Outgoingfile = "ProgramTransferDetailsReportForProgrammer" + FromDate + ".xlsx";
            if (file1.Exists)
            {
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                Response.AddHeader("Content-Length", file1.Length.ToString());
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.WriteFile(file1.FullName);
                Response.Flush();
                Response.Close();
            }
            return View();
        }


        public ActionResult LogReport()
        {
            ViewData["PlantID"] = new SelectList(Serverdb.tblplants.Where(m => m.IsDeleted == 0), "PlantID", "PlantName");
            ViewData["ShopID"] = new SelectList(Serverdb.tblshops.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "ShopID", "ShopName");
            ViewData["CellID"] = new SelectList(Serverdb.tblcells.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "CellID", "CellName");
            ViewData["MachineID"] = new SelectList(Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.PlantID == 999), "MachineID", "MachineDisplayName");
            ViewData["PartNo"] = new SelectList(Serverdb.tblparts.Where(m => m.IsDeleted == 0), "PartID", "PartNo");
            return View();
        }

        [HttpPost]
        public ActionResult LogReport(string FromDate, string Todate, int PlantID = 0, int ShopID = 0, int CellID = 0, int MachineID = 0, int PartNo = 0, int OperationNo = 0)
        {

            var getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0).ToList();

            if (MachineID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.MachineID == MachineID).ToList();
            }
            else if (CellID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.CellID == CellID).ToList();
            }
            else if (ShopID != 0)
            {
                getMachineList = Serverdb.tblmachinedetails.Where(m => m.IsDeleted == 0 && m.ShopID == ShopID).ToList();
            }

            int dateDifference = Convert.ToDateTime(Todate).Subtract(Convert.ToDateTime(FromDate)).Days;

            FileInfo templateFile = new FileInfo(@"C:\TataReport\NewTemplates\LogReport.xlsx");

            ExcelPackage templatep = new ExcelPackage(templateFile);
            ExcelWorksheet Templatews = templatep.Workbook.Worksheets[1];

            String FileDir = @"C:\TataReport\ReportsList\" + Convert.ToDateTime(Todate).ToString("yyyy-MM-dd");
            bool exists = System.IO.Directory.Exists(FileDir);
            if (!exists)
                System.IO.Directory.CreateDirectory(FileDir);

            FileInfo newFile = new FileInfo(System.IO.Path.Combine(FileDir, "LogReport" + Convert.ToDateTime(Todate).ToString("yyyy-MM-dd") + ".xlsx")); //+ " to " + toda.ToString("yyyy-MM-dd") 
            if (newFile.Exists)
            {
                try
                {
                    newFile.Delete();  // ensures we create a new workbook
                    newFile = new FileInfo(System.IO.Path.Combine(FileDir, "LogReport" + Convert.ToDateTime(Todate).ToString("yyyy-MM-dd") + ".xlsx"));
                }
                catch
                {
                    TempData["Excelopen"] = "Excel with same date is already open, please close it and try to generate!!!!";
                    return View();
                }
            }
            //Using the File for generation and populating it
            ExcelPackage p = null;
            p = new ExcelPackage(newFile);
            ExcelWorksheet worksheet = null;

            //Creating the WorkSheet for populating
            try
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(Todate).ToString("dd-MM-yyyy"), Templatews);
            }
            catch { }

            if (worksheet == null)
            {
                worksheet = p.Workbook.Worksheets.Add(Convert.ToDateTime(Todate).ToString("dd-MM-yyyy") + "1", Templatews);
            }

            int sheetcount = p.Workbook.Worksheets.Count;
            p.Workbook.Worksheets.MoveToStart(sheetcount);
            worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            int StartRow = 5;
            int SlNo = 1;
            for (int i = 0; i <= dateDifference; i++)
            {
                DateTime QueryDate = Convert.ToDateTime(FromDate).AddDays(i);
                string correctedDate = QueryDate.ToString("yyyy-MM-dd");
                foreach (var Machine in getMachineList)
                {
                    var GetUtilList = Serverdb.tbllogreports.Where(m => m.MachineID == Machine.MachineID && m.LogDate == QueryDate.Date && m.IsDeleted == 0).ToList();
                    foreach (var MacRow in GetUtilList)
                    {
                        List<string> hyrachy = GetHierarchyData1(MachineID);
                        var partNo = Serverdb.tblparts.Where(m => m.PartID == MacRow.partNo).FirstOrDefault();
                        if(partNo != null)
                        {
                            worksheet.Cells["H" + StartRow].Value = partNo.PartNo;
                        }
                        var username = Serverdb.tblusers.Where(m => m.UserID == MacRow.userId).FirstOrDefault();
                        worksheet.Cells["A" + StartRow].Value = SlNo++;
                        worksheet.Cells["B" + StartRow].Value = hyrachy[0];
                        worksheet.Cells["C" + StartRow].Value = hyrachy[1];
                        worksheet.Cells["D" + StartRow].Value = hyrachy[2];
                        worksheet.Cells["E" + StartRow].Value = hyrachy[3];
                        worksheet.Cells["F" + StartRow].Value = hyrachy[4];
                        worksheet.Cells["G" + StartRow].Value = correctedDate;
                        worksheet.Cells["I" + StartRow].Value = MacRow.operationNo;
                        worksheet.Cells["J" + StartRow].Value = MacRow.programNumber;
                        worksheet.Cells["K" + StartRow].Value = MacRow.version;
                        worksheet.Cells["L" + StartRow].Value = MacRow.LogCapturedTime;
                        worksheet.Cells["M" + StartRow].Value = username.UserName;
                       
                        if (MacRow.LogDescription == "UploadToMachine")
                        {
                            worksheet.Cells["N" + StartRow].Value = "Yes";
                        }
                        else if (MacRow.LogDescription == "UploadToServer")
                        {
                            worksheet.Cells["O" + StartRow].Value = "Yes";
                        }
                        else if (MacRow.LogDescription == "UploadToCNC")
                        {
                            worksheet.Cells["P" + StartRow].Value = "Yes";
                        }
                        else if (MacRow.LogDescription == "Notify")
                        {
                            worksheet.Cells["Q" + StartRow].Value = "Yes";
                            worksheet.Cells["R" + StartRow].Value = MacRow.LogDescription;
                        }
                        else if (MacRow.LogDescription == "UploadAsPreviousVersion")
                        {
                            worksheet.Cells["S" + StartRow].Value ="Yes";
                        }
                        else if (MacRow.LogDescription == "UploadAsNewversion")
                        {
                            worksheet.Cells["T" + StartRow].Value = "Yes";
                        }
                        else if (MacRow.LogDescription == "DeleteProgram")
                        {
                            worksheet.Cells["U" + StartRow].Value = "Yes";
                        }
                        else if (MacRow.LogDescription == "Compare")
                        {
                            worksheet.Cells["V" + StartRow].Value = "Yes";
                        }
                        
                        StartRow++;
                    }
                }
            }
            p.Save();

            //Downloding Excel
            string path1 = System.IO.Path.Combine(FileDir, "LogReport" + FromDate + ".xlsx");
            System.IO.FileInfo file1 = new System.IO.FileInfo(path1);
            //string Outgoingfile = "ToolLifeMonitoringSheet" + frda.ToString("yyyy-MM-dd") + ".xlsx";
            string Outgoingfile = "LogReport" + FromDate + ".xlsx";
            if (file1.Exists)
            {
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Outgoingfile);
                Response.AddHeader("Content-Length", file1.Length.ToString());
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.WriteFile(file1.FullName);
                Response.Flush();
                Response.Close();
            }
            return View();
        }


        List<string> GetHierarchyData1(int MachineID)
        {
            List<string> HierarchyData = new List<string>();
            //1st get PlantName or -
            //2nd get ShopName or -
            //3rd get CellName or -
            //4th get MachineName.

            using (i_facility_talEntities dbMac = new i_facility_talEntities())
            {
                var machineData = dbMac.tblmachinedetails.Where(m => m.MachineID == MachineID).FirstOrDefault();
                int PlantID = Convert.ToInt32(machineData.PlantID);
                string name = "-";
                name = dbMac.tblplants.Where(m => m.PlantID == PlantID).Select(m => m.PlantName).FirstOrDefault();
                HierarchyData.Add(name);

                string ShopIDString = Convert.ToString(machineData.ShopID);
                int value;
                if (int.TryParse(ShopIDString, out value))
                {
                    name = dbMac.tblshops.Where(m => m.ShopID == value).Select(m => m.ShopName).FirstOrDefault();
                    HierarchyData.Add(name.ToString());
                }
                else
                {
                    HierarchyData.Add("-");
                }

                string CellIDString = Convert.ToString(machineData.CellID);
                if (int.TryParse(CellIDString, out value))
                {
                    name = dbMac.tblcells.Where(m => m.CellID == value).Select(m => m.CellName).FirstOrDefault();
                    HierarchyData.Add(name.ToString());
                }
                else
                {
                    HierarchyData.Add("-");
                }
                 HierarchyData.Add(Convert.ToString(machineData.MachineDispName));
                //HierarchyData.Add(Convert.ToString(machineData.MachineID));
                HierarchyData.Add(Convert.ToString(machineData.MachineInvNo));
            }
            return HierarchyData;
        }


        public JsonResult GetShop(int PlantID)
        {
            var ShopData = (from row in Serverdb.tblshops
                            where row.IsDeleted == 0 && row.PlantID == PlantID
                            select new { Value = row.ShopID, Text = row.ShopName });
            return Json(ShopData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCell(int ShopID)
        {
            var CellData = (from row in Serverdb.tblcells
                            where row.IsDeleted == 0 && row.ShopID == ShopID
                            select new { Value = row.CellID, Text = row.CellName });

            return Json(CellData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWC_Cell(int CellID)
        {
            var MachineData = (from row in Serverdb.tblmachinedetails
                               where row.IsDeleted == 0 && row.CellID == CellID
                               select new { Value = row.MachineID, Text = row.MachineInvNo });
            return Json(MachineData, JsonRequestBehavior.AllowGet);
        }


    }

}