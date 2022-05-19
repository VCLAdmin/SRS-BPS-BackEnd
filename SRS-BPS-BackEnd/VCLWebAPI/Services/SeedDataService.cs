using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using VCLWebAPI.Models.Edmx;
using Excel = Microsoft.Office.Interop.Excel;

namespace VCLWebAPI.Services
{
    public class SeedDataService
    {
        private VCLDesignDBEntities _db;
        private readonly string baseUrl = AppDomain.CurrentDomain.BaseDirectory + "Content\\Excel\\";

        //string baseUrl = @"D:/Dropbox/12 Schuco/GitRepo/VCLDesign/VCLDesign/VCLWebAPI/Content/Excel/";
        //string baseUrl = @"C:/inetpub/subdomains/apiweb_test/Content/Excel/";

        public SeedDataService()
        {
            _db = new VCLDesignDBEntities();
        }

        public int ImportArticlesFromExcel()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            string excelPath = baseUrl + "Article_Library_Phase 1A_With_Duplicates.xlsx";
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            Excel.Range column;

            string articleName = String.Empty;
            string productPrettyName = String.Empty;
            string articleTypePrettyName = String.Empty;
            double depth = 00.00;
            double insideWidth = 00.00;
            double outsideWidth = 00.00;
            string offsetReference = String.Empty;
            double leftRebate = -1.00;
            double rightRebate = -1.00;
            double distBetweenIsoBars = -1.00;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];

                    if (column.Value2 == null)
                    {
                        continue;
                    }

                    string value = column.Value2.ToString();

                    switch (j)
                    {
                        case 1:
                            articleName = value;
                            break;

                        case 2:
                            productPrettyName = value;
                            break;

                        case 3:
                            articleTypePrettyName = value;
                            break;

                        case 4:
                            depth = value != "null" ? Convert.ToDouble(value) : 00.00; //Int32.Parse(value);
                            break;

                        case 5:
                            insideWidth = value != "null" ? Convert.ToDouble(value) : 00.00;
                            break;

                        case 6:
                            outsideWidth = value != "null" ? Convert.ToDouble(value) : 00.00;
                            break;

                        case 7:
                            offsetReference = value;
                            break;

                        case 8:
                            leftRebate = value != "null" ? Convert.ToDouble(value) : -1.00;
                            break;

                        case 9:
                            rightRebate = value != "null" ? Convert.ToDouble(value) : -1.00;
                            break;

                        case 10:
                            distBetweenIsoBars = value != "null" ? Convert.ToDouble(value) : -1.00;
                            break;
                    }

                    //Debug.WriteLine(value);
                }

                AddProductIfNotExist(productPrettyName);

                Article article = _db.Article.Where(x => x.Name == "article__" + articleName).SingleOrDefault();
                Product product = GetProductFromPrettyName(productPrettyName);

                if (article == null)
                {
                    Article newArticle = new Article
                    {
                        ArticleGuid = Guid.NewGuid(),
                        Name = "article__" + articleName,
                        Unit = "mm",
                        ArticleTypeId = GetArticleTypeIdFromPrettyName(articleTypePrettyName),
                        CrossSectionUrl = null,
                        Description = articleTypePrettyName + " " + articleName,
                        InsideDimension = insideWidth,
                        OutsideDimension = outsideWidth,
                        Dimension = -1,
                        OffsetReference = offsetReference,
                        LeftRebate = leftRebate,
                        RightRebate = rightRebate,
                        DistBetweenIsoBars = distBetweenIsoBars
                    };

                    newArticle.Product.Add(product);

                    _db.Article.Add(newArticle);
                    _db.SaveChanges();
                }
                else if (!article.Product.Contains(product))
                {
                    article.Product.Add(product);

                    _db.Entry(article).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                else
                {
                    // Do nothing. Product exists and article exists
                }
            }

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            return 1;
        }

        public int ImportInsulatingBarsFromExcel()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            string excelPath = baseUrl + "InsulatingBar_Phase 1A.xlsx";
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            Excel.Range column;

            int articleID = 0;
            string productPrettyName = String.Empty;
            string ptCoatedBefore = "0";
            string ptAnodizedBefore = "0";
            string paCoatedBefore = "0";
            string paCoatedAfter = "0";
            string paAnodizedBefore = "0";
            string paAnodizedAfter = "0";

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];

                    if (column.Value2 == null)
                    {
                        continue;
                    }

                    string value = column.Value2.ToString();

                    switch (j)
                    {
                        case 1:
                            articleID = Convert.ToInt32(value);
                            break;

                        case 2:
                            productPrettyName = value;
                            break;

                        case 3:
                            ptCoatedBefore = value != "null" ? value : "0";
                            break;

                        case 4:
                            ptAnodizedBefore = value != "null" ? value : "0";
                            break;

                        case 5:
                            paCoatedBefore = value != "null" ? value : "0";
                            break;

                        case 6:
                            paCoatedAfter = value != "null" ? value : "0";
                            break;

                        case 7:
                            paAnodizedBefore = value != "null" ? value : "0";
                            break;

                        case 8:
                            paAnodizedAfter = value != "null" ? value : "0";
                            break;
                    }

                    Debug.WriteLine(value);
                }

                InsulatingBar insulatingBar = _db.InsulatingBar.Where(x => x.ArticleId == articleID && x.ProductName == productPrettyName).SingleOrDefault();

                if (insulatingBar == null)
                {
                    InsulatingBar newInsulatingBar = new InsulatingBar
                    {
                        InsulatingBarGuid = Guid.NewGuid(),
                        ArticleId = articleID,
                        ProductName = productPrettyName,
                        PTCoatedBefore = ptCoatedBefore,
                        PTAnodizedBefore = ptAnodizedBefore,
                        PACoatedBefore = paCoatedBefore,
                        PACoatedAfter = paCoatedAfter,
                        PAAnodizedBefore = paAnodizedBefore,
                        PAAnodizedAfter = paAnodizedAfter,
                    };

                    _db.InsulatingBar.Add(newInsulatingBar);
                    _db.SaveChanges();
                }
            }

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            return 1;
        }

        public int ImportThermalBtoBDataFromExcel()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            string excelPath = baseUrl + "bpsolver_phase I _ b2B.xlsx";
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelPath);

            // read standard data
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            Excel.Range column;

            string series = String.Empty;
            string glazingInsulationType = String.Empty;
            string fixedorOperable = String.Empty;
            double glassThickness = 0.0;
            string papt = String.Empty;
            double k = 0.0;
            double I = 0.0;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];

                    if (column.Value2 == null)
                    {
                        continue;
                    }

                    string value = column.Value2.ToString();

                    switch (j)
                    {
                        case 1:
                            series = value;
                            break;

                        case 2:
                            glazingInsulationType = value;
                            break;

                        case 3:
                            fixedorOperable = value;
                            break;

                        case 4:
                            glassThickness = Convert.ToDouble(value);
                            break;

                        case 5:
                            papt = value;
                            break;

                        case 6:
                            k = Convert.ToDouble(value);
                            break;

                        case 7:
                            I = Convert.ToDouble(value);
                            break;
                    }
                }

                ThermalBtoBStandardData newStandardData = new ThermalBtoBStandardData
                {
                    StandardID = i - 1,
                    Series = series,
                    GlazingInsulationType = glazingInsulationType,
                    FixedorOperable = fixedorOperable,
                    GlassThickness = glassThickness,
                    PAPT = papt,
                    k = k,
                    I = I,
                };

                _db.ThermalBtoBStandardData.Add(newStandardData);
                _db.SaveChanges();
            }

            // read block data
            xlWorksheet = xlWorkbook.Sheets[2];
            xlRange = xlWorksheet.UsedRange;

            rowCount = xlRange.Rows.Count;
            colCount = xlRange.Columns.Count;

            double ventFWidth = 0.0;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];

                    if (column.Value2 == null)
                    {
                        continue;
                    }

                    string value = column.Value2.ToString();

                    switch (j)
                    {
                        case 1:
                            series = value;
                            break;

                        case 2:
                            ventFWidth = Convert.ToDouble(value);
                            break;

                        case 3:
                            fixedorOperable = value;
                            break;

                        case 4:
                            glassThickness = Convert.ToDouble(value);
                            break;

                        case 5:
                            papt = value;
                            break;

                        case 6:
                            k = Convert.ToDouble(value);
                            break;

                        case 7:
                            I = Convert.ToDouble(value);
                            break;
                    }
                }

                ThermalBtoBBlockData newBlockData = new ThermalBtoBBlockData
                {
                    BlockID = i - 1,
                    Series = series,
                    VentFWidth = ventFWidth,
                    FixedorOperable = fixedorOperable,
                    GlassThickness = glassThickness,
                    PAPT = papt,
                    k = k,
                    I = I,
                };

                _db.ThermalBtoBBlockData.Add(newBlockData);
                _db.SaveChanges();
            }

            // read direct data
            xlWorksheet = xlWorkbook.Sheets[3];
            xlRange = xlWorksheet.UsedRange;

            rowCount = xlRange.Rows.Count;
            colCount = xlRange.Columns.Count;

            double C382180 = 0, C382200 = 0, C382310 = 0, C382330 = 0,
                C382340 = 0, C382110 = 0, C382170 = 0, C374980 = 0;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];

                    if (column.Value2 == null)
                    {
                        continue;
                    }

                    string value = column.Value2.ToString();

                    switch (j)
                    {
                        case 1:
                            series = value;
                            break;

                        case 2:
                            glazingInsulationType = value;
                            break;

                        case 3:
                            fixedorOperable = value;
                            break;

                        case 4:
                            glassThickness = Convert.ToDouble(value);
                            break;

                        case 5:
                            papt = value;
                            break;

                        case 6:
                            C382180 = Convert.ToDouble(value);
                            break;

                        case 7:
                            C382200 = Convert.ToDouble(value);
                            break;

                        case 8:
                            C382310 = Convert.ToDouble(value);
                            break;

                        case 9:
                            C382330 = Convert.ToDouble(value);
                            break;

                        case 10:
                            C382340 = Convert.ToDouble(value);
                            break;

                        case 11:
                            C382110 = Convert.ToDouble(value);
                            break;

                        case 12:
                            C382170 = Convert.ToDouble(value);
                            break;

                        case 13:
                            C374980 = Convert.ToDouble(value);
                            break;
                    }
                }

                ThermalBtoBDirectData newDirectData = new ThermalBtoBDirectData
                {
                    DirectID = i - 1,
                    Series = series,
                    GlazingInsulationType = glazingInsulationType,
                    FixedorOperable = fixedorOperable,
                    GlassThickness = glassThickness,
                    PAPT = papt,
                    C382180 = C382180,
                    C382200 = C382200,
                    C382310 = C382310,
                    C382330 = C382330,
                    C382340 = C382340,
                    C382110 = C382110,
                    C382170 = C382170,
                    C374980 = C374980,
                };

                _db.ThermalBtoBDirectData.Add(newDirectData);
                _db.SaveChanges();
            }

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            return 1;
        }

        public int SeedUsers()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            string excelPath = baseUrl + "Users.xlsx";
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            Excel.Range column;

            string userName = String.Empty;
            string nameFirst = String.Empty;
            string nameLast = String.Empty;
            string email = String.Empty;
            string language = "en-US";
            string company = String.Empty;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];

                    string value = column.Value2?.ToString();

                    switch (j)
                    {
                        case 1:
                            userName = value;
                            break;

                        case 2:
                            nameFirst = value;
                            break;

                        case 3:
                            nameLast = value;
                            break;

                        case 4:
                            email = value;
                            break;

                        case 5:
                            company = value;
                            break;
                    }

                    Debug.WriteLine(value);
                }

                User user = new User
                {
                    UserName = userName,
                    UserGuid = Guid.NewGuid(),
                    NameFirst = nameFirst,
                    NameLast = nameLast,
                    Email = email,
                    Language = language,
                    Company = company
                };

                //List<AccessRole> accessRoles = GetAccessRolesForUser(userName);

                //foreach (AccessRole accessRole in accessRoles)
                //{
                //    user.AccessRole.Add(accessRole);
                //}

                if (_db.User.Where(x => x.UserName == userName).SingleOrDefault() == null)
                {
                    _db.User.Add(user);
                    _db.SaveChanges();
                }
            }

            AccountService ac = new AccountService();
            ac.SaltAndHashNewUsers();

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            return 1;
        }

        public List<AccessRole> GetAccessRolesForUser(string userName)
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            string excelPath = baseUrl + "UserAccess.xlsx";
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            Excel.Range column;

            List<AccessRole> accessRoles = new List<AccessRole>();

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];

                    string value = column.Value2?.ToString();

                    if (value.Equals(userName))
                    {
                        accessRoles.Add(new AccessRole
                        {
                            AccessRoleId = Int32.Parse(xlRange.Cells[i, j + 1])
                        });

                        continue;
                    }
                }
            }

            AccountService ac = new AccountService();
            ac.SaltAndHashNewUsers();

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            return accessRoles;
        }

        public int SeedAccessRole()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            string excelPath = baseUrl + "AccessRole.xlsx";
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            Excel.Range column;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                int accessRoleId = -1;
                string accessRoleName = String.Empty;
                string value = String.Empty;

                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];
                    value = column.Value2?.ToString();

                    switch (j)
                    {
                        case 1:
                            accessRoleId = Int32.Parse(value);
                            break;

                        case 2:
                            accessRoleName = value;
                            break;
                    }
                }

                AccessRole accessRole = new AccessRole
                {
                    AccessRoleId = accessRoleId,
                    AccessRoleName = accessRoleName
                };

                if (_db.AccessRole.Where(x => x.AccessRoleName == accessRoleName).SingleOrDefault() == null)
                {
                    _db.AccessRole.Add(accessRole);
                    _db.SaveChanges();
                }
            }

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            return 1;
        }

        public int SeedUserAccess()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            string excelPath = baseUrl + "AccessRole.xlsx";
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            Excel.Range column;

            string accessRoleName = String.Empty;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];

                    string value = column.Value2?.ToString();

                    switch (j)
                    {
                        case 2:
                            accessRoleName = value;
                            break;
                    }

                    Debug.WriteLine(value);
                }

                AccessRole accessRole = new AccessRole
                {
                    AccessRoleName = accessRoleName,
                };

                if (_db.AccessRole.Where(x => x.AccessRoleName == accessRoleName).SingleOrDefault() == null)
                {
                    _db.AccessRole.Add(accessRole);
                    _db.SaveChanges();
                }
            }

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            return 1;
        }

        public int SeedProductType()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            string excelPath = baseUrl + "ProductType.xlsx";
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            Excel.Range column;

            string productCode = String.Empty;
            string prettyName = String.Empty;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];

                    string value = column.Value2.ToString();

                    switch (j)
                    {
                        case 1:
                            productCode = value;
                            break;

                        case 2:
                            prettyName = value;
                            break;
                    }

                    Debug.WriteLine(value);
                }

                ProductType productType = new ProductType
                {
                    ProductCode = productCode,
                    PrettyName = prettyName
                };

                if (_db.ProductType.Where(x => x.ProductCode == productCode).SingleOrDefault() == null)
                {
                    _db.ProductType.Add(productType);
                    _db.SaveChanges();
                }
            }

            AccountService ac = new AccountService();
            ac.SaltAndHashNewUsers();

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            return 1;
        }

        private bool AddProductIfNotExist(string productPrettyName)
        {
            string productCode = productPrettyName.ToLower();
            productCode = productCode.Replace(" ", "__");
            productCode = productCode.Replace(".", "_");
            productCode = productCode.Replace("+", "_plus");

            Product product = new Product
            {
                ProductGuid = Guid.NewGuid(),
                Name = productCode,
                PrettyName = productPrettyName
            };

            if (_db.Product.Where(x => x.Name == productCode).SingleOrDefault() == null)
            {
                _db.Product.Add(product);
                _db.SaveChanges();
            }

            return true;
        }

        private Product GetProductFromPrettyName(string productPrettyName)
        {
            string productCode = productPrettyName.ToLower();
            productCode = productCode.Replace(" ", "__");
            productCode = productCode.Replace(".", "_");
            productCode = productCode.Replace("+", "_plus");

            Product product = _db.Product.Where(x => x.Name == productCode).SingleOrDefault();
            return product;
        }

        private int GetArticleTypeIdFromPrettyName(string articlePrettyName)
        {
            int articleTypeId;
            ArticleType articleType = _db.ArticleType.Where(x => x.PrettyName == articlePrettyName).SingleOrDefault();

            if (articleType == null)
            {
                var articleCode = articlePrettyName.ToLower();
                articleCode = articleCode.Replace(" ", "__");

                ArticleType newArticleType = new ArticleType
                {
                    PrettyName = articlePrettyName,
                    Name = articleCode,
                    ArticleTypeGuid = Guid.NewGuid()
                };

                _db.ArticleType.Add(newArticleType);
                _db.SaveChanges();

                newArticleType = _db.ArticleType.Where(x => x.Name == articleCode).SingleOrDefault();

                articleTypeId = newArticleType.ArticleTypeId;
            }
            else
            {
                articleTypeId = articleType.ArticleTypeId;
            }

            return articleTypeId;
        }

        public void SeedDatabase()
        {
            // seed user access role
            // seed users
            // seed user access
            // seed product type
            // import articles from excel
        }

        public int ImportPostCodeDataFromExcel()
        {
            //Create COM Objects. Create a COM object for everything that is referenced
            string excelPath = baseUrl + "StaticWindSnowZoneGermany.xlsx";
            Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(excelPath);
            Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
            Excel.Range xlRange = xlWorksheet.UsedRange;

            int rowCount = xlRange.Rows.Count;
            int colCount = xlRange.Columns.Count;

            Excel.Range column;

            int postCodeID = 0;
            string postCode = String.Empty;
            int windZone = 0;
            string state = String.Empty;
            string district = String.Empty;
            string place = String.Empty;

            //iterate over the rows and columns and print to the console as it appears in the file
            //excel is not zero based!!
            for (int i = 2; i <= rowCount; i++)//rowCount
            {
                for (int j = 1; j <= colCount; j++)
                {
                    column = xlRange.Cells[i, j];

                    if (column.Value2 == null)
                    {
                        continue;
                    }

                    string value = column.Value2.ToString();

                    switch (j)
                    {
                        case 1:
                            postCodeID = Convert.ToInt32(value);
                            break;

                        case 2:
                            postCode = value;
                            break;

                        case 3:
                            state = value;
                            break;

                        case 4:
                            district = value;
                            break;

                        case 5:
                            place = value;
                            break;

                        case 7:
                            windZone = Convert.ToInt32(value);
                            break;
                    }
                }

                WindZoneGermany windzone = _db.WindZoneGermany.Where(x => x.PostCode == postCode).SingleOrDefault();

                if (windzone == null)
                {
                    WindZoneGermany newWindZone = new WindZoneGermany
                    {
                        PostCodeID = postCodeID,
                        PostCode = postCode,
                        WindZone = windZone,
                        State = state,
                        District = district,
                        Place = place
                    };

                    _db.WindZoneGermany.Add(newWindZone);
                    _db.SaveChanges();
                }
            }

            //cleanup
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //rule of thumb for releasing com objects:
            //  never use two dots, all COM objects must be referenced and released individually
            //  ex: [somthing].[something].[something] is bad

            //release com objects to fully kill excel process from running in the background
            Marshal.ReleaseComObject(xlRange);
            Marshal.ReleaseComObject(xlWorksheet);

            //close and release
            xlWorkbook.Close();
            Marshal.ReleaseComObject(xlWorkbook);

            //quit and release
            xlApp.Quit();
            Marshal.ReleaseComObject(xlApp);

            return 1;
        }
    }
}