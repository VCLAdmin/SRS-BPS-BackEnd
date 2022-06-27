using BpsUnifiedModelLib;
using StructuralSolverSBA;
using System;
//using System.Web.Http;
//using System.Web.Http.Description;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using VCLWebAPI.Services;

namespace VCLWebAPI.Controllers
{
    /// <summary>
    /// Defines the <see cref="StructuralController" />.
    /// </summary>
    [Authorize]
    public class StructuralController : BaseController
    {
        /// <summary>
        /// The CalculateDXFSectionProperties.
        /// </summary>
        /// <param name="input">The input<see cref="DXFInput"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Section))]
        [Route("api/Structural/CalculateDXFSectionProperties/")]
        public IActionResult CalculateDXFSectionProperties(DXFInput input)
        {
            try
            {
                var structuralService = new StructuralService();
                Section output = structuralService.CalculateDXFSectionProperties(input);
                return Ok(output);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The CalculateWindLoadDIN.
        /// </summary>
        /// <param name="input">The input<see cref="DinWindLoadInput"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WindLoadOutput))]
        [Route("api/Structural/CalculateWindLoadDIN/")]
        public IActionResult CalculateWindLoadDIN(DinWindLoadInput input)
        {
            try
            {
                StructuralService structuralService = new StructuralService();
                return Ok(structuralService.CalculateWindLoadDIN(input));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The GetWindZone.
        /// </summary>
        /// <param name="PostCode">The PostCode<see cref="string"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(WindZoneOutput))]
        [Route("api/Structural/GetWindZone/{PostCode}")]
        public IActionResult GetWindZone(string PostCode)
        {
            try
            {
                StructuralService structuralService = new StructuralService();
                return Ok(structuralService.GetWindZone(PostCode));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The ReadFacadeSectionProperties.
        /// </summary>
        /// <param name="unifiedModel">The unifiedModel<see cref="BpsUnifiedModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Section))]
        [Route("api/Structural/ReadFacadeSectionProperties/")]
        public IActionResult ReadFacadeSectionProperties(BpsUnifiedModel unifiedModel)
        {
            try
            {
                var structuralService = new StructuralService();
                structuralService.ReadFacadeSectionPropertiesFromDB(unifiedModel);
                return Ok(unifiedModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The ReadSectionProperties.
        /// </summary>
        /// <param name="unifiedModel">The unifiedModel<see cref="BpsUnifiedModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Section))]
        [Route("api/Structural/ReadSectionProperties/")]
        public IActionResult ReadSectionProperties([FromBody]BpsUnifiedModel unifiedModel)
        {
            try
            {
                var structuralService = new StructuralService();
                structuralService.ReadSectionPropertiesFromDB(unifiedModel);
                return Ok(unifiedModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// The ReadSectionProperties.
        /// </summary>
        /// <param name="unifiedModel">The unifiedModel<see cref="BpsUnifiedModel"/>.</param>
        /// <returns>The <see cref="IActionResult"/>.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Section))]
        [Route("api/Structural/ReadSectionPropertiesFromDXF/")]
        public IActionResult ReadSectionPropertiesFromDXF(BpsUnifiedModel unifiedModel)
        {
            try
            {
                var structuralService = new StructuralService();
                structuralService.ReadSectionPropertiesFromDXF(unifiedModel);
                return Ok(unifiedModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
