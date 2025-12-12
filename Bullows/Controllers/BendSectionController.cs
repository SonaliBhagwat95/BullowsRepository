using Bullows.Business;
using Bullows.Database;
using Bullows.Model;
using Bullows.Repositories.Contracts;
using Bullows.Repositories.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharpDX;
using System.IO.Compression;


namespace Bullows.Controllers
{
    public class BendSectionController : BaseController
    {
        static int BID = 0;
        private readonly UnitOfWorks _uow;
        private readonly ISession Session;
        static int SaveFlag = 0;
        public BendSectionController(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            this._uow = uow as UnitOfWorks;
            this.Session = httpContextAccessor.HttpContext.Session;          
        }

        public IActionResult BendSection(int id=0)
        {

            try
            {
                ViewBag.getAllData = _uow.bendSectionRepository.GetAllData();
                if (id > 0)
                {

                }
                else
                {

                    SaveFlag = 0;
                    if (BID == 1)
                        SetSuccessMessage("BendSection has been saved successfully");
                    else if (BID == 2)
                        SetErrorMessage("BendSection has been deleted successfully");
                    else if (BID < 0)
                        SetErrorMessage("Something went wrong while saving BendSection");
                    BID = 0;
                }
                ViewBag.ActivePage = "BendSection";
                return View();

            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("BendSectionController", "BendSection", ex.Message);

                return View();
            }
           
        }

        [HttpPost]
        public IActionResult Save(BendSectionModel model,BendSectionTable tblobj, List<SlotDetail> slotDetailsList)
        {
            try
            {
                var result = _uow.bendSectionRepository.SaveBendSectionDetails(model, tblobj, slotDetailsList);
                return RedirectToAction("BendSection", result);
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("BendSectionController", "Save", ex.Message);
                return RedirectToAction("BendSection");
            }
        }
        //public IActionResult DownloadFile(int id)
        //{
        //    var item = _uow.bendSectionRepository.GetById(id);

        //    if (item == null)
        //    {
        //        return NotFound();
        //    }

        //    // Deserialize the SlotDetails column to get the list of slot details
        //    List<SlotDetail> slotDetails = JsonConvert.DeserializeObject<List<SlotDetail>>(item.SlotDetails);
        //    double pitchDistance = 0;
        //    string slotLocation ="";
        //    string slotDimensions = "";
        //    // Example: Access the first slot location, pitch distance, and slot dimensions
        //    var firstSlot = slotDetails.FirstOrDefault();

        //    if (firstSlot != null)
        //    {
        //         pitchDistance = (double)firstSlot.PitchDistance;
        //         slotLocation = firstSlot.SlotLocation;
        //         slotDimensions = firstSlot.SlotDimensions;
        //    }

        //    // Create a model for the development drawing
        //    var model = new BendSectionModel
        //    {
        //        Length = item.Length,
        //        H = item.H,
        //        W = item.W,
        //        T = item.T,
        //        L = item.L,
        //        L1 = item.L1,
        //        PitchDistance = pitchDistance,
        //        SlotDimentions = slotDimensions,
        //        SlotLocation = slotLocation

        //    };

        //    // Add files for download based on section type
        //    string developmentfilePath = "";
        //    string DWGFilepath = "";
        //    List<string> files = new List<string>();
        //    BendSection bend = new BendSection();

        //    if (item.SectionName == "CSection")
        //    {
        //        developmentfilePath = bend.devlopmentForCSection(model, slotDetails);
        //        DWGFilepath = bend.CSection3DDrawing(model);
        //        files.Add(developmentfilePath);
        //        files.Add(DWGFilepath);
        //    }
        //    else if (item.SectionName == "LSection")
        //    {
        //        developmentfilePath = bend.devlopmentForLSection(model, slotDetails);
        //        DWGFilepath = bend.LSection3DDrawing(model);
        //        files.Add(developmentfilePath);
        //        files.Add(DWGFilepath);
        //    }
        //    else if (item.SectionName == "L1Section")
        //    {
        //        developmentfilePath = bend.devlopmentForL1Section(model, slotDetails);
        //        DWGFilepath = bend.L1Section3DDrawing(model);
        //        files.Add(developmentfilePath);
        //        files.Add(DWGFilepath);
        //    }
        //    else if (item.SectionName == "PanelSupport")
        //    {
        //        developmentfilePath = bend.devlopmentForPanelSupport(model, slotDetails);
        //        DWGFilepath = bend.PanelSupport3DDrawing(model);
        //        files.Add(developmentfilePath);
        //        files.Add(DWGFilepath);
        //    }
        //    else if (item.SectionName == "Corner")
        //    {
        //        developmentfilePath = bend.devlopmentForCorner(model, slotDetails);
        //        DWGFilepath = bend.Corners3DDrawing(model);
        //        files.Add(developmentfilePath);
        //        files.Add(DWGFilepath);
        //    }

        //    string zipFileName = $"BendSectionDrawings {DateTime.Now:dd-MM HH-mm}.zip";
        //    string tempFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        //    Directory.CreateDirectory(tempFolderPath);
        //    string zipFilePath = Path.Combine(tempFolderPath, zipFileName);

        //    using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
        //    {
        //        foreach (var filePath in files)
        //        {
        //            zip.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
        //        }
        //    }

        //    var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
        //    System.IO.File.Delete(zipFilePath);

        //    // Return the file to the user
        //    return File(fileBytes, "application/dwg", zipFileName);
        //}
        public IActionResult DownloadFile(int id)
        {
            try
            {
                var item = _uow.bendSectionRepository.GetById(id);

                if (item == null)
                {
                    return NotFound();
                }

                // Initialize variables for slot details
                List<SlotDetail> slotDetails = new List<SlotDetail>();
                double pitchDistance = 0;
                string slotLocation = "";
                string slotDimensions = "";

                // Check if SlotDetails is "NA" or contains valid JSON
                if (!string.IsNullOrEmpty(item.SlotDetails) && item.SlotDetails != "NA")
                {
                    try
                    {
                        // Deserialize the SlotDetails column to get the list of slot details
                        slotDetails = JsonConvert.DeserializeObject<List<SlotDetail>>(item.SlotDetails);
                        var firstSlot = slotDetails.FirstOrDefault();

                        if (firstSlot != null)
                        {
                            pitchDistance = (double)firstSlot.PitchDistance;
                            slotLocation = firstSlot.SlotLocation;
                            slotDimensions = firstSlot.SlotDimensions;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log or handle deserialization errors
                        Console.WriteLine($"Error deserializing SlotDetails: {ex.Message}");
                        return BadRequest("Invalid slot details format.");
                    }
                }

                // Create a model for the development drawing
                var model = new BendSectionModel
                {
                    Length = item.Length,
                    H = item.H,
                    W = item.W,
                    T = item.T,
                    L = item.L,
                    L1 = item.L1,
                    PitchDistance = pitchDistance,
                    SlotDimentions = slotDimensions,
                    SlotLocation = slotLocation
                };

                // Add files for download based on section type
                string developmentfilePath = "";
                string DWGFilepath = "";
                List<string> files = new List<string>();
                BendSection bend = new BendSection();

                switch (item.SectionName)
                {
                    case "CSection":
                        developmentfilePath = bend.devlopmentForCSection(model, slotDetails);
                        DWGFilepath = bend.CSection3DDrawing(model);
                        break;
                    case "LSection":
                        developmentfilePath = bend.devlopmentForLSection(model, slotDetails);
                        DWGFilepath = bend.LSection3DDrawing(model);
                        break;
                    case "L1Section":
                        developmentfilePath = bend.devlopmentForL1Section(model, slotDetails);
                        DWGFilepath = bend.L1Section3DDrawing(model);
                        break;
                    case "PanelSupport":
                        developmentfilePath = bend.devlopmentForPanelSupport(model, slotDetails);
                        DWGFilepath = bend.PanelSupport3DDrawing(model);
                        break;
                    case "Corner":
                        developmentfilePath = bend.devlopmentForCorner(model, slotDetails);
                        DWGFilepath = bend.Corners3DDrawing(model);
                        break;
                    default:
                        return BadRequest("Invalid section type.");
                }

                files.Add(developmentfilePath);
                files.Add(DWGFilepath);

                // Create zip file
                string zipFileName = $"BendSectionDrawings {DateTime.Now:dd-MM HH-mm}.zip";
                string tempFolderPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempFolderPath);
                string zipFilePath = Path.Combine(tempFolderPath, zipFileName);

                using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    foreach (var filePath in files)
                    {
                        zip.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
                    }
                }

                var fileBytes = System.IO.File.ReadAllBytes(zipFilePath);
                System.IO.File.Delete(zipFilePath);

                // Return the file to the user
                return File(fileBytes, "application/zip", zipFileName);
            }
            catch (Exception ex)
            {
                _uow.exceptionHandlerRepository.SaveException("BendSectionController", "DownloadFile", ex.Message);

                throw;
            }
        }

        public IActionResult Delete(int id=0)
        {
            BID=_uow.bendSectionRepository.Delete(id);
            BID = 2;
            return RedirectToAction("BendSection");
            
        }
    }
}
