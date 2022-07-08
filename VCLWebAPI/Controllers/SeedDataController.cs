//using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using VCLWebAPI.Services;

namespace VCLWebAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="SeedDataController" />.
    /// </summary>
    [Authorize]
    public class SeedDataController : BaseController
    {
        /// <summary>
        /// Defines the _seedDataService.
        /// </summary>
        internal SeedDataService _seedDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeedDataController"/> class.
        /// </summary>
        public SeedDataController()
        {
            _seedDataService = new SeedDataService();
        }

        /// <summary>
        /// The ImportArticlesFromExcel.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int ImportArticlesFromExcel()
        {
            return _seedDataService.ImportArticlesFromExcel();
        }

        /// <summary>
        /// The ImportInsulatingBarsFromExcel.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int ImportInsulatingBarsFromExcel()
        {
            return _seedDataService.ImportInsulatingBarsFromExcel();
        }

        /// <summary>
        /// The ImportPostCodeDataFromExcel.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int ImportPostCodeDataFromExcel()
        {
            return _seedDataService.ImportPostCodeDataFromExcel();
        }

        /// <summary>
        /// The ImportThermalBtoBDataFromExcel.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int ImportThermalBtoBDataFromExcel()
        {
            return _seedDataService.ImportThermalBtoBDataFromExcel();
        }

        /// <summary>
        /// The SeedAccessRole.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int SeedAccessRole()
        {
            return _seedDataService.SeedAccessRole();
        }

        /// <summary>
        /// The SeedProductType.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int SeedProductType()
        {
            return _seedDataService.SeedProductType();
        }

        /// <summary>
        /// The SeedUsers.
        /// </summary>
        /// <returns>The <see cref="int"/>.</returns>
        public int SeedUsers()
        {
            return _seedDataService.SeedUsers();
        }
    }
}
