using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using MySapsApplication.Models;
using MySapsApplication.Models.Suspects;
using System.Diagnostics;
using System.Security.Cryptography;

namespace MySapsApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SuspectsDbContext _context;
       
        public HomeController(ILogger<HomeController> logger, SuspectsDbContext context)
        {
            _logger = logger;
            _context = context;
         
        }
        private static HashSet<int> generatedNumbers = new HashSet<int>();




        //----------home link
        public IActionResult Home()
        {
            return View();
        }



        //-------------number generation

        public ActionResult SuspectNo()

        {
            return View(new SuspectNoModel());
        }

        [HttpPost]
        [HttpPost]
        public ActionResult GenerateNumbers()
        {
            int number = GenerateUniqueNumber(2); // Generate a 2-digit number

            // Ensure the number is unique
            if (generatedNumbers.Add(number))
            {
                return Json(new { success = true, number });
            }
            else
            {
                return Json(new { success = false, message = "Duplicate number generated." });
            }
        }

        private int GenerateUniqueNumber(int digits)
        {
            if (digits < 1 || digits > 10)
                throw new ArgumentException("Digits must be between 1 and 10.");

            Random rand = new Random();
            var availableDigits = Enumerable.Range(0, 10).ToList(); // Digits 0-9
            var selectedDigits = new List<int>();

            // Ensure the first digit is not zero for a 2-digit number
            selectedDigits.Add(availableDigits[rand.Next(1, availableDigits.Count)]); // Pick a non-zero digit
            availableDigits.Remove(selectedDigits[0]);

            // Randomly select the second digit
            int index = rand.Next(0, availableDigits.Count);
            selectedDigits.Add(availableDigits[index]);

            // Convert list of digits to a single integer
            int result = int.Parse(string.Join("", selectedDigits));
            return result;
        }






        //---------------------Index edit Suspect by id
        public IActionResult Index(int? id)
        {
            if (id != null)
            {
                var IndexRecordInDb = _context.IndexModels.SingleOrDefault(IndexModel => IndexModel.Id == id);
                return View(IndexRecordInDb);
            }
            return View();
        }

        //----------------------Index formdata
        public IActionResult IndexForm(IndexModel model)
        {
            if (model.Id == 0)
            {
                _context.IndexModels.Add(model);
            }
            else
            {
                _context.IndexModels.Update(model);
            }
            _context.SaveChanges();
            TempData["AlertMessage"] = "record successfully Added to Database";
            return RedirectToAction("Index");
        }

        //------------------------Index Delete form
        public IActionResult DeleteSuspect(int id)
        {
            var IndexRecordInDb = _context.IndexModels.SingleOrDefault(IndexModel => IndexModel.Id == id);
            _context.IndexModels.Remove(IndexRecordInDb);
            _context.SaveChanges();
            return RedirectToAction("IndexRecord");
        }

        //--------------------------Index record List
        public IActionResult IndexRecord()
        {
            var allSuspects = _context.IndexModels.ToList();
            return View(allSuspects);
        }





        //----------------------Offences
        public IActionResult Offence()
        {
            return View();
        }
        public IActionResult OffenceForm(OffenceModel model)
        {
            _context.OffenceModels.Add(model);
            _context.SaveChanges();
            TempData["AlertMessage"] = "Offence recorded successfully";
            return RedirectToAction("Offence");

        }



        //------------------------criminal record edit by Id

        public IActionResult CrimeRecord(int? id)
        {
            // Retrieve the list of offences for the dropdown
            var offences = _context.OffenceModels.ToList();
            ViewBag.OffenceTypes = new SelectList(offences, "OffenceType", "OffenceType");

            CrimeRecordModel model;

            if (id != null)
            {
                // Fetch the existing crime record by ID
                var indexRecordInDb = _context.CrimeRecordModels.SingleOrDefault(cr => cr.Id == id);
                if (indexRecordInDb != null)
                {
                    model = indexRecordInDb;
                }
                else
                {
                    model = new CrimeRecordModel();
                }
            }
            else
            {
                model = new CrimeRecordModel();
            }

            // Automatically set IssuedBy based on the logged-in user
            if (User.IsInRole("Member"))// Check the username for Joe
            {
                model.IssuedBy = "Police Officer";
            }
            else if (User.IsInRole("Admin"))
            {
                model.IssuedBy = "sam";
            }

            return View(model);
        }



        //--------------------------------Add crime to db
        public IActionResult AddCrimeRecord(CrimeRecordModel model)
        {
            if (model.Id == 0)
            {
                _context.CrimeRecordModels.Add(model);
            }
            else
            {
                _context.CrimeRecordModels.Update(model);
            }
            _context.SaveChanges();
            return RedirectToAction("Search");
        }


        //-------------------crime record delete
        public IActionResult Delete(int id)
        {
            var CrimeRecordInDb = _context.CrimeRecordModels.SingleOrDefault(CrimeRecordModel => CrimeRecordModel.Id == id);
            _context.CrimeRecordModels.Remove(CrimeRecordInDb);
            _context.SaveChanges();
            return RedirectToAction("CrimeRecordList");
        }


        //-----------------------crime record list
        public IActionResult CrimeRecordList()
        {
            var allSuspects = _context.CrimeRecordModels.ToList();
            return View(allSuspects);
        }



        //-------------------------stats for cases

        public IActionResult StatsForCases()
        {
            var offenceStatistics = _context.CrimeRecordModels
                .GroupBy(x => x.Offence) // Group by Offence
                .Select(g => new
                {
                    OffenceType = g.Key,
                    Count = g.Count()
                })
                .ToList();

            ViewBag.OffenceStatistics = offenceStatistics; // Store offence stats in ViewBag

            return View(); // Return the view, no need for allSuspects
        }


        //--------------complete cases
        public IActionResult CompleteCases()
        {
                var completeCases = _context.CrimeRecordModels
                    .Where(record => record.Status == "CompleteCase")
                    .ToList();

                return View(completeCases);
        }



        //----------------------pending cases
        public async Task<IActionResult> Pending()
        {
            // Retrieve records for Sam and Police Officer with specific statuses
            var samRecords = await _context.CrimeRecordModels
                .Where(s => s.IssuedBy == "Sam" && (s.Status == "NotCompleteCase" || s.Status == "PendingCase"))
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            var policeRecords = await _context.CrimeRecordModels
                .Where(s => s.IssuedBy == "Police Officer" && (s.Status == "NotCompleteCase" || s.Status == "PendingCase"))
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            // Count the records
            int samCount = samRecords.Count;
            int policeCount = policeRecords.Count;
            int difference = Math.Abs(samCount - policeCount);

            // Prepare the message and determine leftover records
            List<CrimeRecordModel> leftoverRecords = new List<CrimeRecordModel>();

            if (difference == 0)
            {
                ViewBag.Message = $"Sam and Police Officer have an equal number of cases: {samCount} cases each.";
                ViewBag.ShowTable = false; // Do not show the table
            }
            else
            {
                ViewBag.ShowTable = true; // Show the table
                ViewBag.Message = $"There is a difference of {difference} case(s): Sam has {samCount} cases and Police Officer has {policeCount} cases.";

                // Determine how many records to take based on the difference
                int recordsToTake = Math.Min(difference, Math.Max(samCount, policeCount)); // Take the lesser of difference or count of records

                leftoverRecords = (samCount > policeCount)
                    ? samRecords.Take(recordsToTake).ToList()
                    : policeRecords.Take(recordsToTake).ToList();
            }

            // Set ViewBag properties for records
            ViewBag.SamRecords = samRecords;
            ViewBag.PoliceRecords = policeRecords;
            ViewBag.RemainingRecords = leftoverRecords;

            return View();
        }


        //------------------case manager
        public async Task<IActionResult> CaseManager()
        {
            // Retrieve records for Sam and Police Officer with specific statuses
            var samRecords = await _context.CrimeRecordModels
                .Where(s => s.IssuedBy == "Sam" && (s.Status == "NotCompleteCase" || s.Status == "PendingCase"))
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            var policeRecords = await _context.CrimeRecordModels
                .Where(s => s.IssuedBy == "Police Officer" && (s.Status == "NotCompleteCase" || s.Status == "PendingCase"))
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            // Count the records
            int samCount = samRecords.Count;
            int policeCount = policeRecords.Count;
            int difference = Math.Abs(samCount - policeCount);

            // Prepare the message and determine leftover records
            List<CrimeRecordModel> leftoverRecords = new List<CrimeRecordModel>();

            if (difference == 0)
            {
                ViewBag.Message = $"Sam and Police Officer have an equal number of cases: {samCount} cases each.";
                ViewBag.ShowTable = false; // Do not show the table
            }
            else
            {
                ViewBag.ShowTable = true; // Show the table
                ViewBag.Message = $"There is a difference of {difference} case(s): Sam has {samCount} cases and Police Officer has {policeCount} cases.";

                // Determine how many records to take based on the difference
                int recordsToTake = Math.Min(difference, Math.Max(samCount, policeCount)); // Take the lesser of difference or count of records

                leftoverRecords = (samCount > policeCount)
                    ? samRecords.Take(recordsToTake).ToList()
                    : policeRecords.Take(recordsToTake).ToList();
            }

            // Set ViewBag properties for records
            ViewBag.SamRecords = samRecords;
            ViewBag.PoliceRecords = policeRecords;
            ViewBag.RemainingRecords = leftoverRecords;

            return View();
        }


        //------------------case manager
        public async Task<IActionResult> CaseManagers()
        {
            // Retrieve records for Sam and Police Officer with specific statuses
            var samRecords = await _context.CrimeRecordModels
                .Where(s => s.IssuedBy == "Sam" && (s.Status == "NotCompleteCase" || s.Status == "PendingCase"))
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            var policeRecords = await _context.CrimeRecordModels
                .Where(s => s.IssuedBy == "Police Officer" && (s.Status == "NotCompleteCase" || s.Status == "PendingCase"))
                .OrderByDescending(s => s.Id)
                .ToListAsync();

            // Count the records
            int samCount = samRecords.Count;
            int policeCount = policeRecords.Count;
            int difference = Math.Abs(samCount - policeCount);

            // Prepare the message and determine leftover records
            List<CrimeRecordModel> leftoverRecords = new List<CrimeRecordModel>();

            if (difference == 0)
            {
                ViewBag.Message = $"Sam and Police Officer have an equal number of cases: {samCount} cases each.";
                ViewBag.ShowTable = false; // Do not show the table
            }
            else
            {
                ViewBag.ShowTable = true; // Show the table
                ViewBag.Message = $"There is a difference of {difference} case(s): Sam has {samCount} cases and Police Officer has {policeCount} cases.";

                // Determine how many records to take based on the difference
                int recordsToTake = Math.Min(difference, Math.Max(samCount, policeCount)); // Take the lesser of difference or count of records

                leftoverRecords = (samCount > policeCount)
                    ? samRecords.Take(recordsToTake).ToList()
                    : policeRecords.Take(recordsToTake).ToList();
            }

            // Set ViewBag properties for records
            ViewBag.SamRecords = samRecords;
            ViewBag.PoliceRecords = policeRecords;
            ViewBag.RemainingRecords = leftoverRecords;

            return View();
        }











        //------------------------sam casemanager
        public async Task<IActionResult> SamCaseManager()
        {
            // Retrieve records for Sam directly
            var samRecords = await _context.CrimeRecordModels
                .Where(s => s.IssuedBy == "Sam")
                .ToListAsync();

            // Set ViewBag property for Sam records only
            ViewBag.SamRecords = samRecords;

            return View();
        }




        //------------------------police officer  casemanager
        public async Task<IActionResult> PoliceCaseManager()
        {
            // Retrieve records for "Police Officer" directly
            var policeRecords = await _context.CrimeRecordModels
                .Where(s => s.IssuedBy == "Police Officer")
                .ToListAsync();

            // Set ViewBag property for police records
            ViewBag.policeRecords = policeRecords;

            return View();
        }





        //----------------------------search

        public IActionResult Search()
        {
            var allRegisters = _context.IndexModels.ToList();
            return View(allRegisters);
        }


        //-------------------------------search engine

        [HttpGet]
        public async Task<IActionResult> IndexRecord(string Empsearch)
        {
            ViewData["GetRegistersDetails"] = Empsearch;
            var empquery = _context.IndexModels.AsQueryable();
            if (!string.IsNullOrEmpty(Empsearch))
            {
                empquery = empquery.Where(x => x.SuspectIdentity.Contains(Empsearch));
            }
            var results = await empquery.AsNoTracking().ToListAsync();
            return View(results);
        }


        //-----------------------------searchCrime

        [HttpGet]
        public async Task<IActionResult> CrimeRecordList(string Empsearch)
        {
            ViewData["GetRegistersDetails"] = Empsearch;
            var empquery = _context.CrimeRecordModels.AsQueryable();

            // Check user role
            if (User.IsInRole("Admin"))
            {
                // Filter for Sam
                empquery = empquery.Where(x => x.IssuedBy == "Sam" && x.SuspectNo.Contains(Empsearch));
            }
            else if (User.IsInRole("Member"))
            {
                // Filter for Police Officer
                empquery = empquery.Where(x => x.IssuedBy == "Police Officer" && x.SuspectNo.Contains(Empsearch));
            }

            var results = await empquery.AsNoTracking().ToListAsync();
            return View(results);
        }







        //------------------------Manager Login


        //Record
        [Authorize(Roles = "Manager")]
        public IActionResult Record()
        {
            var viewModel = from i in _context.IndexModels
                            join c in _context.CrimeRecordModels
                            on i.SuspectNo equals c.SuspectNo
                            where i.SuspectNo == c.SuspectNo
                            orderby i.Id, c.Id // Replace Id with the actual property name if different
                            select new Records
                            {
                                IndexModels = i,
                                CrimeRecordModel = c
                            };

            return View(viewModel.ToList()); // Ensure the query is executed
        }






        //-----------------------casemanager 1 sam
        public IActionResult CaseManager1()
        {
            return View();
        }

        //Admin1
        [Authorize(Roles = "Manager")]
        public IActionResult Admin1()
        {
            var allSuspects = _context.CrimeRecordModels
                                      .Where(c => c.IssuedBy == "Sam")
                                      .ToList();
            return View(allSuspects);
        }
        [Authorize(Roles = "Manager")]
        public IActionResult Admin1Record()
        {
            var viewModel = from i in _context.IndexModels
                            join c in _context.CrimeRecordModels
                            on i.SuspectNo equals c.SuspectNo
                            where c.IssuedBy == "Sam" // Filter for IssuedBy == "sam"
                            orderby i.SuspectNo, c.SuspectNo
                            select new Records
                            {
                                IndexModels = i,
                                CrimeRecordModel = c
                            };
            return View(viewModel);
        }





        //-----------------------CaseManager 2 police officer
        public IActionResult CaseManager2()
        {
            return View();
        }

        //Admin2
        [Authorize(Roles = "Manager")]
        public IActionResult Admin2()
        {
            var allSuspects = _context.CrimeRecordModels
                                      .Where(c => c.IssuedBy == "Police Officer")
                                      .ToList();
            return View(allSuspects);
        }
        [Authorize(Roles = "Manager")]
        public IActionResult Admin2Record()
        {
            var viewModel = from i in _context.IndexModels
                            join c in _context.CrimeRecordModels
                            on i.SuspectNo equals c.SuspectNo
                            where c.IssuedBy == "Police Officer"
                            orderby i.SuspectNo, c.SuspectNo
                            select new Records
                            {
                                IndexModels = i,
                                CrimeRecordModel = c
                            };

            // Ensure you are setting the ViewBag
            ViewBag.PoliceOfficerRecords = viewModel.ToList();

            return View(viewModel);
        }



        //--------------------complete cases


        public IActionResult StatsCompletedCases()
        {
            // Count completed cases issued by "sam"
            var samCompletedCasesCount = _context.CrimeRecordModels
                .Count(record => record.Status == "CompleteCase" && record.IssuedBy.ToLower() == "sam");

            // Count completed cases issued by "police officer"
            var officerCompletedCasesCount = _context.CrimeRecordModels
                .Count(record => record.Status == "CompleteCase" && record.IssuedBy.ToLower() == "police officer");

            // Calculate total completed cases
            var totalCompletedCasesCount = samCompletedCasesCount + officerCompletedCasesCount;

            // Count pending cases issued by "sam"
            var samPendingCasesCount = _context.CrimeRecordModels
                .Count(record => record.Status == "PendingCase" && record.IssuedBy.ToLower() == "sam");

            // Count pending cases issued by "police officer"
            var officerPendingCasesCount = _context.CrimeRecordModels
                .Count(record => record.Status == "PendingCase" && record.IssuedBy.ToLower() == "police officer");

            // Calculate total pending cases
            var totalPendingCasesCount = samPendingCasesCount + officerPendingCasesCount;

            // Pass the counts to the view
            ViewBag.SamCompletedCasesCount = samCompletedCasesCount;
            ViewBag.OfficerCompletedCasesCount = officerCompletedCasesCount;
            ViewBag.TotalCompletedCasesCount = totalCompletedCasesCount;

            ViewBag.SamPendingCasesCount = samPendingCasesCount;
            ViewBag.OfficerPendingCasesCount = officerPendingCasesCount;
            ViewBag.TotalPendingCasesCount = totalPendingCasesCount;

            return View();
        }




        //-------------------privacy
        public IActionResult CasesStats()
        {
            var allSuspects = _context.CrimeRecordModels.ToList();

            // Generate offence statistics
            var offenceStatistics = allSuspects
                .GroupBy(x => x.Offence)
                .Select(g => new
                {
                    OffenceType = g.Key,
                    Count = g.Count()
                })
                .ToList();

            ViewBag.OffenceStatistics = offenceStatistics;

            // Count cases for specific suspect "Sam"
            var samCasesCount = allSuspects.Count(c => c.IssuedBy.Equals("Sam", StringComparison.OrdinalIgnoreCase));

            // Count cases for specific police officer
            var officerName = "Police Officer"; // Replace with the actual officer's name
            var officerCasesCount = allSuspects.Count(c => c.IssuedBy.Equals(officerName, StringComparison.OrdinalIgnoreCase));

            // Calculate total cases
            var totalCasesCount = samCasesCount + officerCasesCount;

            // Store counts in ViewBag
            ViewBag.SamCasesCount = samCasesCount;
            ViewBag.OfficerCasesCount = officerCasesCount;
            ViewBag.TotalCasesCount = totalCasesCount; // Store total count

            return View(); // Return the view
        }
    }
}
        