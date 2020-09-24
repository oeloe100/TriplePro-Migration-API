using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TPMApi.Controllers
{
    public class FormController : Controller
    {
        /// <summary>
        /// Return loginform to view
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public IActionResult LoginPartial(string viewName)
        {
            return PartialView("_LoginFormPartial");
        }

        /// <summary>
        /// return registerform to view
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public IActionResult RegisterPartial(string viewName)
        {
            return PartialView("_RegisterFormPartial");
        }
    }
}